using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Common.HRM
{
    public class BankReference
    {
        [Key]
        public int BankReferenceId { get; set; }
  [Display(Name = "Common_BankReference_BankId", ResourceType = typeof(Localization.Resources))]
        public int BankId { get; set; }
        public virtual Bank Bank { get;set; }
        public string ReferenceType { get; set; }
        public string Reference { get; set; }
        [Display(Name = "Common_BankReference_Type", ResourceType = typeof(Localization.Resources))]
        public BankAccountType Type { get; set; }
          [Display(Name = "Common_BankReference_AccountNumber", ResourceType = typeof(Localization.Resources))]
        public string AccountNumber { get; set; }
         [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}