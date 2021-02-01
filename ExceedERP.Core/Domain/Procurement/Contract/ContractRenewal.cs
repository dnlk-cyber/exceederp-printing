using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Finance.AP.Travel;

namespace ExceedERP.Core.Domain.Procurement.Contract
{
    public class ContractRenewal : NewOperations
    {
        public int ContractRenewalId { get; set; }
        public int ContractManagementID { get; set; }
        public virtual ContractManagement ContractManagement { get; set; }
        [Display(Name = "Sign Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime SigningDate { get; set; }
        [Display(Name = "Days Added")]
        public int DaysAdded { get; set; }
        [Display(Name = "Additional Payment")]
        public decimal AdditionalPayment { get; set; }
        public string Remark { get; set; }
        public virtual ICollection<ContractRenewalValidation> RenewalValidations { get; set; }
    }
}