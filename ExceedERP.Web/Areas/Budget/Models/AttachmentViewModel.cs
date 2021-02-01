using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ExceedERP.Core.Domain.Budget;

namespace ExceedERP.Web.Areas.Budget.Models
{

    public class BudgetRequestLineViewModel
    {
        public int BudgetRequestLineId { get; set; }
        public bool Accepted { get; set; }
        public string Remark { get; set; }
        [Required]
        public string AccountCode { get; set; }

        public bool CalculateAuto { get; set; }
     

        [Display(Name = "Request Type")]
        public BudgetRequestType BudgetRequestType { get; set; }
        [Required]
        public string Description { get; set; }

        [Display(Name = "Quantity")]
        public decimal PurchasedRequestUnit { get; set; }
        [Display(Name = " Rate")]
        public decimal PurchasedRequestUnitPrice { get; set; }


        [Display(Name = "Rent Total Hr")]
        public decimal RentTotalHr { get; set; }
        [Display(Name = "  Rent Per Hour")]
        public decimal RentPerHour { get; set; }



        [Display(Name = "Daily Salary")]
        public decimal DailySalary { get; set; }
        [Display(Name = " No Working Days")]
        public decimal NoWorkingDay { get; set; }





        [Display(Name = "Salary Per Month")]
        public decimal SalaryPerMonth { get; set; }



        [Display(Name = "Responsible Allowance")]
        public decimal ResponsibleAllowance { get; set; }
        [Display(Name = "Disturbance Allowance")]
        public decimal DisturbanceAllowance { get; set; }
        [Display(Name = "Proffiessional Allowance")]
        public decimal ProffiessionalAllowance { get; set; }
        public decimal OverTime { get; set; }
        public string Unit { get; set; }



        [Display(Name = " From Month")]
        public string FromMonth { get; set; }


        [Display(Name = " To Month")]
        public string ToMonth { get; set; }

        public decimal Amount { get; set; }
        public string Reason { get; set; }
    }
    public class BudgetRequestLineList
    {
        public List<BudgetRequestLineViewModel> BudgetRequestLine { get; set; }
    }
    public class AttachmentViewModel
    {
        public int AttachmentId { get; set; }

        public string CategoryName { get; set; }
        public string Branch { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string FiscalYear { get; set; }
        public string WorkUnitName { get; set; }
        public decimal Amount { get; set; }

    }
}