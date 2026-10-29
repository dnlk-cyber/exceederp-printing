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
    public class Actual: TrackUserSettingOperation
    {
        [Key]
        public int ActualId { get; set; }
        [Display(Name = "Fiscal Year")]
        public int GlFiscalYearId { get; set; }
        public  virtual GlFiscalYear FiscalYear { get; set; }
        public int? PlanId { get; set; }
        public bool IsDepartnental { get; set; }
        public bool IsPersonal { get; set; }
        [Display(Name = "Description(Key Result Area)")]
        public string Description { get; set; }
       

    }
}
