using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    private readonly List<ClientInfo> clients;
    private UILogger? logger;
    private static readonly JsonSerializerSettings serializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.Auto
    };

    public delegate void RequestReceivedHandler(TcpClient client, string request, Action<TcpClient, string> respondCallback);
    public event RequestReceivedHandler? RequestReceived;

    public TcpConnection(int port)
    {
        listener = new(IPAddress.Any, port);
        clients = new();
    }

    protected virtual void TryLogMessage(string message)
    {
        logger?.LogMessage(message);
    }

    public void AttachLogger(UILogger logger)
    {
        this.logger = logger;
    }

    public void Start()
    {
        listener.Start();
        TryLogMessage("Server started");
        ListenForClientsAsync();
    }

    public void Stop()
    {
        listener.Stop();
        TryLogMessage("Server stopped");
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

            }

        }
        catch (Exception ex)
        {
            //TODO formalize no type
            Response invalidRequestResponse = new("InvalidRequest", "invalid request type", ex.Message);
            RespondToClient(clientInfo.TcpClient, invalidRequestResponse);
        }
    }

    private void HandleAuthorizationRequest(ClientInfo clientInfo, AuthorizationRequest? request)
    {
        // Fail check
        if (request == null)
        {
            Response invalidRequestResponse = new(TcpConnectionDictionary.request_authorization,
                TcpConnectionDictionary.status_fail, "invalid request type");
            RespondToClient(clientInfo.TcpClient, invalidRequestResponse);
            return;
        }

        // Reject check
        if (request.ClientId < 1 || request.ClientId > 99 || clients.Where(c => c.ClientId == request.ClientId).Any())
        {
            Response NotAuthorizedResponse = new(TcpConnectionDictionary.request_authorization,
                TcpConnectionDictionary.status_notAccepted, "id number rejected");
            RespondToClient(clientInfo.TcpClient, NotAuthorizedResponse);
            return;
        }

        // Verify client
        Response authorizationResponse = new(TcpConnectionDictionary.request_authorization,
            TcpConnectionDictionary.status_accepted, "id number accepted");
        clientInfo.IsAuthenticated = true;
        clientInfo.ClientId = request.ClientId;
        RespondToClient(clientInfo.TcpClient, authorizationResponse);
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
                    TryLogMessage($"Client connected - {client.TcpClient.Client.RemoteEndPoint}");
                }

                _ = HandleClientAsync(client);
            }
        }
        catch (ObjectDisposedException)
        {
            TryLogMessage("Lister has been stopped.");
        }
        catch (Exception ex)
        {
            TryLogMessage($"Failed to listen: {ex}");
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
            TryLogMessage($"Error - {ex.Message}");
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

    private static void RespondToClient(TcpClient client, Response response)
    {
        string jsonResponse = JsonConvert.SerializeObject(response);
        var responseBytes = Encoding.UTF8.GetBytes(jsonResponse);
        client.GetStream().WriteAsync(responseBytes, 0, responseBytes.Length);
    }
}
