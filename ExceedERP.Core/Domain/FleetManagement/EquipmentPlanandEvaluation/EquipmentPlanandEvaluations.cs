
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.EquipmentPlanandEvaluation
{
    public partial class EquipmentPlanandEvaluations : Operations
    {
        [ScaffoldColumn(false)]
       public Guid EquipmentPlanandEvaluationsID { get; set; }

        public Guid FleetId { get; set; }
       
        [DisplayName("Equipment Category")]
        public Guid EquipmentCategoryId { get; set; }
        
        [DisplayName("Equipment Type")]
        public Guid EquipmentTypeId { get; set; }
        [NotMapped]
        public string Equipment { get; set; }
         [DisplayName(" Equipment Name")]
        public string EquipmentName { get; set; }
        [Required]
        [DisplayName(" Plan Working time ")]
       public Nullable<int> PlanWorkingtime { get; set; }
        [Required]
        [DisplayName(" Plan Down time ")]
        public Nullable<int> PlanDowntime { get; set; }
        [Required]
        [DisplayName(" Plan Idle time ")]
        public Nullable<int> PlanIdletime { get; set; }
        [Required]
        [DisplayName("Plan Fuel Consumption")]
        public Nullable<decimal> PlanFuelConsumption { get; set; }
      
        [DisplayName("Rental Rate")]
        public Nullable<decimal> RentalRate { get; set; }
        [Required]
        [DisplayName("Unit Of Measurement")]
        public string UnitOfMeasurement { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        [DisplayName("Plan Income")]
        public Nullable<decimal> PlanIncome { get; set; }
        [Required]
        [DisplayName("Travlled KM")]
        public Nullable<int> PlanTyreUtilization { get; set; }
      
        [DisplayName("  Other ")]
        public Nullable<decimal> PlanEquipmentsPittyCashExpenditure { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public string Category { get; set; }
        public string Type { get; set; }
        public string PlateNo { get; set; }
        public string SerialNo { get; set; }

        public string ChasisNo { get; set; }
        public string AssetCode { get; set; }
        public decimal WorkingVolume { get; set; }
        public string VolumeUnit { get; set; }
        public virtual Fleets Fleet { get; set; }
           
    }
}