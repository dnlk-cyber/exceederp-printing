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
    /// Training Programs of trainees
    /// What programs the trainee is enroled in, the status of the training
    /// </summary>
    public class TraineeProgram : TrackUserSettingOperation
    {
        [Key]
        public int TraineeProgramId { get; set; }
        [Display(Name ="Program")]
        public int ProgramId { get; set; }
        [Display(Name ="Trainee")]
        public int CmsTraineeId { get; set; }

        public virtual CmsTrainee Trainee { get; set; }

        [Display(Name ="Thoery")]
        public decimal TheoreticalExamResult { get; set; }
        [Display(Name ="Practical")]
        public decimal PracticalExamResult { get; set; }

        public TrainingProgramStatus TrainingStatus { get; set; }
    }
}
