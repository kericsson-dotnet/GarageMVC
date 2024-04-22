using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GarageMVC.Models;

    public class GarageContext : DbContext
    {
        public GarageContext (DbContextOptions<GarageContext> options)
            : base(options)
        {
        }

        public DbSet<GarageMVC.Models.ParkedVehicle> ParkedVehicle { get; set; } = default!;
    }
