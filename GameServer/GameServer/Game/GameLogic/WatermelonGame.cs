using AgentGameProto;
using GameServer.Common;
using GameServer.Interfaces;
using GameServer.Services;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                default:
                    break;
            }
        }

        public void OnDBMail(MailPacket mail)
        {
            //throw new NotImplementedException();
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
                var score = _roleScoreDict[role.RoleId];
                _roleScoreDict.Remove(role.RoleId);
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
        }

        private void AgentDropBoxRequest(MailPacket mail)
        {

        }

        private void AgentCombineFruitRequest(MailPacket mail)
        {
            var role = ManagerMediator.RoleManager.GetRoleByClientId(mail.ClientId);
            var ctos = WatermelonGameProto.CtoSCombineFruitRequest.Parser.ParseFrom(mail.Content);
            var stoc = new  WatermelonGameProto.StoCCombineFruitReply{ };
            var fruit = ManagerMediator.ConfigManager.FruitConfig.Items.GetValueOrDefault(ctos.CombineFruitId);
            if (fruit != null)
            {
                var score = _roleScoreDict[role.RoleId];
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
}
