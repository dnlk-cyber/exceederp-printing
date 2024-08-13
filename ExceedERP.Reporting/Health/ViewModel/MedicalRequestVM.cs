using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Health.ViewModel
{
   public class MedicalRequestVM
    {
        public string FullName { get; set; }
        public string Department { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public string AmharicName { get; set; }
        public string Abbreviation { get; set; }
        public string Address { get; set; }
        public Image LogoFileName { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        public string City { get; set; }
        public string Fax { get; set; }
        public string Pobox { get; set; }
        public string Tel { get; set; }
        public string Date { get; set; }
        public string GeezDate { get; set; }
        public string Approver { get; set; }
        public string DepApprover { get; set; }
    }
}
