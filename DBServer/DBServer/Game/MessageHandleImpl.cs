using DBServer.Interfaces;
using Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#nullable enable
namespace DBServer.Game
{
    public class MessageHandleImpl : IMessageHandle
    {
        private readonly Func<ForwardMailMessage, Func<MailboxMessage, Task>, Task>? _Handles;

        public MessageHandleImpl()
        {
            _Handles += OnEnterRole;
        }


        public async Task HandleMessage(ForwardMailMessage forwardMail, Func<MailboxMessage, Task> replyMailAction)
        {
            if (_Handles != null)
            {
                await _Handles.Invoke(forwardMail, replyMailAction);
            }
        }

        private async Task OnEnterRole(ForwardMailMessage forwardMail, Func<MailboxMessage, Task> replyMailAction)
        {
            if (forwardMail.Id == 1)
            {
                await replyMailAction(new MailboxMessage { Id = forwardMail.Id, Content = forwardMail.Content });
            }
        }
    }
}
