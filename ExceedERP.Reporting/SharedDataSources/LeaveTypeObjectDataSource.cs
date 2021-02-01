using System.Collections.Generic;
using System.ComponentModel;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class LeaveTypeObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetLeaveType()
        {
            List<string> sourceList = new List<string>
            {
                "ANNUAL",
                "OTHER",
                "MARRIAGE",
                "MATERNITY",
                "PATERNITY",
                "SICK_LEAVE",
                "Leave_With_Out_Pay",
                "Bereavement"
            };

            return sourceList;
        }
    }
}