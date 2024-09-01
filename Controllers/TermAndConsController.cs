using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolProject1640.Data;
using SchoolProject1640.Models;

namespace SchoolProject1640.Controllers
{
    public class TermAndConsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TermAndConsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TermAndCons
        public async Task<IActionResult> Index()
        {
              return _context.TermAndCon != null ? 
                          View(await _context.TermAndCon.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.TermAndCon'  is null.");
        }

        // GET: TermAndCons/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.TermAndCon == null)
            {
                return NotFound();
            }

            var termAndCon = await _context.TermAndCon
                .FirstOrDefaultAsync(m => m.Id == id);
            if (termAndCon == null)
            {
                return NotFound();
            }

            return View(termAndCon);
        }

        // GET: TermAndCons/Create
        public IActionResult Create()
        {
            ViewBag.CheckCreate = _context.TermAndCon.Count();
            return View();
        }

        // POST: TermAndCons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TermsAndCondition")] TermAndCon termAndCon)
        {
            // You may need to generate a unique ID for 'Id' property if it's not auto-generated.
            termAndCon.Id = Guid.NewGuid().ToString();

                _context.Add(termAndCon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
        }


        // GET: TermAndCons/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.TermAndCon == null)
            {
                return NotFound();
            }

            var termAndCon = await _context.TermAndCon.FindAsync(id);
            if (termAndCon == null)
            {
                return NotFound();
            }
            return View(termAndCon);
        }

        // POST: TermAndCons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,TermsAndCondition,CreatedAt,UpdatedAt")] TermAndCon termAndCon)
        {
            termAndCon.UpdatedAt = DateTime.Now;
            if (id != termAndCon.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(termAndCon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TermAndConExists(termAndCon.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(termAndCon);
        }

        // GET: TermAndCons/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.TermAndCon == null)
            {
                return NotFound();
            }

            var termAndCon = await _context.TermAndCon
                .FirstOrDefaultAsync(m => m.Id == id);
            if (termAndCon == null)
            {
                return NotFound();
            }

            return View(termAndCon);
        }

        // POST: TermAndCons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.TermAndCon == null)
            {
                return Problem("Entity set 'ApplicationDbContext.TermAndCon'  is null.");
            }
            var termAndCon = await _context.TermAndCon.FindAsync(id);
            if (termAndCon != null)
            {
                _context.TermAndCon.Remove(termAndCon);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TermAndConExists(string id)
        {
          return (_context.TermAndCon?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
