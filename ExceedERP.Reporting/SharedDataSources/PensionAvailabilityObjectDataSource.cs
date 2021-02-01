using System.Collections.Generic;
using System.ComponentModel;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class PensionAvailabilityObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetPensionAvailability()
        {
            List<string> sourceList = new List<string>
            {
                "ASSIGNED",
                "NOT_ASSIGNED"




            };

            return sourceList;
        }
    }
}