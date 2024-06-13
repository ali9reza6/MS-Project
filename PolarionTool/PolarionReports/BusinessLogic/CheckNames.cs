using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using PolarionReports.Models.Database;

namespace PolarionReports.BusinessLogic
{
    public class CheckNames
    {
        public bool CheckDocumentRelation(DocumentName FromDoc, DocumentName ToDoc, out string ErrorMessage)
        {
            ErrorMessage = "";

            #region Check 20 -> 10
            if (FromDoc.Level == "20")
            {
                // 20 Document muss in ein 10er zeigen
                if (ToDoc.Level == "10")
                {
                    // Level OK
                    if (FromDoc.Prefix != null)
                    {
                        // 20 hat Prefix
                        if (FromDoc.Subindex != null)
                        {
                            // ToDoc Prefix muss im Subindex enthalten sein
                            if (FromDoc.Subindex.Contains(ToDoc.Prefix))
                            {
                                // Alles OK
                                return true;
                            }   
                            else
                            {
                                // Fehler
                                ErrorMessage = "Inv. Subindex";
                                return false;
                            }

                        }
                        else
                        {
                            // nur Prefix: muss mit ToDoc übereinstimmen
                            if (FromDoc.Prefix == ToDoc.Prefix)
                            {
                                // Alles OK
                                return true;
                            }
                            else
                            {
                                // Fehler
                                ErrorMessage = "Inv. Prefix";
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    ErrorMessage = "Link not to 10 Document";
                    return false;
                }
            }
            #endregion



            return true;
        }

    }
}