using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace PolarionReports.Models.MSProjectApi
{
    public class ApiError
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
    }
}