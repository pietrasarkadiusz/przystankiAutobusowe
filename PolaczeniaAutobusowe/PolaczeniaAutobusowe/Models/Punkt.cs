using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PolaczeniaAutobusowe.Models
{
    public class Punkt
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int PolaczenieId { get; set; }
        [Required]
        public int NumerPrzystanku { get; set; }
        [Required]
        public int PrzystanekId { get; set; }
        [Required]
        public string Godzina { get; set; }

        [ForeignKey("PolaczenieId")]
        public virtual Polaczenie Polaczenie { get; set; }

        [ForeignKey("PrzystanekId")]
        public virtual Przystanek Przystanek { get; set; }
    }
}


