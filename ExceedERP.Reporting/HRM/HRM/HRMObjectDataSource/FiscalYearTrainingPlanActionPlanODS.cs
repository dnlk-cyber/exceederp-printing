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
    public class FiscalYearTrainingPlanActionPlanODS:IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FiscalYearTrainingPlanActionPlanVM> GetTrainingActionPlan(string id, string yearId, string trainingType)
        {
            var result = new List<FiscalYearTrainingPlanActionPlanVM>();

            int.TryParse(yearId, out int fiscalYearId);
            int.TryParse(trainingType, out int trainingTypeId);
            int.TryParse(id, out int workUnitId);

            var trainingPlansQuery = db.NeedAssessmentSummary
                .Where(x => (x.IsThirdSendForApproval && x.IsApproved) || x.IsRejected);

            if (fiscalYearId > 0)
                trainingPlansQuery = trainingPlansQuery.Where(x => x.GlFiscalYearId == fiscalYearId);

            if (trainingTypeId > 0)
                trainingPlansQuery = trainingPlansQuery.Where(x => x.LookupId == trainingTypeId);

            if (workUnitId > 0)
            {
                var needIds = db.TrainingNeedRequests
                    .Where(x => x.OrgStructureId == workUnitId)
                    .Select(x => x.AssesmentId)
                    .Distinct()
                    .ToList();

                if (needIds.Any())
                    trainingPlansQuery = trainingPlansQuery.Where(x => needIds.Contains(x.NeedAssessmentSummaryId));
            }

            var trainingPlans = trainingPlansQuery.ToList();

            foreach (var plan in trainingPlans)
            {
                var vm = new FiscalYearTrainingPlanActionPlanVM();

                // Lookups: TrainingType and TargetGroup
                var lookup = db.Lookups.FirstOrDefault(l => l.LookupId == plan.LookupId && l.IsActive);
                if (lookup != null)
                {
                    if (lookup.LookupType == LookUpTypes.COURSE)
                        vm.TrainingType = lookup.Description;
                    if (lookup.LookupType == LookUpTypes.TARGET_GROUP)
                        vm.TrainingCoductBody = lookup.Description;
                }

                // Related Requests (fetched once and reused)
                var assessmentIds = db.NeedAssessmentSummaryDetails
                                        .Where(x => x.AssesmentSummaryId == plan.NeedAssessmentSummaryId)
                                        .Select(x => x.AssesmentId)
                                        .Distinct()
                                        .ToList();

                var relatedRequests = db.TrainingNeedRequests
                    .Where(x => assessmentIds.Contains(x.AssesmentId))
                    .ToList();


                var orgStructureIds = relatedRequests.Select(x => x.OrgStructureId).Distinct();
                var employeeIds = relatedRequests.Select(x => x.EmployeeId).Distinct();

                // WorkUnit names
                var workUnits = db.MainCompanyStructures
                    .Where(x => orgStructureIds.Contains(x.OrgStructureId))
                    .ToList();

                foreach (var unit in workUnits)
                {
                    vm.WorkUnit += "+ " + unit.Name + "\n";
                }

                // Job Titles
                var currentPlacements = db.Placements
                    .Where(x => employeeIds.Contains(x.EmployeeId) && x.isCurrent)
                    .Select(x => x.LookupId)
                    .Distinct()
                    .ToList();

                var jobTitles = db.Lookups
                    .Where(x => currentPlacements.Contains(x.LookupId))
                    .ToList();

                foreach (var job in jobTitles)
                {
                    vm.Jobtitle += "+ " + job.Description + "\n";
                }

                // Provider
                var provider = db.Companies.FirstOrDefault(x => x.MainCompanyId == plan.MainCompanyId);
                if (provider != null)
                    vm.Provider = provider.TradeName;

                // Year and Period
                vm.Year = plan.GlFiscalYear?.Name;
                vm.TraingPeriod = plan.GlFiscalYear?.Name;
                vm.TrainingPlace = plan.DevelopmentBy;

                // Gender Count
                var maleCount = db.Person_Employee
                    .Count(x => employeeIds.Contains(x.EmployeeId) && x.Gender == Gender.MALE);

                var femaleCount = db.Person_Employee
                    .Count(x => employeeIds.Contains(x.EmployeeId) && x.Gender == Gender.FEMALE);

                vm.MaleTrainee = maleCount;
                vm.FeMaleTrainee = femaleCount;

                // Totals
                vm.TotalTrainee = plan.NoOfStaff;
                vm.Cost = (decimal)plan.TotalCost;
                vm.FixedBudget = (decimal)plan.CostPerPerson;

                result.Add(vm);
            }

            return result;
        }
        public void Dispose()
        {
            db.Dispose();
        }

        //[DataObjectMethod(DataObjectMethodType.Select)]
        //public List<FiscalYearTrainingPlanActionPlanVM> GetTrainingActionPlan(string id, string yearId,string traingType)
        //{
        //    List<FiscalYearTrainingPlanActionPlanVM> Lists = new List<FiscalYearTrainingPlanActionPlanVM>();

        //    var courseTrakings = db.CourseTrackings.ToList();
        //    //var trainingPlans = db.TrainingPlans.ToList();
        //    var trainingPlans = db.NeedAssessmentSummary.Where(x => x.IsThirdSendForApproval & x.IsApproved || x.IsRejected).ToList(); ;

        //    if (!string.IsNullOrEmpty(yearId))
        //    {
        //        int.TryParse(yearId, out int fiscalyearld);
        //        trainingPlans = trainingPlans.Where(x => x.GlFiscalYearId == fiscalyearld).ToList();
        //    }
        //    if (!string.IsNullOrEmpty(traingType))
        //    {
        //        int.TryParse(traingType, out int traingTypeId);
        //        trainingPlans = trainingPlans.Where(x => x.LookupId == traingTypeId).ToList();
        //    }
        //    if (!string.IsNullOrEmpty(id))
        //    {
        //        int.TryParse(id, out int workUnitId);
        //        var needIds = db.TrainingNeedRequests.Where(x => x.OrgStructureId == workUnitId).Select(x => x.AssesmentId);
        //        if (needIds.Any())
        //        {
        //            trainingPlans = trainingPlans.Where(x => needIds.Contains(x.NeedAssessmentSummaryId)).ToList();

        //        }
        //    }

        //    foreach (var trainingPlan in trainingPlans)
        //    {
        //        var TrainingActionPlan = new FiscalYearTrainingPlanActionPlanVM();
        //        var courseList = db.Lookups.Where(l => l.LookupType == LookUpTypes.COURSE 
        //        && l.IsActive&& l.LookupId==trainingPlan.LookupId).FirstOrDefault();              
        //        if (courseList != null)
        //        {
        //            TrainingActionPlan.TrainingType = courseList.Description;

        //        }
        //        var targetGroup = db.Lookups.Where(l => l.LookupType == LookUpTypes.TARGET_GROUP && l.IsActive&&l.LookupId==trainingPlan.LookupId).FirstOrDefault();
        //        if (targetGroup !=null)
        //        {
        //            TrainingActionPlan.TrainingCoductBody = targetGroup.Description;
        //        }


        //        var workUnitIds = db.TrainingNeedRequests.Where(x => x.AssesmentId == trainingPlan.NeedAssessmentSummaryId).Select(x => x.OrgStructureId).Distinct();
        //        var empIds = db.TrainingNeedRequests.Where(x => x.AssesmentId == trainingPlan.NeedAssessmentSummaryId).Select(x => x.EmployeeId).Distinct();
        //        foreach (var workUnitId in workUnitIds)
        //        {
        //            var mainCompany = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == workUnitId);
        //            if (mainCompany != null)
        //            {
        //                TrainingActionPlan.WorkUnit +="+ "+ mainCompany.Name +"\n";
        //            }

        //        }
        //        var lookupIds = db.Placements.Where(x => empIds.Contains(x.EmployeeId) && x.isCurrent).Select(x => x.LookupId).Distinct();
        //        foreach (var lookupId in lookupIds)
        //        {
        //            var jobtitle = db.Lookups.FirstOrDefault(x => x.LookupId == lookupId);
        //            if (jobtitle != null)
        //            {
        //                TrainingActionPlan.Jobtitle +="+ "+ jobtitle.Description +"\n";
        //            }

        //        }
        //        var providerlist = db.Companies.FirstOrDefault(x => x.MainCompanyId == trainingPlan.MainCompanyId);
        //        if (providerlist != null)
        //        {
        //            TrainingActionPlan.Provider = providerlist.TradeName;
        //        }
        //        TrainingActionPlan.Year = trainingPlan.GlFiscalYear.Name;
        //        TrainingActionPlan.TraingPeriod = trainingPlan.GlFiscalYear.Name;
        //        TrainingActionPlan.TrainingPlace = trainingPlan.DevelopmentBy;
        //        TrainingActionPlan.MaleTrainee = db.Person_Employee.Where(x=>empIds.Contains(x.EmployeeId)&&x.Gender==Gender.MALE).Distinct().Count();
        //        TrainingActionPlan.FeMaleTrainee = db.Person_Employee.Where(x=>empIds.Contains(x.EmployeeId)&&x.Gender==Gender.FEMALE).Distinct().Count();
        //        TrainingActionPlan.TotalTrainee = trainingPlan.NoOfStaff;
        //        TrainingActionPlan.Cost = (decimal)trainingPlan.TotalCost;
        //        TrainingActionPlan.FixedBudget = (decimal)trainingPlan.CostPerPerson;



        //        Lists.Add(TrainingActionPlan);

        //    }

        //    return Lists;
        //}

    }
}
