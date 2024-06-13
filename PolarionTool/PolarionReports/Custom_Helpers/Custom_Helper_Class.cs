using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace PolarionReports.Custom_Helpers
{
    public static class Custom_Helper_Class
    {
        public static IHtmlString PolarionTable(string Link)
        {
            string LinkStr = "<a style=" + '"' + "margin-left:10px" + '"' + 
                             " data-toggle='tooltip' title='Open items in Polarion table view' target='_blank' href=" + 
                             '"' + Link + '"' + "><img src=" + '"' + "/Images/p25.jpg" + "\"" + " /></a>";
            
            return new HtmlString(LinkStr);
        }

        public static IHtmlString Analyze(string Link)
        {
            string LinkStr = "<a style=" + '"' + "margin-left:10px" + '"' + 
                " data-toggle='tooltip' title='Analyze Requirement' target='_blank' href=" + '"' + Link + '"' + 
                "><img style='width:20px; height:20px' src=" + '"' + "/Images/mag_glass.gif" + "\"" + " /></a>";

            return new HtmlString(LinkStr);
        }

        public static IHtmlString AnalyzeList(string Link, List<Workitem> workitems, string filter, string StartingPoint, string id)
        {
            string param = "";
            // Parameter String erzeugen:
            foreach (Workitem w in workitems)
            {
                if (param.Length > 1)
                {
                    param += "|";
                }
                param += w.Id;
            }
            if (filter != null && filter.Length > 0)
            {
                param += "&filter=" + filter;
            }
            if (StartingPoint != null && StartingPoint.Length > 0)
            {
                param += "&workitemStartingPoint=" + StartingPoint;
            }

            string LinkStr = "<a id=" + '"' + id + '"' + " style=" + '"' + "margin-left:10px" + '"' +
                " data-toggle='tooltip' title='Analyze Requirement List' target='_blank' href=" + '"' + Link + "?workitems=" + param + '"' +
                "><img style='width:20px; height:20px' src=" + '"' + "/Images/mag_glass.gif" + "\"" + " /></a>";

            return new HtmlString(LinkStr);
        }

        public static IHtmlString AnalyzeWorkitem(string Link, Workitem workitem, string filter, string StartingPoint)
        {
            string param = workitem.Id;
         
            if (filter != null && filter.Length > 0)
            {
                param += "&filter=" + filter;
            }
            if (StartingPoint != null && StartingPoint.Length > 0)
            {
                param += "&workitemStartingPoint=" + StartingPoint;
            }
            string LinkStr = "<a style=" + '"' + "margin-left:10px" + '"' +
                " data-toggle='tooltip' title='Analyze Requirement List' target='_blank' href=" + '"' + Link + "?workitems=" + param + '"' +
                "><img style='width:20px; height:20px' src=" + '"' + "/Images/mag_glass.gif" + "\"" + " /></a>";

            return new HtmlString(LinkStr);
        }

        public static string GetPolarionDocumentLink(string projectId, string documentfolder, string documentId)
        {
            return $"http://{Topol.PolarionServer}/polarion/#/project/{projectId}/wiki/{documentfolder}/{documentId}?selection=";
        }

        public static IHtmlString TestcaseResultTable(List<TestcaseResult> trl)
        {
            /*
            string html = "<table class='table'><tr><th>Datum</th><th>Result</th><th>Tester</th></tr>";

            foreach(TestcaseResult tr in trl)
            {
                html += "<tr><td>" + tr.c_executed.ToShortDateString() + "</td>" +
                            "<td>" + tr.c_result + "</td>" +
                            "<td>" + tr.c_name + "</td></tr>";
            }
            html += "</table>";
            return new HtmlString(html);
            */
            string html = "";

            foreach (TestcaseResult tr in trl)
            {
                html += tr.c_executed.ToShortDateString() + " " +
                        tr.c_result + " " +
                        tr.c_name + System.Environment.NewLine;
            }
            html += "";
            return new HtmlString(html);
        }
    }
}