using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.PerformanceManagment
{
    public class PerformanceRequestPeriod: DepartmentOperations
    {
        [Key]
        public int PerformanceRequestPeriodId { get; set; }
        [Required]
        [Display(Name ="Budget Year")]
        public int FiscalYearId { get; set; }
        [Required]
        [Display(Name ="Period")]
        public int PeriodId { get; set; }
        [Display(Name ="Work Unit")]
        public int OrgStructureId { get; set; }
        [NotMapped]
        [Display(Name ="Department Head Name")]
        public string Head { get; set; }
        



    }

}
