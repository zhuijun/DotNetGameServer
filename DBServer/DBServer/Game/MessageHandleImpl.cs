using DBServer.Data;
using DBServer.Interfaces;
using Google.Protobuf;
using Mail;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#nullable enable
namespace DBServer.Game
{
    public class MessageHandleImpl : IMessageHandle
    {
        private readonly DBServerContext _context;
        private readonly Func<ForwardMailMessage, Func<MailboxMessage, Task>, Task>? _Handles;


        public MessageHandleImpl(DBServerContext context)
        {
            _context = context;
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
            if (forwardMail.Id == (int)GameDBProto.MessageId.EnterRoleRequestId)
            {
                var request = GameDBProto.EnterRoleRequest.Parser.ParseFrom(forwardMail.Content);
                var role = await _context.GameRole.AsNoTracking().FirstOrDefaultAsync(r => r.UserId == request.UserId);
                if (role == null)
                {
                    var r = await _context.GameRole.AddAsync(new Models.GameRole { UserId = request.UserId, NickName = request.NickName });
                    await _context.SaveChangesAsync();
                    role = r.Entity;
                }

                var replay = new GameDBProto.EnterRoleReply { Result = new GameDBProto.ReplayResult { ErrorCode = 1 }, RoleId = role.RoleId,  NickName = role.NickName};
                await replyMailAction(new MailboxMessage { Id = (int)GameDBProto.MessageId.EnterRoleReplyId, Content = replay.ToByteString(), Reserve = forwardMail.Reserve });
            }
        }
    }
}
