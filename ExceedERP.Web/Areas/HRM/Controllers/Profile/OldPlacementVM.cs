using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{
    public class OldPlacementVM
    {
        public int PlaceId { get; set; }
        public string Branch { get; set; }
        public string Department { get; set; }
        public int LookupId { get; set; }
        public string JobTitle { get; set; }
        public string AssignmentDate { get; set; }
        public bool IsHead { get; set; }
        public bool IsCurrent { get; set; }
        public decimal Salary { get; set; }
        public AssignmentType AssignmentType { get; set; }
    }
}