﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Database.Api
{
    public class Downlink
    {
        public WorkitemBase Up { get; set; }
        public string c_role { get; set; }
        public WorkitemBase Down { get; set; }
    }
}