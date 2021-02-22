using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Game.Managers
{
    public class Desk
    {
        public long DeskId { get; set; }
        public List<long> Roles { get; set; } = new List<long>();

        public void AddRole(long roleId)
        {
            Roles.Add(roleId);
        }

        public void RemoveRole(long roleId)
        {
            Roles.Remove(roleId);
        }
    }

    public class Room : IDisposable
    {
        private readonly Dictionary<long, Desk> _desks = new Dictionary<long, Desk>();
        private long _deskId = 0;

        public Desk CreateDesk()
        {
            var deskId = ++_deskId;
            var desk = new Desk { DeskId = deskId };
            _desks.Add(deskId, desk);
            return desk;
        }

        public Desk GetDesk(long deskId)
        {
            return _desks.GetValueOrDefault(deskId);
        }

        public void RemoveDesk(long deskId)
        {
            _desks.Remove(deskId);
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }

    public partial class RoomManager : AbstractManager<int, Room>
    {
        private readonly Room _room = new Room();
    }
}
