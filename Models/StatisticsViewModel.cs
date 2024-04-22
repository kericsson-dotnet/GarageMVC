namespace GarageMVC.Models
{
    public class StatisticsViewModel
    {
        public int TotalParkedVehicles { get; set; }
        public int TotalWheels { get; set; }
        public decimal TotalFees { get; set; }
       
        public double LongestDurationHours { get; set; }
        public double ShortestDurationHours { get; set; }
       
    }
}
