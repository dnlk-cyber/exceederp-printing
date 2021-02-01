using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
   public class TerminationAcceptanceLetterVM
    {
        public string Name { get; set; }
        public string AmharicName { get; set; }
        public string EnglishName { get; set; }
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
        /// <summary>
        /// ////////////////////////////////////
        /// </summary>
        public string EmpName { get; set; }
        public string RequestDate { get; set; }
        public string TerminationDate { get; set; }
        public string EmpAddress { get; set; }
        public string Title { get; set; }
        public string TempOne { get; set; }
    }
}
