using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PolaczeniaAutobusowe.Data;
using PolaczeniaAutobusowe.Models.ViewModel;

namespace PolaczeniaAutobusowe.Pages.Szukaj
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<PrzystankiViewModel> PrzystankiVM { get; set; }

        public string Skad;
        public string Dokad;
        public async Task<IActionResult> OnGet(string userId = null, string Skad = null, string Dokad = null)
        {
            PrzystankiVM = new List<PrzystankiViewModel>();
            if (userId == null)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                userId = claim.Value;
            }

            StringBuilder param = new StringBuilder();
            param.Append("&searchSkad=");
            if (Skad != null)
            {
                param.Append(Skad);
            }
            param.Append("&searchDokad=");
            if (Dokad != null)
            {
                param.Append(Dokad);
            }
            if (Dokad != null && Skad != null)
            {
                var przystankilst = (from p in _db.Punkt
                                     join po in _db.Polaczenie
                                     on p.PolaczenieId equals po.Id
                                     join pu in _db.Punkt
                                     on p.PolaczenieId equals pu.PolaczenieId
                                     join pr1 in _db.Przystanek
                                     on p.PrzystanekId equals pr1.Id
                                     join pr2 in _db.Przystanek
                                     on pu.PrzystanekId equals pr2.Id
                                     where pu.NumerPrzystanku - p.NumerPrzystanku > 0
                                     where pr1.Nazwa == Skad
                                     where pr2.Nazwa == Dokad
                                     where po.UserId == userId
                                     select new
                                     {
                                         Id = po.Id,
                                         Skad = pr1.Nazwa,
                                         GodzinaOdjazdu = p.Godzina,
                                         DniKursowania = po.DniKursowania,
                                         Dokad = pr2.Nazwa,
                                         GodzinaPrzyjazdu = pu.Godzina
                                     }).ToList();

                foreach (var item in przystankilst)
                {
                    PrzystankiViewModel pL = new PrzystankiViewModel();
                    pL.Id = item.Id;
                    pL.Skad = item.Skad;
                    pL.GodzinaOdjazdu = item.GodzinaOdjazdu;
                    pL.DniKursowania = item.DniKursowania;
                    pL.Dokad = item.Dokad;
                    pL.GodzinaPrzyjazdu = item.GodzinaPrzyjazdu;
                    PrzystankiVM.Add(pL);
                }
            }
            return Page();
        }
    }
}