using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using EchoSevenUtility;

namespace EchoSeven
{
    public class EchoSevenService : BackgroundService
    {
        private readonly ILogger<EchoSevenService> _logger;
        private readonly EchoServerOptions _options;
        private EchoTcpServer _echoTcpServer;
        private EchoUdpServer _echoUdpServer;

        public EchoSevenService(ILogger<EchoSevenService> logger, EchoServerOptions options)
        {
            _logger = logger;
            _options = options;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("Configure echo service for " + _options.IPAddress);
            
            var echoTcpServer = new EchoTcpServer(IPAddress.Parse(_options.IPAddress), _options.Port, _logger);
            var echoUdpServer = new EchoUdpServer(IPAddress.Parse(_options.IPAddress), _options.Port, _logger);
            if (echoTcpServer.Start())
            {
                _logger.LogInformation("Echo TCP Server started.");
            }
            else
            {
                _logger.LogCritical("Starting Echo TCP Server failed.");
                Environment.Exit(1);
            }
            
            if (echoUdpServer.Start())
            {
                _logger.LogInformation("Echo UDP Server started.");
            }
            else
            {
                _logger.LogCritical("Starting Echo UDP Server failed.");
                Environment.Exit(1);
            }

            Unix.DropRoot(_options.User, _logger);

            _logger.LogInformation("EchoSevenService initialized at: {time}", DateTimeOffset.Now);
            
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("EchoSevenService heartbeat at: {time}", DateTimeOffset.Now);
                await Task.Delay(30000, stoppingToken);
            }

            if (echoUdpServer.Stop())
            {
                _logger.LogInformation("Echo UDP Server stopped successfully.");
            }
            else
            {
                _logger.LogError("Error stopping Echo UDP Server.");
            }

            if (echoTcpServer.Stop())
            {
                _logger.LogInformation("Echo TCP Server stopped successfully.");
            }
            else
            {
                _logger.LogError("Error stopping Echo TCP Server.");
            }
        }
    }

    public class EchoServerOptions
    {
        public int Port { get; set; }
        public string IPAddress { get; set; }
        public string User { get; set; }
    }
}