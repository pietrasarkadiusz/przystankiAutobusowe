using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PolaczeniaAutobusowe.Data;
using PolaczeniaAutobusowe.Models;

namespace PolaczeniaAutobusowe.Pages.ListaPrzystanków
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Przystanek Przystanek { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Przystanek = await _db.Przystanek.FirstOrDefaultAsync(m => m.Id == id);

            if (Przystanek == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var przystanekFromDb = await _db.Przystanek.FirstOrDefaultAsync(s => s.Id == Przystanek.Id);
            przystanekFromDb.Nazwa = Przystanek.Nazwa;
            await _db.SaveChangesAsync();

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrzystanekExists(Przystanek.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool PrzystanekExists(int id)
        {
            return _db.Przystanek.Any(e => e.Id == id);
        }
    }
}
