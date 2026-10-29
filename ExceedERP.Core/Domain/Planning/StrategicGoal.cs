using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Planning
{
    public class StrategicGoal : TrackUserSettingOperation
    {
        public int StrategicGoalId { get; set; }
        public int StrategicPlanId { get; set; }

        [Required]
        [Display(Name = "Strategic Goal")]
        public string Name { get; set; }

        public string Description { get; set; }
        public string Remark { get; set; }

        [Display(Name = "Strategic Objective")]
        public string StrategicObjective { get; set; }

        [Display(Name = "Code / SNo")]
        public string Code { get; set; }

        [Display(Name = "Responsible Body")]
        public string ResponsibleBody { get; set; }

        [Display(Name = "Unit of Measure (UOM)")]
        public string UOM { get; set; }

        [Display(Name = "Baseline")]
        public string Baseline { get; set; }

        [Display(Name = "2015 EFY Target")]
        public string Target2015EFY { get; set; }

        [Display(Name = "2016 EFY Target")]
        public string Target2016EFY { get; set; }

        [Display(Name = "2017 EFY Target")]
        public string Target2017EFY { get; set; }

        [Display(Name = "2018 EFY Target")]
        public string Target2018EFY { get; set; }

        [Display(Name = "2019 EFY Target")]
        public string Target2019EFY { get; set; }

        public virtual StrategicPlan StrategicPlan { get; set; }
    }
}
