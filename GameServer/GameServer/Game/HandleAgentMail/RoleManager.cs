using GameServer.Common;
using GameServer.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Game
{
    public partial class RoleManager : IAgentMail
    {
        public void OnAgentMail(MailMessage mail)
        {
            _logger.LogDebug(mail.ToString());

            switch (mail.Id)
            {
                case 1:
                    var mm = new MailMessage(mail.ClientId, 2, mail.Content);
                    Dispatcher.WriteAgentMail(mm);
                    //Dispatcher.WriteDBMail(mail, Services.DBMailQueueType.Role);
                    break;
                default:
                    break;
            }
        }
    }
}
