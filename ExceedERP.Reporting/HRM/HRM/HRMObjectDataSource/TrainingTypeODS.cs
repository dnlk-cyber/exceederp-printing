using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class TrainingTypeODS:IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TrainingType> GetTrainingType()
        {
            return db.Lookups
                     .Where(l => l.LookupType == LookUpTypes.COURSE && l.IsActive)
                     .Select(e => new TrainingType
                     {
                         Id = e.LookupId, // Unique identifier
                         Type = e.Description ?? e.DescriptionEng  // Fallback to English or remark if needed

                     })
                     .ToList();
        }
        public class TrainingType
        {
            public int Id { get; set; }
            public string Type { get; set; }

        }
        public void Dispose()
        {
            db.Dispose();
        }
    }

   
}
