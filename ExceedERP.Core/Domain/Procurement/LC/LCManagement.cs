using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.LC
{
    public class LCManagement : TrackUserOperation
    {
        public int LCManagementID { get; set; }

        [Display(Name = "LC Requested Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RequestDate { get; set; }
        [Display(Name = "Requested Amount")]
        public decimal RequestAmount { get; set; }
        [Display(Name = "Requested Currency")]
        public string RequestCurrency { get; set; }
        [Display(Name = "Approved Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ApprovedDate { get; set; }
        [Display(Name = "Approved Amount")]
        public decimal ApprovedAmount { get; set; }
        [Display(Name = "Approved Currency")]
        public string ApprovedCurrency { get; set; }
        [Display(Name = "Swift Code")]
        public string SwiftCode { get; set; }
        [Display(Name = "LC No.")]
        public string LCNo { get; set; }
        [Display(Name = "LC Opening Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? LCOpeningDate { get; set; }
        [Display(Name = "LC Beneficiary")]
        public string LCBeneficiary { get; set; }
        [Display(Name = "LC Opening Charge Amount")]
        public decimal OpeningChargeAmount { get; set; }
        [Display(Name = "LC Opening Charge Currency")]
        public string OpeningChargeCurrency { get; set; }
        [Display(Name = "LC Period")]
        public string LCPeriod { get; set; }
        [Display(Name = "LC Amendement Amount")]
        public decimal LCAmendementAmount { get; set; }
        [Display(Name = "LC Amendement Currency")]
        public string LCAmendementCurrency { get; set; }
        [Display(Name = "Additional LC Period Requested")]
        public string LCAdditionalPeriodRequested { get; set; }
        [Display(Name = "Tax of Customs charges")]
        public decimal CustomTaxCharge { get; set; }
        [Display(Name = "Logistics Service charge")]
        public decimal LogisticsCharge { get; set; }
        [Display(Name = "DateCreated of Delivery of goods")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? GoodDeliveryDate { get; set; }
        //new 
        [Display(Name = "Contract Ref No.")]
        public string ContractRefNo { get; set; }
        [Display(Name = "Insurance Cost")]
        public decimal InsuranceCost { get; set; }
        [Display(Name = "Freight Cost")]
        public decimal FreightCost { get; set; }
        [Display(Name = "Inland Haulage Revenue")]
        public decimal InlandHaulageRevenue { get; set; }
        public string Remark { get; set; }
    }
}
