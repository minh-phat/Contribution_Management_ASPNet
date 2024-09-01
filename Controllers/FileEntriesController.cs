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
    public class FileEntriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FileEntriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FileEntries
        public async Task<IActionResult> Index()
        {
              return _context.Files != null ? 
                          View(await _context.Files.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Files'  is null.");
        }

        [HttpPost]
        public async Task<IActionResult> Upload(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return Content("No files selected");

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/SubmitDocx", file.FileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Save file metadata to database using Entity Framework
                    var fileEntry = new FileEntry { FileName = file.FileName, FilePath = path };
                    _context.Files.Add(fileEntry);
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        // GET: FileEntries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Files == null)
            {
                return NotFound();
            }

            var fileEntry = await _context.Files
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fileEntry == null)
            {
                return NotFound();
            }

            return View(fileEntry);
        }

        // GET: FileEntries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FileEntries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Idtest,FileName,FilePath")] FileEntry fileEntry)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fileEntry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fileEntry);
        }

        // GET: FileEntries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Files == null)
            {
                return NotFound();
            }

            var fileEntry = await _context.Files.FindAsync(id);
            if (fileEntry == null)
            {
                return NotFound();
            }
            return View(fileEntry);
        }

        // POST: FileEntries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Idtest,FileName,FilePath")] FileEntry fileEntry)
        {
            if (id != fileEntry.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fileEntry);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FileEntryExists(fileEntry.Id))
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
            return View(fileEntry);
        }

        // GET: FileEntries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Files == null)
            {
                return NotFound();
            }

            var fileEntry = await _context.Files
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fileEntry == null)
            {
                return NotFound();
            }

            return View(fileEntry);
        }

        // POST: FileEntries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Files == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Files'  is null.");
            }
            var fileEntry = await _context.Files.FindAsync(id);
            if (fileEntry != null)
            {
                _context.Files.Remove(fileEntry);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FileEntryExists(int id)
        {
          return (_context.Files?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
