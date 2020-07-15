using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PolaczeniaAutobusowe.Models.ViewModel
{
    public class PrzystankiViewModel
    {
        [Key]
        public int Id { get; set; }
        public string Skad { get; set; }
        public string GodzinaOdjazdu { get; set; }
        public string DniKursowania { get; set; }
        public string Dokad { get; set; }
        public string GodzinaPrzyjazdu { get; set; }
    }
}