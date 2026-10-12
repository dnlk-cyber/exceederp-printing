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
    public class InternalApplicantHeaderObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<InternalApplicantDetail> GetInternalApplicantHeader(string jobTitle, string vacancyCod, DateTime from, DateTime to)
        {
            List<InternalApplicantDetail> internalVacancies = new List<InternalApplicantDetail>();
            var lookups = db.Lookups;
            DateTime dateOne = new DateTime(from.Year, from.Month, from.Day);
            DateTime dateTow = new DateTime(to.Year, to.Month, to.Day);
            var name = lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE && x.LookupId.ToString() == jobTitle).FirstOrDefault();
            if (name != null)
            {
                int jobtitleCode = name.LookupId;
                var recruits = db.RecruitmentPlans.Where(x => x.LookupId == jobtitleCode).ToList();
                var rankList = db.Ranks;

                var structure = db.MainCompanyStructures;

                


                foreach (var r in recruits)
                {
                    var vacan = new RecruitmentVacancy();
                    if (vacancyCod == null)
                    {
                        var vacancydetail = db.VaccancyPlans.FirstOrDefault(x => x.RecruitmentPlanId == r.RecruitmentPlanId);
                        if (vacancydetail != null)
                        {
                            DateTime dateOneC = new DateTime(vacancydetail.From.Year, vacancydetail.From.Month, vacancydetail.From.Day);
                            DateTime dateTowC = new DateTime(vacancydetail.To.Year, vacancydetail.To.Month, vacancydetail.To.Day);
                            if (dateOneC >= dateOne && dateTow <= dateTowC)
                            {
                                vacan = db.RecruitmentVacancy.FirstOrDefault(x => x.VacancyId == vacancydetail.VacancyId);
                                break;
                            }
                        }

                    }
                    else
                    {
                        vacan = db.RecruitmentVacancy.FirstOrDefault(x => x.VacancyId.ToString() == vacancyCod);
                    }

                    if (vacan != null)
                    {
                        var vacancydetail = db.VaccancyPlans.Where(x => x.VacancyId == vacan.VacancyId).FirstOrDefault();
                        if (vacancydetail != null)
                        {
                            var internalVacancy = new InternalApplicantDetail();
                            var orDefaults = lookups.Where(x => x.LookupId == r.LookupId).FirstOrDefault();
                            if (orDefaults != null)
                            {
                                internalVacancy.JobTitle = orDefaults.Description;
                            }
                            var orDefaultses = structure.Where(x => x.OrgStructureId == r.OrgStructureId).FirstOrDefault();
                            if (orDefaultses != null)
                            {
                                var branch = db.Branch.FirstOrDefault(x => x.BranchId == vacancydetail.BranchId);
                                if (branch != null)
                                {
                                    internalVacancy.Location = branch.Name;
                                }
                            }
                            var mapping = db.JobTitleMapping.Where(x => x.LookupId == r.LookupId && x.OrgStructureId == r.OrgStructureId).FirstOrDefault();
                            if (mapping != null)
                            {
                                var detail = db.JobTitleMappingDetail.Where(x => x.JobTitleMappingId == mapping.JobTitleMappingId);
                                if (detail.Any())
                                {
                                    foreach (var d in detail)
                                    {
                                        var firstOrDefau = lookups.Where(x => x.LookupId == d.LookupId).FirstOrDefault();
                                        if (firstOrDefau != null)
                                        {


                                            internalVacancy.EduandExp = " ," + firstOrDefau.Description;
                                        }
                                    }
                                }

                                internalVacancy.EduandExp = " ," + r.RelevantExp + ", " + r.SpecialSkill;

                            }
                            else
                            {


                                internalVacancy.EduandExp = r.RelevantExp + ", " + r.SpecialSkill;
                            }
                            internalVacancy.Salary = r.PlannedSalary;
                            var firstOrDefault = rankList.Where(x => x.RankId == r.RankId).FirstOrDefault();
                            if (firstOrDefault != null)
                                internalVacancy.Rank = firstOrDefault.RankName;
                            internalVacancy.Quntity = r.NoOfVacantPost;
                           // internalVacancy.RecruitmentType = r.RecruitmentType;
                            if (r.RecruitmentType == VacancyType.PERMANENT)
                            {
                              //  internalVacancy.RecruitmentType = "ቋሚ";
                                internalVacancy.RecruitmentType = VacancyType.PERMANENT.ToString().ToLower();

                            }
                            else
                            {
                               // internalVacancy.RecruitmentType = "ጊዜያዊ";
                                internalVacancy.RecruitmentType = VacancyType.CONTRACT.ToString().ToLower();

                            }
                            internalVacancies.Add(internalVacancy);
                        }
                    }
                }
            }
            return internalVacancies;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}