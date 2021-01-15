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
        public event Action<MailMessage>? EventAgentMail;
        public event Action<MailMessage>? EventDBMail;

        public void OnAgentMail(MailMessage mail)
        {
            EventAgentMail?.Invoke(mail);
        }

        public void OnDBMail(MailMessage mail)
        {
            EventDBMail?.Invoke(mail);
        }
    }
}
