using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class AttendanceTypeODS
    {

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetAttendanceType()
        {
            List<string> status = new List<string>();

            status.Add("Absent");
            status.Add("Late");
            status.Add("Leave");
            status.Add("OT");

            return status;
        }
    }
}
