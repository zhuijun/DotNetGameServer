using GameServer.Common;
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

            Task.Run(() => MainLoop(stoppingToken), stoppingToken);


            foreach (var type in Enum.GetValues<DBMailQueueType>())
            {
                var _client = new Mailer.MailerClient(_dbGrpcChannel.Channel);
                var _call = _client.Mailbox(headers: new Metadata { new Metadata.Entry("mailbox-name", "game") }, cancellationToken: stoppingToken);
                var outgoMailQueue = _dbMailQueueRepository.GetOutgoMailQueue(type);
                outgoMailQueue.OnRead += WriteDBMail;

                async Task WriteDBMail(MailMessage mail)
                {
                    var forward = new ForwardMailMessage
                    {
                        Id = mail.Id,
                        Content = ByteString.CopyFrom(mail.Content)
                    };
                    await _call.RequestStream.WriteAsync(forward);
                }
            }

            return Task.CompletedTask;
        }

        private void MainLoop(CancellationToken stoppingToken)
        {
            _mailDispatcher.EventAgentMail += _managerMediator.OnAgentMail;
            _mailDispatcher.EventDBMail += _managerMediator.OnDBMail;

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
