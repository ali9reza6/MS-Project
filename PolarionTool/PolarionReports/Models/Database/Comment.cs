using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Database
{
    public class Comment
    {
        public int C_pk { get; set; }
        public int UserId { get; set; }
        public int Parent { get; set; }
        public int WorkitemId { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string UserName { get; set; }
    }
}