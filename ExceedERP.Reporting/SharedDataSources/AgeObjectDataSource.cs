using System.Collections.Generic;
using System.ComponentModel;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class AgeObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetAge()
        {
            List<string> sourceList = new List<string>
            {
                "18-29",
                "30-40",
                "41-50",
                "51-60",
                "More than 60"



            };

            return sourceList;
        }
    }
}