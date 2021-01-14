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

        public event Action<Mail>? OnRead;

        public MailDispatcher(ILogger<MailDispatcher> logger)
        {
            _logger = logger;
        }

        public void OnReadMail(Mail mail)
        {
            OnRead?.Invoke(mail);
        }
    }
}
