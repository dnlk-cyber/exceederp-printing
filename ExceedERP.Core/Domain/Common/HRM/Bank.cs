using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Common.HRM
{
    public class Bank
    {
        [Key]
        public int BankId { get; set; }
        [Display(Name = "Common_Bank_Name", ResourceType = typeof(Localization.Resources))]
        public string Name { get; set; }
        [Display(Name = "Common_Bank_Category", ResourceType = typeof(Localization.Resources))]
        public string Category { get; set; }
        [Display(Name = "Common_Bank_IsActive", ResourceType = typeof(Localization.Resources))]
        public bool IsActive { get; set; }
        public int? ParentId { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}