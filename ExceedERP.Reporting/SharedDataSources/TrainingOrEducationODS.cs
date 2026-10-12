using System.Collections.Generic;
using System.ComponentModel;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class TrainingOrEducationODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetTrainingType()
        {
            List<string> list = new List<string>();

            list.Add("TRAINING");
            list.Add("EDUCATION");

            return list;
        }
    }
}
