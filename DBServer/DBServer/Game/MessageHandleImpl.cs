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
            _Handles += OnSaveDropBoxRequest;
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
                    var r = _context.GameRole.Add(new GameRole { UserId = request.UserId, NickName = request.NickName });
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
                var fruitConfigProto = new WatermelonConfigProto.FruitConfig();
                var fruitConfigQuery = _context.FruitConfig.AsNoTracking().OrderBy(s => s.FruitId);
                foreach (var item in fruitConfigQuery)
                {
                    fruitConfigProto.Items.Add(item.FruitId, new WatermelonConfigProto.Fruit { Id = item.FruitId, Rate = item.Rate, Image = item.Image, Name = item.Name, 
                        Score = item.Score, CombineFruitId = item.CombineFruitId });
                }

                var truntableConfigProto = new WatermelonConfigProto.TruntableConfig();
                var truntableConfigQuery = _context.TruntableConfig.AsNoTracking();
                foreach (var item in truntableConfigQuery)
                {
                    truntableConfigProto.Items.Add(item.Id, new WatermelonConfigProto.TruntableItem { Id = item.Id, AwardDesc = item.AwardDesc, ImagePath = item.ImagePath, Price = item.Price });
                }


                var replay = new GameDBProto.LoadConfigReply { 
                    FruitConfig = Google.Protobuf.WellKnownTypes.Any.Pack(fruitConfigProto),
                    TruntableConfig = Google.Protobuf.WellKnownTypes.Any.Pack(truntableConfigProto),
                };

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
                    _context.Add(roleScore);
                    await _context.SaveChangesAsync();
                }
            }
        }

        private async Task OnSaveDropBoxRequest(ForwardMailMessage forwardMail, Func<MailboxMessage, Task> replyMailAction)
        {
            if (forwardMail.Id == (int)GameDBProto.MessageId.SaveDropBoxRequestId)
            {
                var request = GameDBProto.SaveDropBoxRequest.Parser.ParseFrom(forwardMail.Content);
                var roleBox = new GameBox { Id = request.BoxId, RoleId = request.RoleId, Amount = request.Amount, CouponsId = request.CouponsId, CreateTime = DateTime.Now, UpateTime = DateTime.Now };
                _context.Add(roleBox);
                await _context.SaveChangesAsync();
            }
         }
     }
}
