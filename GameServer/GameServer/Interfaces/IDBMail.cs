using GameServer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Interfaces
{
    interface IDBMail
    {
        public void OnDBMail(MailPacket mail);
    }
}
