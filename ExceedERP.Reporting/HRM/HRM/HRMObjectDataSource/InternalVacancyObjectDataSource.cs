using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class InternalVacancyObjectDataSource : List<InternalVacancy>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<InternalVacancy> GetInternalVacancy(string vacancyCod, DateTime from, DateTime to)
        {
            List<InternalVacancy> internalVacancies = new List<InternalVacancy>();
            //DateTime dateOne = new DateTime(from.Year, from.Month, from.Day);
           // DateTime dateTow = new DateTime(to.Year, to.Month, to.Day);
           

            var structure = db.MainCompanyStructures;
            var lookups = db.Lookups;
           
            if (vacancyCod != null)
            {
                var recva = db.RecruitmentVacancy.Where(x => x.MediaVacancyCode == vacancyCod).ToList();

                foreach (var vacancydetail in recva)
                {
                    var recruitVacancy = db.VaccancyPlans.Find(vacancydetail.VacancyId);
                    //var recruitmentPlan = db.RecruitmentPlans.Find(recruitVacancy.RecruitmentPlanId);
                    
                    if (recruitVacancy != null)
                    {
                        var recruitPlan = db.RecruitmentPlans.Find(recruitVacancy.RecruitmentPlanId);
                        var recruitmentRequest = db.RecruitmentRequest.Where(x => x.LookupId == recruitPlan.LookupId && x.OrgBranchName == recruitVacancy.OrgBranchName).FirstOrDefault();
                        int sta;
                        //if (recruitmentRequest != null)
                        //{
                        //    sta = recruitmentRequest.NoOfVaccantPostion;

                        //}
                        string planedSalary = "";
                        string job = "";
                       var plan =  db.RecruitmentPlans.FirstOrDefault(x => x.RecruitmentPlanId == recruitVacancy.RecruitmentPlanId);
                        if(plan != null)
                        {
                            planedSalary = plan.PlannedSalary;
                            job = plan.JobtitleName;
                        }

                        var internalVacancy = new InternalVacancy();

                        internalVacancy.VacancyCode = recruitVacancy.VaccancyCode;
                        internalVacancy.JobTitle = job;//recruitVacancy.RecruitmentPlan.JobtitleName,
                                                       // Quntity = sta,
                        internalVacancy.Salary = planedSalary;//recruitVacancy.RecruitmentPlan.PlannedSalary,

                        internalVacancy.RecruitmentType = HrHelper.GetVacancyType(recruitVacancy.VacancyType);                          
                       
                      
                        var branch = db.Branch.FirstOrDefault(x => x.BranchId == recruitVacancy.BranchId);
                        if (branch != null)
                        {
                            internalVacancy.Location = branch.AmharicName;
                        }
                        //if(internalVacancy.RecruitmentType==Core.Domain.Common.VacancyType.PERMANENT)
                        //{
                        //    internalVacancy.RecruitmentType = "ቋሚ";
                        //}
                        //else if (internalVacancy.RecruitmentType == Core.Domain.Common.VacancyType.CONTRACT)
                        //{
                        //    internalVacancy.RecruitmentType = "ጊዜያዊ";
                        //}


                        var vacancyLookUpId = db.VaccancyPlans.Find(vacancydetail.VacancyId);
                        if(vacancyLookUpId!=null)
                        {
                            var planLookUpId = db.RecruitmentPlans.FirstOrDefault(x=>x.RecruitmentPlanId==vacancyLookUpId.RecruitmentPlanId);
                            if (planLookUpId!=null)
                            {
                                var mapping = db.JobTitleMapping.FirstOrDefault(x => x.LookupId == planLookUpId.LookupId);
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
                                                // string x = firstOrDefau.Description;

                                                // internalVacancy.EduandExp =  firstOrDefau.Description;
                                                internalVacancy.EduandExp = d.RelevantExp;
                                                internalVacancy.EduandExpRelated = d.Experience;

                                            }
                                        }
                                    }

                                    // internalVacancy.EduandExp = " ," + recruitVacancy.RecruitmentPlan.RelevantExp;

                                }
                                else
                                {
                                    var recruitmentPlan = db.RecruitmentPlans.Find(recruitVacancy.RecruitmentPlanId);
                                    if (recruitmentPlan != null)
                                    {
                                        internalVacancy.EduandExp = recruitmentPlan.RelevantExp;
                                        internalVacancy.EduandExpRelated = recruitmentPlan.Experience;
                                    }

                                }
                            }
                        }


                        var recPlan = db.RecruitmentPlans.FirstOrDefault(x=>x.RecruitmentPlanId == recruitVacancy.RecruitmentPlanId);
                        if(recPlan != null)
                        {
                            var mainCompanyStructure = structure.FirstOrDefault(x => x.OrgStructureId == recPlan.OrgStructureId);
                            if (mainCompanyStructure != null)
                            {
                                //internalVacancy.Location = vacancydetail.Vacancy.Branch.Name;
                                //internalVacancy.RecruitmentType = vacancydetail.Vacancy.VacancyType;

                            }

                            //var recruitetRequest = db.RecruitmentRequest.FirstOrDefault(x => x.LookupId == recruitPlan.LookupId);
                            var recruitetRequest = db.RecruitmentRequest.Where(x => x.LookupId == recruitPlan.LookupId && x.OrgStructureId.ToString() == recruitVacancy.OrgBranchName).FirstOrDefault();
                            if (recruitetRequest != null)
                            {
                                internalVacancy.Quntity = recruitmentRequest.NoOfVaccantPostion;
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