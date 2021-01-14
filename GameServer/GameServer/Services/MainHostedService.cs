using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer.Services
{
    public class MainHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<MainHostedService> _logger;
        private readonly IServiceProvider _services;
        private readonly Dispatcher _dispatcher;

        public MainHostedService(IServiceProvider services, ILogger<MainHostedService> logger,
            Dispatcher dispatcher)
        {
            _services = services;
            _logger = logger;
            _dispatcher = dispatcher;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("MainHostedService running.");

            Task.Run(() => MainLoop(stoppingToken), stoppingToken);

            return Task.CompletedTask;
        }

        private void MainLoop(CancellationToken stoppingToken)
        {
            _dispatcher.Dispatch(stoppingToken);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("MainHostedService is stopping.");

            return Task.CompletedTask;
        }

        public void Dispose()
        {

        }
    }
}
