using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
   public class CheckUp
    {
        public int CheckUpId { get; set; }
        public bool CriminalCheck { get; set; }
        public bool HealthCheck { get; set; }
        public bool Offer { get; set; }
    }
}
