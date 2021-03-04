using GameServer.Common;
using GameServer.Interfaces;
using GameServer.Services;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Game
{
    public partial class RoleManager : IAgentMail
    {
        public void OnAgentMail(MailPacket mail)
        {
            //_logger.LogDebug(mail.ToString());

            switch (mail.Id)
            {
                case (int)ClientServerProto.MessageId.CtoSenterRoleRequestId:
                    OnEnterRoleRequest(mail);
                    break;
                case (int)ClientServerProto.MessageId.CtoSroleInfoRequestId:
                    OnRoleInfoRequest(mail);
                    break;
                default:
                    break;
            }
        }

        public void OnJoinGame(AgentGameProto.JoinGameRequest request, long clientId)
        {
        }

        public void OnLeaveGame(AgentGameProto.LeaveGameRequest request, long clientId)
        {
            if (_clientRoleDict.TryGetValue(clientId, out var roleId))
            {
                Items.Remove(roleId);
                _clientRoleDict.Remove(clientId);
            }
        }

        public void NoticeRoleInfo(long roleId)
        {
            var role = GetItem(roleId);
            if (role != null)
            {
                var stoc = new ClientServerProto.StoCRoleInfoReply { RoleId = role.RoleId, NickName = role.NickName };
                Dispatcher.WriteAgentMail(new MailPacket { 
                    Id = (int)ClientServerProto.MessageId.StoCroleInfoReplyId, 
                    Content = stoc.ToByteArray(),
                    UserId = role.UserId,
                    ClientId = role.ClientId
                });
            }
        }

        public void NoticeRoleAttribute(long roleId, ClientServerProto.StoCRoleAttributeNotice attributeNotice)
        {
            var role = GetItem(roleId);
            if (role != null)
            {
                Dispatcher.WriteAgentMail(new MailPacket
                {
                    Id = (int)ClientServerProto.MessageId.StoCroleAttributeNoticeId,
                    Content = attributeNotice.ToByteArray(),
                    UserId = role.UserId,
                    ClientId = role.ClientId
                });
            }
        }

        private void OnEnterRoleRequest(MailPacket mail)
        {
            //var request = ClientServerProto.CtoSEnterRoleRequest.Parser.ParseFrom(mail.Content);
            var user = ManagerMediator.UserManager.GetItem(mail.UserId);

            if (!_clientRoleDict.TryGetValue(mail.ClientId, out var roleId))
            {
                var dbRequest = new GameDBProto.EnterRoleRequest { UserId = mail.UserId, NickName = user.NickName };
                var dbMail = new MailPacket
                {
                    Id = (int)GameDBProto.MessageId.EnterRoleRequestId,
                    Content = dbRequest.ToByteArray(),
                    Reserve = mail.Reserve,
                    UserId = mail.UserId,
                    ClientId = mail.ClientId
                };
                Dispatcher.WriteDBMail(dbMail, DBMailQueueType.Role);
            }
            else
            {
                var stoc = new ClientServerProto.StoCEnterRoleReply { Result = new ClientServerProto.ReplayResult { ErrorCode = 2, ErrorInfo = "已进入角色" }, RoleId = roleId, NickName = user.NickName };
                Dispatcher.WriteAgentMail(new MailPacket { 
                    Id = (int)ClientServerProto.MessageId.StoCenterRoleReplyId, 
                    Content = stoc.ToByteArray(), 
                    Reserve = mail.Reserve, 
                    UserId = mail.UserId, 
                    ClientId = mail.ClientId 
                });
            }
        }

        private void OnRoleInfoRequest(MailPacket mail)
        {
            var role = GetRoleByClientId(mail.ClientId);
            if (role != null)
            {
                var stoc = new ClientServerProto.StoCRoleInfoReply { RoleId = role.RoleId, NickName = role.NickName };
                Dispatcher.WriteAgentMail(new MailPacket
                {
                    Id = (int)ClientServerProto.MessageId.StoCroleInfoReplyId,
                    Content = stoc.ToByteArray(),
                    Reserve = mail.Reserve,
                    UserId = mail.UserId,
                    ClientId = mail.ClientId
                });
            }
        }
    }
}
