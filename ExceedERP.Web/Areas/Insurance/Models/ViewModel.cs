using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExceedERP.Web.Areas.Insurance.Models
{
    public class EmployeeVM
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
    }
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
    }

    public class BranchViewModel
    {
        public int BranchId { get; set; }
        public string Name { get; set; }
    }

    public class BusinessUnitViewModel
    {
        public int BusinessUnitId { get; set; }
        public string Name { get; set; }
    }

    public class OrgStructureViewModel
    {
        public int OrgStructureId { get; set; }
        public string Name { get; set; }
    }

    public class FiscalYearViewModel
    {
        public int GlFiscalYearId { get; set; }
        public string Name { get; set; }
    }
    public class GLPeriodViewModel
    {
        public int GLPeriodId { get; set; }
        public string Name { get; set; }
    }

}