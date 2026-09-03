using PortTrackingSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace PortTrackingSystem.Core.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Ship> Ships { get; set; }
        public DbSet<Port> Ports { get; set; }
        public DbSet<ShipVisits> ShipVisits { get; set; }
        public DbSet<Cargoes> Cargoes { get; set; }
        public DbSet<CrewMembers> CrewMembers { get; set; }
        public DbSet<ShipCrewAssignments> ShipCrewAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}