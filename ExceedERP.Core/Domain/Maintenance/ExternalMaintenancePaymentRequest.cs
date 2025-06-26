using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Maintenance
{
    public class ExternalMaintenancePaymentRequest : DepartmentNewOperations
    {
        public int ExternalMaintenancePaymenntrequestId { get; set; }
        public Guid? FleetId { get; set; }
        public string EquipmentName { get; set; }
        [DataType(DataType.Date)]
        public DateTime? EnteranceDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ExitDate { get; set; }
        public int? ServiceCenterId { get; set; }
        [Display(Name ="Maintenance Center")]
        public Guid? MaintenanceCenterId { get; set; }
        public int? BranchId { get; set; }
        public int? BusinessUnitId { get; set; } 
        public int JobNum { get; set; }
        public int? RequestorId { get; set; }
        public bool Paid { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountDue { get; set; }


    }
    public class ExternalMaintenancePaymentRequestValidation : TrackUserOperation
    {
        public int ExternalMaintenancePaymentRequestValidationId { get; set; }
        public int ExternalMaintenancePaymenntrequestId { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
    public class ExternalMaintenancePaymentRequestDepValidation : TrackUserOperation
    {
        public int ExternalMaintenancePaymentRequestDepValidationId { get; set; }
        public int ExternalMaintenancePaymenntrequestId { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
    public class ExternalMaintenancePaymentRequestHeader: DepartmentNewOperations
    {
        public int ExternalMaintenancePaymentRequestHeaderId {get;set;}
        public string MaintenanceCenterId { get; set; }
        public string Description { get; set; }
        public bool Paid { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountDue { get; set; }
    }
    public class ExternalMaintenancePaymentRequestDetail : TrackUserSettingOperation
    {
        public int ExternalMaintenancePaymentRequestDetailId { get; set; }
        public int ExternalPaymentRequestJobId { get; set; }
        public string CostType { get; set; }
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; } 
        public decimal SubTotal { get; set; }
        [Display(Name ="Tax")]
        public int GlTaxRateId { get; set; }
        public decimal Total { get; set; } 

        public string RepairDescribtion { get; set; }
        
        public int ExternalMaintenancePaymenntrequestId { get; set; } 

    }
}
