using AgentGameProto;
using ServicesCore.Common;
using GameServer.Interfaces;
using GameServer.Services;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServicesCore.Services;

namespace GameServer.Game
{
    public class WatermelonGame : IAgentMail, IDBMail, IInnerMail
    {
        public ManagerMediator ManagerMediator { get; }
        private Dictionary<long, long> _roleScoreDict = new Dictionary<long, long>();

        public WatermelonGame(ManagerMediator managerMediator)
        {
            ManagerMediator = managerMediator;
        }

        public void OnAgentMail(MailPacket mail)
        {
            switch (mail.Id)
            {
                case (int)WatermelonGameProto.MessageId.CtoSdropBoxRequestId:
                    AgentDropBoxRequest(mail);
                    break;
                case (int)WatermelonGameProto.MessageId.CtoScombineFruitRequestId:
                    AgentCombineFruitRequest(mail);
                    break;
                case (int)WatermelonGameProto.MessageId.CtoSgameOverRequestId:
                    AgentGameOverRequest(mail);
                    break;
                case (int)WatermelonGameProto.MessageId.CtoSgetRankRequestId:
                    AgentGetRankRequest(mail);
                    break;
                default:
                    break;
            }
        }

        public void OnDBMail(MailPacket mail)
        {
            switch (mail.Id)
            {
                case (int)GameDBProto.MessageId.GetRankReplyId:
                    DBGetRankReply(mail);
                    break;
                default:
                    break;
            }
        }

        public void OnJoinGame(JoinGameRequest request, long clientId)
        {
            //throw new NotImplementedException();
        }

        public void OnLeaveGame(LeaveGameRequest request, long clientId)
        {
            //throw new NotImplementedException();
        }

        public void BeforeLeaveGame(BeforeLeaveGameRequest request, long clientId)
        {
            var role = ManagerMediator.RoleManager.GetRoleByClientId(clientId);
            if (role != null)
            {
                if (_roleScoreDict.TryGetValue(role.RoleId, out var score))
                {
                    if (score > 0)
                    {
                        var dbRequest = new GameDBProto.SaveRoleScoreRequest { RoleId = role.RoleId, Score = score };
                        var dbMail = new MailPacket
                        {
                            Id = (int)GameDBProto.MessageId.SaveRoleScoreRequestId,
                            Content = dbRequest.ToByteArray(),
                            UserId = role.UserId,
                            ClientId = role.ClientId
                        };
                        ManagerMediator.Dispatcher.WriteDBMail(dbMail, DBMailQueueType.Role);
                    }
                    _roleScoreDict.Remove(role.RoleId);
                }
            }
        }

        private void AgentDropBoxRequest(MailPacket mail)
        {
            var role = ManagerMediator.RoleManager.GetRoleByClientId(mail.ClientId);
            if (role != null)
            {
                //TODO
                var boxId = Guid.NewGuid().ToString();
                var stoc = new WatermelonGameProto.StoCDropBoxReply { BoxId = boxId, CouponsId = "test", Amount = 1000  };
                ManagerMediator.Dispatcher.WriteAgentMail(new MailPacket
                {
                    Id = (int)WatermelonGameProto.MessageId.StoCdropBoxReplyId,
                    Content = stoc.ToByteArray(),
                    UserId = mail.UserId,
                    ClientId = mail.ClientId
                });

                var dbRequest = new GameDBProto.SaveDropBoxRequest { RoleId = role.RoleId, CouponsId = "test", Amount = 1000 };
                var dbMail = new MailPacket
                {
                    Id = (int)GameDBProto.MessageId.SaveDropBoxRequestId,
                    Content = dbRequest.ToByteArray(),
                    UserId = mail.UserId,
                    ClientId = mail.ClientId
                };
                ManagerMediator.Dispatcher.WriteDBMail(dbMail, DBMailQueueType.Role);
            }
        }
        private void AgentCombineFruitRequest(MailPacket mail)
        {
            var role = ManagerMediator.RoleManager.GetRoleByClientId(mail.ClientId);
            if (role != null)
            {
                var ctos = WatermelonGameProto.CtoSCombineFruitRequest.Parser.ParseFrom(mail.Content);
                var stoc = new WatermelonGameProto.StoCCombineFruitReply { };
                var fruit = ManagerMediator.ConfigManager.FruitConfig.Items.GetValueOrDefault(ctos.CombineFruitId);
                if (fruit != null)
                {
                    var score = _roleScoreDict.GetValueOrDefault(role.RoleId);
                    score += fruit.Score;
                    _roleScoreDict[role.RoleId] = score;

                    stoc.Score = fruit.Score;
                }

                ManagerMediator.Dispatcher.WriteAgentMail(new MailPacket
                {
                    Id = (int)WatermelonGameProto.MessageId.StoCcombineFruitReplyId,
                    Content = stoc.ToByteArray(),
                    UserId = mail.UserId,
                    ClientId = mail.ClientId
                });
            }
        }

        private void AgentGameOverRequest(MailPacket mail)
        {
            var role = ManagerMediator.RoleManager.GetRoleByClientId(mail.ClientId);
            if (role != null)
            {
                var score = _roleScoreDict.GetValueOrDefault(role.RoleId);
                _roleScoreDict[role.RoleId] = 0;
                var dbRequest = new GameDBProto.SaveRoleScoreRequest { RoleId = role.RoleId, Score = score };
                var dbMail = new MailPacket
                {
                    Id = (int)GameDBProto.MessageId.SaveRoleScoreRequestId,
                    Content = dbRequest.ToByteArray(),
                    UserId = role.UserId,
                    ClientId = role.ClientId
                };
                ManagerMediator.Dispatcher.WriteDBMail(dbMail, DBMailQueueType.Role);
            }

            var stoc = new WatermelonGameProto.StoCGameOverReply();
            ManagerMediator.Dispatcher.WriteAgentMail(new MailPacket
            {
                Id = (int)WatermelonGameProto.MessageId.StoCgameOverReplyId,
                Content = stoc.ToByteArray(),
                UserId = mail.UserId,
                ClientId = mail.ClientId
            });
        }
        
        private void AgentGetRankRequest(MailPacket mail)
        {
            var dbRequest = new GameDBProto.GetRankRequest { };
            var dbMail = new MailPacket
            {
                Id = (int)GameDBProto.MessageId.GetRankRequestId,
                Content = dbRequest.ToByteArray(),
                UserId = mail.UserId,
                ClientId = mail.ClientId
            };
            ManagerMediator.Dispatcher.WriteDBMail(dbMail, DBMailQueueType.Other);
        }

        private void DBGetRankReply(MailPacket mail)
        {
            var replay = GameDBProto.GetRankReply.Parser.ParseFrom(mail.Content);
            var stoc = new WatermelonGameProto.StoCGetRankReply();

            foreach (var item in replay.RoleScores)
            {
                stoc.RoleScores.Add(new WatermelonGameProto.RoleScore { RoleId = item.RoleId, NickName = item.NickName, Score = item.Score });
            }
            ManagerMediator.Dispatcher.WriteAgentMail(new MailPacket
            {
                Id = (int)WatermelonGameProto.MessageId.StoCgetRankReplyId,
                Content = stoc.ToByteArray(),
                UserId = mail.UserId,
                ClientId = mail.ClientId
            });
        }

    }
}
