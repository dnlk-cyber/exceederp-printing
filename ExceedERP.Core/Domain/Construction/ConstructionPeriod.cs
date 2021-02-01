using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Construction
{
    public class ConstructionPeriod : TrackUserOperation
    {
        [Key]
        public int ConstructionPeriodId { get; set; }
        [Required]
        [Display(Name = "GLPeriod_Name", ResourceType = typeof(Localization.Resources))]
        public string Name { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "GLPeriod_DateFrom", ResourceType = typeof(Localization.Resources))]
        public DateTime DateFrom { get; set; }
        [Display(Name = "GLPeriod_DateFrom", ResourceType = typeof(Localization.Resources))]
        public string DateFromAmharic { get; set; }
        [Display(Name = "GLPeriod_DateTo", ResourceType = typeof(Localization.Resources))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateTo { get; set; }
        [Display(Name = "GLPeriod_DateTo", ResourceType = typeof(Localization.Resources))]
        public string DateToAmharic { get; set; }
        [Required]

        public FiscalYearStatus Status { get; set; }
        public bool IsPermanentlyClosed { get; set; }

        public ICollection<PeriodMapping> PerformanceEvaluationPeriods { get; set; }
    }

    public class ConstructionSubPeriod : TrackUserOperation
    {
        [Key]
        public int ConstructionSubPeriodId { get; set; }
        public int ConstructionPeriodId { get; set; }
        [Required]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "From")]
        public DateTime DateFrom { get; set; }

        [Display(Name = "To")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateTo { get; set; }

        public virtual ConstructionPeriod ConstructionPeriod { get; set; }
    }
}
