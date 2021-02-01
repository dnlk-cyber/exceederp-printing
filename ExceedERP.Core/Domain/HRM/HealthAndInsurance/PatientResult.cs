using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.HealthAndInsurance
{
   public class PatientResult
    {
        public int PatientResultId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public string Description { get; set; }
        public bool IsForDespensary { get; set; }
        public bool IsForLaboratory { get; set; }
        public bool IsRefer { get; set; }
        public bool OnChecking { get; set; }
        public DateTime RegisterDate { get; set; }
        public string RegisterDateAm { get; set; }
        public int? MainCompanyId { get; set; }
    }
}
