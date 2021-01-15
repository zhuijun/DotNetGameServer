﻿using GameServer.Common;
using GameServer.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Game
{
    public partial class RoleManager : AbstractManager
    {
        private readonly ILogger<RoleManager> _logger;
        public RoleManager(ILogger<RoleManager> logger)
        {
            _logger = logger;
        }

    }
}
