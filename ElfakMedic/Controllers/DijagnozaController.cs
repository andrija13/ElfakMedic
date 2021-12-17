using ElfakMedic.Models;
using ElfakMedic.Models.ViewModels;
using ElfakMedic.Repositories;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using System.Linq;

namespace ElfakMedic.Controllers
{
    public class DijagnozaController : Controller
    {
        private IDijagnozaRepository repositoryDijagnoza = null;
        private ILekoviRepository repositoryLekovi = null;
        private MapperConfiguration mapperConfiguration = null;
        private Mapper mapper = null;
        public DijagnozaController()
        {
            this.repositoryDijagnoza = new DijagnozaRepository();
            this.repositoryLekovi = new LekoviRepository();

            mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Dijagnoza, DijagnozaBaseViewModel>();

                cfg.CreateMap<Lek, LekoviBaseViewModel>()
                .ForMember(dest => dest.DDD, opts => opts.MapFrom(src => src.LekoviCena.DDD))
                .ForMember(dest => dest.ListaRFZO, opts => opts.MapFrom(src => src.LekoviCena.Lista.SkraceniNaziv))
                .ForMember(dest => dest.NaRecept, opts => opts.MapFrom(src => src.LekoviCena.NaRecept))
                .ForMember(dest => dest.ProcenatUcesca, opts => opts.MapFrom(src => src.LekUcesceDijagnoza.ProcenatUcesca))
                .ForMember(dest => dest.NazivProizvodjaca, opts => opts.MapFrom(src => src.Proizvodjac.Naziv));
            });

            mapper = new Mapper(mapperConfiguration);
        }

        public DijagnozaController(IDijagnozaRepository repository, ILekoviRepository lekoviRepository)
        {
            this.repositoryDijagnoza = repository;
            this.repositoryLekovi = lekoviRepository;
        }

        public ActionResult Index()
        {
            DijagnozaViewModel viewModel = new DijagnozaViewModel();

            List<Dijagnoza> dijagnoze = repositoryDijagnoza.GetByLevel(0);

            viewModel.Dijagnoze = mapper.Map<List<DijagnozaBaseViewModel>>(dijagnoze);

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult DijagnozaPV(string root)
        {
            if (!string.IsNullOrEmpty(root))
            {
                DijagnozaViewModel viewModel = new DijagnozaViewModel();

                List<Dijagnoza> dijagnoze = repositoryDijagnoza.GetByRoot(root);
                viewModel.Dijagnoze = mapper.Map<List<DijagnozaBaseViewModel>>(dijagnoze);

                return View(viewModel);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult GetBySearch(string filter)
        {
            if (!string.IsNullOrEmpty(filter))
            {
                DijagnozaViewModel viewModel = new DijagnozaViewModel();

                List<Dijagnoza> dijagnoze = repositoryDijagnoza.GetBySearch(filter);
                viewModel.Dijagnoze = mapper.Map<List<DijagnozaBaseViewModel>>(dijagnoze);

                return View("DijagnozaPV", viewModel);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult GetBySearchJson(string filter)
        {
            if (!string.IsNullOrEmpty(filter))
            {
                List<AjaxSelectModel> dijagnoze = repositoryDijagnoza.GetBySearchJson(filter);

                return Json(dijagnoze, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetByRootAfterSearch(string root)
        {
            if (!string.IsNullOrEmpty(root))
            {
                DijagnozaViewModel viewModel = new DijagnozaViewModel();

                List<Dijagnoza> dijagnoze = repositoryDijagnoza.GetByRootAfterSearch(root);
                viewModel.Dijagnoze = mapper.Map<List<DijagnozaBaseViewModel>>(dijagnoze);

                return View("DijagnozaPV", viewModel);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult GetByLevel(int level)
        {
            DijagnozaViewModel viewModel = new DijagnozaViewModel();

            List<Dijagnoza> dijagnoze = repositoryDijagnoza.GetByLevel(level);
            viewModel.Dijagnoze = mapper.Map<List<DijagnozaBaseViewModel>>(dijagnoze);

            return View("DijagnozaPV", viewModel);
        }

        [HttpGet]
        public ActionResult GetByRoot(string root)
        {
            List<Dijagnoza> dijagnoze = repositoryDijagnoza.GetByRoot(root);

            return Json(dijagnoze, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetDrugsByDiagnosis(string idDijagnoza, int counter)
        {
            if (!string.IsNullOrEmpty(idDijagnoza))
            {
                DijagnozaLekoviViewModel viewModel = new DijagnozaLekoviViewModel();

                Dijagnoza dijagnoza = repositoryDijagnoza.GetDijagnoza(idDijagnoza);
                viewModel.Dijagnoza = mapper.Map<DijagnozaBaseViewModel>(dijagnoza);

                List<Lek> lekovi = repositoryLekovi.GetDrugsByDiagnosis(idDijagnoza);
                viewModel.Lekovi = mapper.Map<List<LekoviBaseViewModel>>(lekovi);

                viewModel.Count = counter;

                return View("DijagnozaLekoviPV", viewModel);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult CreateDijagnoza()
        {
            CreateDijagnozaViewModel viewModel = new CreateDijagnozaViewModel();

            var zerosCategory = repositoryDijagnoza.GetByLevel(0);

            IEnumerable<SelectListItem> selectList =
            from c in zerosCategory
            select new SelectListItem
            {
                Selected = false,
                Text = c.Id_dijagnoza + " " + c.NazivSrpski,
                Value = c.Id_dijagnoza
            };

            viewModel.SelectListCategory = new SelectList(selectList);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CreateDijagnoza(CreateDijagnozaViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return Json(new { Message = "Uneti podaci nisu u validnom formatu!" });
            }

            model.Id_dijagnoza = model.SelectedSubCategory[0] + model.SelectedIdCategory;

            if(repositoryDijagnoza.CheckIfIdIsUnique(model.Id_dijagnoza))
            {
                repositoryDijagnoza.CreateDijagnoza(model);
                return Json(new { Message = "200" });
            }  
            else
            {
                return Json(new { Message = "Dijagnoza sa ovim ID-jem već postoji!" });
            }
        }
    }
}