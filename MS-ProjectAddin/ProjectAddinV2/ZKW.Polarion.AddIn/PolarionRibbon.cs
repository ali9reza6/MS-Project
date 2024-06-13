using Microsoft.Office.Interop.MSProject;
using Microsoft.Office.Tools.Ribbon;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using ZKW.Polarion.AddIn.Tools;
using ZKW.Shared.MSProjectApi;
using Office = Microsoft.Office.Core;


//  Follow these steps to enable the Ribbon (XML) item:

// 1: Copy the following code block into the ThisAddin, ThisWorkbook, or ThisDocument class.

//  protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
//  {
//      return new Ribbon();
//  }

// 2. Create callback methods in the "Ribbon Callbacks" region of this class to handle user
//    actions, such as clicking a button. Note: if you have exported this Ribbon from the Ribbon designer,
//    move your code from the event handlers to the callback methods and modify the code to work with the
//    Ribbon extensibility (RibbonX) programming model.

// 3. Assign attributes to the control tags in the Ribbon XML file to identify the appropriate callback methods in your code.  

// For more information, see the Ribbon XML documentation in the Visual Studio Tools for Office Help. 


namespace ZKW.Polarion.AddIn
{
    [ComVisible(true)]
    public class PolarionRibbon : Office.IRibbonExtensibility
    {
        private Office.IRibbonUI ribbon;

        Progress progress = new Progress();

#if DEBUG
        private static readonly string APIURL = "http://localhost:57556/api/PlanApi/{0}";
#else
        private static readonly string APIURL = "http://wntopol/api/PlanApi/{0}";
        // private static readonly string APIURL = "http://localhost:57556/api/PlanApi/{0}";
#endif

        private static readonly string WORKITEMURL = "http://zkwsvpol01/polarion/#/project/{0}/workitem?id={1}";
        private static readonly string PLANURL = "http://zkwsvpol01/polarion/#/project/{0}/plan?id={1}"; 

        static HttpClient client = new HttpClient();
        static ProjectModel modelToUpdate = null;
        static ProjectModel modelToInsert = null;
        static bool deleteSuccess = false;

        private static string USER = "";
        private static string PASSWORD = "";

        private static bool SyncOff = false;
        private static string syncMessage = "";
        private static bool RunFirstSync = true;

        public static string GetUser
        {
            get
            {
                if (string.IsNullOrEmpty(USER))
                {
                    UserLogin d = new UserLogin();
                    if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        USER = SymetricKeyEnDecryption.EncryptString(d.USER);
                        PASSWORD = SymetricKeyEnDecryption.EncryptString(d.PASSWORD);
                    }
                }
                return USER;
            }
        }

        #region Delete

        static async System.Threading.Tasks.Task RunTaskDeleteAsync(string projektName, Microsoft.Office.Interop.MSProject.Task task)
        {
            SetDefaultHeaders();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            deleteSuccess = await DeleteTaskAsync(projektName, task);
        }

        static async System.Threading.Tasks.Task<bool> DeleteTaskAsync(string projektName, Microsoft.Office.Interop.MSProject.Task task)
        {
            TaskBase taskToDelete = new TaskBase()
            {
                PlanId = task.Text1,
                WorkitemId = task.Text2
            };
            ProjectModel msp = new ProjectModel()
            {
                ProjectID = projektName,
                Tasks = new System.Collections.Generic.List<TaskBase>() { taskToDelete }
            };
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(string.Format(APIURL, "")),
                Content = new StringContent(JsonConvert.SerializeObject(msp), Encoding.UTF8, "application/json")
            };
            var response = await client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        private void Pro_ProjectBeforeTaskDelete(Microsoft.Office.Interop.MSProject.Task tsk, ref bool Cancel)
        {
            if (SyncOff)
            {
                Cancel = false;
                return;
            }

            ShowProgresswindow();

            string projektName = GetProjektName(out string baseplan);
            //var pro = Globals.ThisAddIn.Application;
            RunTaskDeleteAsync(projektName, tsk).GetAwaiter().GetResult();
            Cancel = !deleteSuccess;
            progress.Hide();
        }

        #endregion

        #region Put

        static async System.Threading.Tasks.Task RunProjectPutAsync(string projektName)
        {
            SetDefaultHeaders();
            modelToUpdate = await PutProjectAsync(projektName);  
        }

        static async System.Threading.Tasks.Task<ProjectModel> PutProjectAsync(string projektName)
        {

            ProjectModel msp = new ProjectModel()
            {
                ProjectID = projektName,
                Tasks = new System.Collections.Generic.List<TaskBase>()
            };

            var pro = Globals.ThisAddIn.Application;
            var proTasks = pro.ActiveProject.Tasks;
            foreach (Microsoft.Office.Interop.MSProject.Task protask in proTasks)
            {
                if (Convert.ToInt32(protask.Duration) == 0)
                {   // Skip Task
                    continue;
                }
                TaskBase poltask = new TaskBase()
                {
                    Start = protask.Start,
                    Finish = protask.Finish,
                    Name = protask.Name,
                    Level = protask.OutlineLevel,
                    Milestone = protask.Milestone,
                    ProjectId = protask.ID,
                    PlanId = protask.Text1,
                    WorkitemId = protask.Text2,
                    //Status = protask.Status == PjStatusType.pjComplete ? "closed" : (protask.Status == PjStatusType.pjOnSchedule ? "inprogress" : "open"),
                    Status = protask.Text3,
                    PtRole = protask.Text4,
                    PtProcess = protask.Text5,
                    WBSCode = protask.WBS
                };
                msp.Tasks.Add(poltask);
            }

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri(string.Format(APIURL, "")),
                Content = new StringContent(JsonConvert.SerializeObject(msp), Encoding.UTF8, "application/json")
            };
            var response = await client.SendAsync(request);
            ProjectModel project = null;
            if (response.IsSuccessStatusCode)
            {
                project = await response.Content.ReadAsAsync<ProjectModel>();
            }
            else
            {
                Debug.WriteLine("API Error in PUT"+response.RequestMessage);
                project = new ProjectModel();
                project.Tasks = new System.Collections.Generic.List<TaskBase>();
                project.ErrorMsg = response.ReasonPhrase;
            }
            return project;
        }

        #endregion

        #region [ Post - New Dataset]

        static async System.Threading.Tasks.Task RunProjectPostAsync(string projektName, Microsoft.Office.Interop.MSProject.Task task)
        {
            SetDefaultHeaders();
            modelToInsert = await PostProjectAsync(projektName, task); 
        }

        static async System.Threading.Tasks.Task<ProjectModel> PostProjectAsync(string projektName, Microsoft.Office.Interop.MSProject.Task task)
        {
            ProjectModel msp = new ProjectModel()
            {
                ProjectID = projektName,
                Tasks = new System.Collections.Generic.List<TaskBase>()
            };

            msp.Tasks.Add(new TaskBase()
            {
                Name = task.Name,
                Level = task.OutlineLevel,
                Start = task.Start,
                Finish = task.Finish,
                Milestone = task.Milestone,
                WBSCode = task.WBS,
                PtRole = task.Text4,
            });

            var request = new HttpRequestMessage  
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(string.Format(APIURL, "")),
                Content = new StringContent(JsonConvert.SerializeObject(msp), Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(request);
            ProjectModel project = null;
            if (response.IsSuccessStatusCode)
            {
                project = await response.Content.ReadAsAsync<ProjectModel>();
            }
            else
            {
                // Fehler im API
                Debug.WriteLine("API ERROR: "+response.RequestMessage);
                project = new ProjectModel();
                project.Tasks = new System.Collections.Generic.List<TaskBase>();
                project.ErrorMsg = response.ReasonPhrase;
            }
            return project;
        }

        #endregion

        #region Get

        static async System.Threading.Tasks.Task RunProjectGetAsync(string projektName)
        {
            SetDefaultHeaders();
            modelToUpdate = await GetProjectAsync(projektName);
        }

        static async System.Threading.Tasks.Task<ProjectModel> GetProjectAsync(string id)
        {
            ProjectModel project = null;
            HttpResponseMessage response = await client.GetAsync(string.Format(APIURL, id));
            if (response.IsSuccessStatusCode)
            {
                project = await response.Content.ReadAsAsync<ProjectModel>();
            }
            else
            {
                Debug.WriteLine("API Error:"+response.RequestMessage);
                project = new ProjectModel();
                project.Tasks = new System.Collections.Generic.List<TaskBase>();
                project.ErrorMsg = response.ReasonPhrase;
            }
            return project;
        }

        private static void SetDefaultHeaders()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Remove("POLUSER");
            client.DefaultRequestHeaders.Remove("POLPWD");
            client.DefaultRequestHeaders.Add("POLUSER", GetUser);
            client.DefaultRequestHeaders.Add("POLPWD", PASSWORD);
        }

        private void UpdateProjectFile(ProjectModel model)
        {
            var pro = Globals.ThisAddIn.Application;
            var proTasks = pro.ActiveProject.Tasks;
            bool firstTimeSync = (proTasks.Count == 0);
            if (model == null)
            {
                // Fehler beim Aufruf
                return;
            }
            foreach (var poltask in model.Tasks.OrderBy(x => x.WbsSortOrder))
            {
                bool found = false;
                foreach (Microsoft.Office.Interop.MSProject.Task protask in proTasks)
                {
                    string wbs = protask.WBS??"";
                    string planId = protask.Text1??"";
                    string workId = protask.Text2??"";

                    string polWbs = poltask.WBSCode; //use this to compare + WI-Id != null

                    if (wbs == poltask.WBSCode) //planId == (poltask.PlanId ?? "") && workId == (poltask.WorkitemId ?? "") //how to differentiate the ones that have the old WI-ID in Pol?
                    {
                        found = true;
                        try
                        {
                            bool changed = UpdateProjektTaskFromPolarion(poltask, protask);
                            SetMessage("Update from Polarion: " + poltask.WorkitemId + " " + poltask.Status + " " + protask.Status + " " + poltask.PtRole + "/" + protask.Text4);
                        }
                        catch (System.Exception ex)
                        {
                            SetMessage("Fehler:" + ex.Message + " bei: " + poltask.WorkitemId + " " + poltask.WBSCode);
                        }
                            // 2020-03-17 keine Farbänderung nach Sync lt. Christian Email
                        // if (changed) SetColor(pro, poltask, protask);
                        break;
                    }
                }
                
                //Updates totally new tasks from Polarion to-> MS-project ...which shouldnt be the case

                //if (!found)
                //{
                //    Microsoft.Office.Interop.MSProject.Task protask = AddNewTaskFromPolarion(proTasks, poltask);
                //    //SetColor(pro, poltask, protask);
                //}
            }

            if (firstTimeSync)
            {
                SetMessage("SetParentFromPolarion wird ausgeführt");
                SetParentFromPolarion(model, proTasks);
            }
            //Rearange MS-project headers if they are not right
            if (pro.CustomFieldGetName(PjCustomField.pjCustomTaskText1) != "PlanId")
            {
                pro.CustomFieldRename(PjCustomField.pjCustomTaskText1, "PlanId");
                pro.CustomFieldRename(PjCustomField.pjCustomTaskText2, "WorkItemId");
                pro.CustomFieldRename(PjCustomField.pjCustomTaskText3, "StatusPolarion");
                pro.CustomFieldRename(PjCustomField.pjCustomTaskText4, "AssignedRole");
                pro.CustomFieldRename(PjCustomField.pjCustomTaskText5, "Process");

                pro.TableEditEx("&Entry", true, NewFieldName: "WBS");
                pro.TableEditEx("&Entry", true, NewFieldName: "WorkItemId");
                pro.TableApply("&Entry");
                pro.Sort("WBS", Renumber: false);
            }
        }

        private static void SetColor(Application pro, TaskBase poltask, Task protask)
        {
            //pro.SelectTaskField(protask.ID, Column: "Name", RowRelative: false, Width: 6) ;
            try
            {
                pro.SelectRow(protask.ID, RowRelative: false);
                if (poltask.Status == "created")
                {
                    pro.FontEx(CellColor: PjColor.pjColorAutomatic);

                }
                else if (poltask.Status == "completed")
                {
                    // pro.FontEx(CellColor: PjColor.pjGreen);
                    pro.Font32Ex(CellColor: 11788485);
                }
                else
                {
                    pro.FontEx(CellColor: PjColor.pjSilver);
                }
            }
            catch (System.Exception ex)
            {
                Debug.Write("ERROR: "+ex.Message);
                throw;
            }
        }

        private static bool UpdateProjektTaskFromPolarion(TaskBase poltask, Microsoft.Office.Interop.MSProject.Task protask)
        {
            bool ret = false;
            ret = (protask.Text3??"") != (poltask.Status??""); //We are not using this at the moment, determines if it chnaged or not

            //protask.Name = poltask.Name;  //shouldnt be updated from polarion
            //protask.WBS = poltask.WBSCode; 2020-07-21 WBS Code wird nur im MS-Project verwaltet/geändert
            //protask.PercentComplete = poltask.PercentComplete; Shoulnt be updated

            protask.Text1 = poltask.PlanId; //necessary?
            protask.Text2 = poltask.WorkitemId; //necessary?
            protask.Milestone = poltask.Milestone;
            protask.OutlineLevel = (short)poltask.Level;
            protask.Text3 = (poltask.Status??"");

            string role = (poltask.PtRole??"").Replace("allocationRole_","");   //only supposed to update by first sync MS -> Pol
            protask.Text4 = role;
            protask.Text5 = poltask.PtProcess;
            return ret;
        }

        /// <summary>
        /// Introduces a new task in the project and takes over the Polarion data
        /// </summary>
        /// <param name="proTasks"></param>
        /// <param name="poltask"></param>
        private static Microsoft.Office.Interop.MSProject.Task AddNewTaskFromPolarion(Tasks proTasks, TaskBase poltask)
        {
            Microsoft.Office.Interop.MSProject.Task newtask = proTasks.Add(poltask.Name);
            newtask.Start = poltask.Start.Year > 1 ? poltask.Start : newtask.Start;
            newtask.Finish = poltask.Finish.Year > 1 ? poltask.Finish : newtask.Finish;
            newtask.WBS = poltask.WBSCode;
            newtask.Text1 = poltask.PlanId;
            newtask.Text2 = poltask.WorkitemId;
            newtask.Text3 = poltask.Status;
            newtask.Text4 = poltask.PtRole;
            newtask.Text5 = poltask.PtProcess;
            newtask.Manual = false;
            newtask.Milestone = poltask.Milestone;
            newtask.OutlineLevel = (short)poltask.Level;
            newtask.PercentComplete = poltask.PercentComplete;
            return newtask;
        }

        /// <summary>
        /// Geht alle Task in Projekt durch und setzt die Parents auf Grund von Polarion ParentId
        /// </summary>
        /// <param name="model"></param>
        /// <param name="tasks"></param>
        private static void SetParentFromPolarion(ProjectModel model, Tasks tasks)
        {
            foreach (var poltask in model.Tasks.Where(x => !string.IsNullOrEmpty(x.ParentId)).OrderBy(x => x.PlanId))
            {
                Microsoft.Office.Interop.MSProject.Task parentTask = null;
                foreach (Microsoft.Office.Interop.MSProject.Task protask in tasks)
                {   // Verknüpfung zu PlanId oder workitemid
                    if (protask.Text1 == poltask.ParentId || protask.Text2 == poltask.ParentId)
                    {
                        parentTask = protask;
                        break;
                    }
                }
                if (parentTask != null)
                {
                    foreach (Microsoft.Office.Interop.MSProject.Task protask in tasks)
                    {
                        if (protask.Text2 == poltask.WorkitemId)
                        {
                            try
                            {
                                protask.Predecessors = parentTask.ID.ToString();
                                parentTask.Rollup = true;
                                break;
                            }
                            catch (System.Exception ex)
                            {
                                Debug.WriteLine("Error" + ex.Message);
                            }
                        }
                    }
                }
            }
        }



        #endregion

        #region IRibbonExtensibility Members

        public string GetCustomUI(string ribbonID)
        {
            return GetResourceText("ZKW.Polarion.AddIn.PolarionRibbon.xml");
        }

        #endregion

        #region Ribbon Callbacks
        //Create callback methods here. For more information about adding callback methods, visit https://go.microsoft.com/fwlink/?LinkID=271226

        public void Ribbon_Load(Office.IRibbonUI ribbonUI)
        {
            this.ribbon = ribbonUI;
            var pro = Globals.ThisAddIn.Application;

            //The following subscibes the method (right) to the event (left), which seems to be the removal of a task
            pro.ProjectBeforeTaskDelete += Pro_ProjectBeforeTaskDelete; 
        }

        /// <summary>
        /// label für Sync Meldungen
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public string getlblMessage(Office.IRibbonControl control)
        {
            return syncMessage;
        }

        /// <summary>
        /// Label für Button Sync ON/OFF
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public string getlblbtnTest(Office.IRibbonControl control)
        {
            if (SyncOff)
            {
                syncMessage = " Polarion Sync disabled";
                ribbon.InvalidateControl("lblMessage");
                return "Sync OFF";
            }
            else
            {
                syncMessage = " Polarion Sync enabled";
                ribbon.InvalidateControl("lblMessage");
                return "Sync ON";
            }
        }

        /// <summary>
        /// Sync On/OFF Button
        /// </summary>
        /// <param name="control"></param>
        public void OnPlarionTest_Click(Office.IRibbonControl control)
        {
            SyncOff = !SyncOff;
            ribbon.InvalidateControl("lblMessage");
            ribbon.InvalidateControl("btnTest");
        }

        //Comment, the base plans should beread form MS-Proj, Task with WBS 1, not to be so "Title" dependent
        /// <summary>
        /// Sync Button
        /// </summary>
        /// <param name="control"></param>
        public void OnPolarion_Click(Office.IRibbonControl control)
        {
            if (SyncOff)
            {
                SetMessage(" Polarion Sync is disabled - click on the ON/OFF Button to enable Sync");
                return;
            }

            ShowProgresswindow();

            string projektName = GetProjektName(out string baseplan);
            string projektnameWithBaseplan = GetProjektNameWithBaseplan();
            int i = 0;

            ProjectModel msp = new ProjectModel()
            {
                ProjectID = projektName,
                Baseplan = baseplan,
                Tasks = new System.Collections.Generic.List<TaskBase>()
            };

            var pro = Globals.ThisAddIn.Application;
            var tasks = pro.ActiveProject.Tasks;
            foreach (Microsoft.Office.Interop.MSProject.Task protask in tasks)
            {
                try
                {
                    if (Convert.ToInt32(protask.Duration) == 0 && string.IsNullOrEmpty(protask.Text1) && string.IsNullOrEmpty(protask.Text2))
                    {
                        SetMessage("Skip Duration " + protask.WBS);
                        continue;
                    }
                    //Duration= 0 AND (PlanId or WIId not empty)
                    if (Convert.ToInt32(protask.Duration) == 0 && (!string.IsNullOrEmpty(protask.Text1) || !string.IsNullOrEmpty(protask.Text2))) 
                    {
                        // keine Laufzeit aber noch Verbindung zu Polarion Plan/Workpackage vorhanden
                        RunTaskDeleteAsync(projektnameWithBaseplan, protask).GetAwaiter().GetResult(); //Deletes 
                        protask.Text1 = "";
                        protask.Text2 = "";
                    }
                }
                catch(System.Exception ex)
                {
                    SetMessage("Error (Checking Duration and Deleting Tasks): " + ex.Message);
                    Debug.WriteLine("Error: " + ex.Message);
                    throw;
                }
                // If WI-Id and Plan-ID are empty AND taskDuration > 0   Means New Task, Post Method Called
                try
                {
                    if (string.IsNullOrEmpty(protask.Text1) && string.IsNullOrEmpty(protask.Text2) && Convert.ToInt32(protask.Duration) > 0)
                    {
                        SetMessage("New Task, Insert Method Entered");
                        RunProjectPostAsync(projektnameWithBaseplan, protask).GetAwaiter().GetResult(); //Post Call
                        if (modelToInsert.ErrorMsg != null)
                        {
                            SetMessage(modelToInsert.ErrorMsg);
                            progress.Hide();
                            return;
                        }
                        else
                        {
                            if (modelToInsert.Tasks.Count == 1)
                            {
                                var polTask = modelToInsert.Tasks[0];
                                protask.Text1 = polTask.PlanId;
                                protask.Text2 = polTask.WorkitemId;
                                SetMessage(polTask.PlanId + " inserted " + i++.ToString());
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.WriteLine("Error at POST: " + ex.Message);
                    SetMessage("POST err:" + ex.Message);
                }
            }
            
            // Put Method called
            try
            {
                SetMessage(" Starting Polarion Sync Phase 2 - resync to Polarion");
                RunProjectPutAsync(projektnameWithBaseplan).GetAwaiter().GetResult(); //Put call 
                SetMessage(" Update Project File - resync to Polarion");
                UpdateProjectFile(modelToUpdate);
                SetMessage(" Sync finished");
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("Error at PUT: "+ ex.Message);
                //SetMessage("PUT err:"+ ex.Message);
                SetMessage(" Sync finished");
                throw;
            }
            
        progress.Hide();
        }

        private void ShowProgresswindow()
        {
            var parent = new WindowHandle(Process.GetCurrentProcess().MainWindowHandle);
            progress.Show(parent);
        }

        /// <summary>
        /// Open Plan in Polarion
        /// </summary>
        /// <param name="control"></param>
        public void OnPolarionPlan_Click(Office.IRibbonControl control)
        {
            if (SyncOff)
            {
                SetMessage("Polarion Sync is disabled - click on the ON/OFF Button to enable Sync");
                return;
            }

            string projektName = GetProjektName(out string baseplan);
            if (projektName == null || projektName.Length == 0) return;
            string id = string.Empty;

            var pro = Globals.ThisAddIn.Application.ActiveSelection;
            if (pro.Tasks != null)
            {
                var task = pro.Tasks[1];
                id = task.Text1;
                if (string.IsNullOrEmpty(id))
                {
                    Globals.ThisAddIn.Application.Message($"Dieser Task hat keinen hinterlegten Plan.", Microsoft.Office.Interop.MSProject.PjMessageType.pjOKOnly);
                    return;
                }
            }
            else
            {
                Globals.ThisAddIn.Application.Message($"Bitte einen Task auswählen.", Microsoft.Office.Interop.MSProject.PjMessageType.pjOKOnly);
                return;
            }
            System.Diagnostics.Process.Start(GetPlanUrl(projektName, id));
        }

        /// <summary>
        /// Open Workitem in Polarion
        /// </summary>
        /// <param name="control"></param>
        public void OnPolarionWorkitem_Click(Office.IRibbonControl control)
        {
            if (SyncOff)
            {
                SetMessage("Polarion Sync is disabled - click on the ON/OFF Button to enable Sync");
                return;
            }
            string projektName = GetProjektName(out string baseplan);
            if (projektName == null || projektName.Length == 0) return;
            string id = string.Empty;

            var pro = Globals.ThisAddIn.Application.ActiveSelection;
            if (pro.Tasks != null)
            {
                var task = pro.Tasks[1];
                id = task.Text2;
                if (string.IsNullOrEmpty(id))
                {
                    Globals.ThisAddIn.Application.Message($"Dieser Task hat kein hinterlegtes WorkItem.", Microsoft.Office.Interop.MSProject.PjMessageType.pjOKOnly);
                    return;
                }
            }
            else
            {
                Globals.ThisAddIn.Application.Message($"Bitte einen Task auswählen.", Microsoft.Office.Interop.MSProject.PjMessageType.pjOKOnly);
            }
            System.Diagnostics.Process.Start(GetWorkItemUrl(projektName, id));
        }

        public void SetMessage(string message)
        {
            //Globals.ThisAddIn.Application.ScreenUpdating = false;
            progress.SetProgress(message);
            syncMessage = message;
            ribbon.InvalidateControl("lblMessage");
            //Globals.ThisAddIn.Application.ScreenUpdating = true;
        }

        #region Versuche

        //string projektName = GetProjektName();
        //if (projektName == null) return;

        //var pro = Globals.ThisAddIn.Application;
        //var tasks = pro.ActiveProject.Tasks;
        //foreach (Task task in tasks)
        //{
        //    var name = task.Name;
        //    var guid = task.Guid;
        //    var custom = task.Text1;
        //    var start = task.Start;
        //    var end = task.EarlyFinish;
        //    var index = task.Index;
        //    var dur = task.Duration;
        //    var preTasks = task.PredecessorTasks;
        //    var id = task.ID;
        //    task.Text1 = Guid.NewGuid().ToString();
        //    if (preTasks.Count > 0)
        //    {
        //        foreach (Task preTask in preTasks)
        //        {
        //            var preIndex = preTask.Index;
        //            var PreGuid = preTask.Guid;
        //            var preid = preTask.ID;
        //        }
        //    }


        //}
        //string link = "http://www.heider.at";

        //System.Diagnostics.Process.Start(link);


        //var Guid = task.Guid;
        //var custom = task.Number1;
        //var start = task.Start;
        //var end = task.EarlyFinish;
        //var index = task.Index;
        //var preTasks = task.PredecessorTasks;
        //var preTask = preTasks[1];
        //var preIndex = preTask.Index;
        //var PreGuid = preTask.Guid;
        //Globals.ThisAddIn.Application.Message($"Verwende Projekt {name} für eine Polarion Verbindung.", Microsoft.Office.Interop.MSProject.PjMessageType.pjOKOnly);

        #endregion

        #endregion

        #region Helpers

        public static string GetApiUrl(string projektName)
        {
            return string.Format(PolarionRibbon.APIURL, projektName);
        }

        public static string GetWorkItemUrl(string projektName, string id)
        {
            return string.Format(PolarionRibbon.WORKITEMURL, projektName, id);
        }

        public static string GetPlanUrl(string projektName, string id)
        {
            return string.Format(PolarionRibbon.PLANURL, projektName, id);
        }

        private string GetProjektName(out string baseplan)
        {
            string projektName = Globals.ThisAddIn.Application.ActiveProject.Name;
            baseplan = "PM_Plan_Template_E2";
            if (projektName.Contains(" "))
            {
                projektName = projektName.Split(' ')[0];
                //Here splits by "/" and get the last part which would be the project name
                //this is necessary if the file comes fom sharepoint and has a name in form of a URL
                projektName = projektName.Split('/').Last();
                if (projektName.Contains('('))
                {
                    string[] parts = projektName.Split('(');
                    projektName = parts[0];
                    baseplan = parts[1].Substring(0, parts[1].Length - 1);
                }
            }
            else
            {
                Globals.ThisAddIn.Application.Message($"Aktueles Projekt '{projektName}' ist kein gültiger Dateiname für eine Polarion Verbindung.", Microsoft.Office.Interop.MSProject.PjMessageType.pjOKOnly);
                return "";
            }
            return projektName;
        }

        private string GetProjektNameWithBaseplan()
        {
            string projektName = Globals.ThisAddIn.Application.ActiveProject.Name;
            if (projektName.Contains(" "))
            {
                projektName = projektName.Split(' ')[0];
                //When project comes from sharepoint
                projektName = projektName.Split('/').Last();
            }
            else
            {
                Globals.ThisAddIn.Application.Message($"Aktueles Projekt '{projektName}' ist kein gültiger Dateiname für eine Polarion Verbindung.", Microsoft.Office.Interop.MSProject.PjMessageType.pjOKOnly);
                return "";
            }
            return projektName;
        }

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        #endregion
    }

    public class WindowHandle : System.Windows.Forms.IWin32Window
    {
        public WindowHandle(IntPtr handle)
        {
            _hwnd = handle;
        }

        public IntPtr Handle
        {
            get { return _hwnd; }
        }

        private IntPtr _hwnd;
    }
}
