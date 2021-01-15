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

        public void OnAgentMail(MailMessage mail)
        {
            EventAgentMail?.Invoke(mail);
        }
    }
}
