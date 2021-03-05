using ExceedERP.Core.Domain.CMS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.CMS.Printing.ViewModels
{
    public class WeldersQualificationTestCertificateViewModel
    {
        public Image ProfilePicture { get; set; }
        public Image Barcode { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string RegistrationNumber { get; set; }
        
        public string FullName { get; set; }
        public string MethodOfIdentification { get; set; }

        public string Identification { get; set; }
        public string PlaceOfBirth { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public WelderPerformanceQualification WelderPerformanceQualification { get; set; }
        public DateTime? CertificateValidUntil { get; set; }
    }
}
