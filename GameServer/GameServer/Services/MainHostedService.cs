using GameServer.Game;
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
        private readonly Dispatcher _dispatcher;
        private readonly MailDispatcher _mailDispatcher;
        private readonly ManagerMediator _managerMediator;


        public MainHostedService(ILogger<MainHostedService> logger,
            Dispatcher dispatcher,
            MailDispatcher mailDispatcher,
            ManagerMediator managerMediator)
        {
            _logger = logger;
            _dispatcher = dispatcher;
            _mailDispatcher = mailDispatcher;
            _managerMediator = managerMediator;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("MainHostedService running.");

            Task.Run(() => MainLoop(stoppingToken), stoppingToken);

            return Task.CompletedTask;
        }

        private void MainLoop(CancellationToken stoppingToken)
        {
            _mailDispatcher.EventAgentMail += _managerMediator.OnAgentMail;

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
