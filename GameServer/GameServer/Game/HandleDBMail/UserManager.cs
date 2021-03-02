using GameServer.Common;
using GameServer.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Game
{
    public partial class UserManager : IDBMail
    {
        public void OnDBMail(MailPacket mail)
        {
            //_logger.LogDebug(mail.ToString());
            //throw new NotImplementedException();
        }
    }
}
