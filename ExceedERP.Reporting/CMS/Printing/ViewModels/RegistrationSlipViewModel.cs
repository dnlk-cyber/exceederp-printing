using ExceedERP.Core.Domain.CMS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.CMS.Printing.ViewModels
{
    public class RegistrationSlipViewModel
    {
        public Image ProfilePicture { get; set; }
        public Image Barcode { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string RegistrationNumber { get; set; }

        public string Program { get; set; }
        public string Course { get; set; }


        public string FullName { get; set; }

        public string CompanyName { get; set; }

        public string MobilePhone { get; set; }

        public int   Type { get; set; }
    }
}
