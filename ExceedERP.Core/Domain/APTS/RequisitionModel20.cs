using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.APTS
{
    public class RequisitionModel20
    {
        public int RequisitionModel20Id { get; set; }
        [Display(Name = "Description")]
        public int DrugListsId { get; set; }
        public DrugLists DrugLists { get; set; }
        public string Description { get; set; }
        public string InternalCode { get; set; }
        public int BegStock { get; set; }
        public int ReceivedQty { get; set; }
        public int Loss { get; set; }
        public int EndStock { get; set; }
        public int CalculatedConsumption { get; set; }
        public int Max { get; set; }
        public int QtyToMax { get; set; }
        public int QtyToBeApproved { get; set; }        
        public string RequestedBy { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? RequestDate { get; set; }
        public string RequestDateAm { get; set; }
        public string ApproverName { get; set; }
        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? ApprovedDate { get; set; }
        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DateCreated { get; set; }
        public string CreatedBy { get; set; }
        [Display(Name ="Is Approved?")]
        public bool IsApproved { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsOut { get; set; }
        public bool IsNew { get; set; }
       
        [Display(Name = "Year")]
        public int GlFiscalYearId { get; set; }
        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }
        public string Requester { get; set; }
        public int Remark { get; set; }
        /////////////////////////////////////////////////////////////////////////////
        public string Unit { get; set; }
        public int? QuantityIssued { get; set; }
        public decimal? UnitCost { get; set; }
        public decimal? TotalCost { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public DateTime? DispensaryEntryDate { get; set; }
        public bool IsClosed { get; set; }

    }
}
