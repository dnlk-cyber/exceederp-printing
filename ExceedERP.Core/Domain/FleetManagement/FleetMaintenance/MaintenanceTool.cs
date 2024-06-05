
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
    public partial class MaintenanceTool : TrackUserOperation
    {

        public MaintenanceTool()
        {
            this.ServiceTools = new HashSet<ServiceTool>();
        }
         [ScaffoldColumn(false)]
        public Guid MaintenanceToolID { get; set; }
        [Required]
        [DisplayName("Tools Name")]
        public string ToolsName {get; set;}
        public enum UnitofMeasurementenum {Set , Pcs}

        public string UnitofMeasurement { get; set; }
        public decimal Price { get; set; }
        [Required]
        public Nullable<int> Quantity {get; set;}
        public enum Status {New,Serviceable,Unserviceable }
       [DisplayName("Owned By")]
        public string OwnedBy {get; set;}
        [Required]
        public string Location {get; set;}
        public string Description { get; set; }
        public ICollection<ServiceTool> ServiceTools { get; set; }
    }
}