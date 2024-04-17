using System.ComponentModel.DataAnnotations;

namespace GarageMVC.Models
{
    public class ParkedVehicle
    {
        [Key]
        public int Id { get; set; }

        public string? VehicleType { get; set; }

        [Required(ErrorMessage = "Register Number is required!")]
        public string RegNumber { get; set; }

        public string? Color { get; set; }

        public string? Make { get; set; }

        public string? Model { get; set; }

        public int? NumberOfWheels { get; set; }

        [Required]
        public DateTime ParkingTime { get; set; }

        [Required]
        public bool IsParked { get; set; }




    }
}
