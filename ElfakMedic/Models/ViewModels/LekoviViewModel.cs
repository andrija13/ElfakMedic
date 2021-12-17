using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ElfakMedic.Models.ViewModels
{
    public class LekoviViewModel
    {
        public List<LekoviBaseViewModel> Lekovi { get; set; } = new List<LekoviBaseViewModel>();
    }

    public class LekoviBaseViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Šifra leka:")]
        public string Sifra { get; set; }

        [Display(Name = "Fabrički naziv:")]
        public string Naziv { get; set; }

        public int ProizvodjacId { get; set; }

        public int? IdATC { get; set; }

        public string NazivProizvodjaca { get; set; }

        [Display(Name = "Na recept:")]
        public bool NaRecept { get; set; }

        [Display(Name = "DDD (dnevna definisana doza):")]
        public decimal? DDD { get; set; }

        public string ListaRFZO { get; set; }

        [Display(Name = "Procenat učešća (%):")]
        public decimal? ProcenatUcesca { get; set; }

        public decimal? UkupnaCena { get; set; }

        public int? Kolicina { get; set; }

        public decimal? Doplata { get; set; }
    }

    public class CreateLekViewModel : LekoviBaseViewModel
    {
        [Display(Name = "Jačina leka:")]
        public string JacinaLeka { get; set; }

        public SelectList SelectListProizvodjaci { get; set; }

        [Display(Name = "Izaberite proizvođača:")]
        public string SelectedProizvodjac { get; set; }

        [Display(Name = "Cena leka na veliko (din.):")]
        public decimal? NabavnaCena { get; set; }

        [Display(Name = "Procenat poreza (%):")]
        public decimal? ProcenatPoreza { get; set; }

        public SelectList SelectListRFZO { get; set; }

        [Display(Name = "Lista RFZO:")]
        public string SelectedRFZO { get; set; }

        public SelectList SelectListDijagnoze { get; set; }

        [Display(Name = "Indikacije:")]
        public List<string> SelectedDijagnoze { get; set; }

        [Display(Name = "Procenat marže (%):")]
        public decimal? ProcenatMarze { get; set; }
    }
}