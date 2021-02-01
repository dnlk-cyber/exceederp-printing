using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CMS
{
    public class CMSCourse : TrackUserSettingOperation
    {
        [Key]
        public int CMSCourseId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        [Display(Name="Credit Hours")]
        public int CreditHours { get; set; }

    }
}
