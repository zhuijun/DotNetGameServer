using ServicesCore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#nullable enable
namespace ServicesCore.Services
{
    public class MailDispatcher
    {
        public event Action<MailPacket>? EventAgentMail;
        public event Action<MailPacket>? EventDBMail;
        public event Action<MailPacket>? EventInnerMail;

        public void OnAgentMail(MailPacket mail)
        {
            EventAgentMail?.Invoke(mail);
        }

        public void OnDBMail(MailPacket mail)
        {
            EventDBMail?.Invoke(mail);
        }

        public void OnInnerMail(MailPacket mail)
        {
            EventInnerMail?.Invoke(mail);
        }
    }
}
