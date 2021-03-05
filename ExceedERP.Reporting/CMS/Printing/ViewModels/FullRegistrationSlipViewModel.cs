using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.CMS.Printing.ViewModels
{
    public class FullRegistrationSlipViewModel
    {
        public Image ProfilePicture { get; set; }
        public Image Barcode { get; set; }
        public string ParticipantNumber { get; set; }
        public string Requirement { get; set; }
        public string FullName { get; set; }
        public string CompanyName { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public string PlaceOfBirth { get; set; }

        public string Address { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }

        public string IdentificationMethod { get; set; }
        public string IdNumber { get; set; }
        public DateTime? IdExpiryDate { get; set; }

        public string CourseShortCode { get; set; }
        public string OrderNumber { get; set; }
        public int Type { get; set; }

        public List<RegistrationSlipCourseViewModel> CourseList { get; set; } = new List<RegistrationSlipCourseViewModel>();
    }

    public class RegistrationSlipCourseViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string PlaceOfTrainingOrExamination { get; set; }

        public DateTime StartOfTraining { get; set; }
        public DateTime EndOfTraining { get; set; }
        public decimal TargetAmount { get; set; }
    }
}
