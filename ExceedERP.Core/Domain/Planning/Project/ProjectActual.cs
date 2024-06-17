using ExceedERP.Core.Domain.Finance.GL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Planning.Project
{
    public class ProjectActual
    {
        [Key]
        public int ProjectActualId { get; set; }
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
        public string Description { get; set; }
        [Display(Name ="Fiscal Year")]
        [Required]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear FiscalYear { get; set; }
        
    }
    public class ProjectMonthlyActual
    {
        [Key]
        public int ProjectMonthlyActualId { get; set; }
        [Required]
        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }
        [Display(Name = "Plan Value")]
        public decimal PlanValue { get; set; }
        [Display(Name = "Actual Value")]
        public decimal ActualValue { get; set; }
        public int ProjectActualId { get; set; }
        public virtual ProjectActual Actual { get; set; }

    }
    public class ProjectWeeklyActual
    {
        [Key]
        public int ProjectWeeklyActualId { get; set; }
        [Display(Name = "Week One")]
        public decimal WeekOne { get; set; }
        [Display(Name = "Week Two")]
        public decimal WeekTwo { get; set; }
        [Display(Name = "Week Three")]
        public decimal WeekThree { get; set; }
        [Display(Name = "Week Four")]
        public decimal WeekFour { get; set; }
        public int ProjectMonthlyActualId { get; set; }
        public virtual ProjectMonthlyActual MonthlyActualDetails { get; set; }


    }
}
