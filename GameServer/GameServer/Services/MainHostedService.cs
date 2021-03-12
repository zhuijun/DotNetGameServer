using ServicesCore.Common;
using GameServer.Game;
using Google.Protobuf;
using Grpc.Core;
using Mail;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServicesCore.Services;

namespace GameServer.Services
{
    public class MainHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<MainHostedService> _logger;
        private readonly Dispatcher _dispatcher;
        private readonly MailDispatcher _mailDispatcher;
        private readonly ManagerMediator _managerMediator;
        private readonly DBGrpcChannel _dbGrpcChannel;
        private readonly DBMailQueueRepository _dbMailQueueRepository;

        private readonly List<DBMailCallProxy> _dbMailCallProxies = new List<DBMailCallProxy>();

        public MainHostedService(ILogger<MainHostedService> logger,
            Dispatcher dispatcher,
            MailDispatcher mailDispatcher,
            ManagerMediator managerMediator,
            DBGrpcChannel dbGrpcChannel,
            DBMailQueueRepository dbMailQueueRepository)
        {
            _logger = logger;
            _dispatcher = dispatcher;
            _mailDispatcher = mailDispatcher;
            _managerMediator = managerMediator;
            _dbGrpcChannel = dbGrpcChannel;
            _dbMailQueueRepository = dbMailQueueRepository;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("MainHostedService running.");

            InitDBMailQueue(stoppingToken);

            Task.Run(() => MainLoop(stoppingToken), stoppingToken);

            return Task.CompletedTask;
        }

        private void InitDBMailQueue(CancellationToken stoppingToken)
        {
            var _client = new Mailer.MailerClient(_dbGrpcChannel.Channel);
            foreach (var type in Enum.GetValues<DBMailQueueType>())
            {
                var proxy = new DBMailCallProxy(_client, _dbMailQueueRepository, type, _logger);
                proxy.Start();
                _dbMailCallProxies.Add(proxy);
            }
        }

        private void MainLoop(CancellationToken stoppingToken)
        {
            _mailDispatcher.EventAgentMail += _managerMediator.OnAgentMail;
            _mailDispatcher.EventDBMail += _managerMediator.OnDBMail;
            _mailDispatcher.EventInnerMail += _managerMediator.OnInnerMail;

            _managerMediator.OnStartUp();

            _dispatcher.Dispatch(stoppingToken);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("MainHostedService is stopping.");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _mailDispatcher.EventAgentMail -= _managerMediator.OnAgentMail;
                _mailDispatcher.EventDBMail -= _managerMediator.OnDBMail;
            }
        }
    }
}
