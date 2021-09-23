using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.CMS
{
    public class CMSCourse : TrackUserSettingOperation
    {
        [Key]
        public int CMSCourseId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        [Display(Name = "Program")]
        public int CMSProgramId { get; set; }

        [Display(Name = "Credit Hours")]
        public int CreditHours { get; set; }

    }
}
