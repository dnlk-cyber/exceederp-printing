using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Budget
{
    public class BudgetCategoryDetail
    {
        [Key]
        public int BudgetCategoryDetailID { get; set; }
        [Display(Name = "Budget Code")]
        public string BudgetCode { get; set; }
        [Required]
        [Display(Name = "Budget Item")]
        public string BudgetItem { get; set; }
        [Required]
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Detail Description")]
        public string BudgetCategoryDetailDescription { get; set; }
        //public decimal? Amount { get; set; }
        public int BudgetCategoryId { get; set; }
        public BudgetCategory BudgetCategory { get; set; }
    }
}
