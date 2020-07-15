using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PolaczeniaAutobusowe.Data;
using PolaczeniaAutobusowe.Models;

namespace PolaczeniaAutobusowe.Pages.ListaPrzystanków
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public DeleteModel(ApplicationDbContext db)
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Przystanek = await _db.Przystanek.FindAsync(id);

            if (Przystanek != null)
            {
                _db.Przystanek.Remove(Przystanek);
                await _db.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
