using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ElfakMedic.Models
{
    public class Dijagnoza
    {
        public int Id { get; set; }

        public string Id_dijagnoza { get; set; }

        public string NazivSrpski { get; set; }

        public string NazivLatinski { get; set; }

        public DateTime? VaziOd { get; set; }

        public DateTime? VaziDo { get; set; }

        public int? SifraGrupaDijagnoze { get; set; }

        public string Root { get; set; }

        public int? Level { get; set; }
    }
}