using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ElfakMedic.Models
{
    public class Lek
    {
        public int Id { get; set; }

        public string Sifra { get; set; }

        public string Naziv { get; set; }

        public int ProizvodjacId { get; set; }

        public Proizvodjac Proizvodjac { get; set; } = new Proizvodjac();

        public int? IdATC { get; set; }

        public LekoviCena LekoviCena { get; set; } = new LekoviCena();

        public LekUcesceDijagnoza LekUcesceDijagnoza { get; set; } = new LekUcesceDijagnoza();

        public decimal? UkupnaCena { get; set; }
    }

    public class Proizvodjac
    {
        public int Id { get; set; }

        public string Naziv { get; set; }

        public string Zemlja { get; set; }
    }

    public class ListaLekova
    {
        public int Id { get; set; }

        public string Naziv { get; set; }

        public string SkraceniNaziv { get; set; }
    }

    public class LekoviCena
    {
        public int Id { get; set; }

        public int IdLek { get; set; }

        public Lek Lek { get; set; }

        public int RedniBroj { get; set; }

        public decimal? Cena { get; set; }

        public DateTime? DatumK { get; set; }

        public decimal? ProcenatPoreza { get; set; }

        public int IdLista { get; set; }

        public ListaLekova Lista { get; set; } = new ListaLekova();

        public decimal? DDD { get; set; }

        public int? VaziDana { get; set; }

        public bool NaRecept { get; set; }

        public int? KodTipaIDBroja { get; set; }
    }

    public class LekUcesceDijagnoza
    {
        public int Id { get; set; }

        public int IdLek { get; set; }

        public Lek Lek { get; set; }

        public int RedniBroj { get; set; }

        public string OpisDijagnoze { get; set; }

        public decimal? ProcenatUcesca { get; set; }

        public int? RedniBrojUcesca { get; set; }

        public int? GodineOd { get; set; }

        public int? GodineDo { get; set; }

        public bool MOPP { get; set; }

        public decimal? KolicinaLeka { get; set; }

        public decimal? ProcenatMarze { get; set; }
    }
}