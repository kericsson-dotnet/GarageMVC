using Microsoft.EntityFrameworkCore;

    public class GarageContext : DbContext
    {
        public GarageContext (DbContextOptions<GarageContext> options)
            : base(options)
        {
        }

        public DbSet<GarageMVC.Models.ParkedVehicle> ParkedVehicle { get; set; } = default!;
    }
