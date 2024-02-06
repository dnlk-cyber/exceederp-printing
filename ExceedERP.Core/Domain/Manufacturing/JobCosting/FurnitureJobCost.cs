using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Manufacturing.JobCosting;
using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation.Setting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.JobCosting
{
    public class FurnitureJobCost : TrackUserSettingOperation
    {
        public int FurnitureJobCostId { get; set; }
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
        public FurnitureJobState Status { get; set; }
        public string Description { get; set; }

    }
    public class FurnitureJobChangeOfOrder : TrackUserSettingOperation
    {
        public int FurnitureJobChangeOfOrderId { get; set; }
        public int FurnitureChangeOrderId { get; set; }
        public decimal AdditionalChange { get; set; }
        public decimal ChargeAmount { get; set; }
        public string Reason { get; set; }
        public int FurnitureJobCostId { get; set; }
        public virtual  FurnitureJobCost JobCost { get; set; }

    }

    public class FurnitureJobLaborCost : TrackUserSettingOperation
    {
        public int FurnitureJobLaborCostId { get; set; }
        public int FurnitureJobCostId { get; set; }
        public virtual FurnitureJobCost JobCost { get; set; }
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

    public class FurnitureJobMaterialCost : TrackUserSettingOperation
    {
        public int FurnitureJobMaterialCostId { get; set; }
        public int FurnitureJobCostId { get; set; }
        public virtual FurnitureJobCost JobCost { get; set; }
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
    public class FurnitureJobMaterialReturn : TrackUserSettingOperation
    {
        public int FurnitureJobMaterialReturnId { get; set; }
        public int FurnitureJobCostId { get; set; }
        public virtual FurnitureJobCost JobCost { get; set; }
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
    public class FurnitureFinishedJob : TrackUserSettingOperation
    {
        public int FurnitureFinishedJobId { get; set; }
        public int FurnitureJobCostId { get; set; }
        public virtual FurnitureJobCost JobCost { get; set; }

        public string FgrNo { get; set; }
        public DateTime Date { get; set; }
        public decimal Quantity { get; set; }
    }
    public class FurnitureDeliveredJob : TrackUserSettingOperation
    {
        public int FurnitureDeliveredJobId { get; set; }
        public int FurnitureJobCostId { get; set; }
        public virtual FurnitureJobCost JobCost { get; set; }
        public string DoNo { get; set; }
        public DateTime Date { get; set; }
        public decimal Quantity { get; set; }
    }
}
