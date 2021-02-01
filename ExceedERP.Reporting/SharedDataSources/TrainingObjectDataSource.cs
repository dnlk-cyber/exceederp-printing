using System.Collections.Generic;
using System.ComponentModel;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class TrainingObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetTraining()
        {

            List<string> trainingType = new List<string>
            {
                "TRAINING",
                "EDUCATION"
            };

            return trainingType;
        }
    }
}