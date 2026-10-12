using ExceedERP.Reporting.HRM.HRM.Letters;
using ExceedERP.Reporting.SharedDataSources;
using System.Collections.Generic;
using System.Drawing;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class PromotionVM : AddressVm//newly added
    {
        public string EmployeeId { get; set; }
        public string HeadName { get; set; }
        public string ApprovedBy { get; set; }
        public string HeadJobTitle { get; set; }
        public string LetteringEmployeeName { get; set; }
        public string LetteringEmployeeOrgStructure { get; set; }
        public string Title { get; set; }
        public string FullName { get; set; }
        public string Sex { get; set; }
        public string CurrentDepartement { get; set; }
        public string CurrentBranch { get; set; }
        public string CurrentSubBranch { get; set; }
        public string CurrentJobTitle { get; set; }
        public string CurrentRank { get; set; }
        public string CurrentStep { get; set; }
        public decimal CurrentSalary { get; set; }
        public string CurrentAmount { get; set; }
        public string PreviousDepartement { get; set; }
        public string PreviousBranch { get; set; }
        public string PreviousSubBranch { get; set; }
        public string PreviousJobTitle { get; set; }
        public string PreviousRank { get; set; }
        public string PreviousStep { get; set; }
        public decimal PreviousSalary { get; set; }
        public string PreviousAmount { get; set; }
        public string AssignmentDate { get; set; }
        public decimal Diffrence { get; set; }
        public string DiffrenceAmount { get; set; }
        public string Name { get; set; }
        public string AmharicName { get; set; }
        public string Abbreviation { get; set; }
        public string Address { get; set; }
        public string Pobox { get; set; }
        public Image LogoFileName { get; set; }
        public Image Signature { get; set; }
        public string Date { get; set; }
        public List<PromotionLetterTwoDetail> PromotionLetters { get; set; }


    }




}