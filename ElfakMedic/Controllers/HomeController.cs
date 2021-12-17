using ElfakMedic.Repositories;
using System;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Collections.Generic;
using ElfakMedic.Models.ViewModels;
using Aspose.Pdf;
using Aspose.Pdf.Text;
using ElfakMedic.Models;
using AutoMapper;

namespace ElfakMedic.Controllers
{
    public class HomeController : Controller
    {
        private IDijagnozaRepository repositoryDijagnoza = null;

        public HomeController()
        {
            this.repositoryDijagnoza = new DijagnozaRepository();
        }

        public HomeController(IDijagnozaRepository repository)
        {
            this.repositoryDijagnoza = repository;
        }

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Dijagnoza");
        }

        [HttpPost]
        public ActionResult UploadXML(HttpPostedFileBase[] xmlFiles)
        {
            try
            {
                var result = new List<string>();
                if (xmlFiles != null)
                {
                    foreach (var file in xmlFiles)
                    {
                        if (file.ContentType != "text/xml" && file.ContentType != "application/xml")
                        {
                            result.Add("Fajl " + file.FileName + " - Nevalidan XML fajl!");
                        }
                        else
                        {
                            var xmlPath = Server.MapPath("~/App_Data/" + file.FileName);
                            file.SaveAs(xmlPath);

                            XmlDocument document = new XmlDocument();
                            document.Load(xmlPath);

                            var root = document.DocumentElement.Name;
                            var nodeNamespace = document.DocumentElement.GetAttribute("xmlns");
                            var node = document.DocumentElement.FirstChild.Name;

                            if (node != "Lek" && node != "sLek" && node != "Dijagnoza" && node != "sListaLek" && node != "sLekUcesceDijagnoza" && node != "Proizvodjac")
                            {
                                result.Add("Fajl " + file.FileName + " - Nema poklapanja ni sa jednom tabelom!");
                            }
                            else
                            {
                                var status = repositoryDijagnoza.UpdateFromXML(document, node, root, nodeNamespace);

                                if (status == "200")
                                {
                                    result.Add("Fajl " + file.FileName + " - Uspesno!");
                                }
                                else
                                {
                                    result.Add("Fajl " + file.FileName + " - " + status);
                                }
                            }
                        }
                    }
                }
                else
                {
                    return Json(new { Message = "Nema izabranih fajlova", File = "" });
                }

                return Json(new { Message = string.Join("\n", result) }); ;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Json(new { Message = e.Message });
            }
        }

        [HttpPost]
        public JsonResult SubmitRecept(ReceptViewModel receptModel)
        {
            string idDocument = GeneratePDFDocument(receptModel);

            return Json(new { Message = "200", IdDocument = idDocument });
        }

        private string GeneratePDFDocument(ReceptViewModel recept)
        {
            Document document = new Document();
            Page page = document.Pages.Add();

            page.Paragraphs.Add(new TextFragment { Text = "Evidencija o pregledu pacijenta", HorizontalAlignment = HorizontalAlignment.Center, TextState = { FontSize = 19, LineSpacing = 3.5f } });
            page.Paragraphs.Add(new TextFragment(""));
            page.Paragraphs.Add(new TextFragment { Text = "Datum pregleda: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm"), TextState = { LineSpacing = 1.5f } });
            page.Paragraphs.Add(new TextFragment(""));
            page.Paragraphs.Add(new TextFragment { Text = "Informacije o pacijentu:", TextState = { FontSize = 14, LineSpacing = 2.5f } });
            page.Paragraphs.Add(new TextFragment { Text = "Ime i prezime: " + recept.ImePrezime, TextState = { LineSpacing = 1.5f } });
            page.Paragraphs.Add(new TextFragment { Text = "JMBG: " + recept.JMBG, TextState = { LineSpacing = 1.5f } });
            page.Paragraphs.Add(new TextFragment { Text = "Datum rođenja: " + recept.DatumRodjenja.ToString("dd.MM.yyyy"), TextState = { LineSpacing = 1.5f } });
            page.Paragraphs.Add(new TextFragment { Text = "Broj kartona: " + recept.BrojKartona, TextState = { LineSpacing = 1.5f } });
            page.Paragraphs.Add(new TextFragment { Text = "Broj knjizice: " + recept.BrojKnjizice, TextState = { LineSpacing = 1.5f } });
            page.Paragraphs.Add(new TextFragment { Text = "LBO: " + recept.LBO, TextState = { LineSpacing = 1.5f } });
            page.Paragraphs.Add(new TextFragment(""));
            page.Paragraphs.Add(new TextFragment { Text = "Anamneza:", TextState = { FontSize = 14, LineSpacing = 2.5f } });
            page.Paragraphs.Add(new TextFragment { Text = recept.Opis, TextState = { LineSpacing = 1.5f } });
            page.Paragraphs.Add(new TextFragment(""));
            page.Paragraphs.Add(new TextFragment { Text = "Terapija:", TextState = { FontSize = 14, LineSpacing = 2.5f } });
            page.Paragraphs.Add(new TextFragment(""));

            Table table = new Table();
            table.Border = new BorderInfo(BorderSide.All, .5f, Color.FromRgb(System.Drawing.Color.Black));
            table.DefaultCellBorder = new BorderInfo(BorderSide.All, .5f, Color.FromRgb(System.Drawing.Color.Black));
            table.ColumnWidths = "120 120 50 65 65"; //440 sve ukupno mora da bude
            table.DefaultColumnWidth = 100.ToString();
            table.DefaultCellPadding = new MarginInfo { Top = 2f, Left = 2f, Right = 0f, Bottom = 2f };

            Row row = table.Rows.Add();
            row.BackgroundColor = Color.LightSteelBlue;
            row.Cells.Add("Dijagnoza");
            row.Cells.Add("Prepisani lekovi");
            row.Cells.Add("Kolicina");
            row.Cells.Add("Cena (din.)");
            row.Cells.Add("Doplata (din.)");

            decimal ukupnaCena = 0.0m;
            decimal ukupnaDoplata = 0.0m;

            foreach (var r in recept.receptDictionary)
            {
                Dijagnoza dijagnoza = repositoryDijagnoza.GetDijagnoza(r.Key);

                row = table.Rows.Add();
                row.Cells.Add(dijagnoza.Id_dijagnoza + " " + dijagnoza.NazivSrpski).RowSpan = r.Value.Count;

                foreach (var lek in r.Value)
                {
                    if (lek != null)
                    {
                        row.Cells.Add("► " + lek.Naziv + " " + lek.NazivProizvodjaca);
                        row.Cells.Add(lek.Kolicina.ToString());

                        if (lek.UkupnaCena != null)
                        {
                            row.Cells.Add(lek.Kolicina + " × " + lek.UkupnaCena?.ToString("0.##")).Alignment = HorizontalAlignment.Center;
                            ukupnaCena += (decimal)lek.Kolicina * (decimal)lek.UkupnaCena;
                        }
                        else
                        {
                            row.Cells.Add("Nepoznato");
                        }

                        if (lek.Doplata != null && lek.ProcenatUcesca > 0)
                        {
                            row.Cells.Add(lek.Kolicina + " × " + lek.Doplata?.ToString("0.##")).Alignment = HorizontalAlignment.Center;
                            ukupnaDoplata += (decimal)lek.Kolicina * (decimal)lek.Doplata;
                        }
                        else if (lek.Doplata == 0)
                        {
                            row.Cells.Add(lek.Doplata?.ToString("0.##")).Alignment = HorizontalAlignment.Center;
                        }
                        else
                        {
                            row.Cells.Add("Nepoznato").Alignment = HorizontalAlignment.Center;
                        }

                        row = table.Rows.Add();
                    }
                    else
                    {
                        row.Cells.Add("/").Alignment = HorizontalAlignment.Center;
                        row.Cells.Add("/").Alignment = HorizontalAlignment.Center;
                        row.Cells.Add("/").Alignment = HorizontalAlignment.Center;
                        row.Cells.Add("/").Alignment = HorizontalAlignment.Center;
                        row = table.Rows.Add();
                    }
                }
            }

            row = table.Rows.Add();
            row.Cells.Add("UKUPNO:").ColSpan = 3;
            row.Cells.Add(ukupnaCena.ToString("0.##"));
            row.Cells.Add(ukupnaDoplata.ToString("0.##")).DefaultCellTextState.ForegroundColor = Color.Red;

            page.Paragraphs.Add(table);

            string idDocument = "dijagnoza_" + DateTime.Now.Ticks.ToString() + ".pdf";

            document.Save("C:\\Users\\Andrija\\source\\repos\\ElfakMedic\\ElfakMedic\\SavedDocuments\\" + idDocument);

            return idDocument;
        }
    }
}