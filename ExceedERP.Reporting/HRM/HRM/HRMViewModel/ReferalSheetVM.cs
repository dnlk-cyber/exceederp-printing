using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
   public class ReferalSheetVM
    {
        public string FullName { get; set; }
        public string Age { get; set; }
        public string Sex { get; set; }
        public DateTime DateRefer { get; set; }
        public string Diagnosis { get; set; }
        public string TreatmentGiven { get; set; }
        public string ReferredFrom { get; set; }
        public string ReferredTo { get; set; }
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
    }
}
