namespace GarageMVC.Models
{
    public class StatisticsViewModel
    {
        public int TotalParkedVehicles { get; set; } = default;
        public int TotalWheels { get; set; } = default;
        public decimal TotalFees { get; set; } = default;

        public double LongestDurationHours { get; set; } = default;
        public double ShortestDurationHours { get; set; } = default;

    }
}
