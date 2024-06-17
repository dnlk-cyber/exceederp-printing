using ExceedERP.Core.Domain.Finance.GL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Planning.Project
{
    public class ProjectPlan
    {
        [Key]
        public int ProjectPlanId { get; set; }
        [Display(Name = "Project Name")]
        [Required]
        public int ProjectInformationId { get; set; }
        [Display(Name = "Objective Name")]
        [Required]
        public int ProjectObjectiveId { get; set; }
        [Display(Name = "KPI Name")]
        [Required]
        public int ProjectKPIId { get; set; }
        [Display(Name = "Sub-KPI Name")]
        [Required]
        public int ProjectSubKPIId { get; set; }
        [Required]
        public decimal Baseline { get; set; }
        public decimal Target { get; set; }
        public decimal Plan { get; set; }
        [Display(Name ="To Date")]
        public decimal ToDate { get; set; }
        public string Description { get; set; }
        [Display(Name = "Fiscal Year")]
        [Required]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear FiscalYear { get; set; }



    }
    public class ProjectMonthlyPlan
    {
        [Key]
        public int ProjectMonthlyPlanId { get; set; }
        [Required]
        [Display(Name ="Period")]
        public int GLPeriodId { get; set; }
        public decimal Value { get; set; }
        public int ProjectPlanId { get; set; }
        public  virtual ProjectPlan GetPlan { get; set; }

    }
    public class ProjectWeeklyPlan
    {
        [Key]
        public int ProjectWeeklyPlanId { get; set; }
        [Display(Name = "Week One")]
        public decimal WeekOne { get; set; }
        [Display(Name = "Week Two")]
        public decimal WeekTwo { get; set; }
        [Display(Name = "Week Three")]
        public decimal WeekThree { get; set; }
        [Display(Name = "Week Four")]
        public decimal WeekFour { get; set; }
        public int ProjectMonthlyPlanId { get; set; }
        public virtual ProjectMonthlyPlan Monthlyplandetal { get; set; }



    }
}
