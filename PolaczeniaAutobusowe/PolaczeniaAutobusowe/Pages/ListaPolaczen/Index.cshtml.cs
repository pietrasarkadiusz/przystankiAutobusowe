using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PolaczeniaAutobusowe.Data;
using PolaczeniaAutobusowe.Models;
using PolaczeniaAutobusowe.Models.ViewModel;

namespace PolaczeniaAutobusowe.Pages.ListaPolaczen
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<PrzystankiViewModel> PrzystankiVM { get; set; }

        public async Task<IActionResult> OnGet(string userId = null)
        {
            PrzystankiVM = new List<PrzystankiViewModel>();
            if (userId == null)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                userId = claim.Value;
            }
            var przystankilst = (from p in _db.Punkt
                                 join po in _db.Polaczenie
                                 on p.PolaczenieId equals po.Id
                                 join pu in _db.Punkt
                                 on p.PolaczenieId equals pu.PolaczenieId
                                 join pr1 in _db.Przystanek
                                 on p.PrzystanekId equals pr1.Id
                                 join pr2 in _db.Przystanek
                                 on pu.PrzystanekId equals pr2.Id
                                 where po.UserId == userId
                                 where p.NumerPrzystanku == 1
                                 select new
                                 {
                                     Id = po.Id,
                                     Skad = pr1.Nazwa,
                                     GodzinaOdjazdu = p.Godzina,
                                     DniKursowania = po.DniKursowania,
                                     Dokad = pr2.Nazwa,
                                     GodzinaPrzyjazdu = pu.Godzina,
                                     Numer = pu.NumerPrzystanku,
                                 })
                                 .ToList();

            var przystaneklstlast = przystankilst
                .GroupBy(u => u.Id)
                .Select(grp => grp.Last())
                .ToList();
            var przystaneklstfirst = przystankilst
                .GroupBy(u => u.Id)
                .Select(grp => grp.First())
                .ToList();

            for(int i=0;i<przystaneklstfirst.Count();i++)
            {
                PrzystankiViewModel pL = new PrzystankiViewModel();
                pL.Id = przystaneklstfirst[i].Id;
                pL.Skad = przystaneklstfirst[i].Skad;
                pL.GodzinaOdjazdu = przystaneklstfirst[i].GodzinaOdjazdu;
                pL.DniKursowania = przystaneklstfirst[i].DniKursowania;
                pL.Dokad = przystaneklstlast[i].Dokad;
                pL.GodzinaPrzyjazdu = przystaneklstlast[i].GodzinaPrzyjazdu;
                PrzystankiVM.Add(pL);
            }
            return Page();
        }
    }
}
