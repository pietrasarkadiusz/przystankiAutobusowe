using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PolaczeniaAutobusowe.Data;
using PolaczeniaAutobusowe.Models;
using PolaczeniaAutobusowe.Models.ViewModel;
using PolaczeniaAutobusowe.Utility;

namespace PolaczeniaAutobusowe.Pages.Users
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public UsersListViewModel UsersListVM { get; set; }

        public async Task<IActionResult> OnGet(int productPage = 1, string searchImie = null, string searchNazwisko = null, string searchEmail = null, string searchMiasto = null)
        {
            UsersListVM = new UsersListViewModel()
            {
                ApplicationUserList = await _db.ApplicationUser.ToListAsync()
            };

            StringBuilder param = new StringBuilder();
            param.Append("/Users?productPage=:");
            param.Append("&searchImie=");
            if (searchImie != null)
            {
                param.Append(searchImie);
            }
            param.Append("&searchNazwisko=");
            if (searchNazwisko != null)
            {
                param.Append(searchNazwisko);
            }
            param.Append("&searchEmail=");
            if (searchEmail != null)
            {
                param.Append(searchEmail);
            }
            param.Append("&searchMiasto=");
            if (searchEmail != null)
            {
                param.Append(searchMiasto);
            }

            if (searchImie != null)
            {
                UsersListVM.ApplicationUserList = await _db.ApplicationUser.Where(u => u.Imie.ToLower().Contains(searchImie.ToLower())).ToListAsync();
            }
            else
            {
                if (searchNazwisko != null)
                {
                    UsersListVM.ApplicationUserList = await _db.ApplicationUser.Where(u => u.Nazwisko.ToLower().Contains(searchNazwisko.ToLower())).ToListAsync();
                }
                else
                {
                    if (searchEmail != null)
                    {
                        UsersListVM.ApplicationUserList = await _db.ApplicationUser.Where(u => u.Email.ToLower().Contains(searchEmail.ToLower())).ToListAsync();
                    }
                }
            }
            var count = UsersListVM.ApplicationUserList.Count;

            UsersListVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = SD.PaginationUsersPageSize,
                TotalItems = count,
                UrlParam = param.ToString()
            };

            UsersListVM.ApplicationUserList = UsersListVM.ApplicationUserList.OrderBy(p => p.Email)
                .Skip((productPage - 1) * SD.PaginationUsersPageSize)
                .Take(SD.PaginationUsersPageSize).ToList();

            return Page();
        }
    }
}