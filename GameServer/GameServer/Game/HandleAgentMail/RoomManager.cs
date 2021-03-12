using AgentGameProto;
using ServicesCore.Common;
using GameServer.Interfaces;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Game
{
    public partial class RoomManager : IAgentMail, IInnerMail
    {
        public void OnAgentMail(MailPacket mail)
        {
            switch (mail.Id)
            {
                case (int)ClientServerProto.MessageId.CtoSjoinRoomRequestId:
                    OnJoinRoomRequest(mail);
                    break;
                case (int)ClientServerProto.MessageId.CtoSleaveRoomRequestId:
                    OnLeaveRoomRequest(mail);
                    break;
                case (int)ClientServerProto.MessageId.CtoScreateDeskRequestId:
                    OnCreateDeskRequest(mail);
                    break;
                case (int)ClientServerProto.MessageId.CtoSjoinDeskRequestId:
                    OnJoinDeskRequest(mail);
                    break;
                case (int)ClientServerProto.MessageId.CtoSleaveDeskRequestId:
                    OnLeaveDeskRequest(mail);
                    break;
                default:
                    break;
            }

            var roleId = ManagerMediator.RoleManager.GetRoleIdByClientId(mail.ClientId);
            if (roleId > 0)
            {
                var deskId = _room.GetRoleDesk(roleId);
                if (deskId > 0)
                {
                    var desk = _room.GetDesk(deskId);
                    if (desk != null)
                    {
                        if (desk.GameLogic is IAgentMail agentMail)
                        {
                            agentMail.OnAgentMail(mail);
                        }
                    }
                }
            }
        }

        public void OnJoinGame(JoinGameRequest request, long clientId)
        {
            var roleId = ManagerMediator.RoleManager.GetRoleIdByClientId(clientId);
            var desk = _room.GetDesk(_room.GetRoleDesk(roleId));
            if (desk != null)
            {
                if (desk.GameLogic is IInnerMail innerMail)
                {
                    innerMail.OnJoinGame(request, clientId);
                }
            }
        }

        public void OnLeaveGame(LeaveGameRequest request, long clientId)
        {
            var roleId = ManagerMediator.RoleManager.GetRoleIdByClientId(clientId);
            var desk = _room.GetDesk(_room.GetRoleDesk(roleId));
            if (desk != null)
            {
                if (desk.GameLogic is IInnerMail innerMail)
                {
                    innerMail.OnLeaveGame(request, clientId);
                }
                desk.RemoveRole(roleId);
                if (desk.RoleCount() == 0)
                {
                    _room.RemoveDesk(desk.DeskId);
                }
                _room.RemoveRoleDesk(roleId);
            }
        }

        public void BeforeLeaveGame(BeforeLeaveGameRequest request, long clientId)
        {
            var roleId = ManagerMediator.RoleManager.GetRoleIdByClientId(clientId);
            var desk = _room.GetDesk(_room.GetRoleDesk(roleId));
            if (desk != null)
            {
                if (desk.GameLogic is IInnerMail innerMail)
                {
                    innerMail.BeforeLeaveGame(request, clientId);
                }
            }
        }

        private void OnJoinRoomRequest(MailPacket mail)
        {
            var stoc = new ClientServerProto.StoCJoinRoomReply();
            var roleId = ManagerMediator.RoleManager.GetRoleIdByClientId(mail.ClientId);
            var deskId = _room.GetRoleDesk(roleId);
            if (deskId == 0)
            {
                deskId = CreateRoleDesk(roleId, _room.GameType, _room.MaxRoleCount);
            }

            if (!(deskId > 0))
            {
                stoc.Result = new ClientServerProto.ReplayResult { ErrorCode = 1, ErrorInfo = "cannot create desk" };
            }

            Dispatcher.WriteAgentMail(new MailPacket
            {
                Id = (int)ClientServerProto.MessageId.StoCjoinRoomReplyId,
                Content = stoc.ToByteArray(),
                Reserve = mail.Reserve,
                UserId = mail.UserId,
                ClientId = mail.ClientId
            });
        }

        private void OnLeaveRoomRequest(MailPacket mail)
        {
            PostLeaveGameMail(mail.UserId, mail.ClientId);

            var stoc = new ClientServerProto.StoCLeaveRoomReply();
            Dispatcher.WriteAgentMail(new MailPacket
            {
                Id = (int)ClientServerProto.MessageId.StoCleaveRoomReplyId,
                Content = stoc.ToByteArray(),
                Reserve = mail.Reserve,
                UserId = mail.UserId,
                ClientId = mail.ClientId
            });
        }

        private void PostLeaveGameMail(long userId, long clientId)
        {
            //退出游戏
            var mail1 = new MailPacket
            {
                Id = (int)AgentGameProto.MessageId.BeforeLeaveGameRequestId,
                Content = new AgentGameProto.BeforeLeaveGameRequest
                {
                    UserId = userId
                }.ToByteArray(),
                ClientId = clientId,
                UserId = userId
            };
            Dispatcher.WriteInnerMail(mail1);

            var mail2 = new MailPacket
            {
                Id = (int)AgentGameProto.MessageId.LeaveGameRequestId,
                Content = new AgentGameProto.LeaveGameRequest
                {
                    UserId = userId
                }.ToByteArray(),
                ClientId = clientId,
                UserId = userId
            };
            Dispatcher.WriteInnerMail(mail2);
        }

        private void OnCreateDeskRequest(MailPacket mail)
        {
            var stoc = new ClientServerProto.StoCCreateDeskReply();
            var roleId = ManagerMediator.RoleManager.GetRoleIdByClientId(mail.ClientId);
            var deskId = _room.GetRoleDesk(roleId);
            if (deskId == 0)
            {
                deskId = CreateRoleDesk(roleId, _room.GameType, _room.MaxRoleCount);

                stoc.DeskId = deskId;
            }
            else
            {
                stoc.Result = new ClientServerProto.ReplayResult { ErrorCode = 1, ErrorInfo = "already on desk" };
                stoc.DeskId = deskId;
            }
            Dispatcher.WriteAgentMail(new MailPacket
            {
                Id = (int)ClientServerProto.MessageId.StoCcreateDeskReplyId,
                Content = stoc.ToByteArray(),
                Reserve = mail.Reserve,
                UserId = mail.UserId,
                ClientId = mail.ClientId
            });
        }

        private long CreateRoleDesk(long roleId, GameType gameType, int maxRoleCount)
        {
            long deskId;
            var game = _gameFactory.CreateGame(gameType, ManagerMediator);
            var desk = _room.CreateDesk(game, maxRoleCount);
            deskId = desk.DeskId;
            desk.AddRole(roleId);
            _room.AddRoleDesk(roleId, deskId);
            return deskId;
        }

        private void OnJoinDeskRequest(MailPacket mail)
        {
            var stoc = new ClientServerProto.StoCJoinDeskReply();

            var roleId = ManagerMediator.RoleManager.GetRoleIdByClientId(mail.ClientId);
            var ctos = ClientServerProto.CtoSJoinDeskRequest.Parser.ParseFrom(mail.Content);
            var deskId = ctos.DeskId;
            var desk = _room.GetDesk(deskId);
            if (desk != null)
            {
                if (desk.MaxRoleCount < desk.Roles.Count)
                {
                    desk.AddRole(roleId);
                    _room.AddRoleDesk(roleId, deskId);
                }
                else
                {
                    stoc.Result = new ClientServerProto.ReplayResult { ErrorCode = 2, ErrorInfo = "无空位" };
                }
            }
            else
            {
                stoc.Result = new ClientServerProto.ReplayResult { ErrorCode = 1, ErrorInfo = "未找到空位" };
            }

            Dispatcher.WriteAgentMail(new MailPacket
            {
                Id = (int)ClientServerProto.MessageId.StoCjoinDeskReplyId,
                Content = stoc.ToByteArray(),
                Reserve = mail.Reserve,
                UserId = mail.UserId,
                ClientId = mail.ClientId
            });
        }

        private void OnLeaveDeskRequest(MailPacket mail)
        {
            var stoc = new ClientServerProto.StoCLeaveDeskReply();

            var roleId = ManagerMediator.RoleManager.GetRoleIdByClientId(mail.ClientId);
            var deskId = _room.GetRoleDesk(roleId);
            if (deskId != 0)
            {
                var desk = _room.GetDesk(deskId);
                if (desk != null)
                {
                    if (desk.GameLogic is IInnerMail innerMail)
                    {
                        var request = new LeaveGameRequest { UserId = mail.UserId };
                        innerMail.OnLeaveGame(request, mail.ClientId);
                    }
                    desk.RemoveRole(roleId);
                    if (desk.RoleCount() == 0)
                    {
                        _room.RemoveDesk(desk.DeskId);
                    }
                    _room.RemoveRoleDesk(roleId);
                }
            }
            else
            {
                stoc.Result = new ClientServerProto.ReplayResult { ErrorCode = 1, ErrorInfo = "不在座位上" };
            }

            Dispatcher.WriteAgentMail(new MailPacket
            {
                Id = (int)ClientServerProto.MessageId.StoCleaveDeskReplyId,
                Content = stoc.ToByteArray(),
                Reserve = mail.Reserve,
                UserId = mail.UserId,
                ClientId = mail.ClientId
            });
        }
    }
}
