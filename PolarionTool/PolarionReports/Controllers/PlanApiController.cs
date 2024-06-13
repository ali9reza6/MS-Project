using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PolarionReports.Models.Database.Api;
using PolarionReports.Models.Database;
using PolarionReports.Models.MSProjectApi;
using PolarionReports.BusinessLogic.Api;
using PolarionReports.BusinessLogic;
using ZKW.Polarion.AddIn.Tools;
using System.Diagnostics;
using Serilog;

namespace PolarionReports.Controllers
{
    public class PlanApiController : ApiController
    {
        private string USER { get; set; }
        private string PASSWORD { get; set; }

        // GET api/<controller>
        public IEnumerable<string> Get() 
        {  
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/{id}  where id is project id
        /// <summary>
        /// WHOLE GET NETHOD NOT BEING USED AT THE MOMENT
        /// Returns the ProjectModel:
        /// Alle PolarionPläne mit den pmWorkPackage Workitems zusammengeführt für MS-Project 
        /// </summary>
        /// <param name="id">Id of the Polarion Project</param>
        /// <returns>ProjectModel</returns>
        public HttpResponseMessage Get(string id)
        {
            TaskReader taskreader = new TaskReader();
            ProjectModel pm = new ProjectModel();
            DatareaderP dr = new DatareaderP();
            PolarionInit pi = new PolarionInit();

            Log.Debug("PlanApiController Get");
            
            bool loginOK = CheckUserAndPasswordHeader();
            if (!loginOK)
            {
                Log.Error("PlanApiController Get - Benutzerdaten ungültig");
                return Request.CreateResponse(HttpStatusCode.Unauthorized, new HttpError("Benutzerdaten ungültig"));
            }

            if (string.IsNullOrWhiteSpace(id))
            {
                Log.Error("PlanApiController Get ProjectId nicht angegeben");
                return Request.CreateResponse(HttpStatusCode.BadRequest, new HttpError("ProjectId nicht angegeben"));
            }

            Connection con = pi.Init(USER, PASSWORD);
            if (con == null)
            {
                Log.Error("PlanApiController Get - Polarion Login not sucessful");
                return Request.CreateResponse(HttpStatusCode.Unauthorized, new HttpError("Benutzerdaten ungültig"));
            }

            if (id.Contains("("))
            {
                // Baseplan in ID
                string[] parts = id.Split('(');
                pm.ProjectID = parts[0];
                pm.Baseplan = parts[1].Substring(0, parts[1].Length - 1);
            }
            else
            {
                pm.ProjectID = id;
                if (string.IsNullOrWhiteSpace(pm.Baseplan)) pm.Baseplan = "PMS-Template";
            }

            Log.Debug("PlanApiController Get - ProjectId:" + pm.ProjectID + " Baseplan:" + pm.Baseplan);
            List<PlanApiDB> plans = dr.GetPlanForProject(pm.ProjectID, out string error);
            pm.Tasks = taskreader.GetPMTasks(dr, plans, pm.Baseplan);

            dr.CloseConnection();

            Log.Debug("PlanApiController Get - Number of Tasks:" + pm.Tasks.Count.ToString());
            return Request.CreateResponse(HttpStatusCode.OK, pm);
        }

        // POST api/<controller>
        /// <summary>
        /// neuen task in Polarion einfügen
        ///  Es sollte nur ein task in der taskliste sein                 Anworten:
        ///  zu klären: dürfen mehrere tasks eingefügt werden ?           nein -> getrennte Aufrufe (zu viele Fehlermöglichkeiten)
        /// </summary>
        /// <param name="msp"></param>
        public HttpResponseMessage Post([FromBody]ProjectModel msp)
        {
            bool loginOK = CheckUserAndPasswordHeader();
            if (!loginOK)
            {
                Log.Error("PlanApiController Post - Benutzerdaten ungültig");
                return Request.CreateResponse(HttpStatusCode.Unauthorized, new HttpError("Benutzerdaten ungültig"));
            }

            if (string.IsNullOrWhiteSpace(msp.ProjectID))
            {
                Log.Error("PlanApiController Post - Benutzerdaten ungültig");
                return Request.CreateResponse(HttpStatusCode.BadRequest, new HttpError("ProjectId nicht angegeben"));
            }

            if (msp.Tasks == null || msp.Tasks.Count != 1)
            {
                Log.Error("PlanApiController Post - Anzahl Task <> 1");
                return Request.CreateResponse(HttpStatusCode.BadRequest, new HttpError("Anzahl Task <> 1"));
            }

            msp.Username = USER;
            msp.Password = PASSWORD;

            if (msp.ProjectID.Contains("("))
            {
                // Baseplan in ID
                string[] parts = msp.ProjectID.Split('(');
                msp.ProjectID = parts[0];
                msp.Baseplan = parts[1].Substring(0, parts[1].Length - 1);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(msp.Baseplan)) msp.Baseplan = "PMS-Template";
            }

            Log.Debug("PlanApiController Post - ProjectId:" + msp.ProjectID + " Baseplan:" + msp.Baseplan);

            InsertTask insertTask = new InsertTask();
            // TODO:AH ab Ebene x nur mehr Workitem einfügen und mit übergeorneten Plan verbinden
            if (insertTask.Insert(msp, out ApiError Error))   
            {
                Log.Debug("PlanApiController Post - Insert OK - WBSCode:" + msp.Tasks[0].WBSCode);
                return Request.CreateResponse(HttpStatusCode.OK, msp);
            }
            else
            {
                Log.Error("PlanApiController Post - Insert Error - StatusCode:" + Error.StatusCode.ToString() + " Message:" + Error.Message);
                HttpResponseMessage m = new HttpResponseMessage();
                m.StatusCode = Error.StatusCode;
                m.ReasonPhrase = Error.Message;
                throw new HttpResponseException(m);
            }
        }

        // PUT api/<controller>/5
        /// <summary>
        /// Änderungen in den tasks werden in Polarion übernommen
        /// Es sollten keine neuen Tasks oder gelöschte Tasks in der Liste sein /Itfilters them in PolarionRibbon by wiId/planId
        /// @@@ zu klären: Fehlermöglichkeiten:
        ///                Pläne / Workpackeges wurden in Polarion gelöscht !!
        ///                Pläne / Workpackeges wurden in Polarion erstellt !!
        /// the put returns an error if the number of tasks does not match
        /// 
        /// Changes in the tasks of Polarion
        /// There should be no new tasks or deleted tasks in the list
        /// 
        /// </summary>
        /// <param name="msp"></param>
        public HttpResponseMessage Put([FromBody]ProjectModel msp)
        {
            Log.Debug("PlanApiController PUT");
            bool loginOK = CheckUserAndPasswordHeader();
            if (!loginOK)
            {
                Log.Error("PlanApiController PUT - Benutzerdaten ungültig");
                return Request.CreateResponse(HttpStatusCode.Unauthorized, new HttpError("Benutzerdaten ungültig"));
            }
            if (msp == null)
            {
                Log.Error("PlanApiController PUT - JSON structure of the datamodel is empty");
                HttpResponseMessage m = new HttpResponseMessage();
                m.StatusCode = HttpStatusCode.BadRequest;
                m.ReasonPhrase = "JSON structure of the datamodel is empty";
                throw new HttpResponseException(m);
            }

            msp.Username = USER;
            msp.Password = PASSWORD;

            if (msp.ProjectID.Contains("("))
            {
                // Baseplan in ID
                string[] parts = msp.ProjectID.Split('(');
                msp.ProjectID = parts[0];
                msp.Baseplan = parts[1].Substring(0, parts[1].Length - 1);
            }
            else
            {
                msp.Baseplan = "PMS-Template";    //Default plan ID
            }

            try
            {
                Log.Debug("PlanApiController PUT - ProjectId:" + msp.ProjectID + " Baseplan:" + msp.Baseplan);

                UpdateTasks update = new UpdateTasks();
                // TODO: Ab Ebene x nur Workitem Update
                msp = update.Update(msp, out ApiError Error); 
                if(msp != null) // Update Task
                {
                    // Alles OK
                    Log.Debug("PlanApiController PUT - Update OK");
                    return Request.CreateResponse(HttpStatusCode.OK, msp);
                }
                else
                {
                    Log.Error("PlanApiController PUT - Update Error - StatusCode:" + Error.StatusCode.ToString() + " Message:" + Error.Message);
                    HttpResponseMessage m = new HttpResponseMessage();
                    m.StatusCode = Error.StatusCode;
                    m.ReasonPhrase = Error.Message;
                    throw new HttpResponseException(m);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Temp Error: at PlanApiController.cs PUT method" + ex.Message);
                throw;
            }
            
        }

        // DELETE api/<controller>/5
        /// <summary>
        /// Ein Plan und damit verbundenes Workitem werden gelöscht
        /// @@@ zu klären - rekursives löschen ? -> Nein Rekursives löschen sollte vom Aufrufer implementiert werden (von unten nach oben)
        ///                 gibt es in MS-Project einen Event beforeDelete ?

        /// Überprüfen, ob der Plan / Workpackage gelöscht werden darf:
        ///    Plan -> darf nur ein WP verknüpft haben
        ///    WP darf keine Verknüpfungen haben (Workitems: (WP oder Task))
        /// </summary>
        /// <param name="MSP"></param>
        public void Delete([FromBody]ProjectModel msp)
        {
            Log.Debug("PlanApiController DELETE");
            bool loginOK = CheckUserAndPasswordHeader();
            if (!loginOK)
            {
                Log.Error("PlanApiController DELETE - Benutzerdaten ungültig");
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Unauthorized, new HttpError("Benutzerdaten ungültig")));
            }

            msp.Username = USER;
            msp.Password = PASSWORD;

            if (msp.ProjectID.Contains("("))
            {
                // Baseplan in ID
                string[] parts = msp.ProjectID.Split('(');
                msp.ProjectID = parts[0];
                msp.Baseplan = parts[1].Substring(0, parts[1].Length - 1);
            }
            else
            {
                msp.Baseplan = "PM_Plan_Template_E";
            }

            Log.Debug("PlanApiController DELETE - ProjectId:" + msp.ProjectID + " Baseplan:" + msp.Baseplan);

            // TODO: AH
            DeleteTask d = new DeleteTask();
            if (d.delete(msp, out ApiError Error))
            {
                // Alles OK
                Log.Debug("PlanApiController DELETE - OK");
            }
            else
            {
                // Fehler:
                Log.Error("PlanApiController DELETE - Error - StatusCode:" + Error.StatusCode.ToString() + " Message:" + Error.Message);
                HttpResponseMessage m = new HttpResponseMessage();
                m.StatusCode = HttpStatusCode.BadRequest;
                m.ReasonPhrase = Error.Message;
                throw new HttpResponseException(m);
            }
        }

        #region Helpers

        private bool CheckUserAndPasswordHeader()
        {
            bool loginOK = false;
#if DEBUG

            //loginOK = true;
            //USER = Request.Headers.GetValues("POLUSER").FirstOrDefault();
            //PASSWORD = Request.Headers.GetValues("POLPWD").FirstOrDefault();
            //return loginOK;
#endif
            if (Request.Headers != null)
            {
                var user = Request.Headers.GetValues("POLUSER").FirstOrDefault();
                var pwd = Request.Headers.GetValues("POLPWD").FirstOrDefault();
                loginOK = !string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(pwd);
                try
                {
                    USER = SymetricKeyEnDecryption.DecryptString(user); //error here
                    PASSWORD = SymetricKeyEnDecryption.DecryptString(pwd);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    loginOK = false;
                }
            }

            return loginOK;
        }

        #endregion
    }
}