using Npgsql;
using PolarionReports.Models.Database.Api;
using PolarionReports.Models.Gantt;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Serilog;

namespace PolarionReports.Models.Database
{
    public static class InitProject
    {
        public static Project GetProject(string ProjectID, string Browser)
        {
            DatareaderP dr;
            Project project = new Project();
            string ErrorMsg = "";
            
            project.Browser = Browser;

            {
                dr = new DatareaderP();
                project = dr.GetProject(ProjectID, out string Error);
                if (Error != null && Error != "GetProject-OK") ErrorMsg = Error + ", ";

                project.Documents = dr.GetDocuments(project.C_pk, out Error);
                if (Error != null && Error != "GetDocuments-OK") ErrorMsg += Error + ", ";

                project.Workitems = dr.GetWorkitems(project.C_pk, out Error);
                if (Error != null && Error != "GetWorkitems-OK") ErrorMsg += Error + ", ";

                project.Uplinks = dr.GetAllUplinks(project.C_pk, out Error);
                if (Error != null && Error != "GetAllUplinks-OK") ErrorMsg += Error + ", ";

                project.WorkitemReferences = dr.GetAllReferences(project.C_pk, out Error);
                if (Error != null && Error != "GetAllReferences-OK") ErrorMsg += Error + ", ";

                project.WorkitemRequirementAllocations = dr.GetAllWorkitemRequirementAllocations(project.C_pk, project.Workitems, out Error);
                if (Error != null && Error != "GetAllWorkitemRequirementAllocation-OK") ErrorMsg += Error + ", ";

                project.Hyperlinks = dr.GetAllHyperlinks(project.C_pk, out Error);
                if (Error != null && Error != "GetAllHyperlinks-OK") ErrorMsg += Error + ", ";

                project.Users = dr.GetAllUsers(out Error);
                if (Error != null && Error != "GetAllUsers-OK") ErrorMsg += Error + ", ";

                project.WorkitemApprovals = dr.GetAllWorkitemApprovals(project.C_pk, out Error);
                if (Error != null && Error != "GetAllWorkitemApprovals-OK") ErrorMsg += Error + ", ";

                project.Comments = dr.GetAllComments(project.C_pk, out Error);
                if (Error != null && Error != "GetAllComments-OK") ErrorMsg += Error + ", ";
            }

            project.DatabaseErrorMessage = ErrorMsg;
            return project;
        }
    }

    #region Postgres Datenbank einlesen
    /// <summary>
    /// Class für Einlesefunktionen auf der Postgres-Datenbank von Polarion
    /// </summary>
    public class DatareaderP
    {
        public DatareaderP()
        {
            string connectionString = ConfigurationManager.AppSettings["PolarionDB"];
            if (string.IsNullOrEmpty(connectionString))
            {
                //connectionString = "Host=wnsvpol1;Username=external;Password=wnPOL%18;Database=polarion;Port=5433";
                connectionString = "Host=atibsrvpol01;Username=polarion;Password=pol%20WN;Database=polarion;Port=5433;Keepalive=100;CommandTimeout=150"; 
            }
            
            this.connection = new NpgsqlConnection(connectionString);
        }

        private void CheckConnection()
        {
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    return;
                }
                else
                {
                    connection.Open();
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error CheckConnection: " + ex.Message);
                Debug.WriteLine(ex.Message);
            }
            
        }

        public void CloseConnection()
        {
            try
            {
                connection.Close();
                connection.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        NpgsqlConnection connection;

        public Project GetProject(string ProjectId, out string Error)
        {
            Error = "GetProject-OK";
            Project p = new Project();
            string queryString = "SELECT c_pk, c_id, c_name, Fk_projectgroup from polarion.project WHERE c_id = '" + ProjectId + "';";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(queryString, connection);

            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    p.C_pk = Convert.ToInt32(reader[0]);
                    p.C_id = reader[1].ToString();
                    p.C_name = reader[2].ToString();
                    p.Fk_projectgroup = Convert.ToInt32(reader[3]);
                    // Debug.WriteLine("\t{0}\t{1}\t{2}\t{3}",
                    //    reader[0], reader[1], reader[2], reader[3]);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Error = ex.Message;
                CloseConnection();
            }

            return p;
        }

        public ProjectDB GetProjectByPK(int pk, out string Error)
        {
            Error = "GetProjectByPK-OK";
            ProjectDB p = new ProjectDB();
            string queryString = "SELECT c_pk, c_id, c_name, Fk_projectgroup, c_trackerprefix from polarion.project WHERE c_pk = " + pk + " ;";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(queryString, connection);
            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    p.C_pk = Convert.ToInt32(reader[0]);
                    p.Id = reader[1].ToString();
                    p.Name = reader[2].ToString();
                    p.Fk_projectgroup = Convert.ToInt32(reader[3]);
                    p.c_trackerprefix = reader[4].ToString();
                    // Debug.WriteLine("\t{0}\t{1}\t{2}\t{3}",
                    //    reader[0], reader[1], reader[2], reader[3]);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Error = ex.Message;
                CloseConnection();
            }

            return p;
        }

        public ProjectDB GetProjectByID(string ProjectId, out string Error)
        {
            Error = "GetProjectByID-OK";
            ProjectDB p = new ProjectDB();
            string queryString = "SELECT c_pk, c_id, c_name, Fk_projectgroup, c_trackerprefix from polarion.project WHERE c_id = '" + ProjectId + "';";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(queryString, connection);
            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    p.C_pk = Convert.ToInt32(reader[0]);
                    p.Id = reader[1].ToString();
                    p.Name = reader[2].ToString();
                    p.Fk_projectgroup = Convert.ToInt32(reader[3]);
                    p.c_trackerprefix = reader[4].ToString();
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Error = ex.Message;
                CloseConnection();
            }

            if (p.C_pk == 0)
            {
                // Project not found - try to find the Project with the Trackerprefix
                queryString = "SELECT c_pk, c_id, c_name, Fk_projectgroup, c_trackerprefix from polarion.project WHERE c_trackerprefix = '" + ProjectId + "';";

                command = new NpgsqlCommand(queryString, connection);
                try
                {
                    CheckConnection();
                    NpgsqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        p.C_pk = Convert.ToInt32(reader[0]);
                        p.Id = reader[1].ToString();
                        p.Name = reader[2].ToString();
                        p.Fk_projectgroup = Convert.ToInt32(reader[3]);
                        p.c_trackerprefix = reader[4].ToString();
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Error = ex.Message;
                    CloseConnection();
                }
            }

            return p;
        }

        public List<Document> GetDocuments(int ProjectID, out string Error)
        {
            Error = "GetDocuments-OK";
            List<Document> dl = new List<Document>();
            Document d;

            string queryString = "SELECT c_pk, fk_project, c_id, c_type, c_modulefolder, c_status from polarion.module " +
                                 "WHERE fk_project = " + ProjectID +
                                 "ORDER BY c_id;";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(queryString, connection);

            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    d = new Document();
                    d.C_pk = Convert.ToInt32(reader[0]);
                    d.ProjectId = Convert.ToInt32(reader[1]);
                    d.C_id = reader[2].ToString();
                    d.C_type = reader[3].ToString();
                    d.C_modulefolder = reader[4].ToString();
                    d.C_status = reader[5].ToString();
                    d.DocName = new DocumentName(d.C_id);
                    // Debug.WriteLine("\t{0}\t{1}\t{2}\t{3}",
                    //    reader[0], reader[1], reader[2], reader[3]);
                    dl.Add(d);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Error = ex.Message;
                CloseConnection();
            }

            return dl;
        }

        public List<DocumentDB> GetDocumentsDB(ProjectDB ProjectDB, out string Error)
        {
            Error = "GetDocumentDBs-OK";
            List<DocumentDB> dl = new List<DocumentDB>();
            DocumentDB d;

            string queryString = "SELECT c_pk, " +
                                        "fk_project, " +
                                        "c_id, " +
                                        "c_type, " +
                                        "c_modulefolder, " +
                                        "c_status " +
                                 "FROM polarion.module " +
                                 "WHERE fk_project = " + ProjectDB.C_pk.ToString() +
                                 "ORDER BY c_id;";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(queryString, connection);

            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    d = new DocumentDB();
                    d.C_pk = Convert.ToInt32(reader[0]);
                    d.ProjectId = Convert.ToInt32(reader[1]);
                    d.C_id = reader[2].ToString();
                    d.C_type = reader[3].ToString();
                    d.C_modulefolder = reader[4].ToString();
                    d.c_status = reader[5].ToString();
                    d.DocName = new DocumentName(d.C_id);
                    d.PolarionDocumentLink = d.GetPolarionDocumentLink(ProjectDB);
                    dl.Add(d);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Error = ex.Message;
                CloseConnection();
            }

            return dl;
        }

        public List<Workitem> GetWorkitems(int ProjectID, out string Error)
        {
            Error = "GetWorkitems-OK";
            List<Workitem> wl = new List<Workitem>();
            Workitem w;

            string queryString = "SELECT c_pk, fk_module, c_id, c_status, c_title, c_type, c_initialestimate, c_timespent, c_remainingestimate, c_duedate, c_description " +
                "from polarion.workitem " +
                "WHERE (fk_project = " + ProjectID +
                " AND c_type <> 'heading')" +
                " ORDER BY c_id;";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(queryString, connection);

            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    w = new Workitem();
                    w.BreakDownIn = Workitem.BreakDownType.Empty;
                    w.C_pk = Convert.ToInt32(reader[0]);
                    if (reader[1] != DBNull.Value) w.DocumentId = Convert.ToInt32(reader[1]);
                    w.Id = reader[2].ToString();
                    w.Status = reader[3].ToString();
                    w.Title = reader[4].ToString();
                    w.Type = reader[5].ToString();
                    if (reader[6] != DBNull.Value) w.InitialEstimate = Convert.ToDouble(reader[6]);
                    if (reader[7] != DBNull.Value) w.TimeSpent = Convert.ToDouble(reader[7]);
                    if (reader[8] != DBNull.Value) w.RemainingEstimate = Convert.ToDouble(reader[8]);
                    if (reader[9] != DBNull.Value) w.DueDate = reader[9].ToString();
                    w.Description = reader[10].ToString();
                    wl.Add(w);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Error = "GetWorkitems-Error: " + ex.Message;
                CloseConnection();
            }

            return wl;
        }

        public Workitem GetWorkitem(int c_pk, out string error)
        {
            Workitem w = new Workitem();
            error = "OK";

            string queryString =
                "SELECT c_pk, fk_module, c_id, c_status, c_title, c_type, c_description " +
                "FROM polarion.workitem " +
                "WHERE (c_pk = " + c_pk.ToString() + ")";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(queryString, connection);

            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    w.BreakDownIn = Workitem.BreakDownType.Empty;
                    w.C_pk = Convert.ToInt32(reader[0]);
                    if (reader[1] != DBNull.Value) w.DocumentId = Convert.ToInt32(reader[1]);
                    w.Id = reader[2].ToString();
                    w.Status = reader[3].ToString();
                    w.Title = reader[4].ToString();
                    w.Type = reader[5].ToString();
                    w.Description = reader[6].ToString();
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Log.Error("Error GetWorkitem " + ex.Message);
                Debug.WriteLine(ex.Message);
                error = "GetWorkitem-Error: " + ex.Message;
                CloseConnection();
            }

            return w;
        }

        public Workitem GetWorkitem(string WorkitemId, out string Error)
        {
            Workitem w = new Workitem();
            Error = "";

            string queryString = 
                "SELECT c_pk, fk_module, c_id, c_status, c_title, c_type, c_description " +
                "FROM polarion.workitem " +
                "WHERE (c_id = '" + WorkitemId + "')";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(queryString, connection);

            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    w.BreakDownIn = Workitem.BreakDownType.Empty;
                    w.C_pk = Convert.ToInt32(reader[0]);
                    if (reader[1] != DBNull.Value) w.DocumentId = Convert.ToInt32(reader[1]);
                    w.Id = reader[2].ToString();
                    w.Status = reader[3].ToString();
                    w.Title = reader[4].ToString();
                    w.Type = reader[5].ToString();
                    w.Description = reader[6].ToString();
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Error = "GetWorkitem-Error: " + ex.Message;
                CloseConnection();
            }

            return w;
        }

        public List<Uplink> GetUplinks(int WorkitemID, out string Error)
        {
            Error = "GetUplinks-OK";
            List<Uplink> ul = new List<Uplink>();
            Uplink u;

            string queryString = "SELECT fk_p_workitem, fk_workitem, c_role from polarion.struct_workitem_linkedworkitems " +
                                 "WHERE (fk_workitem = " + WorkitemID + " AND c_role <> 'parent');";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(queryString, connection);

            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    u = new Uplink();
                    u.WorkitemId = Convert.ToInt32(reader[0]);
                    u.UplinkId = Convert.ToInt32(reader[1]);
                    u.Role = reader[2].ToString();
                    ul.Add(u);
                }
                reader.Close();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Error = "GetUplinks Error: " + ex.Message;
                CloseConnection();
            }

            return ul;
        }

        /// <summary>
        /// Liest alle Uplinks des Projektes ein
        /// </summary>
        /// <param name="ProjectID"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        /// <remarks>History: 
        /// 2018-05-14 auch parent c_role wird eingelesen um die gelöschten Workitems zu finden
        /// </remarks>
        public List<Uplink> GetAllUplinks(int ProjectID, out string Error)
        {
            Error = "GetAllUplinks-OK";
            List<Uplink> ul = new List<Uplink>();
            Uplink u;

            string queryString =
                "SELECT polarion.struct_workitem_linkedworkitems.fk_p_workitem, " +
                "polarion.struct_workitem_linkedworkitems.fk_workitem, " +
                "polarion.struct_workitem_linkedworkitems.c_role " +
                "FROM polarion.workitem " +
                "INNER JOIN polarion.struct_workitem_linkedworkitems " +
                "ON polarion.workitem.c_pk = polarion.struct_workitem_linkedworkitems.fk_p_workitem " +
                "WHERE ((polarion.workitem.fk_project = " + ProjectID.ToString() + ")) " +
                // "AND((polarion.struct_workitem_linkedworkitems.c_role) <> 'parent')) " +
                "ORDER BY polarion.struct_workitem_linkedworkitems.fk_workitem, " +
                "polarion.struct_workitem_linkedworkitems.fk_p_workitem; ";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(queryString, connection);
            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    u = new Uplink
                    {
                        WorkitemId = Convert.ToInt32(reader[0]),
                        UplinkId = Convert.ToInt32(reader[1]),
                        Role = reader[2].ToString()
                    };
                    ul.Add(u);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Error = "GetAllUplinks Error: " + ex.Message;
                CloseConnection();
            }

            return ul;
        }

        public List<WorkitemReference> GetAllReferences(int ProjectID, out string Error)
        {
            int recordcount = 0;
            int checkid = 0;
            Error = "GetAllReferences-OK";

            List<WorkitemReference> wrl = new List<WorkitemReference>();
            WorkitemReference wr;

            string queryString =
                "SELECT polarion.rel_module_workitem.fk_workitem, " +
                       "polarion.rel_module_workitem.fk_uri_module, " +
                       "polarion.module.c_id " +
                "FROM polarion.module " +
                "INNER JOIN polarion.rel_module_workitem " +
                "ON polarion.module.c_pk = polarion.rel_module_workitem.fk_uri_module " +
                "WHERE (((polarion.module.fk_project) = " + ProjectID.ToString() + ") " +
                "AND ((polarion.module.c_modulefolder) = 'Specification'));";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(queryString, connection);
            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    try
                    {
                        checkid = Convert.ToInt32(reader[0]);
                        wr = new WorkitemReference
                        {
                            WorkitemID = Convert.ToInt32(reader[0]),
                            DocumentID = Convert.ToInt32(reader[1]),
                            DocumentName = reader[2].ToString()
                        };
                        wrl.Add(wr);
                        recordcount++;
                    }
                    catch
                    {

                    } 
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Error = "GetAllReferences RecordCount=" + recordcount.ToString() + "Error:" + ex.Message;
                CloseConnection();
            }

            return wrl;
        }

        public List<WorkitemRequirementAllocation> GetAllWorkitemRequirementAllocations(int ProjectID, List<Workitem> Workitems, out string Error)
        {
            List<WorkitemRequirementAllocation> wral = new List<WorkitemRequirementAllocation>();
            WorkitemRequirementAllocation wra;
            string CFName;
            int WorkitemID;

            Error = "GetAllWorkitemRequirementAllocation-OK";
            string queryString =
                "SELECT polarion.workitem.c_pk, " +
                       "polarion.cf_workitem.c_string_value, " +
                       "polarion.cf_workitem.c_boolean_value, " +
                       "polarion.cf_workitem.c_name, " +
                       "polarion.cf_workitem.c_text_value " +
                "FROM polarion.workitem " +
                "INNER JOIN polarion.cf_workitem ON polarion.workitem.c_pk = polarion.cf_workitem.fk_workitem " +
                "WHERE(((polarion.workitem.fk_project) = " + ProjectID.ToString() + ") AND " +
                "(polarion.cf_workitem.c_name = 'requirementAlloc' OR" +
                " polarion.cf_workitem.c_name = 'funcReq' OR" +
                " polarion.cf_workitem.c_name = 'lowLevel' OR" +
                " polarion.cf_workitem.c_name = 'internalComments' OR" +
                " polarion.cf_workitem.c_name = 'clarificationRole' OR" +
                " polarion.cf_workitem.c_name = 'referredDoc' OR" +
                " polarion.cf_workitem.c_name = 'custAction' OR" +
                " polarion.cf_workitem.c_name = 'prioritization' OR" +
                " polarion.cf_workitem.c_name = 'suppAction' OR" +
                " polarion.cf_workitem.c_name = 'featureType' OR" +
                " polarion.cf_workitem.c_name = 'wptype' OR" +
                " polarion.cf_workitem.c_name = 'testType' OR" +                //testResult
                " polarion.cf_workitem.c_name = 'testResult' OR" +
                " polarion.cf_workitem.c_name = 'breakdown' OR" +
                " polarion.cf_workitem.c_name = 'asil' OR" +                    // asil
                " polarion.cf_workitem.c_name = 'asilClassification' OR" +      // asilClassification (Customer Requ)
                " polarion.cf_workitem.c_name = 'csClassification')) " + 
                "ORDER BY polarion.workitem.c_pk;";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(queryString, connection);
            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    CFName = reader[3].ToString();
                    // Debug.WriteLine(CFName);
                    WorkitemID = Convert.ToInt32(reader[0]);
                    // Workitem suchen
                    Workitem w = Workitems.FirstOrDefault(x => x.C_pk == WorkitemID);

                    if (w == null)
                    {
                        continue;
                    }

                    if (CFName == "requirementAlloc")
                    {
                        wra = new WorkitemRequirementAllocation
                        {
                            WorkitemID = WorkitemID,
                            RequirementType = reader[1].ToString()
                        };
                        wral.Add(wra);

                        w.SetRequirementAllocation(wra.RequirementType);
                    }
                    else if (CFName == "funcReq")
                    {
                        w.FunctionalRequirement = true;
                    }
                    else if (CFName == "referredDoc")
                    {
                        w.ReferredDoc = reader[1].ToString();
                    }
                    else if (CFName == "internalComments")
                    {
                        if (reader[4].ToString().Length > 12)
                        {
                            w.InternalComments += reader[4].ToString().Substring(12); // text/plain abschneiden
                        }
                        else
                        {
                            w.InternalComments += reader[1].ToString();
                        }
                    }
                    else if (CFName == "custAction")
                    {
                        if (reader[4].ToString().Length > 12)
                        {
                            w.CustomerAction += reader[4].ToString().Substring(12); // text/plain abschneiden
                        }
                    }
                    else if (CFName == "suppAction")
                    {
                        if (reader[4].ToString().Length > 12)
                        {
                            w.SupplierAction += reader[4].ToString().Substring(12); // text/plain abschneiden
                        }
                    }
                    else if (CFName == "clarificationRole")
                    {
                        w.AddClarificationRole(reader[1].ToString());
                    }
                    else if (CFName == "prioritization")
                    {
                        w.Severity = reader[1].ToString();
                    }
                    else if (CFName == "breakdown")
                    {
                        w.SetBreakdown(reader[1].ToString());
                    }
                    else if (CFName == "featureType" || CFName == "wptype" || CFName == "testType")
                    {
                        w.CFType = reader[1].ToString();
                    }
                    else if (CFName == "testResult")
                    {
                        w.TestResult = reader[1].ToString();
                    }
                    else if (CFName == "asilClassification" || CFName == "asil")
                    {
                        w.Asil = reader[1].ToString();
                    }
                    else if (CFName == "csClassification")
                    {
                        w.Cs = reader[1].ToString();
                    }
                    else
                    {

                    }
                    w.SetLF(CFName);    // LowLevel oder FunctionalRequirement setzen 2019-03-25 erweitert unm asil und csClassification
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Error = ex.Message;
                CloseConnection();
            }

            return wral;
        }

        public List<Hyperlink> GetAllHyperlinks(int ProjectID, out string Error)
        {
            Error = "GetAllHyperlinks-OK";
            List<Hyperlink> hl = new List<Hyperlink>();
            Hyperlink h;

            string queryString =
                "SELECT polarion.struct_workitem_hyperlinks.fk_p_workitem, " +
                       "polarion.struct_workitem_hyperlinks.c_url " +
                "FROM polarion.workitem " +
                "INNER JOIN polarion.struct_workitem_hyperlinks " +
                "ON polarion.workitem.c_pk = polarion.struct_workitem_hyperlinks.fk_p_workitem " +
                "WHERE((polarion.workitem.fk_project = " + ProjectID + ") AND (polarion.struct_workitem_hyperlinks.c_role = 'ref_int'));";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(queryString, connection);

            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    h = new Hyperlink();
                    h.WorkitemID = Convert.ToInt32(reader[0]);
                    h.URL = reader[1].ToString();
                    
                    hl.Add(h);
                }
                reader.Close();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Error = "GetAllHyperlinks Error: " + ex.Message;
                CloseConnection();
            }

            return hl;
        }

        public List<User> GetAllUsers(out string Error)
        {
            Error = "GetAllUsers-OK";

            List<User> ul = new List<User>();
            User u;

            string queryString = "SELECT c_pk, c_id, c_name FROM polarion.t_user";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(queryString, connection);

            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    u = new User();
                    u.C_pk = Convert.ToInt32(reader[0]);
                    u.C_id = reader[1].ToString();
                    u.Name = reader[2].ToString();
                    ul.Add(u);
                }
                reader.Close();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Error = "GetAllUsers Error: " + ex.Message;
                CloseConnection();
            }

            return ul;
        }

        public List<WorkitemApproval> GetAllWorkitemApprovals(int ProjectID, out string Error)
        {
            Error = "GetAllWorkitemApprovals-OK";
            List<WorkitemApproval> wal = new List<WorkitemApproval>();
            WorkitemApproval wa;

            string queryString =
                "SELECT polarion.struct_workitem_approvals.fk_p_workitem, " +
                       "polarion.struct_workitem_approvals.fk_user, " +
                       "polarion.struct_workitem_approvals.c_status " +
                       "FROM polarion.struct_workitem_approvals " +
                       "INNER JOIN polarion.workitem " +
                       "ON polarion.struct_workitem_approvals.fk_p_workitem = polarion.workitem.c_pk " +
                       "WHERE (polarion.workitem.fk_project = " + ProjectID.ToString() + ");";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(queryString, connection);

            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    wa = new WorkitemApproval();
                    if (int.TryParse(reader[0].ToString(), out int _wortitempk))
                    {
                        wa.WorkitemId = _wortitempk;
                    }
                    else
                    {
                        // fehlerhafter Datensatz ignorieren
                        continue;
                    }
                    if (int.TryParse(reader[1].ToString(), out int _userid))
                    {
                        wa.UserId = _userid;
                    }
                    else
                    {
                        // fehlerhafter Datensatz ignorieren
                        continue;
                    }

                    wa.Status = reader[2].ToString();
                    wal.Add(wa);
                }
                reader.Close();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Error = "GetAllWorkitemApprovals Error: " + ex.Message;
                CloseConnection();
            }

            return wal;
        }

        public List<Comment> GetAllComments(int ProjectID, out string Error)
        {
            Error = "GetAllComments-OK";
            List<Comment> cl = new List<Comment>();
            Comment c;

            string queryString =
                "SELECT polarion.comment.c_pk, " +
                "polarion.comment.fk_workitem, " +
                "polarion.comment.fk_author, " +
                "polarion.comment.fk_parentcomment, " +
                "polarion.comment.c_text " +
                "FROM polarion.comment " +
                "WHERE ((polarion.comment.fk_project = " + ProjectID.ToString() + ") " +
                "AND (polarion.comment.c_tags = '{approvals}'));";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(queryString, connection);

            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    c = new Comment();
                    c.C_pk = Convert.ToInt32(reader[0]);
                    c.WorkitemId = Convert.ToInt32(reader[1]);
                    if (int.TryParse(reader[2].ToString(), out int resultUserId))
                    {
                        c.UserId = resultUserId;
                    }
                    
                    string test = reader[3].ToString();
                    if (reader[3].ToString() == "")
                    {
                        c.Parent = 0;
                    }
                    else
                    {
                        c.Parent = Convert.ToInt32(reader[3]);
                    }
                    if (reader[4].ToString() != null && reader[4].ToString().Length > 12)
                    {
                        c.Text = reader[4].ToString().Substring(12);
                    }
                    cl.Add(c);
                }
                reader.Close();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Error = "GetAllComments Error: " + ex.Message;
                CloseConnection();
            }

            return cl;
        }

        
        public List<Comment> GetWorkitemComment(string workitemId, out string error)
        {
            error = "GetWorkitemComment-OK";
            List<Comment> cl = new List<Comment>();
            Comment c;

            string queryString =
                "SELECT polarion.comment.c_pk, " +                          // 0
                       "polarion.comment.fk_author AS UserId, " +           // 1
                       "polarion.comment.fk_parentcomment AS Parent, " +    // 2
                       "polarion.workitem.c_pk AS WorkitemId, " +           // 3
                       "polarion.comment.c_title, " +                       // 4
                       "polarion.comment.c_text, " +                        // 5
                       "polarion.t_user.c_name " +                          // 6
                "FROM (polarion.workitem " +
                "INNER JOIN polarion.comment ON polarion.workitem.c_pk = polarion.comment.fk_workitem) " +
                "INNER JOIN polarion.t_user ON polarion.comment.fk_author = polarion.t_user.c_pk " +
                "WHERE((polarion.workitem.c_id = '" + workitemId + "'))";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(queryString, connection);

            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    c = new Comment();
                    c.C_pk = Convert.ToInt32(reader[0]);
                    c.UserId = Convert.ToInt32(reader[1]);
                    if (reader[2] != DBNull.Value) c.Parent = Convert.ToInt32(reader[2]);
                    c.WorkitemId = Convert.ToInt32(reader[3]);
                    c.Title = reader[4].ToString();
                    if (reader[5].ToString() != null && reader[5].ToString().Length > 12)
                    {
                        c.Text = reader[5].ToString().Substring(12);
                    }
                    c.UserName = reader[6].ToString();
                    cl.Add(c);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                error = "GetWorkitemComment Error: " + ex.Message;
                CloseConnection();
            }

            return cl;
        }

        #region public List<string> GetWorkitemCustomFieldString(string workitemId, string CustomFieldName,  out string error)
        /// <summary>
        /// Liest die String-Customfields eines bestimmten Typs für ein Workitem ein.
        /// </summary>
        /// <param name="workitemId">Primray Key des Workitems</param>
        /// <param name="CustomFieldName">name des Custom Fields</param>
        /// <param name="error">Fehlermeldung</param>
        /// <returns>Liste mit Strings der CustomFiled Einträge</returns>
        public List<string> GetWorkitemCustomFieldString(int workitemId, string CustomFieldName,  out string error)
        {
            error = "GetWorkitemCustomField-OK";
            List<string> customFields = new List<string>();

            string queryString =
                "SELECT polarion.cf_workitem.c_string_value " +
                "FROM polarion.cf_workitem " +
                "WHERE(((polarion.cf_workitem.fk_workitem) = " + workitemId.ToString() + ") " +
                "AND((polarion.cf_workitem.c_name) = '" + CustomFieldName + "')) ";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(queryString, connection);

            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    customFields.Add(reader[0].ToString());
                }
                reader.Close();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                error = "GetWorkitemCustomField Error: " + ex.Message;
                CloseConnection();
            }

            return customFields;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Error"></param>
        /// <returns></returns>
        /// <remarks>
        /// History: 2020-07-06 
        /// </remarks>
        public List<Projectgroup> GetAllProjectGroups(out string Error)
        {
            Error = "GetAllProjectGroups-OK";
            List<Projectgroup> pgl = new List<Projectgroup>();
            Projectgroup pg;

            string queryString =
                "SELECT polarion.projectgroup.c_pk, " +
                       "polarion.projectgroup.c_name, " +
                       "polarion.projectgroup.fk_parent " +
                "FROM polarion.projectgroup " +
                "WHERE (polarion.projectgroup.fk_parent > 0) " +
                "ORDER BY polarion.projectgroup.c_name;";

            CheckConnection();
            //connection.Open();
            NpgsqlCommand command = new NpgsqlCommand(queryString, connection);
           
            try
            {
                
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    pg = new Projectgroup();
                    pg.C_pk = Convert.ToInt32(reader[0]);
                    pg.Name = reader[1].ToString();
                    pg.Parent = Convert.ToInt32(reader[2]);

                    pgl.Add(pg);
                }
                reader.Close();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Error = "GetAllProjectGroups Error: " + ex.Message;
                CloseConnection();
            }

            return pgl;
        }

        public List<ProjectDB> GetAllProjects(out string Error)
        {
            Error = "GetAllProjects-OK";
            List<ProjectDB> pl = new List<ProjectDB>();
            ProjectDB p;

            string queryString =
                "SELECT polarion.project.c_pk, " +
                       "polarion.project.c_id, " +
                       "polarion.project.c_name, " +
                       "polarion.project.fk_projectgroup " +
                "FROM polarion.project " +
                "ORDER BY polarion.project.fk_projectgroup, polarion.project.c_name ;";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(queryString, connection);

            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    p = new ProjectDB();
                    p.C_pk = Convert.ToInt32(reader[0]);
                    p.Id = reader[1].ToString();
                    p.Name = reader[2].ToString();
                    p.Fk_projectgroup = Convert.ToInt32(reader[3]);

                    pl.Add(p);
                }
                reader.Close();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Error = "GetAllProjects Error: " + ex.Message;
                CloseConnection();
            }

            return pl;
        }

        public List<WorkitemAssignee> GetAssigneesFromWorkitem(int WorkitemPK, out string Error)
        {
            Error = "GetAssigneesFromWorkitem-OK";
            List<WorkitemAssignee> wal = new List<WorkitemAssignee>();
            WorkitemAssignee wa;

            string queryString =
                "SELECT polarion.rel_workitem_user_assignee.fk_workitem, " +
                       "polarion.rel_workitem_user_assignee.fk_user, " +
                       "polarion.t_user.c_name " +
                "FROM polarion.rel_workitem_user_assignee " +
                "INNER JOIN polarion.t_user ON polarion.rel_workitem_user_assignee.fk_user = polarion.t_user.c_pk " +
                "WHERE(((polarion.rel_workitem_user_assignee.fk_workitem) = " + WorkitemPK.ToString() + "));";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(queryString, connection);

            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    wa = new WorkitemAssignee();
                    wa.Workitem_PK = Convert.ToInt32(reader[0]);
                    wa.User_PK = Convert.ToInt32(reader[1]);
                    wa.UserName = reader[2].ToString();
                    wal.Add(wa);
                }
                reader.Close();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Error = "GetAssigneesFromWorkitem Error: " + ex.Message;
                CloseConnection();
            }

            return wal;
        }

        public List<PlanTemplate> GetAllPlanTemplatesFromProject(int ProjectPK, out string errorGetAllPlanTemplatesFromProject)
        {
            errorGetAllPlanTemplatesFromProject = "OK";
            List<PlanTemplate> ptl = new List<PlanTemplate>();

            string queryString =
                "SELECT polarion.plan.c_pk, " +             // 0
                       "polarion.plan.c_id, " +             // 1
                       "polarion.plan.c_name, " +           // 2
                       "polarion.plan.c_allowedtypes, " +   // 3
                       "polarion.plan.c_color, " +          // 4
                       "polarion.plan.c_description, " +    // 5
                       "polarion.plan.c_homepagecontent " + // 6
                "FROM polarion.plan " +
                "WHERE(((polarion.plan.fk_project) = " + ProjectPK.ToString() + ") AND((polarion.plan.c_istemplate) = 'true')) " +
                "ORDER BY polarion.plan.c_pk, polarion.plan.c_name; ";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(queryString, connection);

            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    PlanTemplate pt = new PlanTemplate();
                    if (int.TryParse(reader[0].ToString(), out int c_pk)) pt.C_Pk = c_pk;
                    pt.C_Id = reader[1].ToString();
                    pt.C_Name = reader[2].ToString();
                    pt.C_Allowedtypes = reader[3].ToString();
                    pt.C_Color = reader[4].ToString();
                    pt.C_Description = reader[5].ToString();
                    pt.C_Homepagecontent = reader[6].ToString();
                    ptl.Add(pt);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                errorGetAllPlanTemplatesFromProject = e.Message;
                connection.Close();
            }

            return ptl;
        }

        public PlanList GetAllPlansFromProject(int ProjectPK, out string Error)
        {
            DateTime Datehelper;
            Error = "GetAllPlansFromProject-OK";
            PlanList pl = new PlanList();
            pl.Plans = new List<Plan>();
            Plan p;

            string queryString =
                "SELECT polarion.plan.c_pk, " +             // 0
                       "polarion.plan.fk_parent, " +        // 1
                       "polarion.plan.fk_project, " +       // 2
                       "polarion.plan.c_id, " +             // 3
                       "polarion.plan.c_name, " +           // 4
                       "polarion.plan.c_description, " +    // 5
                       "polarion.plan.c_status, " +         // 6
                       "polarion.plan.c_created, " +        // 7
                       "polarion.plan.c_finishedon, " +     // 8
                       "polarion.plan.c_istemplate, " +     // 9
                       "polarion.plan.c_color, " +          // 10
                       "polarion.plan.fk_template, " +      // 11
                       "polarion.plan.c_sortorder " +       // 12
                "FROM polarion.plan " +
                "WHERE (polarion.plan.fk_project = " + ProjectPK.ToString() + ") " +
                "ORDER BY polarion.plan.fk_parent, polarion.plan.c_name;";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(queryString, connection);

            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    p = new Plan();
                    p.Plandb = new PlanDB();
                    p.Plandb.PK = Convert.ToInt32(reader[0]);
                    if (reader[1] != DBNull.Value) p.Plandb.Parent = Convert.ToInt32(reader[1]);

                    p.Plandb.ProjectPK = Convert.ToInt32(reader[2]);
                    p.Plandb.Id = reader[3].ToString();
                    p.Plandb.Name = reader[4].ToString();
                    p.Plandb.Description = reader[5].ToString();
                    p.Plandb.Status = reader[6].ToString();

                    DateTime.TryParse(reader[7].ToString(), out Datehelper);
                    p.Plandb.Created = Datehelper;

                    DateTime.TryParse(reader[8].ToString(), out Datehelper);
                    p.Plandb.Finished = Datehelper;

                    // Debug.WriteLine(reader[4].ToString() + " - " + reader[9].ToString());

                    if (reader[9].ToString().Length > 0)
                    {
                        if (reader[9].ToString() == "True")
                        {
                            p.Plandb.IsTemplate = true;
                        }
                    }
                    else
                    {
                        p.Plandb.IsTemplate = false;
                    }
                    

                    if (reader[10].ToString() == "#16a085")
                    {
                        p.Plandb.Color = 1;
                    }
                    else if (reader[10].ToString() == "#8c08dd")
                    {
                        p.Plandb.Color = 2;
                    }
                    else if (reader[10].ToString() == "#e67e22")
                    {
                        p.Plandb.Color = 3;
                    }
                    else
                    {
                        p.Plandb.Color = 0;
                    }

                    if (reader[11] != DBNull.Value) p.Plandb.TemplatePK = Convert.ToInt32(reader[11]);
                    if (reader[12] != DBNull.Value) p.Plandb.c_sortorder = Convert.ToInt32(reader[12]);

                    pl.Plans.Add(p);
                }
                reader.Close();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Error = ex.Message;
                CloseConnection();
            }

            return pl;
        }

        public PlanDB GetPlanByPK(int PlanPK, out string Error)
        {
            DateTime Datehelper;
            Error = "GetPlanByPK-OK";
            PlanDB p = new PlanDB();

            string queryString =
                "SELECT polarion.plan.c_pk, " +
                       "polarion.plan.fk_parent, " +
                       "polarion.plan.fk_project, " +
                       "polarion.plan.c_id, " +
                       "polarion.plan.c_name, " +
                       "polarion.plan.c_description, " +
                       "polarion.plan.c_status, " +
                       "polarion.plan.c_created, " +
                       "polarion.plan.c_finishedon, " +
                       "polarion.plan.c_istemplate, " +
                       "polarion.plan.c_color, " +
                       "polarion.plan.fk_template " +
                "FROM polarion.plan " +
                "WHERE (polarion.plan.c_pk = " + PlanPK.ToString() + ");";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(queryString, connection);

            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    p.PK = Convert.ToInt32(reader[0]);

                    try
                    {
                        p.Parent = Convert.ToInt32(reader[1]);
                    }
                    catch
                    {
                        p.Parent = 0;
                    }

                    p.ProjectPK = Convert.ToInt32(reader[2]);
                    p.Id = reader[3].ToString();
                    p.Name = reader[4].ToString();
                    p.Description = reader[5].ToString();
                    p.Status = reader[6].ToString();

                    DateTime.TryParse(reader[7].ToString(), out Datehelper);
                    p.Created = Datehelper;

                    DateTime.TryParse(reader[8].ToString(), out Datehelper);
                    p.Finished = Datehelper;

                    // Debug.WriteLine(reader[4].ToString() + " - " + reader[9].ToString());

                    if (reader[9].ToString().Length > 0)
                    {
                        if (reader[9].ToString() == "True")
                        {
                            p.IsTemplate = true;
                        }
                    }
                    else
                    {
                        p.IsTemplate = false;
                    }


                    if (reader[10].ToString() == "#16a085")
                    {
                        p.Color = 1;
                    }
                    else if (reader[10].ToString() == "#8c08dd")
                    {
                        p.Color = 2;
                    }
                    else if (reader[10].ToString() == "#e67e22")
                    {
                        p.Color = 3;
                    }
                    else
                    {
                        p.Color = 0;
                    }

                    try
                    {
                        p.TemplatePK = Convert.ToInt32(reader[11]);
                    }
                    catch
                    {
                        p.TemplatePK = 0;
                    }
                }
                reader.Close();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Error = ex.Message;
                CloseConnection();
            }

            return p;
        }

        public List<PlanCustomField> GetAllPlanCustomFields(int planPK, out string error)
        {
            error = "OK";
            List<PlanCustomField> pcfl = new List<PlanCustomField>();
            PlanCustomField pcf;
            string queryString =
                         "SELECT polarion.cf_plan.fk_plan, " +              // 0
                                "polarion.cf_plan.c_name, " +               // 1
                                "polarion.cf_plan.c_boolean_value, " +      // 2
                                "polarion.cf_plan.c_long_value, " +         // 3
                                "polarion.cf_plan.c_float_value, " +        // 4
                                "polarion.cf_plan.c_currency_value, " +     // 5
                                "polarion.cf_plan.c_date_value, " +         // 6
                                "polarion.cf_plan.c_dateonly_value, " +     // 7
                                "polarion.cf_plan.c_durationtime_value, " + // 8
                                "polarion.cf_plan.c_string_value, " +       // 9
                                "polarion.cf_plan.c_text_value " +          //10
                         "FROM polarion.cf_plan " +
                         "WHERE(polarion.cf_plan.fk_plan = " + planPK.ToString() + ")";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(queryString, connection);

            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    pcf = new PlanCustomField();
                    if (int.TryParse(reader[0].ToString(), out int fk)) { pcf.Fk_plan = fk; }
                    pcf.CfName = reader[1].ToString();
                    if (reader[2] != DBNull.Value) pcf.CfValue = reader[2].ToString();
                    else if (reader[3] != DBNull.Value) pcf.CfValue = reader[3].ToString();
                    else if (reader[4] != DBNull.Value) pcf.CfValue = reader[4].ToString();
                    else if (reader[5] != DBNull.Value) pcf.CfValue = reader[5].ToString();
                    else if (reader[6] != DBNull.Value) pcf.CfValue = reader[6].ToString();
                    else if (reader[7] != DBNull.Value) pcf.CfValue = reader[7].ToString();
                    else if (reader[8] != DBNull.Value) pcf.CfValue = reader[8].ToString();
                    else if (reader[9] != DBNull.Value) pcf.CfValue = reader[9].ToString();
                    else if (reader[10] != DBNull.Value) pcf.CfValue = reader[10].ToString();
                    pcfl.Add(pcf);
                }
                reader.Close();

            }
            catch (Exception ex)
            {
                Log.Error("Fehler bei GetAllPlanCustomFields, " + ex.Message);
                Debug.WriteLine(ex.Message);
                error = ex.Message;
                CloseConnection();
            }
            return pcfl;
        }

        public List<TestrunCustomField> GetAllTestrunCustomFields(int testrunPK, out string error)
        {
            error = "OK";
            List<TestrunCustomField> trcfl = new List<TestrunCustomField>();
            TestrunCustomField trcf;
            string queryString =
                         "SELECT polarion.cf_testrun.fk_testrun, " +        // 0
                                "polarion.cf_testrun.c_name, " +               // 1
                                "polarion.cf_testrun.c_boolean_value, " +      // 2
                                "polarion.cf_testrun.c_long_value, " +         // 3
                                "polarion.cf_testrun.c_float_value, " +        // 4
                                "polarion.cf_testrun.c_currency_value, " +     // 5
                                "polarion.cf_testrun.c_date_value, " +         // 6
                                "polarion.cf_testrun.c_dateonly_value, " +     // 7
                                "polarion.cf_testrun.c_durationtime_value, " + // 8
                                "polarion.cf_testrun.c_string_value, " +       // 9
                                "polarion.cf_testrun.c_text_value " +          //10
                         "FROM polarion.cf_testrun " +
                         "WHERE(polarion.cf_plan.fk_plan = " + testrunPK.ToString() + ")";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(queryString, connection);

            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    trcf = new TestrunCustomField();
                    if (int.TryParse(reader[0].ToString(), out int fk)) { trcf.Fk_testrun = fk; }
                    trcf.CfName = reader[1].ToString();
                    if (reader[2] != DBNull.Value) trcf.CfValue = reader[2].ToString();
                    else if (reader[3] != DBNull.Value) trcf.CfValue = reader[3].ToString();
                    else if (reader[4] != DBNull.Value) trcf.CfValue = reader[4].ToString();
                    else if (reader[5] != DBNull.Value) trcf.CfValue = reader[5].ToString();
                    else if (reader[6] != DBNull.Value) trcf.CfValue = reader[6].ToString();
                    else if (reader[7] != DBNull.Value) trcf.CfValue = reader[7].ToString();
                    else if (reader[8] != DBNull.Value) trcf.CfValue = reader[8].ToString();
                    else if (reader[9] != DBNull.Value) trcf.CfValue = reader[9].ToString();
                    else if (reader[10] != DBNull.Value) trcf.CfValue = reader[10].ToString();
                    trcfl.Add(trcf);
                }
                reader.Close();

            }
            catch (Exception ex)
            {
                Log.Error("Fehler bei GetAllTestrunCustomFields, " + ex.Message);
                Debug.WriteLine(ex.Message);
                error = ex.Message;
                CloseConnection();
            }

            return trcfl;
        }

        public string GetPlanDependencies(int planPK, out string error)
        {
            string dependency = "";
            error = "OK-GetPlanDependencies";

            string queryString =
                "SELECT polarion.cf_plan.c_string_value " +
                "FROM polarion.cf_plan " +
                "WHERE(((polarion.cf_plan.fk_plan) = " + planPK.ToString() + ") AND((polarion.cf_plan.c_name) = 'ganttDependencies'))";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(queryString, connection);

            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    dependency = reader[0].ToString();
                }
                reader.Close();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                error = ex.Message;
                CloseConnection();
            }

            return dependency;
        }

        public List<WorkitemCustomField> GetWorkitemDependencies(int projectPK, out string error)
        {
            error = "OK";
            List<WorkitemCustomField> wcfl = new List<WorkitemCustomField>();
            NpgsqlDataReader reader = null;

            string queryString =
                "SELECT polarion.workitem.c_id, " +
                       "polarion.cf_workitem.c_string_value " +
                "FROM polarion.workitem INNER JOIN polarion.cf_workitem ON polarion.workitem.c_pk = polarion.cf_workitem.fk_workitem " +
                "WHERE(((polarion.workitem.fk_project) = " + projectPK.ToString() + ") AND((polarion.cf_workitem.c_name) = 'dependencies'))";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(queryString, connection);

            try
            {
                reader = command.ExecuteReader();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                error = ex.Message;
                CloseConnection();
                return wcfl;
            }
            try
            {
                while (reader.Read())
                {
                    WorkitemCustomField wcf = new WorkitemCustomField();
                    wcf.WorkitemId = reader[0].ToString();
                    wcf.CfName = "dependencies";
                    wcf.CfValue = reader[1].ToString();
                    wcfl.Add(wcf);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                error = ex.Message;
                reader.Close();
                CloseConnection();
            }

            return wcfl;
        }

        public PlanWorkitemLinkList GetPlanWorkitemLinkListForProject(int ProjectPK, out string Error)
        {
            Error = "GetPlanWorkitemLinkListForProject-OK";
            PlanWorkitemLinkList pwl = new PlanWorkitemLinkList();
            pwl.PlanWorkitemLinks = new List<PlanWorkitemLink>();
            PlanWorkitemLink pw;

            string queryString =
                "SELECT polarion.struct_plan_records.fk_p_plan, " +
                       "polarion.struct_plan_records.fk_item, " +
                       "polarion.workitem.c_id, " +
                       "polarion.plan.c_id " +
                "FROM (polarion.workitem " +
                "INNER JOIN polarion.struct_plan_records ON polarion.workitem.c_pk = polarion.struct_plan_records.fk_item) " +
                "INNER JOIN polarion.plan ON polarion.struct_plan_records.fk_p_plan = polarion.plan.c_pk " +
                "WHERE((polarion.workitem.fk_project = " + ProjectPK.ToString() + "));";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(queryString, connection);

            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    pw = new PlanWorkitemLink();
                    pw.PlanPK = Convert.ToInt32(reader[0]);
                    pw.WorkitemPK = Convert.ToInt32(reader[1]);
                    pw.WorkitemId = reader[2].ToString();
                    pw.PlanId = reader[3].ToString();
                    pwl.PlanWorkitemLinks.Add(pw);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Error = ex.Message;
                CloseConnection();
            }

            return pwl;
        }

        public PlanWorkitemLinkList GetPlanWorkitemLinkList(int PlanPK, out string Error)
        {
            Error = "GetPlanWorkitemLinkList-OK";
            PlanWorkitemLinkList pwl = new PlanWorkitemLinkList();
            pwl.PlanWorkitemLinks = new List<PlanWorkitemLink>();
            PlanWorkitemLink pw;

            string queryString =
                "SELECT polarion.struct_plan_records.fk_p_plan, " +
                       "polarion.struct_plan_records.fk_item, " +
                       "polarion.workitem.c_id " +
                "FROM polarion.struct_plan_records " +
                "INNER JOIN polarion.workitem ON polarion.struct_plan_records.fk_item = polarion.workitem.c_pk " +
                "WHERE(((polarion.struct_plan_records.fk_p_plan) = " + PlanPK + ")); ";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(queryString, connection);

            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    pw = new PlanWorkitemLink();
                    pw.PlanPK = Convert.ToInt32(reader[0]);
                    pw.WorkitemPK = Convert.ToInt32(reader[1]);
                    pw.WorkitemId = reader[2].ToString();
                    pwl.PlanWorkitemLinks.Add(pw);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Error = ex.Message;
                CloseConnection();
            }

            // command.Dispose();

            return pwl;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PlanPK"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        /// <remarks>
        /// History:
        ///  2019-07-05: Typ des Workitems auf "projectTask" geändert 
        /// </remarks>
        public List<PmWorkPackageDB> GetPmWorkPackageForPlan(int PlanPK, out string Error)
        {
            Error = "";
            DateTime Datehelper;
            List<PmWorkPackageDB> PmWorkPackageDBs = new List<PmWorkPackageDB>();
            PmWorkPackageDB pmWorkPackage;

            /*
             * SELECT polarion_plan.c_pk, polarion_plan.c_id, polarion_workitem.c_id, polarion_workitem.c_title, polarion_workitem.c_plannedstart, polarion_workitem.c_duedate, polarion_workitem.c_resolvedon, polarion_workitem.c_status, polarion_workitem.c_initialestimate, polarion_workitem.c_timespent, polarion_workitem.c_updated
               FROM polarion_plan 
               INNER JOIN (polarion_struct_plan_records 
               INNER JOIN polarion_workitem ON polarion_struct_plan_records.fk_item = polarion_workitem.c_pk) 
               ON polarion_plan.c_pk = polarion_struct_plan_records.fk_p_plan
               WHERE (((polarion_plan.c_pk)=512982) AND ((polarion_workitem.c_type)="projectTask"));
             */
            string SQL = "SELECT polarion.plan.c_pk, " +                        // 0
                                "polarion.plan.c_id, " +                        // 1
                                "polarion.workitem.c_id, " +                    // 2
                                "polarion.workitem.c_title, " +                 // 3
                                "polarion.workitem.c_plannedstart, " +          // 4
                                "polarion.workitem.c_duedate, " +               // 5
                                "polarion.workitem.c_resolvedon, " +            // 6
                                "polarion.workitem.c_status, " +                // 7
                                "polarion.workitem.c_initialestimate, " +       // 8
                                "polarion.workitem.c_timespent, " +             // 9
                                "polarion.workitem.c_updated " +                // 10
                         "FROM  polarion.plan " +
                         "INNER JOIN(polarion.struct_plan_records " +
                         "INNER JOIN polarion.workitem ON polarion.struct_plan_records.fk_item = polarion.workitem.c_pk) " +
                         "ON polarion.plan.c_pk = polarion.struct_plan_records.fk_p_plan " +
                         $"WHERE ((polarion.plan.c_pk={PlanPK.ToString()}) AND (polarion.workitem.c_type='workPackage')); "; // workPackage projectTask

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(SQL, connection);

            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    pmWorkPackage = new PmWorkPackageDB();
                    pmWorkPackage.fk_plan_c_pk = Convert.ToInt32(reader[0]);
                    pmWorkPackage.fk_plan_c_id = reader[1].ToString();
                    pmWorkPackage.c_id = reader[2].ToString();
                    pmWorkPackage.c_title = reader[3].ToString();
                    if (DateTime.TryParse(reader[4].ToString(), out Datehelper)) pmWorkPackage.c_plannedstart = Datehelper;
                    if (DateTime.TryParse(reader[5].ToString(), out Datehelper)) pmWorkPackage.c_duedate = Datehelper;
                    if (DateTime.TryParse(reader[6].ToString(), out Datehelper)) pmWorkPackage.c_resolvedon = Datehelper;
                    pmWorkPackage.c_status = reader[7].ToString();
                    if (double.TryParse(reader[8].ToString(), out double result1)) pmWorkPackage.c_initialestimate = result1;
                    if (double.TryParse(reader[9].ToString(), out double result2)) pmWorkPackage.c_timespent = result2;
                    if (DateTime.TryParse(reader[10].ToString(), out Datehelper)) pmWorkPackage.c_updated = Datehelper;
                    PmWorkPackageDBs.Add(pmWorkPackage);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Error = ex.Message;
                CloseConnection();
            }

            foreach(PmWorkPackageDB wp in PmWorkPackageDBs)
            {
                wp.cf_startdate = GetStartDatePMWorkpackage(wp.c_id, out Error);
            }

            return PmWorkPackageDBs;
        }

        public List<PmWorkPackageDB> GetPmWorkPackageForWP(string c_id, out string Error)
        {
            Error = "";
            DateTime Datehelper;
            List<PmWorkPackageDB> PmWorkPackageDBs = new List<PmWorkPackageDB>();
            PmWorkPackageDB pmWorkPackage;

            /*
                SELECT polarion_workitem_1.c_id, polarion_workitem_1.c_title, polarion_workitem.c_plannedstart, polarion_workitem.c_duedate, polarion_workitem.c_resolvedon, polarion_workitem.c_status, polarion_workitem.c_initialestimate, polarion_workitem.c_timespent, polarion_workitem.c_updated
                FROM (polarion_workitem 
                INNER JOIN polarion_struct_workitem_linkedworkitems ON polarion_workitem.c_pk = polarion_struct_workitem_linkedworkitems.fk_workitem) 
                INNER JOIN polarion_workitem AS polarion_workitem_1 ON polarion_struct_workitem_linkedworkitems.fk_p_workitem = polarion_workitem_1.c_pk
                WHERE (((polarion_workitem.c_id)="PMA-2809") 
                AND ((polarion_struct_workitem_linkedworkitems.c_role)="relates_to") 
                AND ((polarion_workitem_1.c_type)="pmWorkPackage"));
             */
            string SQL = "SELECT polarion.workitem.c_id, " +                    // 0
                                "workitem2.c_id, " +                  // 1
                                "workitem2.c_title, " +               // 2
                                "workitem2.c_plannedstart, " +        // 3
                                "workitem2.c_duedate, " +             // 4
                                "workitem2.c_resolvedon, " +          // 5
                                "workitem2.c_status, " +              // 6
                                "workitem2.c_initialestimate, " +     // 7
                                "workitem2.c_timespent, " +           // 8
                                "workitem2.c_updated " +              // 9
                         "FROM (polarion.workitem " +
                         "INNER JOIN polarion.struct_workitem_linkedworkitems ON polarion.workitem.c_pk = polarion.struct_workitem_linkedworkitems.fk_workitem) " +
                         "INNER JOIN polarion.workitem AS workitem2 ON polarion.struct_workitem_linkedworkitems.fk_p_workitem = workitem2.c_pk " +
                         "WHERE (((polarion.workitem.c_id) = '" + c_id + "') " +
                         "AND ((polarion.struct_workitem_linkedworkitems.c_role) = 'parent') " + // relates_to war hier
                         "AND ((workitem2.c_type) = 'workPackage')); "; // workPackage projectTask

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(SQL, connection);

            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    pmWorkPackage = new PmWorkPackageDB();
                    pmWorkPackage.fk_wp_c_id = reader[0].ToString();
                    pmWorkPackage.c_id = reader[1].ToString();
                    pmWorkPackage.c_title = reader[2].ToString();
                    if (DateTime.TryParse(reader[3].ToString(), out Datehelper)) pmWorkPackage.c_plannedstart = Datehelper;
                    if (DateTime.TryParse(reader[4].ToString(), out Datehelper)) pmWorkPackage.c_duedate = Datehelper;
                    if (DateTime.TryParse(reader[5].ToString(), out Datehelper)) pmWorkPackage.c_resolvedon = Datehelper;
                    pmWorkPackage.c_status = reader[6].ToString();
                    if (double.TryParse(reader[7].ToString(), out double result1)) pmWorkPackage.c_initialestimate = result1;
                    if (double.TryParse(reader[8].ToString(), out double result2)) pmWorkPackage.c_timespent = result2;
                    if (DateTime.TryParse(reader[9].ToString(), out Datehelper)) pmWorkPackage.c_updated = Datehelper;
                    pmWorkPackage.cf_startdate = GetStartDatePMWorkpackage(pmWorkPackage.c_id, out Error);
                    PmWorkPackageDBs.Add(pmWorkPackage);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Error = ex.Message;
                CloseConnection();
            }

            return PmWorkPackageDBs;
        }

        /// <summary>
        /// Liefert alle Custom Fields eines Workitems.
        /// Alle CustomField Typen werden in String umgewandelt
        /// </summary>
        /// <param name="c_id"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        public List<WorkitemCustomField> GetAllCustomFieldsFromWorkitem(string c_id, out string Error)
        {
            List<WorkitemCustomField> wcfl = new List<WorkitemCustomField>();
            WorkitemCustomField wcf;
            Error = "";

            string SQL = "SELECT polarion.workitem.c_id, " +                    // 0
                                "polarion.cf_workitem.c_name, " +               // 1
                                "polarion.cf_workitem.c_boolean_value, " +      // 2
                                "polarion.cf_workitem.c_long_value, " +         // 3
                                "polarion.cf_workitem.c_float_value, " +        // 4
                                "polarion.cf_workitem.c_currency_value, " +     // 5
                                "polarion.cf_workitem.c_date_value, " +         // 6
                                "polarion.cf_workitem.c_dateonly_value, " +     // 7
                                "polarion.cf_workitem.c_durationtime_value, " + // 8
                                "polarion.cf_workitem.c_string_value, " +       // 9
                                "polarion.cf_workitem.c_text_value " +          //10
                         "FROM polarion.workitem " +
                         "INNER JOIN polarion.cf_workitem ON polarion.workitem.c_pk = polarion.cf_workitem.fk_workitem " +
                         "WHERE ((polarion.workitem.c_id) = '" + c_id + "')";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(SQL, connection);

            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    wcf = new WorkitemCustomField();
                    wcf.WorkitemId = reader[0].ToString();
                    wcf.CfName = reader[1].ToString();
                    if (reader[2] != DBNull.Value) wcf.CfValue = reader[2].ToString();
                    else if (reader[3] != DBNull.Value) wcf.CfValue = reader[3].ToString();
                    else if (reader[4] != DBNull.Value) wcf.CfValue = reader[4].ToString();
                    else if (reader[5] != DBNull.Value) wcf.CfValue = reader[5].ToString();
                    else if (reader[6] != DBNull.Value) wcf.CfValue = reader[6].ToString();
                    else if (reader[7] != DBNull.Value) wcf.CfValue = reader[7].ToString();
                    else if (reader[8] != DBNull.Value) wcf.CfValue = reader[8].ToString();
                    else if (reader[9] != DBNull.Value) wcf.CfValue = reader[9].ToString();
                    else if (reader[10] != DBNull.Value) wcf.CfValue = reader[10].ToString();
                    wcfl.Add(wcf);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Error = ex.Message;
                CloseConnection();
            }

            return wcfl;
        }

        /// <summary>
        /// Liefert das Customfield "startDate" des pmWorkpackage
        /// </summary>
        /// <param name="c_id">ID des pmWorkpackages</param>
        /// <param name="Error">Fehler: Datenbank ?</param>
        /// <returns></returns>
        public DateTime GetStartDatePMWorkpackage(string c_id, out string Error)
        {
            DateTime StartDate = new DateTime();
            Error = "";
            string SQL = "SELECT polarion.cf_workitem.c_dateonly_value " +
                         "FROM polarion.workitem " +
                         "INNER JOIN polarion.cf_workitem ON polarion.workitem.c_pk = polarion.cf_workitem.fk_workitem " +
                         "WHERE(((polarion.workitem.c_id) = '" + c_id + "') AND((polarion.cf_workitem.c_name) = 'startDate')) ";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(SQL, connection);

            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    DateTime.TryParse(reader[0].ToString(), out StartDate);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Error = ex.Message;
                // AH CloseConnection();
            }

            return StartDate;
        }

        public PlanApiDB GetPlanByID(string Project_c_id, string Plan_c_id, out string Error)
        {
            Error = "";
            PlanApiDB plan = null;
            DateTime Datehelper;

            string SQL = "SELECT polarion.project.c_id, " +         // 0
                                "polarion.plan.c_pk, " +            // 1
                                "polarion.plan.c_id, " +            // 2
                                "polarion.plan.c_name, " +          // 3
                                "polarion.plan.c_startdate, " +     // 4
                                "polarion.plan.c_duedate, " +       // 5
                                "polarion.plan.c_startedon, " +     // 6
                                "polarion.plan.c_finishedon, " +    // 7
                                "polarion.plan.c_status, " +        // 8
                                "polarion.plan.fk_parent, " +       // 9
                                "polarion.plan.c_updated, " +       // 10
                                "polarion.plan.c_sortorder " +      // 11
                         "FROM polarion.project " +
                         "INNER JOIN polarion.plan ON polarion.project.c_pk = polarion.plan.fk_project " +
                         "WHERE((polarion.project.c_id = '" + Project_c_id + "') AND (polarion.plan.c_id = '" + Plan_c_id + "'));";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(SQL, connection);

            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    plan = new PlanApiDB();
                    plan.c_pk = Convert.ToInt32(reader[1]);
                    plan.c_id = reader[2].ToString();
                    plan.c_name = reader[3].ToString();
                    if (DateTime.TryParse(reader[4].ToString(), out Datehelper)) plan.c_startdate = Datehelper;
                    if (DateTime.TryParse(reader[5].ToString(), out Datehelper)) plan.c_duedate = Datehelper;
                    if (DateTime.TryParse(reader[6].ToString(), out Datehelper)) plan.c_startedon = Datehelper;
                    if (DateTime.TryParse(reader[7].ToString(), out Datehelper)) plan.c_finishedon = Datehelper;
                    plan.c_status = reader[8].ToString();
                    if (reader[9] != DBNull.Value) plan.fk_parent = Convert.ToInt32(reader[9]);
                    if (DateTime.TryParse(reader[10].ToString(), out Datehelper)) plan.c_updated = Datehelper;
                    if (reader[11] != DBNull.Value) plan.c_sortorder = Convert.ToInt32(reader[11]);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Error = ex.Message;
                CloseConnection();
                return null;
            }

            return plan;
        }

        public PlanApiDB GetPlanByWBSCode(string Project_c_id, string WBSCode, out string Error)
        {
            Error = "";
            PlanApiDB plan = null;
            DateTime Datehelper;

            string SQL = "SELECT polarion.project.c_id, " +         // 0
                                "polarion.plan.c_pk, " +            // 1
                                "polarion.plan.c_id, " +            // 2
                                "polarion.plan.c_name, " +          // 3
                                "polarion.plan.c_startdate, " +     // 4
                                "polarion.plan.c_duedate, " +       // 5
                                "polarion.plan.c_startedon, " +     // 6
                                "polarion.plan.c_finishedon, " +    // 7
                                "polarion.plan.c_status, " +        // 8
                                "polarion.plan.fk_parent, " +       // 9
                                "polarion.plan.c_updated, " +       // 10
                                "polarion.plan.c_sortorder " +      // 11
                         "FROM polarion.project " +
                         "INNER JOIN polarion.plan ON polarion.project.c_pk = polarion.plan.fk_project " +
                         "WHERE((polarion.project.c_id = '" + Project_c_id + "') AND (polarion.plan.c_name like '" + WBSCode + " %'));";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(SQL, connection);

            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    plan = new PlanApiDB();
                    plan.c_pk = Convert.ToInt32(reader[1]);
                    plan.c_id = reader[2].ToString();
                    plan.c_name = reader[3].ToString();
                    if (DateTime.TryParse(reader[4].ToString(), out Datehelper)) plan.c_startdate = Datehelper;
                    if (DateTime.TryParse(reader[5].ToString(), out Datehelper)) plan.c_duedate = Datehelper;
                    if (DateTime.TryParse(reader[6].ToString(), out Datehelper)) plan.c_startedon = Datehelper;
                    if (DateTime.TryParse(reader[7].ToString(), out Datehelper)) plan.c_finishedon = Datehelper;
                    plan.c_status = reader[8].ToString();
                    if (reader[9] != DBNull.Value) plan.fk_parent = Convert.ToInt32(reader[9]);
                    if (DateTime.TryParse(reader[10].ToString(), out Datehelper)) plan.c_updated = Datehelper;
                    if (reader[11] != DBNull.Value) plan.c_sortorder = Convert.ToInt32(reader[11]);
                }
                reader.Close();

                if (plan == null && WBSCode.Length>1)
                {
                    return GetPlanByWBSCode(Project_c_id, WBSCode.Substring(0, WBSCode.LastIndexOf('.')), out Error);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Error = ex.Message;
                CloseConnection();
                return null;
            }

            return plan;
        }

        public string PlanIdByNameSortorder(string projectid, string name, int sortorder, out string Error)
        {
            Error = "";
            string planId = "";
            string SQL = "SELECT polarion.plan.c_id " +
                         "FROM polarion.project " +
                         "INNER JOIN polarion.plan ON polarion.project.c_pk = polarion.plan.fk_project " +
                         "WHERE(((polarion.project.c_id) = '" + projectid + "') " +
                         "AND((polarion.plan.c_sortorder) = " + sortorder.ToString() + ") " +
                         "AND((polarion.plan.c_name) = '" + name + "'))";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(SQL, connection);
            NpgsqlDataReader reader = command.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    planId = reader[0].ToString();
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Error = ex.Message;
                reader.Close();
                CloseConnection();
                return null;
            }

            return planId;
        }

        public List<PlanApiDB> GetPlanForProject(string Project_c_id, out string Error)
        {
            Error = "";
            DateTime Datehelper;
            List<PlanApiDB> PlanApiDBs = new List<PlanApiDB>();
            PlanApiDB plan;

            string SQL = "SELECT polarion.project.c_id, " +         // 0
                                "polarion.plan.c_pk, " +            // 1
                                "polarion.plan.c_id, " +            // 2
                                "polarion.plan.c_name, " +          // 3
                                "polarion.plan.c_startdate, " +     // 4
                                "polarion.plan.c_duedate, " +       // 5
                                "polarion.plan.c_startedon, " +     // 6
                                "polarion.plan.c_finishedon, " +    // 7
                                "polarion.plan.c_status, " +        // 8
                                "polarion.plan.fk_parent, " +       // 9
                                "polarion.plan.c_updated, " +       // 10
                                "polarion.plan.c_sortorder, " +     // 11
                                "polarion.plan.fk_template " +      // 12
                         "FROM polarion.project " +
                         "INNER JOIN polarion.plan ON polarion.project.c_pk = polarion.plan.fk_project " +
                         "WHERE(((polarion.project.c_id) = '" + Project_c_id + "'));";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(SQL, connection);

            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    plan = new PlanApiDB();
                    plan.c_pk = Convert.ToInt32(reader[1]);
                    plan.c_id = reader[2].ToString();
                    plan.c_name = reader[3].ToString();
                    if (DateTime.TryParse(reader[4].ToString(), out Datehelper)) plan.c_startdate = Datehelper;
                    if (DateTime.TryParse(reader[5].ToString(), out Datehelper)) plan.c_duedate = Datehelper;
                    if (DateTime.TryParse(reader[6].ToString(), out Datehelper)) plan.c_startedon = Datehelper;
                    if (DateTime.TryParse(reader[7].ToString(), out Datehelper)) plan.c_finishedon = Datehelper;
                    plan.c_status = reader[8].ToString();
                    if (reader[9] != DBNull.Value) plan.fk_parent = Convert.ToInt32(reader[9]);
                    if (DateTime.TryParse(reader[10].ToString(), out Datehelper)) plan.c_updated = Datehelper;
                    if (reader[11] != DBNull.Value) plan.c_sortorder = Convert.ToInt32(reader[11]);
                    if (reader[12] != DBNull.Value) plan.fk_template = Convert.ToInt32(reader[12]);
                    PlanApiDBs.Add(plan);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Error = ex.Message;
                CloseConnection();
            }

            return PlanApiDBs;
        }

        public List<Api.Downlink> GetWorkitemDownlinks(string WorkitemId, out string Error)
        {
            List<Api.Downlink> Downlinks = new List<Api.Downlink>();
            Api.Downlink dl;
            Error = "";
            string SQL = "SELECT polarion.workitem.c_id, " +                            // 0
                                "polarion.workitem.c_title, " +                         // 1
                                "polarion.workitem.c_type, " +                          // 2
                                "polarion.struct_workitem_linkedworkitems.c_role, " +   // 3
                                "workitem_1.c_id, " +                          // 4
                                "workitem_1.c_title, " +                       // 5
                                "workitem_1.c_type " +                         // 6
                                "FROM (polarion.workitem " +            
                                "INNER JOIN polarion.struct_workitem_linkedworkitems ON polarion.workitem.c_pk = polarion.struct_workitem_linkedworkitems.fk_workitem) " +
                                "INNER JOIN polarion.workitem AS workitem_1 ON polarion.struct_workitem_linkedworkitems.fk_p_workitem = workitem_1.c_pk " +
                                "WHERE ((polarion.workitem.c_id = '" + WorkitemId + "'))";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(SQL, connection);

            try
            {
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    dl = new Api.Downlink();
                    dl.Up = new WorkitemBase();
                    dl.Up.c_id = reader[0].ToString();
                    dl.Up.c_title = reader[1].ToString();
                    dl.Up.c_type = reader[2].ToString();
                    dl.c_role = reader[3].ToString();
                    dl.Down.c_id = reader[4].ToString();
                    dl.Down.c_title = reader[5].ToString();
                    dl.Down.c_type = reader[6].ToString();
                    Downlinks.Add(dl);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Error = ex.Message;
                CloseConnection();
            }

            return Downlinks;
        }

        public List<Api.Uplink> GetWorkitemUplinks(string WorkitemId, out string Error)
        {
            Error = "";
            List<Api.Uplink> Uplinks = new List<Api.Uplink>();
            Api.Uplink ul;
            NpgsqlDataReader reader = null;

            string SQL = "SELECT polarion.workitem.c_id, " +
                                "polarion.workitem.c_title, " +
                                "polarion.workitem.c_type, " +
                                "polarion.struct_workitem_linkedworkitems.c_role, " +
                                "workitem_1.c_id, " +
                                "workitem_1.c_title, " +
                                "workitem_1.c_type " +
                         "FROM (polarion.workitem " +
                         "INNER JOIN polarion.struct_workitem_linkedworkitems ON polarion.workitem.c_pk = polarion.struct_workitem_linkedworkitems.fk_p_workitem) " +
                         "INNER JOIN polarion.workitem AS workitem_1 ON polarion.struct_workitem_linkedworkitems.fk_workitem = workitem_1.c_pk " +
                         "WHERE (polarion.workitem.c_id = '" + WorkitemId + "')";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(SQL, connection);

            try
            {
                reader = command.ExecuteReader();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Error = ex.Message;
                CloseConnection();
            }
            try
            {
                while (reader.Read())
                {
                    ul = new Api.Uplink();
                    ul.Down = new WorkitemBase();
                    ul.Up = new WorkitemBase();
                    ul.Down.c_id = reader[0].ToString();
                    ul.Down.c_title = reader[1].ToString();
                    ul.Down.c_type = reader[2].ToString();
                    ul.c_role = reader[3].ToString();
                    ul.Up.c_id = reader[4].ToString();
                    ul.Up.c_title = reader[5].ToString();
                    ul.Up.c_type = reader[6].ToString();
                    Uplinks.Add(ul);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Error = ex.Message;
                reader.Close();
                CloseConnection();
            }

            return Uplinks;
        }

        /// <summary>
        /// Liefert alle Workpackages die zu einem Plan gehören (plannedin)
        /// </summary>
        /// <param name="projectPK"></param>
        /// <param name="planId"></param>
        /// <param name="error"></param>
        /// <returns>List<GanttWorkpackage></returns>
        public List<GanttWorkpackage> GetWorkpackageForPlan(int projectPK, string planId, out string error)
        {
            List<GanttWorkpackage> gwpl = new List<GanttWorkpackage>();
            GanttWorkpackage gwp;
            NpgsqlDataReader reader = null;
            DateTime Datehelper;
            int intHelper;
            error = "";

            string SQL = "SELECT polarion.workitem.c_pk, " +                // 0
                                "polarion.workitem.c_id, " +                // 1
                                "polarion.workitem.c_title, " +             // 2
                                "polarion.workitem.c_status, " +            // 3
                                "polarion.workitem.c_duedate, " +           // 4
                                "polarion.workitem.c_initialestimate, " +   // 5
                                "polarion.workitem.c_timespent " +          // 6
                         "FROM (polarion.plan " +
                         "INNER JOIN polarion.struct_plan_records ON polarion.plan.c_pk = polarion.struct_plan_records.fk_p_plan) " +
                         "INNER JOIN polarion.workitem ON polarion.struct_plan_records.fk_item = polarion.workitem.c_pk " +
                         "WHERE (((polarion.plan.fk_project) = " + projectPK.ToString() +") " +
                         "AND ((polarion.plan.c_id) = '" + planId + "') " +
                         "AND ((polarion.workitem.c_type) = 'workPackage'));";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(SQL, connection);

            try
            {
                reader = command.ExecuteReader();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                error = ex.Message;
                CloseConnection();
            }
            try
            {
                while (reader.Read())
                {
                    gwp = new GanttWorkpackage();
                    gwp.c_pk = Convert.ToInt32(reader[0]);
                    gwp.c_id = reader[1].ToString();
                    gwp.c_title = reader[2].ToString();
                    gwp.c_status = reader[3].ToString();
                    if (DateTime.TryParse(reader[4].ToString(), out Datehelper)) gwp.DueDate = Datehelper;
                    if (Int32.TryParse(reader[5].ToString(), out intHelper)) gwp.c_initialestimate = intHelper;
                    if (Int32.TryParse(reader[6].ToString(), out intHelper)) gwp.c_timespent = intHelper;

                    gwpl.Add(gwp);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                error = ex.Message;
                reader.Close();
                CloseConnection();
            }

            return gwpl;
        }

        public List<DocSignatureStatus> GetDocSignatureStatus(int docPK, out string error)
        {
            error = "";
            NpgsqlDataReader reader = null;
            List<DocSignatureStatus> dssl = new List<DocSignatureStatus>();
            DocSignatureStatus dss;
            
            int intHelper;

            string SQL = "SELECT " +
                "polarion.struct_documentworkflowsignature_signatures.c_verdict, " +            // 0
                "polarion.struct_documentworkflowsignature_signatures.c_verdicttime, " +        // 1
                "polarion.struct_documentworkflowsignature_signatures.c_signedrevision, " +     // 2
                "polarion.struct_documentworkflowsignature_signatures.c_signerrole," +          // 3
                "polarion.t_user.c_name, " +                                                    // 4
                "polarion.module.c_status, " +                                                  // 5
                "polarion.modulecomment.c_created, " +                                          // 6
                "polarion.modulecomment.c_text " +                                              // 7
            "FROM (((polarion.documentworkflowsignature " +
            "INNER JOIN polarion.struct_documentworkflowsignature_signatures " +
            "ON polarion.documentworkflowsignature.c_pk = polarion.struct_documentworkflowsignature_signatures.fk_p_documentworkflowsignature) " +
            "INNER JOIN polarion.t_user ON polarion.struct_documentworkflowsignature_signatures.fk_signedby = polarion.t_user.c_pk) " +
            "INNER JOIN polarion.module ON polarion.documentworkflowsignature.fk_workflowobject = polarion.module.c_pk) " +
            "LEFT JOIN polarion.modulecomment ON polarion.struct_documentworkflowsignature_signatures.fk_verdictcomment = polarion.modulecomment.c_pk " +
            "WHERE (((polarion.documentworkflowsignature.fk_workflowobject) = " + docPK.ToString() + ") " +
            "AND ((polarion.modulecomment.c_tags) = '{signatures}' OR (polarion.modulecomment.c_tags) Is Null)) " +
            "ORDER BY polarion.struct_documentworkflowsignature_signatures.c_verdicttime desc";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(SQL, connection);

            try
            {
                reader = command.ExecuteReader();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                error = ex.Message;
                CloseConnection();
            }
            try
            {
                while (reader.Read())
                {
                    dss = new DocSignatureStatus();
                    dss.c_verdict = reader[0].ToString();
                    if (DateTime.TryParse(reader[1].ToString(), out DateTime Datehelper)) dss.c_verdicttime = Datehelper;
                    if (Int32.TryParse(reader[2].ToString(), out intHelper)) dss.c_signedrevision = intHelper;
                    dss.c_signerrole = reader[3].ToString();
                    dss.c_name = reader[4].ToString();
                    dss.c_status = reader[5].ToString();
                    if (DateTime.TryParse(reader[6].ToString(), out DateTime Datehelper2)) dss.c_created = Datehelper2;
                    dss.c_text = reader[7].ToString();
                    if (dss.c_text.Length > 12) dss.c_text = dss.c_text.Substring(12);
                    dssl.Add(dss);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                error = ex.Message;
                reader.Close();
                CloseConnection();
            }
            return dssl;
        }

        public List<TestcaseResult> GetTestcaseResults(string WorkitemId, out string error)
        {
            error = "";
            NpgsqlDataReader reader = null;
            List<TestcaseResult> trl = new List<TestcaseResult>();
            TestcaseResult tr;
            string sql =
                "SELECT polarion.struct_testrun_records.c_executed, " +
                       "polarion.struct_testrun_records.c_result, " +
                       "polarion.t_user.c_name " +
                "FROM (polarion.workitem " +
                "INNER JOIN polarion.struct_testrun_records ON polarion.workitem.c_pk = polarion.struct_testrun_records.fk_testcase) " +
                "INNER JOIN polarion.t_user ON polarion.struct_testrun_records.fk_executedby = polarion.t_user.c_pk " +
                "WHERE (polarion.workitem.c_id = '" + WorkitemId + "') " +
                "ORDER BY polarion.struct_testrun_records.c_executed DESC; ";

            CheckConnection();
            NpgsqlCommand command = new NpgsqlCommand(sql, connection);

            try
            {
                reader = command.ExecuteReader();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                error = ex.Message;
                CloseConnection();
            }

            try
            {
                while (reader.Read())
                {
                    tr = new TestcaseResult();
                    if (DateTime.TryParse(reader[0].ToString(), out DateTime Datehelper)) tr.c_executed = Datehelper;
                    tr.c_result = reader[1].ToString();
                    tr.c_name = reader[2].ToString();
                    trl.Add(tr);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                error = ex.Message;
                reader.Close();
                CloseConnection();
            }

            return trl;
        }
    }
    #endregion

}