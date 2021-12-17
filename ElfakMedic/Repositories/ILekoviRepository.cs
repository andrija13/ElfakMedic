using ElfakMedic.Models;
using ElfakMedic.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElfakMedic.Repositories
{
    public interface ILekoviRepository
    {
        List<Lek> GetAllLekovi();

        List<Lek> GetDrugsByDiagnosis(string idDijagnoze);

        List<Proizvodjac> GetProizvodjaci();

        bool CheckIfSifraIsUnique(string sifra);

        void CreateLek(CreateLekViewModel model);
    }
}
