using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Controllers.Cmd
{
    public partial class CmdController : Controller
    {

        public void UpdateEmployeeBranch()
        {
            var placmenets = db.Placements.Where(x => x.isCurrent).ToList();
            var employees = db.Person_Employee.ToList();

            foreach (var placement in placmenets)
            {
                var employee = employees.Find(p => p.EmployeeId == placement.EmployeeId);

                if (employee != null)
                {

                    employee.Status = placement.EmployementType;
                    employee.OrgBranchName = placement.BranchId.ToString();
                    employee.ModifiedBy = User.Identity.Name;
                    employee.LastModified = DateTime.Now;
                    db.Person_Employee.Attach(employee);
                    db.Entry(employee).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }
    }
}
    
