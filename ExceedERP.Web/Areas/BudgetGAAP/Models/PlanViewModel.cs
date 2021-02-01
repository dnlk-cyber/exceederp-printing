using ExceedERP.Core.Domain.Budget.GAAP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ExceedERP.Web.Areas.BudgetGAAP.Models
{

    public class ProfitLossViewModel
    {
        public int ProfitLossStatementId { get; set; }
        [DisplayName("Category")]
        public int ProfitLossStatementCategoryId { get; set; }
        [Required]
        public string Name { get; set; }
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
        public Enums.ProfitLostType CategoryType { get; set; }

        [DisplayName("From Account")]
        public string FromAccCode { get; set; }
        [DisplayName("To Account")]
        public string ToAccCode { get; set; }
        [DisplayName("Year")]
        public int GlFiscalYearId { get; set; }
    }
    public class PlanViewModel
    {
        public int Id { get; set; }
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

        public string CategoryType { get; set; }

        [DisplayName("From Account")]
        public string FromAccCode { get; set; }
        [DisplayName("To Account")]
        public string ToAccCode { get; set; }
        [DisplayName("Year")]
        public int GlFiscalYearId { get; set; }
    }
}