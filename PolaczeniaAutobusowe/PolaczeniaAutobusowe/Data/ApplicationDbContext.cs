using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PolaczeniaAutobusowe.Models;
using PolaczeniaAutobusowe.Models.ViewModel;

namespace PolaczeniaAutobusowe.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUser  { get; set; }
        public DbSet<Polaczenie> Polaczenie { get; set; }
        public DbSet<Przystanek> Przystanek { get; set; }
        public DbSet<Punkt> Punkt { get; set; }
        public DbSet<PolaczeniaAutobusowe.Models.ViewModel.PrzystankiViewModel> PrzystankiViewModel { get; set; }
        //public DbSet<PolaczenieViewModel> PolaczenieViewModel { get; set; }
        }

}
