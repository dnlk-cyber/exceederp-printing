using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class NewHeaderObjectDataSource : List<ExternalApplicantDetail>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<InternalApplicantDetail> GetHeaders(string jobTitle, string vacancyCod, DateTime from, DateTime to)
        {
            List<InternalApplicantDetail> internalVacancies = new List<InternalApplicantDetail>();
            var lookups = db.Lookups;
            DateTime dateOne = new DateTime(from.Year, from.Month, from.Day);
            DateTime dateTow = new DateTime(to.Year, to.Month, to.Day);
            var name = lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE && x.LookupId.ToString() == jobTitle).FirstOrDefault();
            if (name != null)
            {
                var internalVacancy = new InternalApplicantDetail();
                int jobtitleCode = name.LookupId;
                var recruits = db.RecruitmentPlans.Where(x => x.LookupId == jobtitleCode).ToList();
                var rankList = db.Ranks;

                var structure = db.MainCompanyStructures;
                var vacancy = db.VaccancyPlans.Where(x => x.VacancyId.ToString() == vacancyCod).FirstOrDefault();
                if (vacancy != null)
                {
                    var recr = db.RecruitmentPlans.Where(x => x.RecruitmentPlanId == vacancy.RecruitmentPlanId).FirstOrDefault();
                    if (recr != null)
                    {
                        var applicantRegistration = db.ApplicantRegistration.Where(x => x.VacancyId == vacancy.VacancyId).FirstOrDefault();
                        if (applicantRegistration != null)
                        {
                            internalVacancy.JobTitle = applicantRegistration.JobtitleName;
                            internalVacancy.Salary = recr.PlannedSalary;
                            internalVacancy.Quntity = recr.NoOfVacantPost;

                            internalVacancies.Add(internalVacancy);
                        }
                    }
                }
            }
            return internalVacancies;
        }
    }
    }