using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Finance.AP.Travel;

namespace ExceedERP.Core.Domain.Procurement.Contract
{
    public class ContractPayment : NewOperations
    {
        public int ContractPaymentId { get; set; }
        public int ContractManagementID { get; set; }
        public virtual ContractManagement ContractManagement { get; set; }
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [Display(Name = "Supplier Invoice")]
        public string SupplierInvoice { get; set; }

        [Display(Name = "Payment Amount")]
        public decimal PaymentAmount { get; set; }
        [Display(Name = "Apply Penality")]
        public bool ApplyPenality { get; set; }
        [Display(Name = "Penality Amount")]
        public decimal PenalityAmount { get; set; }
        [Display(Name = "Net Payable")]
        public decimal NetPayable { get; set; }
        [Display(Name = "Is Paid?")]
        public bool Paid { get; set; }
        [Display(Name = "With Out GRN/SRN?")]
        public bool WithOutGRN { get; set; }
        public virtual ICollection<ContractPaymentAttachment> ContractPaymentAttachements { get; set; }
        public virtual ICollection<ContractPaymentGrn> ContractPaymentGrns { get; set; }
        public virtual ICollection<ContractPaymentValidation> ContractPaymentValidations { get; set; }
    }
}