using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
   public class EvaluationCriteriaODS:IDisposable
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PerformanceCriteriaVM> GetCriterias()
        {
            List<PerformanceCriteriaVM> criteriaList = new List<PerformanceCriteriaVM>();
            var criterias = db.MeasurementDefinitions.ToList();

            foreach (var cri in criterias)
            {
                var criteria = new PerformanceCriteriaVM();

                string type = "";
                var look = db.Lookups.FirstOrDefault(x => x.LookupId == cri.LookupId);
                if(look != null)
                {
                    type = look.Description;
                }

                criteria.EvaluationCriteriaId = cri.MeasurementDefinitionId;
                criteria.Name = cri.EvaluationCriteria + " - " + type + " - " + cri.Role;

                criteriaList.Add(criteria);
            }

            return criteriaList;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }

    public class PerformanceCriteriaVM
    {
        public int EvaluationCriteriaId { get; set; }
        public string Name { get; set; }        
    }
}
