using LAPEinnahmeAusgabeRechner.Models.dboSchema;

namespace LAPEinnahmeAusgabeRechner.ViewModels
{
    public class HomeIndexViewModel
    {
        public IEnumerable<Einnahme>? EinnahmenEinträge { get; set; }
        public IEnumerable<Ausgabe>? AusgabenEinträge { get; set; }

        public DateTime? StartDatum { get; set; }
        public DateTime? EndDatum { get; set; }
        public long? SelectedKategorieId { get; set; }
    }
}
