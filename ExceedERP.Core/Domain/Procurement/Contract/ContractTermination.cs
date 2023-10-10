using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.Contract
{
    public class ContractTermination : NewOperations
    {
        public int ContractTerminationId { get; set; }
        public int ContractManagementID { get; set; }
        public virtual ContractManagement ContractManagement { get; set; }
        [Display(Name = "Termination Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public string Reason { get; set; }
        [Display(Name = "Generate Award ?")]
        public bool GiveAwardToNextSupplier { get; set; }
        public virtual ICollection<ContractTerminationAttachment> ContractTerminationAttachments { get; set; }
        public virtual ICollection<ContractTerminationValidation> ContractTerminationValidations { get; set; }
    }
}