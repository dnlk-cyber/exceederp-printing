using System;

using System.Collections.Generic;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    #region Employee Placement Models

    public class EmployeePlacementVM
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public string FromBranch { get; set; }
        public string Branch { get; set; }
        public string BussinessUnit { get; set; }
        public string ParentStructureName { get; set; }
        public string Departement { get; set; }
        public string Boss { get; set; }
        public string JobTitle { get; set; }
        public string HireDate { get; set; }
        public DateTime AssignmentDate { get; set; }
        public string Grade { get; set; }
        public string Age { get; set; }
        public string Step { get; set; }
        public string AssignmentType { get; set; }
        public int YearOfPromotion { get; set; }
        public string ExistingScale { get; set; }
        public decimal Salary { get; set; }
        public string NextScale { get; set; }
        public decimal NextSalary { get; set; }
        public decimal Increement { get; set; }
        public string ScaleDate { get; set; }
        public string NextScaleDate { get; set; }
    }

    #endregion

    #region Salary Range By Project Models

    public class SalaryRangeByProjectReportVM
    {
        public string CompanyName { get; set; } = "MCG CONSTRUCTION P.L.C";
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<ProjectSalaryRangeRowVM> Rows { get; set; } = new List<ProjectSalaryRangeRowVM>();
        public ProjectSalaryRangeRowVM GrandTotal { get; set; } = new ProjectSalaryRangeRowVM { ProjectName = "Total" };
    }

    public class ProjectSalaryRangeRowVM
    {
        public int? BranchId { get; set; }
        public string ProjectName { get; set; }

        public int RangeA_Female { get; set; }
        public int RangeA_Male { get; set; }
        public int RangeA_Total => RangeA_Female + RangeA_Male;

        public int RangeB_Female { get; set; }
        public int RangeB_Male { get; set; }
        public int RangeB_Total => RangeB_Female + RangeB_Male;

        public int RangeC_Female { get; set; }
        public int RangeC_Male { get; set; }
        public int RangeC_Total => RangeC_Female + RangeC_Male;

        public int RangeD_Female { get; set; }
        public int RangeD_Male { get; set; }
        public int RangeD_Total => RangeD_Female + RangeD_Male;

        public int RangeE_Female { get; set; }
        public int RangeE_Male { get; set; }
        public int RangeE_Total => RangeE_Female + RangeE_Male;

        public int RangeF_Female { get; set; }
        public int RangeF_Male { get; set; }
        public int RangeF_Total => RangeF_Female + RangeF_Male;

        public int RowTotal => RangeA_Total + RangeB_Total + RangeC_Total +
                               RangeD_Total + RangeE_Total + RangeF_Total;
    }
    #endregion

    #region Salary Range By Department Models

    public class SalaryRangeByDepartmentReportVM
    {
        public string CompanyName { get; set; } = "MCG CONSTRUCTION P.L.C";
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<DepartmentSalaryRangeRowVM> Rows { get; set; } = new List<DepartmentSalaryRangeRowVM>();
        public DepartmentSalaryRangeRowVM GrandTotal { get; set; } = new DepartmentSalaryRangeRowVM { DepartmentName = "Total" };
    }

    public class DepartmentSalaryRangeRowVM
    {
        public int OrgStructureId { get; set; }
        public string DepartmentName { get; set; }

        // Range A: 0 - 10,000
        public int RangeA_Female { get; set; }
        public int RangeA_Male { get; set; }
        public int RangeA_Total { get { return RangeA_Female + RangeA_Male; } }

        // Range B: 10,001 - 20,000
        public int RangeB_Female { get; set; }
        public int RangeB_Male { get; set; }
        public int RangeB_Total { get { return RangeB_Female + RangeB_Male; } }

        // Range C: 20,001 - 30,000
        public int RangeC_Female { get; set; }
        public int RangeC_Male { get; set; }
        public int RangeC_Total { get { return RangeC_Female + RangeC_Male; } }

        // Range D: 30,001 - 40,000
        public int RangeD_Female { get; set; }
        public int RangeD_Male { get; set; }
        public int RangeD_Total { get { return RangeD_Female + RangeD_Male; } }

        // Range E: 40,001 - 50,000
        public int RangeE_Female { get; set; }
        public int RangeE_Male { get; set; }
        public int RangeE_Total { get { return RangeE_Female + RangeE_Male; } }

        // Range F: > 50,000
        public int RangeF_Female { get; set; }
        public int RangeF_Male { get; set; }
        public int RangeF_Total { get { return RangeF_Female + RangeF_Male; } }

        // Row Aggregates
        public int TotalFemale { get { return RangeA_Female + RangeB_Female + RangeC_Female + RangeD_Female + RangeE_Female + RangeF_Female; } }
        public int TotalMale { get { return RangeA_Male + RangeB_Male + RangeC_Male + RangeD_Male + RangeE_Male + RangeF_Male; } }
        public int RowTotal { get { return RangeA_Total + RangeB_Total + RangeC_Total + RangeD_Total + RangeE_Total + RangeF_Total; } }
    }
    #endregion

    #region Common & Shared Structures

    public class GenderCountRange
    {
        public int Female { get; set; }
        public int Male { get; set; }
        public int Total => Female + Male;
    }

    public class SalaryRangeFilterVM
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? BranchId { get; set; }
        public int? OrgStructureId { get; set; }
    }

    #endregion
}