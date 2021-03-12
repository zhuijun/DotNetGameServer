using ServicesCore.Common;
using GameServer.Interfaces;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Game
{
    public partial class RoleManager : IDBMail
    {
        public void OnDBMail(MailPacket mail)
        {
            switch (mail.Id)
            {
                case (int)GameDBProto.MessageId.EnterRoleReplyId:
                    DBEnterRoleReply(mail);
                    break;
                default:
                    break;
            }
        }

        private void DBEnterRoleReply(MailPacket mail)
        {
            var replay = GameDBProto.EnterRoleReply.Parser.ParseFrom(mail.Content);
            var stoc = new ClientServerProto.StoCEnterRoleReply {
                Result =  new ClientServerProto.ReplayResult { ErrorCode = replay.Result.ErrorCode, ErrorInfo = replay.Result.ErrorInfo }, 
                RoleId = replay.RoleId, 
                NickName = replay.NickName 
            };

            Dispatcher.WriteAgentMail(new MailPacket { 
                Id = (int)ClientServerProto.MessageId.StoCenterRoleReplyId, 
                Content = stoc.ToByteArray(), 
                Reserve = mail.Reserve, 
                UserId = mail.UserId, 
                ClientId = mail.ClientId 
            });

            _clientRoleDict.Add(mail.ClientId, replay.RoleId);
            var role = new Role { RoleId = replay.RoleId, NickName = replay.NickName, UserId = mail.UserId, ClientId = mail.ClientId };
            AddItem(replay.RoleId, role);
        }
    }
}
