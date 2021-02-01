using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.HealthAndInsurance
{
   public class PatientResultDetail
    {
        public int PatientResultDetailId { get; set; }
        public int PatientResultId { get; set; }    
        public string Description { get; set; }
        public string TestType { get; set; }

    }
}
