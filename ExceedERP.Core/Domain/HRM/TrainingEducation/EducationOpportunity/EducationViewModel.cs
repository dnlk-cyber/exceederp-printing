using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.TrainingEducation.EducationOpportunity
{
   public class EducationViewModel
    {
        public string EducationLevel { get; set; }
        public string EducationField { get; set; }
        public decimal TotalExperience { get; set; }
        public decimal Discipline { get; set; }

        public decimal Performance { get; set; }



    }
}
