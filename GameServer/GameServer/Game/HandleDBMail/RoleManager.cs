using GameServer.Common;
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
                case (int)GameDBProto.MessageID.EnterRoleReplyId:
                    OnEnterRoleReply(mail);
                    break;
                default:
                    break;
            }
        }

        private void OnEnterRoleReply(MailPacket mail)
        {
            var replay = GameDBProto.EnterRoleReply.Parser.ParseFrom(mail.Content);
            var stoc = new ClientServerProto.StoCEnterRoleReply {Result =  new ClientServerProto.StoCReplayResult { ErrorCode = replay.Result.ErrorCode, ErrorInfo = replay.Result.ErrorInfo }, RoleID = replay.RoleID };
            Dispatcher.WriteAgentMail(new MailPacket { Id = (int)ClientServerProto.MessageID.StoCenterRoleReplyId, Content = stoc.ToByteArray(), Reserve = mail.Reserve, UserId = mail.UserId, ClientId = mail.ClientId });
        }
    }
}
