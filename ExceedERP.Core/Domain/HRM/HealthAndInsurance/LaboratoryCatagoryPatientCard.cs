using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.HealthAndInsurance
{
   public class LaboratoryCatagoryPatientCard
    {
        public int LaboratoryCatagoryPatientCardId { get; set; }
        public int PatientId { get; set; }
        public int PatientCardDetailId { get; set; }
        public string Catagory { get; set; }
    }
}
