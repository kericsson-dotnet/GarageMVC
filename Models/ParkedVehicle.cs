using System.ComponentModel.DataAnnotations;

namespace GarageMVC.Models
{
    public class ParkedVehicle
    {

        public int Id { get; set; }

        public string VehicleType { get; set; }

        [Required(ErrorMessage = "Register Number is required!")]
        public string RegNumber { get; set; }

        public string Color { get; set; }

        public string Make { get; set; }

        public string Model { get; set; }

        public int NumberOfWheels { get; set; }

        [Required]
        public DateTime CheckInTime { get; set; } = DateTime.Now;

       [Required]
        public bool IsParked { get; set; }




    }
}
