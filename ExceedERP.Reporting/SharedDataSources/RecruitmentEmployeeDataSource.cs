using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{

    [DataObject]
    public class RecruitmentEmployeeDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Person_Employee> GetEmployees()
        {
            EmployeeIdView view = new EmployeeIdView();
            List<EmployeeIdView> viewList = new List<EmployeeIdView>();
            var employees = from e in db.Person_Employee
                join p in db.Placements
                    on e.EmployeeId equals p.EmployeeId
                where p.isCurrent && p.AssignmentType == AssignmentType.NEW
                select (new
                {
                    e
                });

            var list = employees.Select(x => x.e).ToList();

            return list;

        }
    }
}
