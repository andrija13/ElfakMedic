using AutoMapper;
using ElfakMedic.Models;
using ElfakMedic.Models.ViewModels;
using ElfakMedic.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ElfakMedic.Controllers
{
    public class LekoviController : Controller
    {
        private ILekoviRepository repositoryLekovi = null;
        private MapperConfiguration mapperConfiguration = null;
        private Mapper mapper = null;

        public LekoviController()
        {
            this.repositoryLekovi = new LekoviRepository();
            mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Lek, LekoviBaseViewModel>()
                .ForMember(dest => dest.DDD, opts => opts.MapFrom(src => src.LekoviCena.DDD))
                .ForMember(dest => dest.ListaRFZO, opts => opts.MapFrom(src => src.LekoviCena.Lista.SkraceniNaziv))
                .ForMember(dest => dest.NaRecept, opts => opts.MapFrom(src => src.LekoviCena.NaRecept))
                .ForMember(dest => dest.ProcenatUcesca, opts => opts.MapFrom(src => src.LekUcesceDijagnoza.ProcenatUcesca))
                .ForMember(dest => dest.NazivProizvodjaca, opts => opts.MapFrom(src => src.Proizvodjac.Naziv));
            });

            mapper = new Mapper(mapperConfiguration);
        }

        public LekoviController(ILekoviRepository repositoryLekovi)
        {
            this.repositoryLekovi = repositoryLekovi;
        }
        public ActionResult Index()
        {
            LekoviViewModel viewModel = new LekoviViewModel();

            List<Lek> lekovi = repositoryLekovi.GetAllLekovi();
            viewModel.Lekovi = mapper.Map<List<LekoviBaseViewModel>>(lekovi);

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult CreateLek()
        {
            CreateLekViewModel viewModel = new CreateLekViewModel();

            var proizvodjaci = repositoryLekovi.GetProizvodjaci();

            IEnumerable<SelectListItem> selectList =
            from p in proizvodjaci
            select new SelectListItem
            {
                Selected = false,
                Text = p.Naziv + " " + p.Zemlja,
                Value = p.Id.ToString()
            };

            viewModel.SelectListProizvodjaci = new SelectList(selectList);

            viewModel.SelectListRFZO = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "A lista", Value = "1"},
                new SelectListItem { Text = "B lista", Value = "2"},
                new SelectListItem { Text = "C lista", Value = "3"},
                new SelectListItem { Text = "D lista", Value = "4"},
            });

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CreateLek(CreateLekViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Message = "Uneti podaci nisu u validnom formatu!" });
            }

            if(!model.NaRecept && !string.IsNullOrEmpty(model.ProcenatUcesca.ToString()))
            {
                return Json(new { Message = "Lek koji ne ide na recept ne moze imati procenat ucesca!" });
            }

            if (repositoryLekovi.CheckIfSifraIsUnique(model.Sifra))
            {
                repositoryLekovi.CreateLek(model);
                return Json(new { Message = "200" });
            }
            else
            {
                return Json(new { Message = "Lek sa ovom sifrom vec postoji!" });
            }
        }
    }
}