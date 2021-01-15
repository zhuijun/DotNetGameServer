﻿using GameServer.Common;
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
        private readonly List<AbstractManager> _managers = new List<AbstractManager>();

        public Dispatcher Dispatcher { get; }
        public RoleManager RoleManager { get; }

        public ManagerMediator(Dispatcher dispatcher, 
            RoleManager roleManager)
        {
            Dispatcher = dispatcher;
            RoleManager = roleManager;
            AddManager(RoleManager);
        }

        private bool AddManager<T>(T manager) where T : AbstractManager
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

        public void OnAgentMail(MailMessage mail)
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

        public void OnDBMail(MailMessage mail)
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
