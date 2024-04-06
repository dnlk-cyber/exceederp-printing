using System.Collections.Generic;
using System.ComponentModel;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class LeaveStatusObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetLeaveStatus()
        {
            List<string>listItem = new List<string>
            {
                "WAITING_FOR_APPROVAL",
                "DEPARTMENT_APPROVAL",
                "APPROVE",
                "PEND",
                "REJECT"


            };

            return listItem;
        }
    }
}