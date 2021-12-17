using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ElfakMedic.Models.ViewModels
{
    public class DijagnozaViewModel
    {
        public List<DijagnozaBaseViewModel> Dijagnoze { get; set; } = new List<DijagnozaBaseViewModel>();
    }

    public class DijagnozaBaseViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Id dijagnoze:")]
        public string Id_dijagnoza { get; set; }

        [Display(Name = "Naziv na srpskom:")]
        public string NazivSrpski { get; set; }

        [Display(Name = "Naziv na latinskom:")]
        public string NazivLatinski { get; set; }

        public DateTime? VaziOd { get; set; }

        public DateTime? VaziDo { get; set; }

        public int? SifraGrupaDijagnoze { get; set; }

        public string Root { get; set; }

        public int? Level { get; set; }
    }

    public class CreateDijagnozaViewModel : DijagnozaBaseViewModel
    {
        public SelectList SelectListCategory { get; set; }

        [Display(Name = "Izaberite kategoriju:")]
        public string SelectedCategory { get; set; }

        [Display(Name = "Izaberite podkategoriju:")]
        public string SelectedSubCategory { get; set; }

        public string SelectedIdCategory { get; set; }
    }

    public class AjaxSelectModel
    {
        public string id { get; set; }
        public string text { get; set; }
    }
}