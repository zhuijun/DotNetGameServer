using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Game
{
    public class User : IDisposable
    {
        public long UserID { get; set; }
        public string NickName { get; set; }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }



    public partial class UserManager : AbstractManager<long, User>
    {
        private readonly ILogger<UserManager> _logger;
        public UserManager(ILogger<UserManager> logger)
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
