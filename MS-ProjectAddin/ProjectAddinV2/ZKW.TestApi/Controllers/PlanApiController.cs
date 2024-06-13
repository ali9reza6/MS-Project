using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ZKW.Shared.MSProjectApi;
using System.Linq;
using System.Diagnostics;
using ZKW.Polarion.AddIn.Tools;


namespace PolarionReports.Controllers
{
    public class PlanApiController : ApiController
    {
        private string USER { get; set; }
        private string PASSWORD { get; set; }
       
        /// <summary>
        /// Returns the ProjectModel:
        /// All Polarion plans with the pmWorkPackage work items together for MS Project
        /// </summary>
        /// <param name="id"></param>
        /// <returns>ProjectModel</returns>
        public HttpResponseMessage Get(string id)
        {
            bool loginOK = CheckUserAndPasswordHeader();
            if (!loginOK) return Request.CreateResponse(HttpStatusCode.Unauthorized, new HttpError("Benutzerdaten ungültig"));
            if (string.IsNullOrWhiteSpace(id)) return Request.CreateResponse(HttpStatusCode.BadRequest, new HttpError("ProjectId nicht angegeben"));

            ProjectModel pm = new ProjectModel();
            using (StreamReader r = new StreamReader($"{Path.GetTempPath()}test.json"))
            {
                string json = r.ReadToEnd();
                pm = JsonConvert.DeserializeObject<ProjectModel>(json);
            }
            return Request.CreateResponse(HttpStatusCode.OK, pm);
        }
        

        /// <summary>
        /// neuen task in Polarion einfügen
        /// Es sollte nur ein task in der taskliste sein                     Anworten:
        /// @@@ zu klären: dürfen mehrere tasks eingefügt werden ?           nein -> getrennte Aufrufe (zu viele Fehlermöglichkeiten)
        ///                dürfen auch geänderte tasks in der Liste sein ?   nein -> getrennte Aufrufe 
        /// </summary>
        /// <param name="task"></param>
        // POST api/<controller>
        public HttpResponseMessage Post([FromBody]ProjectModel msp)
        {
            bool loginOK = CheckUserAndPasswordHeader();
            if (!loginOK) return Request.CreateResponse(HttpStatusCode.Unauthorized, new HttpError("Benutzerdaten ungültig"));
            if (string.IsNullOrWhiteSpace(msp.ProjectID)) return Request.CreateResponse(HttpStatusCode.BadRequest, new HttpError("ProjectId nicht angegeben"));
            if (msp.Tasks == null || msp.Tasks.Count != 1) return Request.CreateResponse(HttpStatusCode.BadRequest, new HttpError("Anzahl Task <> 1"));

            // Hier liefer ich einmal Fakewerte zum Testen
            msp.Tasks[0].WorkitemId = Guid.NewGuid().ToString();
            msp.Tasks[0].PlanId = Guid.NewGuid().ToString();

            Debug.WriteLine($"+++ POST Projekt: {msp.ProjectID} Task: {msp.Tasks.FirstOrDefault()?.ProjectId} / {msp.Tasks.FirstOrDefault()?.WorkitemId} using ({USER}/{PASSWORD}");

            return Request.CreateResponse(HttpStatusCode.OK, msp);
        }

        /// <summary>
        /// Änderungen in den tasks werden in Polarion übernommen
        /// Es sollten keine neuen Tasks oder gelöschte Tasks in der Liste sein
        ///  @@@ zu klären: Fehlermöglichkeiten:
        ///                  Pläne / Workpackeges wurden in Polarion gelöscht !!
        ///                  Pläne / Workpackeges wurden in Polarion erstellt !!
        /// </summary>
        /// <param name="id"></param>
        /// <param name="task"></param>
        // PUT api/<controller>/5
        public HttpResponseMessage Put([FromBody]ProjectModel msp)
        {
            bool loginOK = CheckUserAndPasswordHeader();
            if (!loginOK) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Unauthorized, new HttpError("Benutzerdaten ungültig")));

            Debug.WriteLine($"+++ PUT Projekt: {msp.ProjectID} using ({USER}/{PASSWORD}");

            ProjectModel pm = new ProjectModel();
            using (StreamReader r = new StreamReader($"{Path.GetTempPath()}test.json"))
            {
                string json = r.ReadToEnd();
                pm = JsonConvert.DeserializeObject<ProjectModel>(json);
            }
            return Request.CreateResponse(HttpStatusCode.OK, pm);
        }

        /// <summary>
        /// Based on WorkItem/PlanId
        /// </summary>
        /// <param name="id"></param>
        // DELETE api/<controller>/5
        public void Delete([FromBody]ProjectModel msp)
        {
            bool loginOK = CheckUserAndPasswordHeader();
            if (!loginOK) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Unauthorized, new HttpError("Benutzerdaten ungültig")));
            if (string.IsNullOrWhiteSpace(msp.ProjectID)) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, new HttpError("ProjectId nicht angegeben")));
            if (msp.Tasks == null || msp.Tasks.Count != 1) throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest, new HttpError("Anzahl Task <> 1")));

            // TODO: Delete the Task
            Debug.WriteLine($"+++ DELETE Projekt: {msp.ProjectID} Task: {msp.Tasks.FirstOrDefault()?.ProjectId} / {msp.Tasks.FirstOrDefault()?.WorkitemId} using ({USER}/{PASSWORD}");
        }

        #region Helpers

        private bool CheckUserAndPasswordHeader()
        {
            bool loginOK = false;
            if (Request.Headers != null)
            {
                var user = Request.Headers.GetValues("POLUSER").FirstOrDefault();
                var pwd = Request.Headers.GetValues("POLPWD").FirstOrDefault();
                loginOK = !string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(pwd);
                try
                {
                    USER = SymetricKeyEnDecryption.DecryptString(user);
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
