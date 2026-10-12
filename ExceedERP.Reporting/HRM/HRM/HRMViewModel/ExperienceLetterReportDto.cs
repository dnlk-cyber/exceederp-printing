using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class ExperienceLetterReportDto
    {
        // Reference & Dates
        public string ReferenceNumber { get; set; }
        public string LetterDateAmharic { get; set; }
        public string LetterDateGregorian { get; set; }

        // Employee Names & Titles
        public string EmployeeCode{ get; set; }  // አቶ / ወ/ሮ
        public string EmployeeTitleAmharic { get; set; }  // አቶ / ወ/ሮ
        public string EmployeeTitleEnglish { get; set; }  // Mr. / Mrs.
        public string FullNameAmharic { get; set; }
        public string FullNameEnglish { get; set; }
        public string PronounSubject { get; set; }        // he / she
        public string PronounPossessive { get; set; }     // his / her
        public string PronounObject { get; set; }         // him / her

        // Placement & Position
        public string DepartmentAmharic { get; set; }
        public string DepartmentEnglish { get; set; }
        public string JobTitleAmharic { get; set; }
        public string JobTitleEnglish { get; set; }
        public string StartDateAmharic { get; set; }
        public string StartDateGregorian { get; set; }
        public string EndDateAmharic { get; set; }
        public string EndDateGregorian { get; set; }
        public string InternalExperienceEnglish { get; set; }
        public string InternalExperienceAmharic { get; set; }
        public string ResignationDateGregorian { get; set; }
        public string ResignationDateAmharic { get; set; }
        public bool IsCurrentEmployee { get; set; }

        // Earnings & Allowances
        public decimal MonthlyGrossSalary { get; set; }
        public decimal ResponsibilityAllowance { get; set; }
        public decimal ProfessionalAllowance { get; set; }
        public decimal HouseAllowance { get; set; }
        public decimal TransportAllowance { get; set; }
        public decimal PositionAllowance { get; set; }

        // Signer / Authorized Head
        public string SignerNameAmharic { get; set; }
        public string SignerNameEnglish { get; set; }
        public string SignerJobTitleAmharic { get; set; }
        public string SignerJobTitleEnglish { get; set; }
        public Image SignatureImage { get; set; }
        public Image CompanyStamp { get; set; }
    }
}
