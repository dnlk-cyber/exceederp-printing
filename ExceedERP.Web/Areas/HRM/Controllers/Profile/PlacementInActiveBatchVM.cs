using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{
    public class PlacementInActiveBatchVM
    {
        public int PlacementId { get; set; }
        public int BranchId { get; set; }
        public int EmployeeId { get; set; }
        public int OrgStructureId { get; set; }
        public int JobTitleId { get; set; }
        public bool Check { get; set; }
    }
}