using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class ApplicantDetail
    {
        public int ApplicantDetailId { get; set; }
        public int ApplicantRegistrationId { get; set; }
        public Gender Sex { get; set; }
        public int Age { get; set; }
        public string ApplicantName { get; set; }
        public string Address { get; set; }
        public string Training { get; set; }
        public string VacancyCode { get; set; }
        [NotMapped]
        public double Result { get; set; }
        public string Description { get; set; }
        public Languages Language { get; set; }
      //  [Display(Name ="Language")]
       // public string LanguageForDisplay { get; set; }
        [DisplayName("Upload File")]
        public string FilePath { get; set; }
        [NotMapped]
        public HttpPostedFileBase DocumentFile { get; set; }
        public string FileName { get; set; }

    }
}
