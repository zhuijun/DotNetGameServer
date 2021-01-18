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
        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }

    public partial class RoleManager : AbstractManager<long, Role>
    {
        private readonly ILogger<RoleManager> _logger;
        public RoleManager(ILogger<RoleManager> logger)
        {
            _logger = logger;
        }

        public override void Dispose()
        {
            base.Dispose();
            //throw new NotImplementedException();
        }
    }
}
