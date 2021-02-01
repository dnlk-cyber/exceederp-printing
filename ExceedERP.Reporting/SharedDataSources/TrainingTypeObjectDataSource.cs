using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class TrainingTypeObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Lookup> GetTrainingType()
        {
            List<Lookup> list = new List<Lookup> { };


            var trainigType = db.Lookups.Where(x => x.LookupType == LookUpTypes.TRAINING_TYPE && x.IsActive).ToList();
            if (!trainigType.Any())
            {
                return list;
            }

            return trainigType;//.Select(x => x.Description).ToList();
        }
    }
}