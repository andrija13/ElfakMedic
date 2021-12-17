using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ElfakMedic.Models.ViewModels
{
    public class ReceptViewModel
    {
        public Dictionary<string, List<LekoviBaseViewModel>> receptDictionary { get; set; } = new Dictionary<string, List<LekoviBaseViewModel>>();

        [Display(Name = "Ime i prezime:")]
        [Required]
        [StringLength(255)]
        public string ImePrezime { get; set; }

        [Display(Name = "JMBG:")]
        [Required]
        [StringLength(255)]
        public string JMBG { get; set; }

        [Display(Name = "Datum rođenja:")]
        [Required]
        public DateTime DatumRodjenja { get; set; }

        [Display(Name = "Anamneza:")]
        [StringLength(255)]
        public string Opis { get; set; }

        [Display(Name = "Broj knjižice:")]
        [Required]
        [StringLength(255)]
        public string BrojKnjizice { get; set; }

        [Display(Name = "LBO:")]
        [Required]
        [StringLength(255)]
        public string LBO { get; set; }

        [Display(Name = "Broj kartona:")]
        [Required]
        [StringLength(255)]
        public string BrojKartona { get; set; }
    }
}