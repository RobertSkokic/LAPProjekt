namespace LAPEinnahmeAusgabeRechner.ViewModels
{
    public class StatisticsViewModel
    {
        public decimal TotalEinnahmen { get; set; }
        public decimal TotalAusgaben { get; set; }
        public decimal AvgEinnahmen { get; set; }
        public decimal AvgAusgaben { get; set; }
        public int CountEinnahmen { get; set; }
        public int CountAusgaben { get; set; }
    }
}
