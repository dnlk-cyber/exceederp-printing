using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;

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

        [Display(Name ="Date of Issue")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfIssue { get; set; }

        [Display(Name = "Date of Expiry")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfExpiry { get; set; }

        public string Remark { get; set; }
    }
}
