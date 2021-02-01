using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.Budget
{


    public enum BudgetRequestType
    {
        Material_Purchase,

        Equipment_Rent,
        Gas_Oil,
        Equipment_Purchase,
        SubContract_Aggrement,
        Other_Expense,
        Skilled_Daily_Labour,
        Staff_Monthly_Salary,
        Other

    }
    public class BudgetRequestLine : Operations
    {
        public int BudgetRequestLineId { get; set; }
        public int BudgetRequestId { get; set; }
        [Required]
        public string AccountCode { get; set; }

        public bool CalculateAuto { get; set; }
        public bool Accepted { get; set; }

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

        [ForeignKey("BudgetRequestId")]
        public virtual BudgetRequest BudgetRequest { get; set; }
    }
}
