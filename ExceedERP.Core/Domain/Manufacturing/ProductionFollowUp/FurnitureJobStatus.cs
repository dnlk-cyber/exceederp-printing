using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation.Setting;
using ExceedERP.Core.Domain.Printing.ProductionFollowUp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp
{
    public class FurnitureJobStatus : TrackUserSettingOperation
    {
        public int FurnitureJobStatusId { get; set; }
        public int FurnitureJobId { get; set; }
        [Display(Name = "User")]
        public string FurnitureUserId { get; set; }
        //started time
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Time { get; set; }
        //finished time
        //[Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }
        // [Required]
        [Display(Name = "Department")]
        public FurnitureProductionDepartment JobPhase { get; set; }
        // [Required]
        [Display(Name = "Type")]
        public FurnitureJobState Status { get; set; }
        public FurnitureJob JobOrder { get; set; }
    }

    public class FurnitureJobStatusLineItem : TrackUserSettingOperation
    {
        public int FurnitureJobStatusLineItemId { get; set; }
        public int FurnitureJobStatusId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }
        [Display(Name = "Quantity")]
        public decimal Quantiy { get; set; }
        [Display(Name = "Remainig Quantity")]
        public decimal? RemainigQuantiy { get; set; }
        [Display(Name = "Total Quantity")]
        public decimal? TotalQuantiy { get; set; }
        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }
        [Display(Name = "Invoice")]
        public string Invoice { get; set; }
        [Display(Name = "Sales Voucher No")]
        public string SalesVoucherNo { get; set; }
        [Display(Name = "Cash")]
        public decimal? Cash { get; set; }
        [Display(Name = "Credit")]
        public decimal? Credit { get; set; }
        [Display(Name = "Cash Credit Invoice")]
        public string CashCreditInvoice { get; set; }
        public FurnitureJobStatus JobStatus { get; set; }
    }
}
