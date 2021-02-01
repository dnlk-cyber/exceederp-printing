using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExceedERP.Web.Areas.FleetManagement.Models
{
    public class CheckListViewModel 
    {
        public string InspectionId { get; set; }
        public string CheckListId { get; set; }
        public string FleetId { get; set; }
        public string EquipmentTypeId { get; set; }
        public int LineId { get; set; }
        public string CategoryName { get; set; }
        public string LineCode { get; set; }
        public string LineName { get; set; }
        public string Remark { get; set; }
        public string IsActive { get; set; }
        public bool IsActiveInspection { get; set; }

    }
}