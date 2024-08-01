using LAPEinnahmeAusgabeRechner.Models;
using LAPEinnahmeAusgabeRechner.Models.dboSchema;
using LAPEinnahmeAusgabeRechner.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.Text.Json;

namespace LAPEinnahmeAusgabeRechner.Controllers
{
    /// <summary>
    /// Zuständig für das Anzeigen und filtern der Startseite
    /// </summary>
    public class HomeController : Controller
    {
        private readonly LAPEinnahmeAusgabeRechnerContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(LAPEinnahmeAusgabeRechnerContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Zuständig für das Anzeigen und Filtern der Startseite
        /// </summary>
        /// <param name="startDatum"></param>
        /// <param name="endDatum"></param>
        /// <param name="KategorieId"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index(DateTime startDatum, DateTime endDatum, long? KategorieId)
        {
            //setzt das Start und Enddatum auf die letzten 7 Tage, damit der User Standardmäßig die letzte Woche sieht
            if (startDatum == DateTime.MinValue)
            {
                startDatum = DateTime.Today.AddDays(-7);
            }
            if (endDatum == DateTime.MinValue)
            {
                endDatum = DateTime.Today;
            }

            //Holt sich alle Einträge + Kategorien aus der Datenbank die Valid=1 (softdelete) sind mit SPs
            var einnahmenEinträgeResult = await _context.Procedures.EinnahmeGetAllAsync(1);
            var ausgabenEinträgeResult = await _context.Procedures.AusgabeGetAllAsync(1);
            var kategorieEinträgeResult = await _context.Procedures.KategorieGetAllAsync(1);

            //filtert die Einnahmen je nach den Filteroptionen heißt Datum und kategorie, und select diese dann und mapped sie auf die Einnahme Klasse zum weiterverwenden
            var einnahmenEinträge = einnahmenEinträgeResult.Where(x =>
            x.DATUM >= startDatum && x.DATUM <= endDatum &&
            (!KategorieId.HasValue || x.KATEGORIEID == KategorieId)).Select(e => new Einnahme
                {
                    Einnahmeid = e.EINNAHMEID,
                    Beschreibung = e.BESCHREIBUNG,
                    Betrag = e.BETRAG,
                    Datum = e.DATUM,
                    Kategorie = new Kategorie
                    {
                        Kategorieid = e.KATEGORIEID,
                        Bezeichnung = e.KATEGORIEBEZEICHNUNG,
                        Farbe = e.KATEGORIEFARBE
                    }
                }).ToList();

            //genau das gleiche wie bei den Einnahmen nur für die Ausgaben
            var ausgabenEinträge = ausgabenEinträgeResult.Where(x =>
            x.DATUM >= startDatum && x.DATUM <= endDatum &&
            (!KategorieId.HasValue || x.KATEGORIEID == KategorieId)).Select(a => new Ausgabe
            {
                Ausgabeid = a.AUSGABEID,
                Beschreibung = a.BESCHREIBUNG,
                Betrag = a.BETRAG,
                Datum = a.DATUM,
                Kategorie = new Kategorie
                {
                    Kategorieid = a.KATEGORIEID,
                    Bezeichnung = a.KATEGORIEBEZEICHNUNG,
                    Farbe = a.KATEGORIEFARBE
                }
            }).ToList();

            //berechnet die Statistiken für die Startseite und packt sie in ein ViewModel + ViewBag
            var totalEinnahmen = einnahmenEinträge.Sum(e => e.Betrag);
            var totalAusgaben = ausgabenEinträge.Sum(a => a.Betrag);
            var avgEinnahmen = einnahmenEinträge.Count > 0 ? Math.Round(einnahmenEinträge.Average(e => e.Betrag), 2) : 0;
            var avgAusgaben = ausgabenEinträge.Count > 0 ? Math.Round(ausgabenEinträge.Average(a => a.Betrag), 2) : 0;


            var statistics = new StatisticsViewModel
            {
                TotalEinnahmen = einnahmenEinträge.Sum(e => e.Betrag),
                TotalAusgaben = ausgabenEinträge.Sum(a => a.Betrag),
                AvgEinnahmen = einnahmenEinträge.Any() ? einnahmenEinträge.Average(e => e.Betrag) : 0,
                AvgAusgaben = ausgabenEinträge.Any() ? ausgabenEinträge.Average(a => a.Betrag) : 0,
                CountEinnahmen = einnahmenEinträge.Count,
                CountAusgaben = ausgabenEinträge.Count
            };
            ViewBag.Statistics = statistics;

            //Kategorien für das Dropdown Menü auf der Startseite
            ViewBag.Kategorien = kategorieEinträgeResult.Select(c => new SelectListItem
            {
                Value = c.KATEGORIEID.ToString(),
                Text = c.BEZEICHNUNG
            }).ToList();

            //Top 5 Einnahmen und Ausgaben für das Diagramm auf der Startseite von den Einträgen holen und in ein JSON Objekt packen
            var top5EinnahmenEinträge = einnahmenEinträge.OrderByDescending(e => e.Betrag).Take(5).ToList();
            var top5EinnahmenDaten = new
            {
                labels = top5EinnahmenEinträge.Select(x => x.Beschreibung),
                data = top5EinnahmenEinträge.Select(x => x.Betrag),
                colors = top5EinnahmenEinträge.Select(x => x.Kategorie.Farbe)
            };
            ViewData["top5EinnahmenEinträge"] = JsonSerializer.Serialize(top5EinnahmenDaten);

            var top5AusgabenEinträge = ausgabenEinträge.OrderByDescending(a => a.Betrag).Take(5).ToList();
            var top5AusgabeDaten = new
            {
                labels = top5AusgabenEinträge.Select(x => x.Beschreibung),
                data = top5AusgabenEinträge.Select(x => x.Betrag),
                colors = top5AusgabenEinträge.Select(x => x.Kategorie.Farbe)
            };
            ViewData["top5AusgabenEinträge"] = JsonSerializer.Serialize(top5AusgabeDaten);

            //als letztes wird das ViewModel für die Startseite erstellt und zurückgegeben
            var model = new HomeIndexViewModel
            {
                EinnahmenEinträge = einnahmenEinträge,
                AusgabenEinträge = ausgabenEinträge,
                StartDatum = startDatum,
                EndDatum = endDatum,
                SelectedKategorieId = KategorieId
            };

            //View returnen mit allen Daten die wir brauchen
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
