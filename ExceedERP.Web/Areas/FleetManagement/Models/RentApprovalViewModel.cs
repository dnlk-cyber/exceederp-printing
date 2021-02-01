using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExceedERP.Web.Areas.FleetManagement.Models
{
    public class RentApprovalViewModel
    {
        public int RentOtherApprovalID { get; set; }
        public Guid RentId { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
}