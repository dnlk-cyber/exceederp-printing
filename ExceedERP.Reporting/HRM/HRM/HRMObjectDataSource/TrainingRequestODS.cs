using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{

    [DataObject]
    public class TrainingRequestODS : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TrainingReqeustVM> GetTrainingRequest(string orgStructureId)
        {
            List<TrainingReqeustVM> list = new List<TrainingReqeustVM> { };

            var trainingRequests = db.TrainingApplicants.ToList();

            if(orgStructureId !=null)
            {
                int.TryParse(orgStructureId, out int orgId);
                var orgIds = db.MainCompanyStructures.Where(x=>x.OrgStructureId == orgId || x.OrgStructureId == orgId).Select(x=>x.OrgStructureId).ToList();
                trainingRequests = db.TrainingApplicants.Where(x=>orgIds.Contains(x.OrgStructureId)).ToList();
            }

            foreach (var train in trainingRequests)
            {
                var training = new TrainingReqeustVM();

                var workunit = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == train.OrgStructureId);
                if (workunit != null)
                {
                    training.Department = workunit.Name;
                }

                training.NoOfTrainee = train.NoofTrainee;
                training.Date = train.DateAmharic;

                var lookup = db.Lookups.FirstOrDefault(x=>x.LookupId == train.LookupId && x.LookupType == Core.Domain.Common.LookUpTypes.COURSE);
                if(lookup != null)
                {
                    training.CourseName = lookup.Description;
                }

                list.Add(training);
            }
            return list;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}