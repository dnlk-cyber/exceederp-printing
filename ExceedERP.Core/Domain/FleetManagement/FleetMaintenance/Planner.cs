
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{

    using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public partial class Planner : TrackUserOperation
    {
        public Planner()
        {

            this.Services = new HashSet<Service>();
        }

        [ScaffoldColumn(false)]
        public Guid PlannerID { get; set; }
        [DisplayName("Name")]
        [Required]
        public string PlannerName { get; set; }
        [DisplayName("Track By Date")]
        public bool TrackByDate { get; set; }
        [DisplayName("Track By Mileage")]
        public bool TrackByMileage { get; set; }
        public Nullable<System.DateTime> DateCrated { get; set; }
        public virtual ICollection<Service> Services { get; set; }





    }
}
