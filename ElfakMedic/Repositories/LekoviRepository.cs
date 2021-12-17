using ElfakMedic.Models;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;
using ElfakMedic.Models.ViewModels;

namespace ElfakMedic.Repositories
{
    public class LekoviRepository : ILekoviRepository
    {
        private readonly SqlConnection conn = null;

        public LekoviRepository()
        {
            conn = new SqlConnection(@"Server =.\SQLEXPRESS; Database = Medical; Trusted_Connection = True;");
        }

        public List<Lek> GetAllLekovi()
        {
            List<Lek> retValue = new List<Lek>();

            conn.Open();
            SqlCommand cmd = new SqlCommand("GetAllDrugs", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    Lek lek = new Lek();
                    lek = MapFullDataModel(lek, rdr);
                    retValue.Add(lek);
                }
                conn.Close();
                return retValue;
            }
        }

        public List<Lek> GetDrugsByDiagnosis(string idDijagnoze)
        {
            List<Lek> retValue = new List<Lek>();

            conn.Open();
            SqlCommand cmd = new SqlCommand("GetDrugsByDiagnosis2", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Id_dijagnoza", SqlDbType.NVarChar, 20).Value = idDijagnoze;

            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    Lek lek = new Lek();
                    lek = MapFullDataModel(lek, rdr);
                    retValue.Add(lek);
                }
                conn.Close();
                return retValue;
            }
        }

        public List<Proizvodjac> GetProizvodjaci()
        {
            List<Proizvodjac> retValue = new List<Proizvodjac>();

            conn.Open();
            SqlCommand cmd = new SqlCommand("GetAllProizvodjaci", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    Proizvodjac proizvodjac = new Proizvodjac();
                    proizvodjac = MapProizvodjac(proizvodjac, rdr);
                    retValue.Add(proizvodjac);
                }
                conn.Close();
                return retValue;
            }
        }

        public bool CheckIfSifraIsUnique(string sifra)
        {
            bool isUnique = false;
            conn.Open();
            SqlCommand cmd = new SqlCommand("CheckIfSifraIsUnique", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Sifra", SqlDbType.NVarChar, 20).Value = sifra;

            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    isUnique = Convert.ToBoolean(rdr["IsUnique"]);
                }
                conn.Close();
            }

            return isUnique;
        }

        public void CreateLek(CreateLekViewModel model)
        {
            conn.Open();

            DataTable dt = new DataTable();
            dt.Columns.Add("Opis_dijagnoze");
            SqlCommand cmd = new SqlCommand("CreateLek", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Sifra", SqlDbType.NVarChar, 20).Value = model.Sifra;
            cmd.Parameters.Add("@FabrickoIme", SqlDbType.NVarChar, 255).Value = model.Naziv + " " + model?.JacinaLeka;
            cmd.Parameters.Add("@IdProizvodjac", SqlDbType.Int).Value = int.Parse(model.SelectedProizvodjac);
            cmd.Parameters.Add("@Cena", SqlDbType.Decimal).Value = model.NabavnaCena;
            cmd.Parameters.Add("@DatumP", SqlDbType.DateTime).Value = DateTime.Now;
            cmd.Parameters.Add("@DatumK", SqlDbType.DateTime).Value = DBNull.Value;
            cmd.Parameters.Add("@ProcenatPoreza", SqlDbType.Decimal).Value = model.ProcenatPoreza;
            cmd.Parameters.Add("@IdLista", SqlDbType.Int).Value = int.Parse(model.SelectedRFZO);
            cmd.Parameters.Add("@DDD", SqlDbType.Decimal).Value = model.DDD;
            cmd.Parameters.Add("@NaRecept", SqlDbType.Bit).Value = model.NaRecept;
            cmd.Parameters.Add("@ProcenatUcesca", SqlDbType.Decimal).Value = model.ProcenatUcesca;
            cmd.Parameters.Add("@ProcenatMarze", SqlDbType.Decimal).Value = model.ProcenatMarze;
            cmd.Parameters.Add("@OpisiDijagnoza", SqlDbType.Structured).Value = CreateOpisiTable(model.SelectedDijagnoze, dt);

            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    //status procedure
                }
                conn.Close();
            }
        }

        private Lek MapFullDataModel(Lek lek, SqlDataReader rdr)
        {
            lek.Id = (int)rdr["Id_lek"];
            lek.Sifra = (string)rdr["Sifra_leka"];
            lek.Naziv = rdr["Fabricko_ime"] != DBNull.Value ? (string)rdr["Fabricko_ime"] : string.Empty;
            lek.ProizvodjacId = (int)rdr["Id_proizvodjac"];
            lek.IdATC = rdr["Id_ATC"] != DBNull.Value ? (int?)rdr["Id_ATC"] : null;
            lek.Proizvodjac.Naziv = rdr["Proizvodjac_naziv"] != DBNull.Value ? (string)rdr["Proizvodjac_naziv"] : string.Empty;
            lek.LekoviCena.NaRecept = rdr["Na_recept"] != DBNull.Value ? (bool)rdr["Na_recept"] : false;
            lek.LekoviCena.DDD = rdr["DDD"] != DBNull.Value ? (decimal?)rdr["DDD"] : null;
            lek.LekoviCena.Lista.SkraceniNaziv = rdr["Lista_RFZO"] != DBNull.Value ? (string)rdr["Lista_RFZO"] : null;
            if (rdr.FieldCount > 9)
            {
                lek.LekUcesceDijagnoza.ProcenatUcesca = rdr["Procenat_ucesce"] != DBNull.Value ? (decimal?)rdr["Procenat_ucesce"] : null;
                lek.UkupnaCena = rdr["Ukupna_cena"] != DBNull.Value ? (decimal?)rdr["Ukupna_cena"] : null;
            }

            return lek;
        }

        private Proizvodjac MapProizvodjac(Proizvodjac proizvodjac, SqlDataReader rdr)
        {
            proizvodjac.Id = (int)rdr["Id_proizvodjac"];
            proizvodjac.Naziv = (string)rdr["Naziv"];
            proizvodjac.Zemlja = rdr["Zemlja"] != DBNull.Value ? (string)rdr["Zemlja"] : string.Empty;

            return proizvodjac;
        }

        private DataTable CreateOpisiTable(List<string> dijagnoze, DataTable dt)
        {
            if (dijagnoze != null)
            {
                foreach (string d in dijagnoze)
                {
                    dt.Rows.Add(d);
                }
            }

            return dt;
        }
    }
}