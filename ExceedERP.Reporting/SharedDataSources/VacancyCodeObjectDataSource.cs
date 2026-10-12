using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Reporting.SharedDataSources
{
    
    [DataObject]
    public class VacancyCodeObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<RecruitmentVacancy> GetMediaVacancyCode()
        {
            return db.RecruitmentVacancy.DistinctByField(x => x.MediaVacancyCode).ToList();
            
        }
    }
}
