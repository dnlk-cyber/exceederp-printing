
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.Maintenance
{
    public class MechanicCrew : TrackUserOperation
    {
        public MechanicCrew()
        {
            this.ServiceCrews = new HashSet<ServiceCrew>();
        }
        [ScaffoldColumn(false)]
        public Guid MechanicCrewID { get; set; }

        [DisplayName(" Employee ")]
        public int EmployeeId { get; set; }
        public Person_Employee Person_Employee { get; set; }

        [DisplayName(" Section ")]
        public Guid SectionId { get; set; }
        [DisplayName(" Mechanic Type ")]
        public MechanicType MechanicType { get; set; }
        [Display(Name = "Mechanic Type")]
        public int MechanicTypeClassId { get; set; }

        [DisplayName("Full Name")]
        public string FullName { get; set; }

        [DisplayName("Supervisor Name")]
        public string SupervisorName { get; set; }

        [DisplayName("Hourly Wage")]
        public decimal HourlyWage { get; set; }
        public string MaintenanceCenterId { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }
        public string ServiceCenterId {get;set;}
        public PostionenumCrew Postionenum { get; set; }
        public string Postion { get; set; }

        [DisplayName("Date Hired")]
        public Nullable<DateTime> DateHired { get; set; }

        [NotMapped]
        [DisplayName("Job Title")]
        public string JobTitle { get; set; }


        public ICollection<ServiceCrew> ServiceCrews { get; set; }


    }
    public enum PostionenumCrew
    {
        Chief,
        Assistant
    }
    public class MechanicTypeClass
    {
        public int MechanicTypeClassId { get; set; }
        public string MechanicTypeTitle { get; set; }
        public string Description { get; set; }

    }
    public enum MechanicType
    {

        [Display(Name = "Mechanical")]
        [Description("Mechanical")]

        Mechanical,
        [Display(Name = "Electrical & Electronics")]
        [Description("Electrical & Electronics")]
        Electrical_and_Electronics,
        [Display(Name = "Body")]
        [Description("Body")]
        Body,
        [Display(Name = "Print")]
        [Description("Print")]
        Print,
        [Display(Name = "Welding")]
        [Description("Welding")]
        Welding,

        Machinist,

        Engineer,
        [Display(Name = "Machanic Type")]
        [Description("Machanic Type")]
        Machanic_Type,
        [Display(Name = "Service Man")]
        [Description("Service Man")]
        Service_Man,
        [Display(Name = "Tyre Repair Man")]
        [Description("Tyre Repair Man")]
        Tyre_Repair_Man,
        [Display(Name = "Up Holster Man")]
        [Description("Up Holster Man")]
        Up_Holster_Man,
        Painter

    }
}