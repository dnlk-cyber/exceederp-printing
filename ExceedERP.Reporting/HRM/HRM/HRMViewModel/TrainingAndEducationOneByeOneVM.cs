using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
   public class TrainingAndEducationOneByeOneVM
    {
        public string EmployeeName { get; set; }
        public string Branch { get; set; }
        public string WorkUnit { get; set; }
        public List<Year> Years { get; set; }

        //public List<EducationVM> EducationVMs { get; set; }
        //public List<TrainingVM> TrainingVMs { get; set; }
    }
    //public class EducationVM
    //{
    //    public string EduType { get; set; }
    //}
    //public class TrainingVM
    //{
    //    public string TrainingType { get; set; }
    //}
    public class Year
    {
        public string Name { get; set; }
        public string EduType { get; set; }
        public string TrainingType { get; set; }

    }
}
