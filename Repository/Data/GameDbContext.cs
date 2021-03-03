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
        public DbSet<FruitConfig> FruitConfig { get; set; }
        public DbSet<TruntableConfig> TruntableConfig { get; set; }
        public DbSet<GameRole> GameRole { get; set; }
        public DbSet<GameBox> GameBox { get; set; }
        public DbSet<GameScore> GameScore { get; set; }
        public DbSet<GameTruntable> GameTruntable { get; set; }

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

            modelBuilder.Entity<FruitConfig>()
                .Property(b => b.Id)
                .HasDefaultValueSql("newid()");
            modelBuilder.Entity<FruitConfig>()
                .HasIndex(b => b.FruitId)
                .HasFilter("[FruitId] IS NOT NULL");
        }
    }
}
