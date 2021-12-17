using ElfakMedic.Models;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System;
using ElfakMedic.Models.ViewModels;
using System.Xml;
using System.Data.SqlTypes;

namespace ElfakMedic.Repositories
{
    public class DijagnozaRepository : IDijagnozaRepository
    {
        private readonly SqlConnection conn = null;

        public DijagnozaRepository()
        {
            conn = new SqlConnection(@"Server =.\SQLEXPRESS; Database = Medical; Trusted_Connection = True;");
        }

        public List<Dijagnoza> GetByRoot(string root)
        {
            List<Dijagnoza> retValue = new List<Dijagnoza>();

            conn.Open();
            SqlCommand cmd = new SqlCommand("GetByRoot", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Root", SqlDbType.NVarChar, 20).Value = root;

            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    Dijagnoza dijagnoza = new Dijagnoza();
                    dijagnoza = MapDataModel(dijagnoza, rdr);
                    retValue.Add(dijagnoza);
                }
                conn.Close();
                return retValue;
            }
        }

        public List<Dijagnoza> GetByLevel(int level)
        {
            List<Dijagnoza> retValue = new List<Dijagnoza>();

            conn.Open();
            SqlCommand cmd = new SqlCommand("GetByLevel", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Level", SqlDbType.NVarChar, 20).Value = level;

            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    Dijagnoza dijagnoza = new Dijagnoza();
                    dijagnoza = MapDataModel(dijagnoza, rdr);
                    retValue.Add(dijagnoza);
                }
                conn.Close();
                return retValue;
            }
        }

        public List<Dijagnoza> GetBySearch(string filter)
        {
            List<Dijagnoza> retValue = new List<Dijagnoza>();

            conn.Open();
            SqlCommand cmd = new SqlCommand("GetBySearch", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Filter", SqlDbType.NVarChar, 255).Value = filter;

            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    Dijagnoza dijagnoza = new Dijagnoza();
                    dijagnoza = MapDataModel(dijagnoza, rdr);

                    retValue.Add(dijagnoza);
                }
                conn.Close();
                return retValue;
            }
        }

        public List<AjaxSelectModel> GetBySearchJson(string filter)
        {
            List<AjaxSelectModel> retValue = new List<AjaxSelectModel>();

            conn.Open();
            SqlCommand cmd = new SqlCommand("GetBySearch", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Filter", SqlDbType.NVarChar, 255).Value = filter;

            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    AjaxSelectModel dijagnoza = new AjaxSelectModel();
                    dijagnoza = MapAjaxDataModel(dijagnoza, rdr);

                    retValue.Add(dijagnoza);
                }
                conn.Close();
                return retValue;
            }
        }

        public List<Dijagnoza> GetByRootAfterSearch(string root)
        {
            List<Dijagnoza> retValue = new List<Dijagnoza>();

            conn.Open();
            SqlCommand cmd = new SqlCommand("GetByRootAfterSearch", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Root", SqlDbType.NVarChar, 20).Value = root;

            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    Dijagnoza dijagnoza = new Dijagnoza();
                    dijagnoza = MapDataModel(dijagnoza, rdr);

                    retValue.Add(dijagnoza);
                }
                conn.Close();
                return retValue;
            }
        }

        public Dijagnoza GetDijagnoza(string idDijagnoza)
        {
            Dijagnoza dijagnoza = new Dijagnoza();

            conn.Open();
            SqlCommand cmd = new SqlCommand("GetDijagnoza", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Id_dijagnoze", SqlDbType.NVarChar, 20).Value = idDijagnoza;

            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    dijagnoza = MapDataModel(dijagnoza, rdr);
                }
                conn.Close();
                return dijagnoza;
            }
        }

        public void CreateDijagnoza(CreateDijagnozaViewModel model)
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("CreateDijagnoza", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Id_dijagnoza", SqlDbType.NVarChar, 20).Value = model.Id_dijagnoza;
            cmd.Parameters.Add("@Naziv_srpski", SqlDbType.NVarChar, 255).Value = model.NazivSrpski;
            cmd.Parameters.Add("@Naziv_latinski", SqlDbType.NVarChar, 255).Value = model.NazivLatinski;
            cmd.Parameters.Add("@Vazi_od", SqlDbType.DateTime).Value = DateTime.Now;
            cmd.Parameters.Add("@Vazi_do", SqlDbType.DateTime).Value = DBNull.Value;
            cmd.Parameters.Add("@Root", SqlDbType.NVarChar, 20).Value = model.SelectedSubCategory;
            cmd.Parameters.Add("@Level", SqlDbType.Int).Value = 2;

            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    //status procedure
                }
                conn.Close();
            }
        }

        public bool CheckIfIdIsUnique(string id)
        {
            bool isUnique = false;
            conn.Open();
            SqlCommand cmd = new SqlCommand("CheckIfIdIsUnique", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Id_dijagnoza", SqlDbType.NVarChar, 20).Value = id;

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

        public string UpdateFromXML(XmlDocument xmlDocument, string firstNode, string root, string nodeNamespace)
        {
            string result = string.Empty;
            conn.Open();
            SqlCommand cmd = new SqlCommand("UpdateTableFromXML", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            var value = new SqlXml(new XmlTextReader(xmlDocument.InnerXml, XmlNodeType.Document, null));

            cmd.Parameters.Add("@document", SqlDbType.Xml).Value = value;
            cmd.Parameters.Add("@root", SqlDbType.NVarChar, 255).Value = root;
            cmd.Parameters.Add("@node", SqlDbType.NVarChar, 255).Value = firstNode;
            cmd.Parameters.Add("@namespace", SqlDbType.NVarChar, 255).Value = nodeNamespace;

            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    bool status = Convert.ToBoolean(rdr["Status"]);
                    if(status)
                    {
                        result = "200";
                    }
                    else
                    {
                        result = rdr["ErrorMessage"].ToString();
                    }
                }
                conn.Close();
            }

            return result;
        }

        private Dijagnoza MapDataModel(Dijagnoza dijagnoza, SqlDataReader rdr)
        {
            dijagnoza.Id = (int)rdr["Sifra"];
            dijagnoza.Id_dijagnoza = (string)rdr["ID_dijagnoza"];
            dijagnoza.NazivSrpski = rdr["Naziv_srpski"] != DBNull.Value ? (string)rdr["Naziv_srpski"] : string.Empty;
            dijagnoza.NazivLatinski = rdr["Naziv_latinski"] != DBNull.Value ? (string)rdr["Naziv_latinski"] : string.Empty;
            dijagnoza.VaziOd =  rdr["Vazi_od"] != DBNull.Value ? (DateTime?)rdr["Vazi_od"] : null;
            dijagnoza.VaziDo = rdr["Vazi_do"] != DBNull.Value ? (DateTime?)rdr["Vazi_od"] : null;
            dijagnoza.SifraGrupaDijagnoze = rdr["Grupa_dijagnoza_sifra"] != DBNull.Value ? (int?)rdr["Grupa_dijagnoza_sifra"] : null;
            dijagnoza.Root = rdr["Root"] != DBNull.Value ? (string)rdr["Root"] : string.Empty;
            dijagnoza.Level = (int?)rdr["Level"];

            return dijagnoza;
        }

        private AjaxSelectModel MapAjaxDataModel(AjaxSelectModel model, SqlDataReader rdr)
        {
            model.id = (string)rdr["ID_dijagnoza"];
            model.text = rdr["Naziv_srpski"] != DBNull.Value ? model.id + " " + (string)rdr["Naziv_srpski"] : string.Empty;

            return model;
        }
    }
}