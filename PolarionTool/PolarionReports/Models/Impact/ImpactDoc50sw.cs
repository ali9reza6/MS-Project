﻿using PolarionReports.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Impact
{
    public class ImpactDoc50sw
    {
        public ImpactDocument Doc50sw { get; set; }
        public List<Workitem> Interfaces { get; set; }
        public string TooltipInterfaces { get; set; }
        public List<Workitem> Components { get; set; }
        public string TooltipComponents { get; set; }

    }
}