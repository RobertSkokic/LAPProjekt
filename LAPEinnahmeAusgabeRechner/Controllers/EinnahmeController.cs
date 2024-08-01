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
    /// CRUDS für die Einnahmen mit SPs
    /// </summary>
    public class EinnahmeController : Controller
    {
        private readonly LAPEinnahmeAusgabeRechnerContext _context;
        private readonly ILogger<EinnahmeController> _logger;

        public EinnahmeController(LAPEinnahmeAusgabeRechnerContext context, ILogger<EinnahmeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Einnahme
        public async Task<IActionResult> Index()
        {
            _logger.LogDebug($"Entering EinnahmeIndex");
            try
            {
                var lAPEinnahmeAusgabeRechnerContext = _context.Einnahme.Include(e => e.Kategorie);
                return View(await lAPEinnahmeAusgabeRechnerContext.ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error EinnahmeIndex: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        // GET: Einnahme/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            _logger.LogDebug($"Entering EinnahmeDetails");
            if (id == null)
            {
                return NotFound();
            }
           
            try
            {
                var einnahme = await _context.Einnahme
                .Include(e => e.Kategorie)
                .FirstOrDefaultAsync(m => m.Einnahmeid == id);
                if (einnahme == null)
                {
                    return NotFound();
                }

                return View(einnahme);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error EinnahmeDetails: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        // GET: Einnahme/Create
        public IActionResult Create()
        {
            _logger.LogDebug($"Entering EinnahmeCreateIndex");
            try
            {
                ViewData["Kategorieid"] = new SelectList(_context.Kategorie, "Kategorieid", "Bezeichnung");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error EinnahmeCreateIndex: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        // POST: Einnahme/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Einnahmeid,Kategorieid,Beschreibung,Betrag,Datum,Valid,ModUser,ModTimestamp,CrUser,CrTimestamp")] Einnahme einnahme)
        {
            _logger.LogDebug($"Entering EinnahmeCreate");
            try
            {
                var result = await _context.Procedures.EinnahmeInsertAsync(einnahme.Kategorieid, einnahme.Beschreibung, einnahme.Betrag, einnahme.Datum, 1);
                return result != null ? RedirectToAction("Index", "Home") : BadRequest("EinnahmeCreate result is null");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error EinnahmeCreate: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        // GET: Einnahme/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            _logger.LogDebug($"Entering EinnahmeEditIndex");
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var einnahme = await _context.Einnahme.FindAsync(id);
                if (einnahme == null)
                {
                    return NotFound();
                }
                ViewData["Kategorieid"] = new SelectList(_context.Kategorie, "Kategorieid", "Bezeichnung", einnahme.Kategorieid);
                return View(einnahme);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error EinnahmeEditIndex: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        // POST: Einnahme/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Einnahmeid,Kategorieid,Beschreibung,Betrag,Datum,Valid,ModUser,ModTimestamp,CrUser,CrTimestamp")] Einnahme einnahme)
        {
            _logger.LogDebug($"Entering EinnahmeEdit");
            if (id != einnahme.Einnahmeid)
            {
                return NotFound();
            }

            try
            {
                var result = await _context.Procedures.EinnahmeUpdateAsync(einnahme.Einnahmeid, einnahme.Kategorieid, einnahme.Beschreibung, einnahme.Betrag, einnahme.Datum, einnahme.Valid);
                return result != 0 ? RedirectToAction("Index", "Home") : BadRequest("EinnahmeEdit result is 0");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error EinnahmeEdit: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        // GET: Einnahme/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            _logger.LogDebug($"Entering EinnahmeDeleteIndex");
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var einnahme = await _context.Einnahme
               .Include(e => e.Kategorie)
               .FirstOrDefaultAsync(m => m.Einnahmeid == id);
                if (einnahme == null)
                {
                    return NotFound();
                }

                return View(einnahme);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error EinnahmeDeleteIndex: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        // POST: Einnahme/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            _logger.LogDebug($"Entering EinnahmeDelete");
            try
            {
                var result = await _context.Procedures.EinnahmeDeleteAsync(id);
                return result != 0 ? RedirectToAction("Index", "Home") : BadRequest("EinnahmeDelete result is 0");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error EinnahmeDelete: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        private bool EinnahmeExists(long id)
        {
            _logger.LogDebug("Entering EinnahmeExists");
            try
            {
                return _context.Einnahme.Any(e => e.Einnahmeid == id);
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"Error EinnahmeExists: {ex.Message}");
                return false;
            }
        }
    }
}
