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
    public partial class ConfigManager : IDBMail
    {
        public void OnDBMail(MailPacket mail)
        {
            switch (mail.Id)
            {
                case (int)GameDBProto.MessageId.LoadConfigReplyId:
                    DBLoadConfigReply(mail);
                    break;
                default:
                    break;
            }
        }

        private void DBLoadConfigReply(MailPacket mail)
        {
            var replay = GameDBProto.LoadConfigReply.Parser.ParseFrom(mail.Content);
            FruitConfig = replay.FruitConfig.Unpack<WatermelonConfigProto.FruitConfig>();
        }
    }
}
