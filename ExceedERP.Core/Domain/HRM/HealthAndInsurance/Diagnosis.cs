using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.HealthAndInsurance
{
   public class Diagnosis
    {
        public int DiagnosisId { get; set; }
        public int PatientResultId { get; set; }
        public string Description { get; set; }
    }
}
