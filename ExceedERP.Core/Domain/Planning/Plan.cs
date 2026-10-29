using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Planning
{
    public class Plan: TrackUserSettingOperation
    {
        [Key]
        public int PlanId { get; set; }
        [Display(Name = "Is Departnental Plan")]
        public bool IsDepartnental { get; set; }
        [Display(Name = "Is Personal Plan")]
        public bool IsPersonal { get; set; }
        [Display(Name = "Description(Key Result Area)")]
        public string Description { get; set; }
        [StringLength(2000)]
        [Display(Name = "Reason")]
        public string Reason { get; set; }
        [Display(Name = "Fiscal Year")]
        public int GlFiscalYearId { get; set; }
        public GlFiscalYear GlFiscalYear { get; set; }
     

    }
}
