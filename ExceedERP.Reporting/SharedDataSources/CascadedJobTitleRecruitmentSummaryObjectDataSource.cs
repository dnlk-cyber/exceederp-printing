using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class CascadedJobTitleRecruitmentSummaryObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Lookup> GetJobTitleCascaded(string vacancyCod, string vacancyId)
        {
            List<Lookup> list = new List<Lookup> { };
            if (vacancyCod != null && vacancyId == null)
            {

                var recruitmentVacancy = db.RecruitmentVacancy.Where(x => x.MediaVacancyCode == vacancyCod).ToList();
                if (recruitmentVacancy.Any())
                {
                    foreach (var recruitment in recruitmentVacancy)
                    {
                        var vacancy = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == recruitment.VacancyId);

                        if (vacancy != null)
                        {
                            var recruitmentPlan = db.RecruitmentPlans.Find(vacancy.RecruitmentPlanId);
                            if (recruitmentPlan != null)
                            {

                                var lookups = db.Lookups.FirstOrDefault(x => x.LookupId == recruitmentPlan.LookupId && x.LookupType == LookUpTypes.JOB_TITLE);

                                if (lookups != null)
                                {
                                    list.Add(lookups);

                                }

                            }

                        }
                    }


                }

            }
            else if (vacancyCod != null && vacancyId != null)
            {

                var recruitmentVacancy = db.RecruitmentVacancy.FirstOrDefault(x => x.MediaVacancyCode == vacancyCod && x.VacancyId.ToString() == vacancyId);
                if (recruitmentVacancy != null)
                {

                    var vacancy = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == recruitmentVacancy.VacancyId && x.RecruSource == RecruitmentSource.EXTERNAL);

                    if (vacancy != null)
                    {
                        var recruitmentPlan = db.RecruitmentPlans.Find(vacancy.RecruitmentPlanId);
                        if (recruitmentPlan != null)
                        {

                            var lookups = db.Lookups.FirstOrDefault(x => x.LookupId == recruitmentPlan.LookupId && x.LookupType == LookUpTypes.JOB_TITLE);

                            if (lookups != null)
                            {
                                list.Add(lookups);

                            }

                        }

                    }


                }

            }
            return list;
        }
    }
}