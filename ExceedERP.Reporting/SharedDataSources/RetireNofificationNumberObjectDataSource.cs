using System.Collections.Generic;
using System.ComponentModel;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class RetireNofificationNumberObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetRetireNofification()
        {
            List<string> sourceList = new List<string>
            {
                "1",
                "2",
                "3",
                "4",
                "5",
                "10"



            };

            return sourceList;
        }
    }
}