using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{

    [DataObject]
    public class VacancyForMediaObjectDataSource : List<InternalVacancy>
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
                        var vacancy = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == rec.VacancyId && x.RecruSource==RecruitmentSource.EXTERNAL && x.IsActive);
                        if (vacancy != null)
                        {
                            internalVacancy.Location = vacancy.Branch.AmharicName;
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
                                internalVacancy.VacancyCode = recPlan.VacancyCode;
                                internalVacancy.JobTitle = recPlan.JobtitleName;
                                internalVacancy.Salary = recPlan.PlannedSalary;

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

                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    internalVacancy.EduandExp = recPlan.RelevantExp;
                                    internalVacancy.EduandExpRelated = recPlan.Experience;
                                }

                                var recRequest = db.RecruitmentRequest.FirstOrDefault(x => x.LookupId == recPlan.LookupId && x.OrgStructureId == recPlan.OrgStructureId);
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
