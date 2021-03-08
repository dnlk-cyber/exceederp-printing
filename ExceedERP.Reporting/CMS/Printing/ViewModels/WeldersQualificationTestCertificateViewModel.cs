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
        public string Designation { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string RegistrationNumber { get; set; }
        
        public string FullName { get; set; }
        public string MethodOfIdentification { get; set; }

        public string Identification { get; set; }
        public string PlaceOfBirth { get; set; }
        public DateTime? DateOfBirth { get; set; }

        // 
        public string PlateOrPipe { get; set; }
        public string RangeOfPlateOrPipe { get; set; }

        public string ThicknessOfWeldingMetal { get; set; }
        public string RangeOfThicknessOfWeldingMetal { get; set; }

        public string OutsideDiameter { get; set; }
        public string RangeOfOutsideDiameter { get; set; }

        public WelderPerformanceQualification WelderPerformanceQualification { get; set; }
        public DateTime CertificateValidUntil { get; set; }
    }
}
