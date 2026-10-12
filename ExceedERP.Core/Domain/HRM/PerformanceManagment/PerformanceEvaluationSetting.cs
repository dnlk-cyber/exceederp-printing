using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.PerformanceManagment
{
    public class PerformanceEvaluationSetting : TrackUserSettingOperation
    {
        public int PerformanceEvaluationSettingId { get; set; }

        [Display(Name = "Setting Name")]
        public string Name { get; set; }
        [Display(Name = "Total KPI Weight")]
        public decimal? TotalWeight { get; set; }
        // If null → default to 100
        [Display(Name = "Maximum Score Per KPI")]
        public decimal MaxScorePerMeasurement { get; set; }
        [Display(Name = "Enforce Total Weight Validation")]
        //  Enforce total weight validation before approval?
        public bool EnforceTotalWeightValidation { get; set; }
        // If not manual, use predefined scoring scale
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
    }


    public class PerformanceAllowedScore : TrackUserSettingOperation
    {
        public int PerformanceAllowedScoreId { get; set; }

        // Example: Poor, Fair, Good, Very Good, Excellent
        public string Description { get; set; }

        // Example: 4, 8, 12, 16, 20
        public decimal ScoreValue { get; set; }

        public int Order { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
    }

}
