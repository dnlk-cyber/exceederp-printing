using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class NotPlacedEmployeesObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<NotPlacedEmployeeVM> GetUnPlacedEmployees(string status)
        {
            List<NotPlacedEmployeeVM> employeeList = new List<NotPlacedEmployeeVM>();

            var employees = db.Person_Employee.ToList();
            if (status == "Active")
            {
                employees = employees.Where(x => x.IsActive).ToList(); ;
            }
            if (status == "InActive")
            {
                employees = employees.Where(x => !x.IsActive).ToList(); ;
            }
            foreach (var emp in employees)
            {
                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);
                if (placement == null)
                {
                    var notPlaced = new NotPlacedEmployeeVM();

                    notPlaced.EmployeeID = emp.EmployeeCode;
                    notPlaced.EmployementType = emp.Status.ToString().ToLower();
                    notPlaced.FullName = emp.FullNameaEnglish;
                    if (emp.IsActive)
                    {
                        notPlaced.Status = "Active";
                    }
                    else
                    {
                        notPlaced.Status = "In Active";
                    }
                    employeeList.Add(notPlaced);
                }

            }
            return employeeList;
        }
    }

    [DataObject]
    public class PlacedButInActiveInEmployeesProfileObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<NotPlacedEmployeeVM> GetPlacedButInActive(string branchId)
        {
            List<NotPlacedEmployeeVM> employeeList = new List<NotPlacedEmployeeVM>();

            var placements = db.Placements.Where(x => x.isCurrent).ToList();

            if(branchId != null)
            {
                int.TryParse(branchId, out int braId);
                placements = placements.Where(x => x.BranchId == braId).ToList();
            }

            foreach (var place in placements)
            {
                var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == place.EmployeeId && !x.IsActive);
                if (emp != null)
                {
                    var notPlaced = new NotPlacedEmployeeVM();

                    notPlaced.EmployeeID = emp.EmployeeCode;
                    notPlaced.EmployementType = emp.Status.ToString().ToLower();
                    notPlaced.FullName = emp.FullNameaEnglish;

                    var branch = db.Branch.FirstOrDefault(x => x.BranchId == place.BranchId);
                    if(branch != null)
                    {
                        notPlaced.Branch = branch.Name;
                    }

                    if (emp.IsActive)
                    {
                        notPlaced.Status = "Active";
                    }
                    else
                    {
                        notPlaced.Status = "In Active";
                    }
                    employeeList.Add(notPlaced);
                }

            }
            return employeeList;
        }
    }

    [DataObject]
    public class DoublePlacedObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<NotPlacedEmployeeVM> GetDoublePlaced(string branchId)
        {
            List<NotPlacedEmployeeVM> employeeList = new List<NotPlacedEmployeeVM>();

            var placements = db.Placements.Where(x => x.isCurrent).GroupBy(x=>x.EmployeeId).ToList();

            //if (branchId != null)
            //{
            //    int.TryParse(branchId, out int braId);
            //    placements = placements.Where(x => x.BranchId == braId).ToList();
            //}

            //foreach (var place in placements)
            //{
            //    var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == place.EmployeeId && !x.IsActive);
            //    if (emp != null)
            //    {
            //        var notPlaced = new NotPlacedEmployeeVM();

            //        notPlaced.EmployeeID = emp.EmployeeCode;
            //        notPlaced.EmployementType = emp.Status.ToString().ToLower();
            //        notPlaced.FullName = emp.FullNameaEnglish;

            //        var branch = db.Branch.FirstOrDefault(x => x.BranchId == place.BranchId);
            //        if (branch != null)
            //        {
            //            notPlaced.Branch = branch.Name;
            //        }

            //        if (emp.IsActive)
            //        {
            //            notPlaced.Status = "Active";
            //        }
            //        else
            //        {
            //            notPlaced.Status = "In Active";
            //        }
            //        employeeList.Add(notPlaced);
            //    }

            //}
            return employeeList;
        }
    }
}
