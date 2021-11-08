using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
   public class ApplicantCertefication
    {
        public int ApplicantCerteficationId { get; set; }
        public int ApplicantDetailId { get; set; }
        public int ApplicantRegistrationId { get; set; }
        public string CerteficationName { get; set; }
        public string CerteficationType { get; set; }
        public string Remark { get; set; }
    }
}
