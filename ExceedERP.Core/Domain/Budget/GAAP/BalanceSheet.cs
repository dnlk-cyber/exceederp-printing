using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.Budget.GAAP
{

    public class BalanceSheet : TrackUserOperation
    {

        public int BalanceSheetId { get; set; }
        [Required]
        public string Name { get; set; }
        [DisplayName("Category")]
        public int BalanceSheetCategoryId { get; set; }
        [DisplayName("1st Quarter")]
        public decimal First { get; set; }
        [DisplayName("2nd Quarter")]
        public decimal Second { get; set; }
        [DisplayName("3rd Quarter")]
        public decimal Third { get; set; }
        [DisplayName("4th Quarter")]
        public decimal Fourth { get; set; }
        [NotMapped]
        public decimal Total { get; set; }
        [NotMapped]
        [DisplayName("Type")]
        public Enums.BalanceSheetType CategoryType { get; set; }

        [DisplayName("From Account")]
        public string FromAccCode { get; set; }
        [DisplayName("To Account")]
        public string ToAccCode { get; set; }
        [DisplayName("Year")]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
        public virtual BudgetSheetCategory BalanceSheetCategory { get; set; }
    }

}
