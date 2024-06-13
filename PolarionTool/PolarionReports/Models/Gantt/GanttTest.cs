using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PolarionReports.Models.Gantt
{
    public class GanttTest
    {
        public List<GanttData> GetGanttData()
        {
            List<GanttData> gdl = new List<GanttData>();

            gdl.Add(new GanttData()
            {
                TaskId = 1,
                TaskName = "Project initiation",
                StartDate = new DateTime(2019, 04, 02),
                EndDate = new DateTime(2019, 04, 21),
                ParentId = null
            });
            gdl.Add(new GanttData()
            {
                TaskId = 2,
                TaskName = "Identify site location",
                StartDate = new DateTime(2019, 04, 02),
                Duration = 4,
                Progress = 70,
                ParentId = 1
            });
            gdl.Add(new GanttData()
            {
                TaskId = 3,
                TaskName = "Perform soil test",
                StartDate = new DateTime(2019, 04, 02),
                Duration = 4,
                Progress = 50,
                ParentId = 1,
                Dependency = "2"
            });

            gdl.Add(new GanttData()
            {
                TaskId = 4,
                TaskName = "Soil test approval",
                StartDate = new DateTime(2019, 04, 02),
                Duration = 4,
                Progress = 50,
                ParentId = 1,
                Dependency = "3"
            });
            gdl.Add(new GanttData()
            {
                TaskId = 5,
                TaskName = "Project estimation",
                StartDate = new DateTime(2019, 04, 02),
                EndDate = new DateTime(2019, 04, 21),
                ParentId = null
            });
            gdl.Add(new GanttData()
            {
                TaskId = 6,
                TaskName = "Develop floor plan for estimation",
                StartDate = new DateTime(2019, 04, 04),
                Duration = 3,
                Progress = 70,
                ParentId = 5
            });
            gdl.Add(new GanttData()
            {
                TaskId = 7,
                TaskName = "List materials",
                StartDate = new DateTime(2019, 04, 04),
                Duration = 3,
                Progress = 50,
                ParentId = 5
            });
            gdl.Add(new GanttData()
            {
                TaskId = 8,
                TaskName = "test materials",
                StartDate = new DateTime(2019, 04, 04),
                Duration = 4,
                Progress = 0,
                ParentId = 5,
                Dependency = "6FS+1d, 7FS+2d"
            });

            return gdl;
        }
    }
}