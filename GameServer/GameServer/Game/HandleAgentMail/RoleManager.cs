﻿using GameServer.Common;
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
        public void OnAgentMail(MailPacket mail)
        {
            _logger.LogDebug(mail.ToString());

            switch (mail.Id)
            {
                case 1:
                    var mm = new MailPacket { Id = mail.Id, Content = mail.Content, Reserve = mail.Reserve, ClientId = mail.ClientId};
                    Dispatcher.WriteAgentMail(mm);
                    Dispatcher.WriteDBMail(mail, Services.DBMailQueueType.Role);
                    break;
                default:
                    break;
            }
        }
    }
}
