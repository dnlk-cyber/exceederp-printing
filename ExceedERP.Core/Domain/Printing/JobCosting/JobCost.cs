using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.printing.PrintingEstimation.Setting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.printing.JobCosting
{
    public class JobCost : TrackUserSettingOperation
    {
        public int JobCostId { get; set; }
        public int JobId { get; set; }
        public int JobCategoryId { get; set; }
        public int CustomerId { get; set; }
        public DateTime JobReceivedDate { get; set; }
        public DateTime Date { get; set; }
        public string ReciptNo { get; set; }
        public decimal Quantity { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal OrderSize { get; set; }
        public bool Void { get; set; }
        public JobState Status { get; set; }
        public string Description { get; set; }

    }
    public class JobChangeOfOrder : TrackUserSettingOperation
    {
        public int JobChangeOfOrderId { get; set; }
        public int JobCostId { get; set; }
        public int ChangeOrderId { get; set; }
        public decimal AdditionalChange { get; set; }
        public decimal ChargeAmount { get; set; }
        public string Reason { get; set; }
    }

    public class JobLaborCost : TrackUserSettingOperation
    {
        public int JobLaborCostId { get; set; }
        public int JobCostId { get; set; }
        public string CostCenter { get; set; }
        public int GlFiscalYearId { get; set; }
        public int GLPeriodId { get; set; }
        public string Reference { get; set; }
        public decimal PermanetNormalHour  { get; set; }
        [Display(Name ="Permanent Direct Material")]
        public decimal PermanetLaborCost { get; set; }
        public decimal PermanetOverHeadCost  { get; set; }
        public decimal PermanetOTHour { get; set; }
        public decimal ContractNormalHour { get; set; }
        public decimal ContractOTHour { get; set; }

        public decimal ContractLaborCost { get; set; }
        public decimal ContractOverHeadCost { get; set; }

        public bool Beginning { get; set; }
        public decimal BeginningHour { get; set; }
    }

    public class JobMaterialCost : TrackUserSettingOperation
    {
        public int JobMaterialCostId { get; set; }
        public int JobCostId { get; set; }
        public string ItemCategoryCode { get; set; }
        public string ItemCode { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [Display(Name ="Issue Ref")]
        public string Reference { get; set; }
        public decimal Quantity { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Cost { get; set; }
        public string ItemDescription { get; set; }

    }
    public class JobMaterialReturn : TrackUserSettingOperation
    {
        public int JobMaterialReturnId { get; set; }
        public int JobCostId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)] public DateTime Date { get; set; }
        public string ItemCategoryCode { get; set; }
        public string ItemCode { get; set; }
        public string Reference { get; set; }
        public decimal Quantity { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Cost { get; set; }
        public string ItemDescription { get; set; }
    }
    public class FinishedJob : TrackUserSettingOperation
    {
        public int FinishedJobId { get; set; }
        public int JobCostId { get; set; }

        public string FgrNo { get; set; }
        public DateTime Date { get; set; }
        public decimal Quantity { get; set; }
    }
    public class DeliveredJob : TrackUserSettingOperation
    {
        public int DeliveredJobId { get; set; }
        public int JobCostId { get; set; }
        public string DoNo { get; set; }
        public DateTime Date { get; set; }
        public decimal Quantity { get; set; }
    }
}
