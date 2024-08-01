using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LAPEinnahmeAusgabeRechner.Models;
using LAPEinnahmeAusgabeRechner.Models.dboSchema;

namespace LAPEinnahmeAusgabeRechner.Controllers
{
    /// <summary>
    /// CRUDS für die Kategorien mit SPs + Delete mit Anpassung der zugehörigen Einträge
    /// </summary>
    public class KategorieController : Controller
    {
        private readonly LAPEinnahmeAusgabeRechnerContext _context;
        private readonly ILogger<KategorieController> _logger;

        public KategorieController(LAPEinnahmeAusgabeRechnerContext context, ILogger<KategorieController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Kategorie
        public async Task<IActionResult> Index()
        {
            _logger.LogDebug("Entering KategorieIndex");
            try
            {
                return View(await _context.Kategorie.ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"Error KategorieIndex: {ex.Message}");
                return BadRequest(ex.Message);
            } 
        }

        // GET: Kategorie/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            _logger.LogDebug("Entering KategorieDetails");
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var kategorie = await _context.Kategorie
                .FirstOrDefaultAsync(m => m.Kategorieid == id);
                if (kategorie == null)
                {
                    return NotFound();
                }

                return View(kategorie);
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"Error KategorieDetails: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        // GET: Kategorie/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Kategorie/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Kategorieid,Bezeichnung,Farbe,Valid,ModUser,ModTimestamp,CrUser,CrTimestamp")] Kategorie kategorie)
        {
            _logger.LogDebug("Entering KategorieCreate");
            try
            {
                var result = await _context.Procedures.KategorieInsertAsync(kategorie.Bezeichnung, kategorie.Farbe, 1);
                return result != null ? RedirectToAction("Index", "Home") : BadRequest("KategorieCreate result is null");
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"Error KategorieCreate: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        // GET: Kategorie/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            _logger.LogDebug("Entering KategorieEdit");
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var kategorie = await _context.Kategorie.FindAsync(id);
                if (kategorie == null)
                {
                    return NotFound();
                }
                return View(kategorie);
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"Error KategorieEdit: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        // POST: Kategorie/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Kategorieid,Bezeichnung,Farbe,Valid,ModUser,ModTimestamp,CrUser,CrTimestamp")] Kategorie kategorie)
        {
            _logger.LogDebug("Entering KategorieEdit");
            if (id != kategorie.Kategorieid)
            {
                return NotFound();
            }

            try
            {
                var result = await _context.Procedures.KategorieUpdateAsync(kategorie.Kategorieid, kategorie.Bezeichnung, kategorie.Farbe, kategorie.Valid);
                return result != 0 ? RedirectToAction("Index", "Home") : BadRequest("KategorieEdit result is 0");
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"Error KategorieEdit: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        // GET: Kategorie/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            _logger.LogDebug("Entering KategorieDelete");
            if (id == null)
            {
                return NotFound();
            }
            
            try
            {
                var kategorie = await _context.Kategorie
                .FirstOrDefaultAsync(m => m.Kategorieid == id);
                if (kategorie == null)
                {
                    return NotFound();
                }

                //checken ob es zugehörige Anträge gibt
                var zugehörigeEinnahmenEinträge = await _context.Procedures.EinnahmeGetByKategorieIDAsync(id, 1);
                var zugehörigeAusgabenEinträge = await _context.Procedures.AusgabeGetByKategorieIDAsync(id, 1);

                //wenn ja dann alle Kategorien außer die aktuelle in ein Dropdown packen und an die View schicken
                if (zugehörigeEinnahmenEinträge.Any() || zugehörigeAusgabenEinträge.Any())
                {
                    var kategorien = await _context.Procedures.KategorieGetAllAsync(1);
                    ViewBag.Kategorien = kategorien
                        .Where(c => c.KATEGORIEID != id)
                        .Select(c => new SelectListItem
                        {
                            Value = c.KATEGORIEID.ToString(),
                            Text = c.BEZEICHNUNG
                        }).ToList();
                }

                return View(kategorie);
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"Error KategorieDelete: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        // POST: Kategorie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id, long? neueKategorieId)
        {
            _logger.LogDebug("Entering KategorieDelete");
            try
            {
                //nocheinmal schauen ob es die zugehörige Kategorien gibt
                var zugehörigeEinnahmenEinträge = await _context.Procedures.EinnahmeGetByKategorieIDAsync(id, 1);
                var zugehörigeAusgabenEinträge = await _context.Procedures.AusgabeGetByKategorieIDAsync(id, 1);

                //wenn ja dann schauen wir ob wir auch die neue Kategorie von der Frontend bekommen haben und updaten einfach die zugehörigen Einträge
                if (zugehörigeEinnahmenEinträge.Any() || zugehörigeAusgabenEinträge.Any())
                {
                    if (neueKategorieId != null)
                    {
                        foreach (var income in zugehörigeEinnahmenEinträge)
                        {
                            await _context.Procedures.EinnahmeUpdateAsync(income.EINNAHMEID, neueKategorieId, income.BESCHREIBUNG, income.BETRAG, income.DATUM, 1);
                        }

                        foreach (var expense in zugehörigeAusgabenEinträge)
                        {
                            await _context.Procedures.AusgabeUpdateAsync(expense.AUSGABEID, neueKategorieId, expense.BESCHREIBUNG, expense.BETRAG, expense.DATUM, 1);
                        }
                    }
                }

                var result = await _context.Procedures.KategorieDeleteAsync(id);
                return result != 0 ? RedirectToAction("Index", "Home") : BadRequest("KategorieDelete result is 0");
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"Error KategorieDelete: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        private bool KategorieExists(long id)
        {
            _logger.LogDebug("Entering KategorieExists");
            try
            {
                return _context.Kategorie.Any(e => e.Kategorieid == id);
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"Error KategorieExists: {ex.Message}");
                return false;
            }
        }
    }
}
