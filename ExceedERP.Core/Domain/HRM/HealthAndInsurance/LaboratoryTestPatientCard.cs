using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.HealthAndInsurance
{
   public class LaboratoryTestPatientCard
    {
        public int LaboratoryTestPatientCardId { get; set; }
        public int PatientId { get; set; }
        public int PatientCardDetailId { get; set; }
        public int LaboratoryCatagoryPatientCardId { get; set; }
        public string Test { get; set; }
        public string Reference { get; set; }
        public string Result { get; set; }
        // this boolean field is added to hide Send to doctor button and edit button
        public bool IsSentBackToDoctor { get; set; }
    }
}
