using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet
{
    public class WagonDrillTimesheet: OperationsCostTimeSheet
    {
        public int WagonDrillTimesheetId { get; set; }
        [DisplayName("Supplier Name")]
        public int? ProcurementSupplierID { get; set; }
        [Display(Name ="Equipment Category")]
        public Guid? EquipmentCategoryId { get; set; }
        [Display(Name = "Equipment Type")]
        public Guid? EquipmentTypeId { get; set; }
        [Display(Name = "Equipment Name")]
        public Guid? EquipmentNameId { get; set; }
        [Display(Name ="Plate/Category")]
        public Guid FleetsID { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Date")]
        [Column(TypeName = "DATE")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [DisplayName("Work Item")]
        public string WorkItem { get; set; }
        [DisplayName("Net Working Hour")]
        public decimal OperationNetWorkingHour { get; set; }
        [DisplayName("Drilled Depth of hole(m)")]        
        public decimal OperationDrilledHoleDepth { get; set; }
        [DisplayName("No of holes drilled")]
        public decimal NumberOfHolesDrilled { get; set; }
        [DisplayName("Rate/m")]
        public decimal OperationRateForMeter { get; set; }
        [DisplayName("Operation Amount")]
        public decimal OperationAmount { get; set; }
        [DisplayName("Idle Hour")]
        public decimal IdleHour { get; set; }
        [DisplayName("Idle Rate")]
        public decimal IdleRateHour { get; set; }
        [DisplayName("Idle Amount")]
        public decimal IdleAmount { get; set; }
        [DisplayName("Down Time")]
        public decimal DownTime { get; set; }
        [DisplayName("Total Amount Before VAT")]
        public decimal TotalAmountBeforeVAT { get; set; }
        [DisplayName("VAT")]
        public decimal VAT { get; set; }
        [DisplayName("Total Amount")]
        public decimal TotalAmount { get; set; }
        [DisplayName("Fuel /Lit")]
        public decimal FuelInLiter { get; set; }
        [DisplayName("Fuel rate")]
        public decimal FuelRate { get; set; }
        [DisplayName("Fuel Cost")]
        public decimal FuelCost { get; set; }
        [DisplayName("Service Charge")]
        public decimal ServiceCharge { get; set; }
        [DisplayName("Lubricant cost")]
        public decimal LubricantCost { get; set; }
        [DisplayName("Maintenance cost")]
        public decimal MaintenanceCost { get; set; }
        [DisplayName("Tire Cost")]
        public decimal TireCost { get; set; }
        [DisplayName("Total Cost")]
        public decimal TotalCost { get; set; }
        [DisplayName("Net Amount")]
        public decimal NetAmount { get; set; }
        public virtual Fleets Fleets { get; set; }
        public decimal Perdium { get; set; }
        public decimal Salary { get; set; }
        [Display(Name ="Reason For Down")]
        public string ReasonForDown { get; set; }
        [Display(Name ="Reason For Idle")]
        public string ReasonForIdle { get; set; }

        [Display(Name="Branch")]
        public int BranchId { get; set; }
        [Display(Name ="Business")]
        public int? BusinessUnitId { get; set; }
        [Display(Name = "Operator")]
        public Guid? OperatorId { get; set; }
        [Display(Name = "External Operator")]
        public string ExternalOperator { get; set; }
        [Display(Name ="Rate(M)")]
        public decimal RateInMeter { get; set; } 
    }
}
