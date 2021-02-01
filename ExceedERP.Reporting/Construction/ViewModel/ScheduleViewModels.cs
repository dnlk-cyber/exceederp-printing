using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Construction.ViewModel
{
    //public class ScheduleViewModels
    //{
    //}
    public class GanttChartViewModel
    {
        public string Title { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

    public class ScheduleVerionViewModel
    {
        public int Id { get; set; }
        public string Version { get; set; }
    }
}
