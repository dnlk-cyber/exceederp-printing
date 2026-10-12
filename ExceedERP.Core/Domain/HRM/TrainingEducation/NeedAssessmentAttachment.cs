using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.TrainingEducation
{
    public class NeedAssessmentAttachment: TrackUserSettingOperation
    {
        [Key]
        public int NeedAssessmentAttachmentId { get; set; }

        [Required]
        public int NeedAssessmentId { get; set; }

        public virtual NeedAssessment NeedAssessment { get; set; }

        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }

        public string ContentType { get; set; }

        public long FileSize { get; set; }

    }
}
