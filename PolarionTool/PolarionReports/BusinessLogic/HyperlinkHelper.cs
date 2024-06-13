using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.BusinessLogic
{
    public class HyperlinKHelper
    {
        public List<Hyperlink> GetHyperlinksFromWorkitemDescription(Workitem w)
        {
            List<Hyperlink> hyperlinks = new List<Hyperlink>();
            int pos = 1;
            int start;
            int end;

            if (w == null || w.Description == null) return hyperlinks;

            // Hyperlink in der Description suchen
            if (w.Description.Contains("href=" + '"' + "https:") ||
                w.Description.Contains("href=" + '"' + "http:"))
            {
                // Description enthält Hyperlink href aus <a> Tag extrahieren
                do
                {
                    start = w.Description.IndexOf("href=", pos);
                    if (start > 0)
                    {
                        start = w.Description.IndexOf('"', start);
                        if (start > 0)
                        {
                            end = w.Description.IndexOf('"', start + 6);
                            if (end > 0)
                            {
                                Hyperlink h = new Hyperlink();
                                h.WorkitemID = w.C_pk;
                                h.URL = w.Description.Substring(start, end - start);
                                hyperlinks.Add(h);
                                pos = end;
                            }
                        }
                        else
                        {
                            pos = start;
                        }
                    }
                    else
                    {
                        pos = start;
                    }
                } while (pos > 0);
            }


            return hyperlinks;
        }
    }
}