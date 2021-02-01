using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExceedERP.Web.Areas.FleetManagement.Models
{
    public class ServiceCrewAssignViewModel
    {
        public Guid ServiceCrewId { get; set; }
        public string IsActive { get; set; }
        public string Remark { get; set; }
    }

    public class Crews 
    {
        public List<ServiceCrewAssignViewModel> ServiceCrews { get; set; }
    }

}