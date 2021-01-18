using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using GameServer.Common;

#nullable enable
namespace GameServer.Services
{
    public class Dispatcher
    {
        private readonly AgentMailQueueRepository _mailQueueRepository;
        private readonly MailDispatcher _mailDispatcher;
        private readonly ConcurrentQueue<Action> _performAtNextLoop = new ConcurrentQueue<Action>();
        private readonly DateTime _centuryBegin = new DateTime(1970, 1, 1, 8, 0, 0);

        public long TicksCache 
        {
            get { return (DateTimeCache - _centuryBegin).Ticks; }
        }
        public long TimestampCache 
        { 
            get { return TicksCache / TimeSpan.TicksPerSecond; } 
        }

        private DateTime DateTimeCache { get; set; }
        
        

        public Dispatcher(AgentMailQueueRepository mailQueueRepository
            , MailDispatcher mailDispatcher)
        {
            _mailQueueRepository = mailQueueRepository;
            _mailDispatcher = mailDispatcher;
        }

        public void Dispatch(CancellationToken stoppingToken)
        {
            
            DateTimeCache = DateTime.Now;

            while (!stoppingToken.IsCancellationRequested)
            {
                var elapsed = DateTime.Now - DateTimeCache;
                if (elapsed < TimeSpan.FromMilliseconds(25))
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(25) - elapsed);
                }
                DateTimeCache = DateTime.Now;

                while (_performAtNextLoop.TryDequeue(out var action))
                {
                    action();
                }

                while (TryReadAgentMail(out var mail))
                {
                    _mailDispatcher.OnAgentMail(mail);
                }
            }
        }

        private bool TryReadAgentMail([NotNullWhen(true)] out MailMessage? mail)
        {
            var incomeMailQueue = _mailQueueRepository.GetIncomeMailQueue();
            return incomeMailQueue.TryReadMail(out mail);
        }

        public bool WriteAgentMail(MailMessage mail)
        {
            var outgoMailQueue = _mailQueueRepository.GetOutgoMailQueue(mail.ClientId);
            return outgoMailQueue.TryWriteMail(mail);
        }

        public void PerformAtNextLoop(Action action)
        {
            _performAtNextLoop.Enqueue(action);
        }

    }
}
