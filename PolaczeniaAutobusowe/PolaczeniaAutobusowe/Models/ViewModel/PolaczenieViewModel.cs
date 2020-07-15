using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolaczeniaAutobusowe.Models.ViewModel
{
    public class PolaczenieViewModel
    {
        public Polaczenie Polaczenie { get; set; }
        public IEnumerable<Punkt> Punkty { get; set; }
    }
}
