using System.Collections.Generic;
using System.ComponentModel;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeStatusObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetEmpStatus()
        {
            List<string> sourceList = new List<string>
            {
                "PERMANENT",
                "CONTRACT",
                "PROBATION",
                "FREELANCER",
                "NO_STATUS"
                
            };

            return sourceList;
        }
    }
}