using GameServer.Interfaces;
using ServicesCore.Common;
using ServicesCore.Services;
using System;
using System.Collections.Generic;

namespace GameServer.Game
{
    public class ManagerMediator
    {
        private readonly List<IManager> _managers = new List<IManager>();

        public Dispatcher Dispatcher { get; }
        public ConfigManager ConfigManager { get; }
        public UserManager UserManager { get; }
        public RoleManager RoleManager { get; }
        public RoomManager RoomManager { get; }

        public ManagerMediator(Dispatcher dispatcher,
            ConfigManager configManager,
            UserManager userManager,
            RoleManager roleManager,
            RoomManager roomManager)
        {
            Dispatcher = dispatcher;

            ConfigManager = configManager;
            AddManager(ConfigManager);

            UserManager = userManager;
            AddManager(UserManager);

            RoleManager = roleManager;
            AddManager(RoleManager);

            RoomManager = roomManager;
            AddManager(RoomManager);
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

        public void OnStartUp()
        {
            static void doAction(IManager manager)
            {
                if (manager is IStartUp agentMail)
                {
                    agentMail.OnStartUp();
                }
            }
            ForEachMananger(doAction);
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
                    agentMail.OnAgentMail(mail);
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

        public void OnInnerMail(MailPacket mail)
        {
            void doAction(IManager manager)
            {
                if (manager is IInnerMail innerMail)
                {
                    innerMail.OnInnerMailEx(mail);
                }
            }
            ForEachMananger(doAction);
        }
    }
}
