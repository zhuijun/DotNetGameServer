using ServicesCore.Common;
using GameServer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Game
{
    public partial class RoomManager : IDBMail
    {
        public void OnDBMail(MailPacket mail)
        {
            var roleId = ManagerMediator.RoleManager.GetRoleIdByClientId(mail.ClientId);
            var desk = _room.GetDesk(_room.GetRoleDesk(roleId));
            if (desk != null)
            {
                if (desk.GameLogic is IDBMail dbMail)
                {
                    dbMail.OnDBMail(mail);
                }
            }
        }
    }
}
