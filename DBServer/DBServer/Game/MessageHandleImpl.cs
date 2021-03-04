using DBServer.Interfaces;
using Google.Protobuf;
using Mail;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#nullable enable
namespace DBServer.Game
{
    public class MessageHandleImpl : IMessageHandle
    {
        private readonly GameDbContext _context;
        private readonly Func<ForwardMailMessage, Func<MailboxMessage, Task>, Task>? _Handles;


        public MessageHandleImpl(GameDbContext context)
        {
            _context = context;
            _Handles += OnEnterRole;
            _Handles += OnLoadConfigRequest;
            _Handles += OnSaveRoleScoreRequest;
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
                    var r = await _context.GameRole.AddAsync(new GameRole { UserId = request.UserId, NickName = request.NickName });
                    await _context.SaveChangesAsync();
                    role = r.Entity;
                }

                var replay = new GameDBProto.EnterRoleReply { Result = new GameDBProto.ReplayResult { ErrorCode = 1 }, RoleId = role.RoleId,  NickName = role.NickName};
                await replyMailAction(new MailboxMessage { Id = (int)GameDBProto.MessageId.EnterRoleReplyId, Content = replay.ToByteString(), 
                    Reserve = forwardMail.Reserve, UserId = forwardMail.UserId, ClientId = forwardMail.ClientId });
            }
        }

        private async Task OnLoadConfigRequest(ForwardMailMessage forwardMail, Func<MailboxMessage, Task> replyMailAction)
        {
            if (forwardMail.Id == (int)GameDBProto.MessageId.LoadConfigRequestId)
            {
                var fruitConfig = await _context.FruitConfig.AsNoTracking().OrderBy(s => s.FruitId).ToListAsync();

                var fruitConfigProto = new WatermelonConfigProto.FruitConfig();
                foreach (var item in fruitConfig)
                {
                    fruitConfigProto.Items.Add(item.FruitId, new WatermelonConfigProto.Fruit { Id = item.FruitId, Rate = item.Rate, Image = item.Image, Name = item.Name, 
                        Score = item.Score, CombineFruitId = item.CombineFruitId });
                }

                var replay = new GameDBProto.LoadConfigReply { FruitConfig = Google.Protobuf.WellKnownTypes.Any.Pack(fruitConfigProto) };
                await replyMailAction(new MailboxMessage
                {
                    Id = (int)GameDBProto.MessageId.LoadConfigReplyId,
                    Content = replay.ToByteString(),
                    Reserve = forwardMail.Reserve,
                    UserId = forwardMail.UserId,
                    ClientId = forwardMail.ClientId
                });
            }
        }

        private async Task OnSaveRoleScoreRequest(ForwardMailMessage forwardMail, Func<MailboxMessage, Task> replyMailAction)
        {
            if (forwardMail.Id == (int)GameDBProto.MessageId.SaveRoleScoreRequestId)
            {
                var request = GameDBProto.SaveRoleScoreRequest.Parser.ParseFrom(forwardMail.Content);
                var roleScore = await _context.GameScore.AsNoTracking().FirstOrDefaultAsync(s => s.RoleId == request.RoleId);
                if (roleScore != null)
                {
                    if (roleScore.Score < request.Score)
                    {
                        roleScore.Score = request.Score;
                        roleScore.UpateTime = DateTime.Now;
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    roleScore = new GameScore { RoleId = request.RoleId, Score = request.Score, CreateTime = DateTime.Now, UpateTime = DateTime.Now };
                    await _context.AddAsync(roleScore);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
