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
        public event Action<MailPacket>? EventAgentMail;
        public event Action<MailPacket>? EventDBMail;

        public void OnAgentMail(MailPacket mail)
        {
            EventAgentMail?.Invoke(mail);
        }

        public void OnDBMail(MailPacket mail)
        {
            EventDBMail?.Invoke(mail);
        }
    }
}
