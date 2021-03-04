using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using GameServer.Common;
using Google.Protobuf;

#nullable enable
namespace GameServer.Services
{
    public class Dispatcher
    {
        private readonly AgentMailQueueRepository _agentMailQueueRepository;
        private readonly DBMailQueueRepository _dbMailQueueRepository;
        private readonly MailDispatcher _mailDispatcher;
        private readonly AgentClientIdProvider _agentClientIdProvider;
        private readonly IncomeMailQueue _innerMailQueue = new IncomeMailQueue();
        private readonly ConcurrentQueue<Action> _performAtNextLoop = new ConcurrentQueue<Action>();

        public TicksProvider TicksProvider { get; }
        public QuickTimer QuickTimer { get; }


        public Dispatcher(AgentMailQueueRepository agentMailQueueRepository,
            DBMailQueueRepository dbMailQueueRepository,
            MailDispatcher mailDispatcher,
            AgentClientIdProvider agentClientIdProvider,
            TicksProvider ticksProvider,
            QuickTimer quickTimer)
        {
            _agentMailQueueRepository = agentMailQueueRepository;
            _dbMailQueueRepository = dbMailQueueRepository;
            _mailDispatcher = mailDispatcher;
            _agentClientIdProvider = agentClientIdProvider;
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

                while (TryReadInnerMail(out var mail))
                {
                    _mailDispatcher.OnInnerMail(mail);
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
            var outgoMailQueue = _agentMailQueueRepository.TryGetOutgoMailQueue(mail.ClientId);
            if (outgoMailQueue != null)
            {
                return outgoMailQueue.TryWriteMail(mail);
            }
            return false;
        }

        public bool WriteAgentMail(int id, byte[] content, long reserve, long userId)
        {
            var clientId = _agentClientIdProvider.GetUserClientId(userId);
            var mail = new MailPacket { Id = id, Content = content, Reserve = reserve, UserId = userId, ClientId = clientId };
            return WriteAgentMail(mail);
        }

        public bool WriteAgentMail(int id, IMessage message, long reserve, long userId)
        {
            var clientId = _agentClientIdProvider.GetUserClientId(userId);
            var mail = new MailPacket { Id = id, Content = message.ToByteArray(), Reserve = reserve, UserId = userId, ClientId = clientId };
            return WriteAgentMail(mail);
        }

        private bool TryReadDBMail([NotNullWhen(true)] out MailPacket? mail)
        {
            var incomeMailQueue = _dbMailQueueRepository.GetIncomeMailQueue();
            return incomeMailQueue.TryReadMail(out mail);
        }

        public bool WriteDBMail(MailPacket mail, DBMailQueueType type)
        {
            var outgoMailQueue = _dbMailQueueRepository.GetOrAddOutgoMailQueue(type);
            return outgoMailQueue.TryWriteMail(mail);
        }

        public void PerformAtNextLoop(Action action)
        {
            _performAtNextLoop.Enqueue(action);
        }

        public bool WriteInnerMail(MailPacket mail)
        {
            return _innerMailQueue.TryWrite(mail);
        }

        private bool TryReadInnerMail([NotNullWhen(true)] out MailPacket? mail)
        {
            return _innerMailQueue.TryReadMail(out mail);
        }
    }
}
