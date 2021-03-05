using AgentGameProto;
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
    public partial class ConfigManager : IAgentMail
    {
        public void OnAgentMail(MailPacket mail)
        {
            switch (mail.Id)
            {
                case (int)WatermelonGameProto.MessageId.CtoSgetConfigRequestId:
                    OnGetConfigRequest(mail);
                    break;
                default:
                    break;
            }
        }

        private void OnGetConfigRequest(MailPacket mail)
        {
            var stoc = new WatermelonGameProto.StoCGetConfigReply { 
                FruitConfig = FruitConfig,
                TruntableConfig = TruntableConfig
            };
            Dispatcher.WriteAgentMail(new MailPacket
            {
                Id = (int)WatermelonGameProto.MessageId.StoCgetConfigReplyId,
                Content = stoc.ToByteArray(),
                UserId = mail.UserId,
                ClientId = mail.ClientId
            });
        }
    }
}
