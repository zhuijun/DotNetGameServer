using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repository.Models;

namespace Repository.Data
{
    public class GameDbContext : DbContext
    {
        public GameDbContext (DbContextOptions<GameDbContext> options)
            : base(options)
        {
        }

        public DbSet<GameRole> GameRole { get; set; }
        public DbSet<WatermelonConfig> WatermelonConfig { get; set; }

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

            modelBuilder.Entity<WatermelonConfig>()
                .Property(b => b.Id)
                .HasDefaultValueSql("newid()");
        }
    }
}
