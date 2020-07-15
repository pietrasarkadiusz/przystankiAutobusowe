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
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public IList<Przystanek> Przystanek { get; set; }
        public async Task<IActionResult> OnGet()
        {
            Przystanek = await _db.Przystanek.ToListAsync();
            return Page();
        }
    }
}