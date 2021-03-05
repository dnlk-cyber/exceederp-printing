using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CMS
{
    /// <summary>
    /// course of trainee
    /// what course, when it starts, when it ends, amount
    /// </summary>
   public class CMSTraineeCourse : TrackUserSettingOperation
    {
        [Key]
        public int CMSTraineeCourseId { get; set; }

        [Display(Name ="Course")]
        public int CMSCourseId { get; set; }

        public virtual CMSCourse Course { get; set; }
        [Display(Name = "Trainee")]
        public int CmsTraineeId { get; set; }

        public virtual CmsTrainee Trainee { get; set; }

        [Display(Name = "Thoery")]
        public decimal TheoreticalExamResult { get; set; }
        [Display(Name = "Practical")]
        public decimal PracticalExamResult { get; set; }

        public TrainingProgramStatus TrainingStatus { get; set; }

        [Display(Name ="Place")]
        public string PlaceOfTrainingOrExamination { get; set; }

        [Display(Name ="From")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartOfTraining { get; set; }

        [Display(Name = "To")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndOfTraining { get; set; }

        [Display(Name = "Target Amount")]
        public decimal TargetAmount { get; set; }

    }
}
