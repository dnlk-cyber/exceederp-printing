using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.CRM
{
    public  class CRMDeposit : Operations
    {
        [Key]
        public int CRMDepositId { get; set; }

        // sales person user name
        [Display(Name ="Sales Person")]
        public string SalesPersonUserName { get; set; }
        public string SalesPersonFullName { get; set; }

        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        [Display(Name="Store")]
        public int StoreID { get; set; }
        [Display(Name = "Period")]
        public int CRMPeriodId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
    }

    public class CRMDepositLine : TrackUserOperation
    {
        public int CRMDepositLineId { get; set; }
        public int CRMDepositId { get; set; }
        public virtual CRMDeposit CRMDeposit { get; set; }
        [Display(Name ="Receive Method")]
        public int ReceiveMethodId { get; set; }
        public decimal DepositAmount { get; set; }
        public string Reference { get; set; }
        public string Remark { get; set; }

    }

    public class CRMDepositValidation : VoucherValidation
    {
        [Key]
        public int CRMDepositValidationId { get; set; }
        public int CRMDepositId { get; set; }
        public virtual CRMDeposit CRMDeposit { get; set; }
    }
    public class CRMDepositDistribution : VoucherDistribution
    {
        [Key]
        public int CRMDepositDistributionId { get; set; }
        public int CRMDepositId { get; set; }
        public virtual CRMDeposit CRMDeposit { get; set; }
    }
}
