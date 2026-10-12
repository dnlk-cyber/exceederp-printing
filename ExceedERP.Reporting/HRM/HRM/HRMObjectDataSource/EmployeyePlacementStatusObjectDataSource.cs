using System.Collections.Generic;
using System.ComponentModel;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeyePlacementStatusObjectDataSource
    {
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetPlacementStatus()
        {
            List<string> sourceList = new List<string>
            {
                "Pend",
                "Unpend"
            };

            return sourceList;
        }
    }
}