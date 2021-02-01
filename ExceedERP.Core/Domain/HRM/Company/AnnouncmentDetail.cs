using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.HRM.Company
{
    public class AnnouncmentDetail : TrackUserOperation
    {
        [Key]
        public int AnnouncDetailId { get; set; }
        [Display(Name = "HR_Company_AnnouncmentDetail_Detail", ResourceType = typeof(Localization.Resources))]
        [AllowHtml]
        public string Detail { get; set; }
        public int AnnouncmentId { get; set; }
        public Announcment Announcment { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}