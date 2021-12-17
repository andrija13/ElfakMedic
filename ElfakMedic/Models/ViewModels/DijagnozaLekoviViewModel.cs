using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElfakMedic.Models.ViewModels
{
    public class DijagnozaLekoviViewModel
    {
        public DijagnozaBaseViewModel Dijagnoza { get; set; }

        public List<LekoviBaseViewModel> Lekovi { get; set; } = new List<LekoviBaseViewModel>();

        public int Count { get; set; }
    }
}