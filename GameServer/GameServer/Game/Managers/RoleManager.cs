using GameServer.Common;
using GameServer.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Game
{
    public class Role : IDisposable
    {
        public long RoleId { get; set; }
        public string NickName { get; set; }
        public long UserId { get; set; }
        public long ClientId { get; set; }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }

    public partial class RoleManager : AbstractManager<long, Role>
    {
        private readonly ILogger<RoleManager> _logger;
        private readonly Dictionary<long, long> _clientRoleDict = new Dictionary<long, long>();

        public RoleManager(ILogger<RoleManager> logger)
        {
            _logger = logger;
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public long GetRoleIdByClientId(long clientId)
        {
            return _clientRoleDict.GetValueOrDefault(clientId);
        }

        public Role GetRoleByClientId(long clientId)
        {
            if (_clientRoleDict.TryGetValue(clientId, out var roleId))
            {
                return Items.GetValueOrDefault(roleId);
            }
            return null;
        }
    }
}
