using System.Net; 
using System.Net.Sockets; 
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetCoreServer;

/*
 * Echo Protocol Implementation
 *
 * Echo protocol implementation based on RFC 862 (https://tools.ietf.org/html/rfc862)
 *
 * Binds to port 7 by default. Can be configured to bind to a different port.
 *
 * Based on NetCoreServer: https://chronoxor.github.io/NetCoreServer/
 */
namespace EchoSeven
{
    /*
    TCP Based Echo Service

    One echo service is defined as a connection based application on TCP.
    A server listens for TCP connections on TCP port 7.  Once a
    connection is established any data received is sent back.  This
    continues until the calling user terminates the connection.
     */
    public class EchoTcpServer : TcpServer
    {
        
        private ILogger<BackgroundService> Logger { get; }
        
        public EchoTcpServer(IPAddress address, int port, ILogger<BackgroundService> logger) : base(address, port)
        {
            Logger = logger;
        }
        
        protected override TcpSession CreateSession() { return new EchoTcpSession(this, Logger); }

        protected override void OnError(SocketError error)
        {
            Logger.LogError($"Echo TCP server caught an error with code {error}");
        }
    }

    public class EchoTcpSession : TcpSession
    {
        
        private ILogger<BackgroundService> Logger { get; }
        
        public int OptionReceiveTimeout
        {
            get => Socket.ReceiveTimeout;
            set => Socket.ReceiveTimeout = value;
        }
        
        public int OptionSendTimeout
        {
            get => Socket.SendTimeout;
            set => Socket.SendTimeout = value;
        }
        
        public EchoTcpSession(TcpServer server, ILogger<BackgroundService> logger) : base(server)
        {
            Logger = logger;
        }
        
        protected override void OnConnected()
        {
            Logger.LogInformation("Session connected: " + this.Id);
            ReceiveAsync();
        }
        
        protected override void OnDisconnected()
        {
            Logger.LogInformation("Session disconnected: " + this.Id);
        }
        
        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            Logger.LogInformation("Session '" + Id + "' received: " + message);
            
            SendAsync(buffer, offset, size);
            Logger.LogInformation("Session '" + Id + "' responded: " + message);
        }

    }

    /*
    UDP Based Echo Service

    Another echo service is defined as a datagram based application on
    UDP.  A server listens for UDP datagrams on UDP port 7.  When a
    datagram is received, the data from it is sent back in an answering
    datagram.
     */
    public class EchoUdpServer : UdpServer
    {
        private ILogger<BackgroundService> Logger { get; }

        public EchoUdpServer(IPAddress address, int port, ILogger<BackgroundService> logger) : base(address, port)
        {
            Logger = logger;
        }

        protected override void OnStarted()
        {
            ReceiveAsync();
        }
        
        protected override void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
        {
            Logger.LogInformation("Received: " + Encoding.UTF8.GetString(buffer, (int)offset, (int)size));
            
            SendAsync(endpoint, buffer, 0, size);
        }
        
        protected override void OnSent(EndPoint endpoint, long sent)
        {
            ReceiveAsync();
        }

        protected override void OnError(SocketError error)
        {
            Logger.LogError($"Echo UDP server caught an error with code {error}");
        }
    }

}