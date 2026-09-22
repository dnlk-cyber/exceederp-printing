using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement.RentalPayment
{
    public class RentalPaymentRequestLine
    {
        public int RentalPaymentRequestLineId { get; set; }
        public int RentalPaymentRequestId { get; set; }
        [Display(Name ="Equipment Category")]
        public Guid EquipmentCategoryId { get; set; }
        [DisplayName("Equipment Type")]
        public Guid EquipmnetTypeId { get; set; }
        [DisplayName("Equipment Name")]
        public Guid EquipmentNameId { get; set; }
        [Display(Name ="Equipment")]
        public Guid FleetId { get; set; }
        [DisplayName("Total Amount before Tax")]
        public decimal Amount { get; set; }
        [DisplayName("Tax")]
        public int? GLTaxRateID { get; set; }
        [DisplayName("Tax Amount")]
        public decimal TaxAmount { get; set; }
        [DisplayName("Sub Total Amount")]
        public decimal SubTotalAmount { get; set; }
        [Display(Name ="Total Cost")]
        public decimal TotalCost { get; set; }
        [DisplayName("Fuel Amount")]
        public decimal FuelAmount { get; set; }
        [DisplayName("Commission")]
        public decimal Commission { get; set; }
        [DisplayName("Advanced Payment")]
        public decimal AdvancedPayment { get; set; }
        [DisplayName("Total Amount")]
        public decimal TotalAmount { get; set; }
        public int CostSheetSummaryId { get; set; }
        public decimal GrossBeforeVat { get; set; }
        public decimal SparePartCost { get; set; }
        public decimal OilLubCost { get; set; }
        public decimal MaintenanceCost { get; set; }
        public decimal FuelCost { get; set; }
        public decimal VAT { get; set; }
        public decimal WithholdingAmount { get; set; }
        public virtual RentalPaymentRequest RentalPaymentRequest { get; set; }
        

    }
}
// PerformanceAuditWeeklyWorkTimeDetail PerformanceWeeklyWorkTimeControlReportActual