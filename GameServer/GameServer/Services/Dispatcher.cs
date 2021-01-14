using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

#nullable enable
namespace GameServer.Services
{
    public class Dispatcher
    {
        private readonly MailQueueRepository _messageQueueRepository;
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
        

        public Dispatcher(MailQueueRepository messageQueueRepository)
        {
            _messageQueueRepository = messageQueueRepository;
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

                while (TryReadMail(out var mail))
                {
                    TryWriteMail(mail);
                }
            }
        }

        public bool TryReadMail([NotNullWhen(true)] out Mail? mail)
        {
            var incomeMailQueue = _messageQueueRepository.GetIncomeMailQueue();
            return incomeMailQueue.TryReadMail(out mail);
        }

        public bool TryWriteMail(Mail mail)
        {
            var outgoMailQueue = _messageQueueRepository.GetOutgoMailQueue(mail.ClientId);
            return outgoMailQueue.TryWriteMail(mail);
        }

        public void PerformAtNextLoop(Action action)
        {
            _performAtNextLoop.Enqueue(action);
        }

    }
}
