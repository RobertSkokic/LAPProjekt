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
    /// CRUDS für die Ausgaben mit SPs
    /// </summary>
    public class AusgabeController : Controller
    {
        private readonly LAPEinnahmeAusgabeRechnerContext _context;
        private readonly ILogger<AusgabeController> _logger;

        public AusgabeController(LAPEinnahmeAusgabeRechnerContext context, ILogger<AusgabeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Ausgabe
        public async Task<IActionResult> Index()
        {
            _logger.LogDebug($"Entering AusgabeIndex");
            try
            {
                var lAPEinnahmeAusgabeRechnerContext = _context.Ausgabe.Include(a => a.Kategorie);
                return View(await lAPEinnahmeAusgabeRechnerContext.ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error AusgabeIndex: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        // GET: Ausgabe/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            _logger.LogDebug($"Entering AusgabeDetails");
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var ausgabe = await _context.Ausgabe
                .Include(a => a.Kategorie)
                .FirstOrDefaultAsync(m => m.Ausgabeid == id);
                if (ausgabe == null)
                {
                    return NotFound();
                }

                return View(ausgabe);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error AusgabeDetails: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        // GET: Ausgabe/Create
        public IActionResult Create()
        {
            _logger.LogDebug($"Entering AusgabeCreateIndex");
            try
            {
                ViewData["Kategorieid"] = new SelectList(_context.Kategorie, "Kategorieid", "Bezeichnung");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error AusgabeCreateIndex: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        // POST: Ausgabe/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Ausgabeid,Kategorieid,Beschreibung,Betrag,Datum,Valid,ModUser,ModTimestamp,CrUser,CrTimestamp")] Ausgabe ausgabe)
        {
            _logger.LogDebug($"Entering AusgabeCreate");
            try
            {
                var result = await _context.Procedures.AusgabeInsertAsync(ausgabe.Kategorieid, ausgabe.Beschreibung, ausgabe.Betrag, ausgabe.Datum, 1);
                return result != null ? RedirectToAction("Index", "Home") : BadRequest("AusgabeCreate result is null");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error AusgabeCreate: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        // GET: Ausgabe/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            _logger.LogDebug($"Entering AusgabeEditIndex");
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var ausgabe = await _context.Ausgabe.FindAsync(id);
                if (ausgabe == null)
                {
                    return NotFound();
                }
                ViewData["Kategorieid"] = new SelectList(_context.Kategorie, "Kategorieid", "Bezeichnung", ausgabe.Kategorieid);
                return View(ausgabe);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error AusgabeEditIndex: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        // POST: Ausgabe/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Ausgabeid,Kategorieid,Beschreibung,Betrag,Datum,Valid,ModUser,ModTimestamp,CrUser,CrTimestamp")] Ausgabe ausgabe)
        {
            _logger.LogDebug($"Entering AusgabeEdit");
            if (id != ausgabe.Ausgabeid)
            {
                return NotFound();
            }

            try
            {
                var result = await _context.Procedures.AusgabeUpdateAsync(ausgabe.Ausgabeid, ausgabe.Kategorieid, ausgabe.Beschreibung, ausgabe.Betrag, ausgabe.Datum, 1);
                return result != 0 ? RedirectToAction("Index", "Home") : BadRequest("AusgabeEdit result is 0");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error AusgabeEdit: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        // GET: Ausgabe/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            _logger.LogDebug($"Entering AusgabeDeleteIndex");
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var ausgabe = await _context.Ausgabe
                    .Include(a => a.Kategorie)
                    .FirstOrDefaultAsync(m => m.Ausgabeid == id);
                if (ausgabe == null)
                {
                    return NotFound();
                }

                return View(ausgabe);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error AusgabeDeleteIndex: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        // POST: Ausgabe/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            _logger.LogDebug($"Entering AusgabeDelete");
            try
            {
                var result = await _context.Procedures.AusgabeDeleteAsync(id);
                return result != 0 ? RedirectToAction("Index", "Home") : BadRequest("AusgabeDelete result is 0");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error AusgabeDelete: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        private bool AusgabeExists(long id)
        {
            _logger.LogDebug("Entering AusgabeExists");
            try
            {
                return _context.Ausgabe.Any(e => e.Ausgabeid == id);
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"Error AusgabeExists: {ex.Message}");
                return false;
            }
        }
    }
}
