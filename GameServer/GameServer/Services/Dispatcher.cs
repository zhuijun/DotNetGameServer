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
        private readonly AgentMailQueueRepository _agentMailQueueRepository;
        private readonly DBMailQueueRepository _dbMailQueueRepository;
        private readonly MailDispatcher _mailDispatcher;

        private readonly ConcurrentQueue<Action> _performAtNextLoop = new ConcurrentQueue<Action>();

        public TicksProvider TicksProvider { get; }
        public QuickTimer QuickTimer { get; }


        public Dispatcher(AgentMailQueueRepository agentMailQueueRepository,
            DBMailQueueRepository dbMailQueueRepository,
            MailDispatcher mailDispatcher,
            TicksProvider ticksProvider,
            QuickTimer quickTimer)
        {
            _agentMailQueueRepository = agentMailQueueRepository;
            _dbMailQueueRepository = dbMailQueueRepository;
            _mailDispatcher = mailDispatcher;
            TicksProvider = ticksProvider;
            QuickTimer = quickTimer;
        }

        public void Dispatch(CancellationToken stoppingToken)
        {

            TicksProvider.DateTimeCache = DateTime.Now;

            while (!stoppingToken.IsCancellationRequested)
            {
                var elapsed = DateTime.Now - TicksProvider.DateTimeCache;
                if (elapsed < TimeSpan.FromMilliseconds(25))
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(25) - elapsed);
                }
                TicksProvider.DateTimeCache = DateTime.Now;

                while (_performAtNextLoop.TryDequeue(out var action))
                {
                    action();
                }

                QuickTimer.Update();

                while (TryReadAgentMail(out var mail))
                {
                    _mailDispatcher.OnAgentMail(mail);
                }

                while (TryReadDBMail(out var mail))
                {
                    _mailDispatcher.OnDBMail(mail);
                }
            }
        }

        private bool TryReadAgentMail([NotNullWhen(true)] out MailPacket? mail)
        {
            var incomeMailQueue = _agentMailQueueRepository.GetIncomeMailQueue();
            return incomeMailQueue.TryReadMail(out mail);
        }

        public bool WriteAgentMail(MailPacket mail)
        {
            var outgoMailQueue = _agentMailQueueRepository.GetOutgoMailQueue(mail.ClientId);
            return outgoMailQueue.TryWriteMail(mail);
        }

        private bool TryReadDBMail([NotNullWhen(true)] out MailPacket? mail)
        {
            var incomeMailQueue = _dbMailQueueRepository.GetIncomeMailQueue();
            return incomeMailQueue.TryReadMail(out mail);
        }

        public bool WriteDBMail(MailPacket mail, DBMailQueueType type)
        {
            var outgoMailQueue = _dbMailQueueRepository.GetOutgoMailQueue(type);
            return outgoMailQueue.TryWriteMail(mail);
        }

        public void PerformAtNextLoop(Action action)
        {
            _performAtNextLoop.Enqueue(action);
        }

    }
}
