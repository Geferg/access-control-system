using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

/*
 * Concerns:
 * - Exception handling in respondtoclient
 * - Handling partial messages
 * - Resource management for tcpclient and networkstream (using or dispose)
 * - Concurrency for multiple clients
 * - Fixed buffer size
 * - Testing different network scenarios
 */

namespace SentralLibrary;
public class TcpConnection
{
    private readonly TcpListener listener;
    private List<ClientInfo> clients;
    private static readonly JsonSerializerSettings serializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.Auto
    };

    public delegate void AlarmReportHandler(ClientInfo clientInfo, AlarmReportRequest request);
    public event AlarmReportHandler? AlarmReport;
    public delegate void AccessReportHandler(ClientInfo clientInfo, AccessReportRequest request);
    public event AccessReportHandler? AccessReport;
    public delegate void AccessHandler(ClientInfo clientInfo, AccessRequest request);
    public event AccessHandler? Access;

    public TcpConnection(int port)
    {
        listener = new(IPAddress.Any, port);
        clients = new();
    }

    public void Start()
    {
        listener.Start();
        //TryLogMessage("Server started");
        ListenForClientsAsync();
    }

    public void Stop()
    {
        listener.Stop();
        //TryLogMessage("Server stopped");
        lock (clients)
        {
            foreach (var client in clients)
            {
                client.TcpClient.Close();
            }
            clients.Clear();
        }
    }

    public int GetAutorizedClientCount()
    {
        lock (clients)
        {
            return clients.Where(c => c.IsAuthenticated).Count();
        }
    }

    public int GetUnauthorizedClientCount()
    {
        lock (clients)
        {
            return clients.Where(c => !c.IsAuthenticated).Count();
        }
    }

    public List<int> GetClientIds()
    {
        List<int> ids = new();
        lock (clients)
        {
            foreach(var client in clients.Where(c => c.IsAuthenticated))
            {
                ids.Add(client.ClientId);
            }
        }
        return ids;
    }

    private void DelegateRequests(ClientInfo clientInfo, string request)
    {
        try
        {

            BaseRequest? requestDeserialized = JsonConvert.DeserializeObject<BaseRequest>(request) ?? throw new Exception();

            switch (requestDeserialized.RequestType)
            {
                //TODO Add cases
                //TODO add generic type response handler
                case TcpConnectionDictionary.request_authorization:
                    HandleAuthorizationRequest(clientInfo, JsonConvert.DeserializeObject<AuthorizationRequest>(request));
                    break;

                case TcpConnectionDictionary.request_alarmReport:
                    HandleAlarmReportRequest(clientInfo, JsonConvert.DeserializeObject<AlarmReportRequest>(request));
                    break;

                case TcpConnectionDictionary.request_access:
                    HandleAccessRequest(clientInfo, JsonConvert.DeserializeObject<AccessRequest>(request));
                    break;
            }

        }
        catch (Exception ex)
        {
            //TODO formalize no type
            Response invalidRequestResponse = new("InvalidRequest", "invalid request type", ex.Message);
            RespondToClient(clientInfo.TcpClient, invalidRequestResponse);
        }
    }

    private void HandleAccessRequest(ClientInfo clientInfo, AccessRequest? request)
    {
        string actionType = TcpConnectionDictionary.request_access;
        string message;
        string status;

        if (request == null)
        {
            status = TcpConnectionDictionary.status_fail;
            message = "invalid request type";
        }
        else if(true)
        {
            status = TcpConnectionDictionary.status_notAccepted;
            message = "invalid credentials";
        }
        else
        {
            status= TcpConnectionDictionary.status_accepted;
            message = "access granted";
        }

        SendResponseToClient(clientInfo, actionType, status, message);
    }

    private void HandleAlarmReportRequest(ClientInfo clientInfo, AlarmReportRequest? request)
    {
        string actionType = TcpConnectionDictionary.request_alarmReport;
        string message;
        string status;

        if (request == null)
        {
            status = TcpConnectionDictionary.status_fail;
            message = "invalid request type";
        }
        else if (request.AlarmType != TcpConnectionDictionary.alarm_breach &&
            request.AlarmType != TcpConnectionDictionary.alarm_timeout)
        {
            status = TcpConnectionDictionary.status_notAccepted;
            message = "invalid alarm type";
        }
        else
        {
            status = TcpConnectionDictionary.status_accepted;
            message = "report is logged";
            AlarmReport?.Invoke(clientInfo, request);
        }

        SendResponseToClient(clientInfo, actionType, status, message);
    }

    private void HandleAuthorizationRequest(ClientInfo clientInfo, AuthorizationRequest? request)
    {
        string actionType = TcpConnectionDictionary.request_authorization;
        string message;
        string status;

        if (request == null)
        {
            status = TcpConnectionDictionary.status_fail;
            message = "invalid request type";
        }
        else if (request.ClientId < 1 || request.ClientId > 99 || clients.Any(c => c.ClientId == request.ClientId))
        {
            status = TcpConnectionDictionary.status_notAccepted;
            message = "id number rejected";
        }
        else
        {
            status = TcpConnectionDictionary.status_accepted;
            message = "id number accepted";
            clientInfo.IsAuthenticated = true;
            clientInfo.ClientId = request.ClientId;
        }

        SendResponseToClient(clientInfo, actionType, status, message);
    }

    private async void ListenForClientsAsync()
    {
        try
        {
            while (true)
            {
                TcpClient newClient = await listener.AcceptTcpClientAsync();
                ClientInfo client = new(newClient);
                lock (clients)
                {
                    clients.Add(client);
                    //TryLogMessage($"Client connected - {client.TcpClient.Client.RemoteEndPoint}");
                }

                _ = HandleClientAsync(client);
            }
        }
        catch (ObjectDisposedException)
        {
            //TryLogMessage("Lister has been stopped.");
        }
        catch (Exception ex)
        {
            //TryLogMessage($"Failed to listen: {ex}");
        }
    }

    private async Task HandleClientAsync(ClientInfo client)
    {
        try
        {
            NetworkStream stream = client.TcpClient.GetStream();
            byte[] buffer = new byte[1024];
            while (client.TcpClient.Connected)
            {
                int bytesRead = await stream.ReadAsync(buffer);
                if (bytesRead == 0) break;

                string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                DelegateRequests(client, request);
                //RequestReceived?.Invoke(client.TcpClient, request, RespondToClient);
            }
        }
        catch (Exception ex)
        {
            //TryLogMessage($"Error - {ex.Message}");
        }
        finally
        {
            client.TcpClient.Close();
            lock (clients)
            {
                clients.Remove(client);
            }

        }
    }

    private static void SendResponseToClient(ClientInfo clientInfo, string actionType, string status, string message)
    {
        Response response = new(actionType, status, message);
        RespondToClient(clientInfo.TcpClient, response);
    }

    private static void RespondToClient(TcpClient client, Response response)
    {
        string jsonResponse = JsonConvert.SerializeObject(response);
        var responseBytes = Encoding.UTF8.GetBytes(jsonResponse);
        client.GetStream().WriteAsync(responseBytes, 0, responseBytes.Length);
    }
}
