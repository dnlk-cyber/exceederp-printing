using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class TrainingNeedRequestODS : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TrainingNeedRequestVM> GetTrainingActionPlan(string id, string yearId, string businessUnit)
        {
            var lookups = db.Lookups.Where(l => l.IsActive).ToList();
            var companies = db.Companies.ToList();
            var mainCompanies = db.MainCompanyStructures.ToList();
            var employees = db.Person_Employee.ToList();
            var trainingNeeds = db.TrainingNeedRequests.ToList();
            var fiscalYears = db.GLFiscalYears.ToList();
            var needAssessments = db.NeedAssessments.AsQueryable();
            if (int.TryParse(yearId, out int fiscalYearId))
                needAssessments = needAssessments.Where(x => x.GlFiscalYearId == fiscalYearId);
            if (int.TryParse(id, out int workUnitId))
                needAssessments = needAssessments.Where(x => x.OrgStructureId == workUnitId);
            //if (int.TryParse(businessUnit, out int businessUnitId))
                //needAssessments = needAssessments.Where(x => x.b == businessUnitId);

            var filteredAssessments = needAssessments.ToList();
            var result = new List<TrainingNeedRequestVM>();
            foreach (var assessment in filteredAssessments)
            {
                var plan = new TrainingNeedRequestVM
                {
                    TrainingType = lookups.FirstOrDefault(l => l.LookupType == LookUpTypes.COURSE && l.LookupId == assessment.LookupId)?.Description,
                    TrainingCoductBody = lookups.FirstOrDefault(l => l.LookupType == LookUpTypes.TARGET_GROUP && l.LookupId == assessment.RequiredTraining)?.Description,
                    WorkUnit = mainCompanies.FirstOrDefault(m => m.OrgStructureId == assessment.OrgStructureId)?.Name,
                    Provider = companies.FirstOrDefault(c => c.MainCompanyId == assessment.MainCompanyId)?.TradeName,
                    Year = fiscalYears.FirstOrDefault(f => f.GlFiscalYearId == assessment.GlFiscalYearId)?.Name,
                    TraingPeriod = fiscalYears.FirstOrDefault(f => f.GlFiscalYearId == assessment.GlFiscalYearId)?.Name,
                    TotalTrainee = assessment.NoOfStaff,
                    TrainingNeedRequests = trainingNeeds
                        .Where(t => t.AssesmentId == assessment.AssesmentId)
                        .Select(t => {
                            var emp = employees.FirstOrDefault(e => e.EmployeeId == t.EmployeeId);
                            var placement = db.Placements.Where(x => x.EmployeeId == t.EmployeeId).OrderByDescending(x => x.PlacementId).FirstOrDefault();
                            return new TrainingNeedRequestDetailVM
                            {
                                Employee = emp != null ? $"{emp.EmployeeCode}-{emp.FirstNameEnglish} {emp.MiddleNameEnglish} {emp.LastNameEnglish}" : "",
                                EmployeePerformanceGap = t.EmployeePerformanceGap,
                                WorkUnit = placement !=null ? mainCompanies.FirstOrDefault(x=>x.OrgStructureId==placement.OrgStructureId)?.Name:null,
                                JobTitle = lookups != null ? lookups.FirstOrDefault(x=>x.LookupId==placement.LookupId)?.Description:null,
                                Prerequest = t.Prerequest,
                                TrainingNeedForEmployeePerformanceGap = t.TrainingNeedForEmployeePerformanceGap,
                                TheNeedForTrainingRelatedToInistitution = t.TheNeedForTrainingRelatedToInistitution,
                                ExpectedResultFromTraining = t.ExpectedResultFromTraining
                            };
                        }).ToList()
                };

                result.Add(plan);
            }

            return result;
        }
      
        public void Dispose()
        {
            db.Dispose();
        }
    

        //public List<TrainingNeedRequestVM> GetTrainingActionPlan(string id, string yearId,string businessUnit)
        //{
        //    List<TrainingNeedRequestVM> Lists = new List<TrainingNeedRequestVM>();

        //    var courseTrakings = db.CourseTrackings.ToList();
        //    var needAssessments = db.NeedAssessments.ToList();

        //    if (!string.IsNullOrEmpty(yearId))
        //    {
        //        int.TryParse(yearId, out int fiscalyearld);
        //        needAssessments = needAssessments.Where(x => x.GlFiscalYearId == fiscalyearld).ToList();
        //    }
        //    if (!string.IsNullOrEmpty(id ))
        //    {
        //        int.TryParse(id, out int workUnitId);

        //        needAssessments = needAssessments.Where(x => x.OrgStructureId == workUnitId).ToList();
        //    }
        //    if (!string.IsNullOrEmpty(businessUnit ))
        //    {
        //        int.TryParse(businessUnit, out int businessUnitId);

        //        needAssessments = needAssessments.Where(x => x.BusinessUnitId == businessUnitId).ToList();
        //    }

        //    foreach (var trainingPlan in needAssessments)
        //    {
        //        var TrainingActionPlan = new TrainingNeedRequestVM();
        //        var courseList = db.Lookups.Where(l => l.LookupType == LookUpTypes.COURSE && l.IsActive&& l.LookupId==trainingPlan.LookupId).FirstOrDefault();              
        //        if (courseList != null)
        //        {
        //            TrainingActionPlan.TrainingType = courseList.Description;

        //        }
        //        var targetGroup = db.Lookups.Where(l => l.LookupType == LookUpTypes.TARGET_GROUP && l.IsActive&&l.LookupId==trainingPlan.RequiredTraining).FirstOrDefault();
        //        if (targetGroup !=null)
        //        {
        //            TrainingActionPlan.TrainingCoductBody = targetGroup.Description;
        //        }
        //        var mainCompany = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == trainingPlan.OrgStructureId);
        //        if (mainCompany != null)
        //        {
        //          TrainingActionPlan.WorkUnit = mainCompany.Name;
        //        }

        //        var providerlist = db.Companies.FirstOrDefault(x => x.MainCompanyId == trainingPlan.MainCompanyId);
        //        if (providerlist != null)
        //        {
        //            TrainingActionPlan.Provider = providerlist.TradeName;
        //        }
        //        TrainingActionPlan.Year = trainingPlan.GlFiscalYear.Name;
        //        TrainingActionPlan.TraingPeriod = trainingPlan.GlFiscalYear.Name;
        //        TrainingActionPlan.TotalTrainee = trainingPlan.NoOfStaff;
        //        TrainingActionPlan.TrainingNeedRequests = new List<TrainingNeedRequestDetailVM>();
        //        var trainingNeedRequests = db.TrainingNeedRequests.Where(x => x.AssesmentId == trainingPlan.AssesmentId).ToList();
        //        foreach(var trainingNeedRequest in trainingNeedRequests)
        //        {
        //            var needRequest = new TrainingNeedRequestDetailVM();
        //            var employee = db.Person_Employee.Find(trainingNeedRequest.EmployeeId);
        //            if(employee != null)
        //            {
        //                needRequest.Employee = employee.EmployeeCode + "-" + employee.FirstNameEnglish +" "+ employee.MiddleNameEnglish + " " + employee.LastNameEnglish;
        //            }
        //            needRequest.EmployeePerformanceGap = trainingNeedRequest.EmployeePerformanceGap;
        //            needRequest.Prerequest = trainingNeedRequest.Prerequest;
        //            needRequest.TrainingNeedForEmployeePerformanceGap = trainingNeedRequest.TrainingNeedForEmployeePerformanceGap;
        //            needRequest.TheNeedForTrainingRelatedToInistitution = trainingNeedRequest.TheNeedForTrainingRelatedToInistitution;
        //            needRequest.ExpectedResultFromTraining = trainingNeedRequest.ExpectedResultFromTraining;
        //            TrainingActionPlan.TrainingNeedRequests.Add(needRequest);
        //        }



        //        Lists.Add(TrainingActionPlan);

        //    }

        //    return Lists;
        //}
    }
}
