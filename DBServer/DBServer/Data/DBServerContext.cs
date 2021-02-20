using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DBServer.Models;

namespace DBServer.Data
{
    public class DBServerContext : DbContext
    {
        public DBServerContext (DbContextOptions<DBServerContext> options)
            : base(options)
        {
        }

        public DbSet<DBServer.Models.GameRole> GameRole { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameRole>()
                .Property(b => b.Id)
                .HasDefaultValueSql("newid()");
            modelBuilder.Entity<GameRole>()
                .Property(b => b.CreateTime)
                .HasDefaultValueSql("getdate()");
            modelBuilder.Entity<GameRole>()
                .Property(b => b.UpateTime)
                .HasDefaultValueSql("getdate()");
        }
    }
}
