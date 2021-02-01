using System.Collections.Generic;
using System.ComponentModel;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class TrainingStatusObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetTrainingStatus()
        {
            List<string> sourceList = new List<string>
            {

                "COMPLETED",
                "NOT_COMPLETED"


            };

            return sourceList;
        }
    }
}