using ElfakMedic.Models;
using ElfakMedic.Models.ViewModels;
using System.Collections.Generic;
using System.Xml;

namespace ElfakMedic.Repositories
{
    public interface IDijagnozaRepository
    { 
        List<Dijagnoza> GetByRoot(string root);

        List<Dijagnoza> GetByLevel(int level);

        List<Dijagnoza> GetBySearch(string filter);

        List<Dijagnoza> GetByRootAfterSearch(string root);

        Dijagnoza GetDijagnoza(string idDijagnoze);

        bool CheckIfIdIsUnique(string id);

        void CreateDijagnoza(CreateDijagnozaViewModel model);

        List<AjaxSelectModel> GetBySearchJson(string filter);

        string UpdateFromXML(XmlDocument xmlDocument, string firstNode, string root, string nodeNamespace);
    }
}
