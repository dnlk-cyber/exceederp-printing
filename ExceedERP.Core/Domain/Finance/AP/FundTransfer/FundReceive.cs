using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP.FundTransfer
{

    public class FundReceive : DepartmentNewOperations
    {
        public int FundReceiveId { get; set; }
        public int FundTransferId { get; set; }
        public FundTransfer FundTransfer { get; set; }
        [Display(Name = "Receive Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReceiveDate { get; set; }      
        public decimal AmountReceived { get; set; }      
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        [Display(Name = "Business Unit")]
        public int BusinessUnitId { get; set; }

        [Display(Name = "From Branch/Project")]
        public int FromBranchId { get; set; }
        [Display(Name = "From Business Unit")]
        public int FromBusinessUnitId { get; set; }
       
        [Display(Name = "To Bank Account")]
        public int BankAccountId { get; set; }      
     
        //for access or view
        public int CreatedAtBranchId { get; set; }
        public int CreatedAtBusinessUnitId { get; set; }
        public int CreatedAtOrgStructureId { get; set; }
        public virtual ICollection<FundReceiveDistribution> FundReceiveDistributions { get; set; }
        public virtual ICollection<FundReceiveValidation> FundReceiveValidations { get; set; }
       
    }

    public class FundReceiveValidation : VoucherValidation
    {
        public int FundReceiveValidationId { get; set; }
        public int FundReceiveId { get; set; }
        public virtual FundReceive FundReceive { get; set; }

    }

    public class FundReceiveDistribution : VoucherDistribution
    {
        public FundReceiveDistribution()
        {
            Debit = 0;
            Credit = 0;
        }
        public int FundReceiveDistributionId { get; set; }
        public int FundReceiveId { get; set; }
        public virtual FundReceive FundReceive { get; set; }
    }


}
