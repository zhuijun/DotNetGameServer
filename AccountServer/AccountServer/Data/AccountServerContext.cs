using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AccountServer.Models;

namespace AccountServer.Data
{
    public class AccountServerContext : DbContext
    {
        public AccountServerContext (DbContextOptions<AccountServerContext> options)
            : base(options)
        {
        }

        public DbSet<AccountServer.Models.UserAccount> UserAccount { get; set; }
    }
}
