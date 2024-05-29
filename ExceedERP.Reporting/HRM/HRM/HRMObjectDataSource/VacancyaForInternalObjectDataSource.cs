using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{

    [DataObject]
    public class VacancyaForInternalObjectDataSource : List<InternalVacancy>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<InternalVacancy> GetInternalVacancy(string vacancyCod, DateTime from, DateTime to)
        {
            List<InternalVacancy> internalVacancies = new List<InternalVacancy>();


            var structure = db.MainCompanyStructures;
            var lookups = db.Lookups;

            if (vacancyCod != null)
            {
                var recVacancy = db.RecruitmentVacancy.Where(x => x.MediaVacancyCode == vacancyCod).ToList();
                if (recVacancy.Any())
                {
                    foreach (var rec in recVacancy)
                    {
                        var internalVacancy = new InternalVacancy();
                        var vacancy = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == rec.VacancyId && x.RecruSource == RecruitmentSource.INTERNAL && x.IsActive);
                        if (vacancy != null)
                        {
                            var branch = db.Branch.FirstOrDefault(x => x.BranchId == vacancy.BranchId);
                            if (branch != null)
                            {
                                internalVacancy.Location = branch.AmharicName;
                            }
                            internalVacancy.VacancyCode = vacancy.VaccancyCode;
                            if (vacancy.VacancyType == VacancyType.PERMANENT)
                            {
                                internalVacancy.RecruitmentType = "ቋሚ";
                            }
                            else if (vacancy.VacancyType == VacancyType.CONTRACT)
                            {
                                internalVacancy.RecruitmentType = "ጊዜያዊ";
                            }

                            var recPlan = db.RecruitmentPlans.FirstOrDefault(x => x.RecruitmentPlanId == vacancy.RecruitmentPlanId);
                            if (recPlan != null)
                            {
                                //internalVacancy.VacancyCode = recPlan.VacancyCode;
                                internalVacancy.JobTitle = recPlan.JobtitleName;


                                var mapping = db.JobTitleMapping.FirstOrDefault(x => x.BranchId == recPlan.BranchId && x.OrgStructureId == recPlan.OrgStructureId && x.LookupId == recPlan.LookupId);
                                if (mapping != null)
                                {
                                    var detail = db.JobTitleMappingDetail.Where(x => x.JobTitleMappingId == mapping.JobTitleMappingId);
                                    if (detail.Any())
                                    {
                                        foreach (var d in detail)
                                        {
                                            var firstOrDefau = lookups.FirstOrDefault(x => x.LookupId == d.LookupId);
                                            if (firstOrDefau != null)
                                            {

                                                internalVacancy.EduandExp = d.RelevantExp;
                                                internalVacancy.EduandExpRelated = d.Experience;
                                                internalVacancy.Salary = d.PlannedSalary;
                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    internalVacancy.EduandExp = recPlan.RelevantExp;
                                    internalVacancy.EduandExpRelated = recPlan.Experience;
                                    internalVacancy.Salary = recPlan.PlannedSalary;
                                }

                                var recRequest = db.RecruitmentRequest.FirstOrDefault(x => x.LookupId == recPlan.LookupId && x.OrgStructureId == recPlan.OrgStructureId && x.BranchId == recPlan.BranchId && x.GlFiscalYearId == recPlan.GlFiscalYearId);
                                if (recRequest != null)
                                {
                                    internalVacancy.Quntity = recRequest.NoOfVaccantPostion;

                                }
                            }
                            internalVacancies.Add(internalVacancy);
                        }


                    }
                }

            }

            return internalVacancies;


        }
    }
}
