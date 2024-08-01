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
    /// Zust�ndig f�r das Anzeigen und filtern der Startseite
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
        /// Zust�ndig f�r das Anzeigen und Filtern der Startseite
        /// </summary>
        /// <param name="startDatum"></param>
        /// <param name="endDatum"></param>
        /// <param name="KategorieId"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index(DateTime startDatum, DateTime endDatum, long? KategorieId)
        {
            //setzt das Start und Enddatum auf die letzten 7 Tage, damit der User Standardm��ig die letzte Woche sieht
            if (startDatum == DateTime.MinValue)
            {
                startDatum = DateTime.Today.AddDays(-7);
            }
            if (endDatum == DateTime.MinValue)
            {
                endDatum = DateTime.Today;
            }

            //Holt sich alle Eintr�ge + Kategorien aus der Datenbank die Valid=1 (softdelete) sind mit SPs
            var einnahmenEintr�geResult = await _context.Procedures.EinnahmeGetAllAsync(1);
            var ausgabenEintr�geResult = await _context.Procedures.AusgabeGetAllAsync(1);
            var kategorieEintr�geResult = await _context.Procedures.KategorieGetAllAsync(1);

            //filtert die Einnahmen je nach den Filteroptionen hei�t Datum und kategorie, und select diese dann und mapped sie auf die Einnahme Klasse zum weiterverwenden
            var einnahmenEintr�ge = einnahmenEintr�geResult.Where(x =>
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

            //genau das gleiche wie bei den Einnahmen nur f�r die Ausgaben
            var ausgabenEintr�ge = ausgabenEintr�geResult.Where(x =>
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

            //berechnet die Statistiken f�r die Startseite und packt sie in ein ViewModel + ViewBag
            var totalEinnahmen = einnahmenEintr�ge.Sum(e => e.Betrag);
            var totalAusgaben = ausgabenEintr�ge.Sum(a => a.Betrag);
            var avgEinnahmen = einnahmenEintr�ge.Count > 0 ? Math.Round(einnahmenEintr�ge.Average(e => e.Betrag), 2) : 0;
            var avgAusgaben = ausgabenEintr�ge.Count > 0 ? Math.Round(ausgabenEintr�ge.Average(a => a.Betrag), 2) : 0;


            var statistics = new StatisticsViewModel
            {
                TotalEinnahmen = einnahmenEintr�ge.Sum(e => e.Betrag),
                TotalAusgaben = ausgabenEintr�ge.Sum(a => a.Betrag),
                AvgEinnahmen = einnahmenEintr�ge.Any() ? einnahmenEintr�ge.Average(e => e.Betrag) : 0,
                AvgAusgaben = ausgabenEintr�ge.Any() ? ausgabenEintr�ge.Average(a => a.Betrag) : 0,
                CountEinnahmen = einnahmenEintr�ge.Count,
                CountAusgaben = ausgabenEintr�ge.Count
            };
            ViewBag.Statistics = statistics;

            //Kategorien f�r das Dropdown Men� auf der Startseite
            ViewBag.Kategorien = kategorieEintr�geResult.Select(c => new SelectListItem
            {
                Value = c.KATEGORIEID.ToString(),
                Text = c.BEZEICHNUNG
            }).ToList();

            //Top 5 Einnahmen und Ausgaben f�r das Diagramm auf der Startseite von den Eintr�gen holen und in ein JSON Objekt packen
            var top5EinnahmenEintr�ge = einnahmenEintr�ge.OrderByDescending(e => e.Betrag).Take(5).ToList();
            var top5EinnahmenDaten = new
            {
                labels = top5EinnahmenEintr�ge.Select(x => x.Beschreibung),
                data = top5EinnahmenEintr�ge.Select(x => x.Betrag),
                colors = top5EinnahmenEintr�ge.Select(x => x.Kategorie.Farbe)
            };
            ViewData["top5EinnahmenEintr�ge"] = JsonSerializer.Serialize(top5EinnahmenDaten);

            var top5AusgabenEintr�ge = ausgabenEintr�ge.OrderByDescending(a => a.Betrag).Take(5).ToList();
            var top5AusgabeDaten = new
            {
                labels = top5AusgabenEintr�ge.Select(x => x.Beschreibung),
                data = top5AusgabenEintr�ge.Select(x => x.Betrag),
                colors = top5AusgabenEintr�ge.Select(x => x.Kategorie.Farbe)
            };
            ViewData["top5AusgabenEintr�ge"] = JsonSerializer.Serialize(top5AusgabeDaten);

            //als letztes wird das ViewModel f�r die Startseite erstellt und zur�ckgegeben
            var model = new HomeIndexViewModel
            {
                EinnahmenEintr�ge = einnahmenEintr�ge,
                AusgabenEintr�ge = ausgabenEintr�ge,
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
