using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace GarageMVC.Models
{
    public enum VehicleType
    {
        Car,
        Motorcycle,
        Bus,
    }
    [Index(nameof(RegNumber), IsUnique = true)]
    public class ParkedVehicle
    {
        internal readonly DateTime checkInTime;

        [Key]
        public int Id { get; set; }

        public VehicleType VehicleType { get; set; }

        [RegularExpression(@"^[A-Z]{3}\d{3}$", ErrorMessage = "Registration number must be in the format ABC123")]
        public string RegNumber { get; set; }

        [MaxLength(20)]
        public string? Color { get; set; }

        [MaxLength(20)]
        public string? Make { get; set; }

        [MaxLength(20)]
        public string? Model { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Number of wheels must be a positive number")]
        public int? NumberOfWheels { get; set; }

        public DateTime CheckInTime { get; set; } = DateTime.Now;
        

    }
}
