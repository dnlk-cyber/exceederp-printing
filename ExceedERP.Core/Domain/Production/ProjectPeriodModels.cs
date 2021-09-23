using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Production
{
    public class BudgetYear
    {
        [Key]
        public Guid BudgetYearId { get; set; }

        [Display(Name = "Year")]
        public string Name { get; set; }

        [Display(Name = "Start Date ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateFrom { get; set; }

        [Display(Name = "End Date ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateTo { get; set; }

        [Required]
        public TimeRangeStatus Status { get; set; }

        public virtual ICollection<ProductionPeriod> ProductionPeriods { get; set; }
        public bool IsPermanentlyClosed { get; set; }
    }
    public class ProductionYear : TrackUserOperation
    {
        [Key]
        public int ProductionYearId { get; set; }

        [Display(Name = "Year")]
        public string Name { get; set; }

        [Display(Name = "Start Date ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateFrom { get; set; }

        [Display(Name = "End Date ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateTo { get; set; }

        [Required]
        public FiscalYearStatus Status { get; set; }
        public bool IsPermanentlyClosed { get; set; }
    }
    public class ProductionPeriod : TrackUserOperation
    {
        [Key]
        public int ProductionPeriodId { get; set; }

        [Required]
        //[Display(Name = "GLPeriod_Name", ResourceType = typeof(Localization.Resources))]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "From")]
        public DateTime DateFrom { get; set; }

        //[Display(Name = "GLPeriod_DateFrom", ResourceType = typeof(Localization.Resources))]
        public string DateFromAmharic { get; set; }

        [Display(Name = "To")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateTo { get; set; }

        //[Display(Name = "GLPeriod_DateTo", ResourceType = typeof(Localization.Resources))]
        public string DateToAmharic { get; set; }

        [Required]
        public FiscalYearStatus Status { get; set; }

        public bool IsPermanentlyClosed { get; set; }
    }
}
