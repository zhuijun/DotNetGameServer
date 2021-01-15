using GameServer.Common;
using GameServer.Game;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#nullable enable
namespace GameServer.Services
{
    public class MailDispatcher
    {
        private readonly ILogger<MailDispatcher> _logger;
        private readonly ManagerMediator _managerMediator;


        public MailDispatcher(ILogger<MailDispatcher> logger,
            ManagerMediator managerMediator)
        {
            _logger = logger;
            _managerMediator = managerMediator;
        }

        public void OnAgentMail(MailMessage mail)
        {
            _managerMediator.OnAgentMail(mail);
        }
    }
}
