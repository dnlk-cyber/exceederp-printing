
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
 using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
    public partial class MechanicCrew : TrackUserOperation
    {
        public MechanicCrew()
        {
            this.ServiceCrews = new HashSet<ServiceCrew>();
        }
        [ScaffoldColumn(false)]
        public Guid MechanicCrewID { get; set; }
        [DisplayName(" Employee ")]
        public int EmployeeId { get; set; }
          [DisplayName(" Section ")]
        public Guid SectionId { get; set; }
        [DisplayName("Full Name")]
        public string FullName { get; set; }
        [DisplayName("Supervisor Name")]
        public string SupervisorName { get; set; }
        [DisplayName("Hourly Wage")]
        public decimal HourlyWage { get; set; }
        public enum Postionenum { Chief, Assistant }
        public string Postion { get; set; }
        [DisplayName("Date Hired")]
        public Nullable<DateTime> DateHired { get; set; }
        [NotMapped]
        [DisplayName("Job Title")]
        public string JobTitle { get; set; }

     
        public ICollection<ServiceCrew> ServiceCrews { get; set; }


    }
}