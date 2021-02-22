using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Game
{
    public class Desk
    {
        public long DeskId { get; set; }
        public List<long> Roles { get; set; } = new List<long>();
        public object GameLogic { get; set; }
        public int MaxRoleCount { get; set; }

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
        private readonly Dictionary<long, long> _roleDeskDict = new Dictionary<long, long>();
        private readonly Dictionary<long, Desk> _desks = new Dictionary<long, Desk>();
        private long _deskId = 0;

        public Desk CreateDesk(object gameLogic, int maxRoleCount)
        {
            var deskId = ++_deskId;
            var desk = new Desk { DeskId = deskId, GameLogic = gameLogic, MaxRoleCount = maxRoleCount };
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

        public void AddRoleDesk(long roleId, long deskId)
        {
            _roleDeskDict.Add(roleId, deskId);
        }

        public long GetRoleDesk(long roleId)
        {
            return _roleDeskDict.GetValueOrDefault(roleId);
        }

        public void RemoveRoleDesk(long roleId)
        {
            _roleDeskDict.Remove(roleId);
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }

    public partial class RoomManager : AbstractManager<int, Room>
    {
        private readonly Room _room = new Room();
        private readonly GameFactory _gameFactory;

        public RoomManager(GameFactory gameFactory)
        {
            _gameFactory = gameFactory;
        }

    }
}
