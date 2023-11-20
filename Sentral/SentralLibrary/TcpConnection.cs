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
    private readonly List<TcpClient> clients;
    private UILogger? logger;

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
                client.Close();
            }
            clients.Clear();
        }
    }

    public int GetActiveClientcount()
    {
        lock (clients)
        {
            return clients.Count;
        }
    }

    private async void ListenForClientsAsync()
    {
        try
        {
            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                lock (clients)
                {
                    clients.Add(client);
                    TryLogMessage($"Client connected - {client.Client.RemoteEndPoint}");
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
            TryLogMessage($"Failed to listen {ex}");
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        try
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            while (client.Connected)
            {
                int bytesRead = await stream.ReadAsync(buffer);
                if (bytesRead == 0) break;

                string request = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                RequestReceived?.Invoke(client, request, RespondToClient);
            }
        }
        catch (Exception ex)
        {
            TryLogMessage($"Error - {ex.Message}");
        }
        finally
        {
            client.Close();
            lock (clients)
            {
                TryLogMessage($"Client disconnected - {client.Client.RemoteEndPoint}");
                clients.Remove(client);
            }

        }
    }

    private void RespondToClient(TcpClient client, string response)
    {
        var responseBytes = Encoding.ASCII.GetBytes(response);
        client.GetStream().WriteAsync(responseBytes, 0, responseBytes.Length);
    }
}
