using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CMS
{
    public class CmsTraineeIdentification : TrackUserSettingOperation
    {
        [Key]
        public int CmsTraineeIdentificationId { get; set; }
        public int CmsTraineeId { get; set; }
        public virtual CmsTrainee CmsTrainee { get; set; }
        public CmsTraineeIdentificationType IdentificationType { get; set; }
        public string IdNumber { get; set; }
        public string Remark { get; set; }
    }
}
