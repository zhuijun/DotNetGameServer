using DBServer.Interfaces;
using Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DBServer.Game
{
    public class MessageHandleImpl : IMessageHandle
    {
        public async Task HandleMessage(ForwardMailMessage forwardMail, Func<MailboxMessage, Task> replyMailAction)
        {

            await replyMailAction(new MailboxMessage { Id = forwardMail.Id, Content = forwardMail.Content });
        }
    }
}
