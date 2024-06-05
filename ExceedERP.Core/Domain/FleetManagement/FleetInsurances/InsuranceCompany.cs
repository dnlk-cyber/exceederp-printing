using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;

namespace ExceedERP.Core.Domain.FleetManagement.FleetInsurances
{
    public class InsuranceCompany : Operations
    {
        public int InsuranceCompanyId { get; set; }
        [Display(Name = "Company Name")]
        public string InsuranceCompanyName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

    }
}
