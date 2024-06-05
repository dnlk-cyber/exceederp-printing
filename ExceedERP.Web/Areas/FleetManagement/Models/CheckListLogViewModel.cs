using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExceedERP.Core.Domain.Maintenance;

namespace ExceedERP.Web.Areas.FleetManagement.Models
{
    public class CheckListLogViewModel
    {
        public List<CheckListViewModel> CheckListViewModel { get; set; }
    }

    public class ServiceCrewViewModel
    {
        public List<ServiceCrew> ServiceCrew { get; set; }
    }
}