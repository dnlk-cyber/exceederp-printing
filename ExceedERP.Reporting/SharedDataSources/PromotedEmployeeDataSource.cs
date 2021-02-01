using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
   
    [DataObject]
    public class internalVacancyDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Person_Employee> GetEmployees()
        {
            EmployeeIdView view = new EmployeeIdView();
            List<EmployeeIdView> viewList = new List<EmployeeIdView>();
            var employees = from e in db.Person_Employee
                join p in db.Placements
                    on e.EmployeeId equals p.EmployeeId
                where p.isCurrent && p.AssignmentType == AssignmentType.PROMOTION
                select (new
                {
                    e
                });
            var list = employees.Select(x=> x.e).ToList();
            //foreach (var e in employees)
            //{
            //    view = new EmployeeIdView();
            //    view.EmployeeCode = e.e.EmployeeCode;
            //    view.FullName = HrHelper.GetEmployeeFullName(e.e.EmployeeId);
            //    view.g = e.e.EmployeeCode;
            //    view.EmployeeCode = e.e.EmployeeCode;
            //}
            return list;
        }

    }
}
