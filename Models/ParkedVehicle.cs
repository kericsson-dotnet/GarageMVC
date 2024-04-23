using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GarageMVC.Models
{
    public enum VehicleType
    {
        Car,
        Motorcycle,
        Bus,
        Airplane,
        Truck,
    }
    public static class VehicleTypeExtensions
    {
        public static int GetNumberOfSlots(this VehicleType vehicleType)
        {
            switch (vehicleType)
            {
                case VehicleType.Motorcycle:
                case VehicleType.Car:
                    return 1;
                case VehicleType.Truck:
                case VehicleType.Bus:
                    return 2;
                case VehicleType.Airplane:
                    return 3;
                default:
                    throw new ArgumentOutOfRangeException(nameof(vehicleType), vehicleType, null);
            }
        }
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

        public int Slot { get; set; }
    }
}
