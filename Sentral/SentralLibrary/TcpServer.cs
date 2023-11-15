using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SentralLibrary;
public class TcpServer
{
    private TcpListener listener;
    private readonly List<TcpClient> clients;

    public event EventHandler<MessageReceivedEventArgs>? RequestReceived;
    public event EventHandler<string>? LogMessage;

    public TcpServer(int port)
    {
        listener = new(IPAddress.Any, port);
        clients = new();
    }

    protected virtual void OnLogMessage(string message)
    {
        LogMessage?.Invoke(this, message);
    }

    public void Start()
    {
        listener.Start();
        OnLogMessage("Server started");
        ListenForClientsAsync();
    }

    public void Stop()
    {
        listener.Stop();
        OnLogMessage("Server stopped");
        lock (clients)
        {
            foreach (var client in clients)
            {
                client.Close();
            }
            clients.Clear();
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
                    OnLogMessage($"Client connected: {client.Client.RemoteEndPoint}");
                }

                _ = HandleClientAsync(client);
            }
        }
        catch (ObjectDisposedException)
        {
            OnLogMessage("Lister has been stopped.");
        }
        catch (Exception ex)
        {
            OnLogMessage($"Failed to listen {ex}");
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
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                string request = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                RequestReceived?.Invoke(this, new MessageReceivedEventArgs(request));
            }
        }
        catch (Exception ex)
        {
            OnLogMessage($"Error: {ex.Message}");
        }
        finally
        {
            client.Close();
            lock (clients)
            {
                clients.Remove(client);
                OnLogMessage($"Client disconnected: {client.Client.RemoteEndPoint}");
            }

        }
    }
}
