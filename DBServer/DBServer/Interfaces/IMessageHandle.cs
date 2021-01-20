using Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DBServer.Interfaces
{
    public interface IMessageHandle
    {
        public Task HandleMessage(ForwardMailMessage forwardMail, Func<MailboxMessage, Task> replyMailAction);
    }
}
