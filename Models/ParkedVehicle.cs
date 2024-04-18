using System.ComponentModel.DataAnnotations;

namespace GarageMVC.Models
{
    public enum VehicleType
    {
        Car,
        Motorcycle,
        Bus,
    }
    public class ParkedVehicle
    {
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

        [Required]
        public DateTime ParkingTime { get; set; }
    }
}
