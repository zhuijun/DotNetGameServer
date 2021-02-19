using GameServer.Common;
using GameServer.Interfaces;
using GameServer.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Game
{
    public class ManagerMediator
    {
        private readonly List<IManager> _managers = new List<IManager>();

        public Dispatcher Dispatcher { get; }
        public RoleManager RoleManager { get; }
        public UserManager UserManager { get; }

        public ManagerMediator(Dispatcher dispatcher, 
            RoleManager roleManager,
            UserManager userManager)
        {
            Dispatcher = dispatcher;
            RoleManager = roleManager;
            AddManager(RoleManager);
            UserManager = userManager;
            AddManager(UserManager);
        }

        private bool AddManager<T>(T manager) where T : IManager, IDisposable
        {
            if (_managers.Contains(manager))
            {
                return false;
            }
            manager.ManagerMediator = this;
            manager.Dispatcher = Dispatcher;
            _managers.Add(manager);
            return true;
        }

        public void ForEachMananger(Action<IManager> action)
        {
            foreach (var manager in _managers)
            {
                action(manager);
            }
        }

        public void OnAgentMail(MailPacket mail)
        {
            void doAction(IManager manager)
            {
                if (manager is IAgentMail agentMail)
                {
                    agentMail.OnAgentMailEx(mail);
                }
            }
            ForEachMananger(doAction);
        }

        public void OnDBMail(MailPacket mail)
        {
            void doAction(IManager manager)
            {
                if (manager is IDBMail dbMail)
                {
                    dbMail.OnDBMail(mail);
                }
            }
            ForEachMananger(doAction);
        }
    }
}
