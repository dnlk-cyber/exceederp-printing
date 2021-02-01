using System.Collections.Generic;
using System.ComponentModel;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class WeekObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetWeekList()
        {
            List<string> sourceList = new List<string>
            {
                "1",
                "2",
                "3",
                "4"



            };

            return sourceList;
        }
    }
}