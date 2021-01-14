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
        private readonly MailQueueRepository _messageQueueRepository;

        public MainHostedService(IServiceProvider services, ILogger<MainHostedService> logger, 
            MailQueueRepository messageQueueRepository)
        {
            _services = services;
            _logger = logger;
            _messageQueueRepository = messageQueueRepository;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            Task.Run(() => MainLoop(stoppingToken), stoppingToken);

            return Task.CompletedTask;
        }

        private void MainLoop(CancellationToken stoppingToken)
        {
            Stopwatch sw = new Stopwatch();

            while (!stoppingToken.IsCancellationRequested)
            {
                if (sw.ElapsedTicks < 25)
                {
                    Thread.Sleep(TimeSpan.FromTicks(sw.ElapsedTicks - 25));
                }
                sw.Reset();
                sw.Start();

                var incomeMailQueue = _messageQueueRepository.GetIncomeMailQueue();
                if (incomeMailQueue.TryReadMail(out var mail))
                {
                    var outgoMailQueue = _messageQueueRepository.GetOutgoMailQueue(mail.ClientId);
                    outgoMailQueue.TryWriteMail(mail);
                }
            }

            sw.Stop();
            _logger.LogInformation("MainLoop is stopping.");
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            return Task.CompletedTask;
        }

        public void Dispose()
        {

        }
    }
}
