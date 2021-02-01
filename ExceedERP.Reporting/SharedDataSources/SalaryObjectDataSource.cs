using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.HRM.SalaryStructure;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class SalaryObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Salary> GetSalary()
        {
            List<Salary> list = new List<Salary> { };


            var salaries = db.Salaries.ToList();
            if (!salaries.Any())
            {
                return list;
            }

            return salaries;//.Select(x => x.Description).ToList();
        }
    }
}