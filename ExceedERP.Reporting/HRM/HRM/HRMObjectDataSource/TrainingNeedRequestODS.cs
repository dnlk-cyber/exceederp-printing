using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class TrainingNeedRequestODS
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TrainingNeedRequestVM> GetTrainingActionPlan(string id, string yearId)
        {
            List<TrainingNeedRequestVM> Lists = new List<TrainingNeedRequestVM>();

            var courseTrakings = db.CourseTrackings.ToList();
            var needAssessments = db.NeedAssessments.ToList();

            if (yearId != null)
            {
                int.TryParse(yearId, out int fiscalyearld);
                needAssessments = needAssessments.Where(x => x.GlFiscalYearId == fiscalyearld).ToList();
            }
            if (id != null)
            {
                int.TryParse(id, out int workUnitId);

                needAssessments = needAssessments.Where(x => x.OrgStructureId == workUnitId).ToList();
            }

            foreach (var trainingPlan in needAssessments)
            {
                var TrainingActionPlan = new TrainingNeedRequestVM();
                var courseList = db.Lookups.Where(l => l.LookupType == LookUpTypes.COURSE && l.IsActive&& l.LookupId==trainingPlan.LookupId).FirstOrDefault();              
                if (courseList != null)
                {
                    TrainingActionPlan.TrainingType = courseList.Description;

                }
                var targetGroup = db.Lookups.Where(l => l.LookupType == LookUpTypes.TARGET_GROUP && l.IsActive&&l.LookupId==trainingPlan.RequiredTraining).FirstOrDefault();
                if (targetGroup !=null)
                {
                    TrainingActionPlan.TrainingCoductBody = targetGroup.Description;
                }
                var mainCompany = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == trainingPlan.OrgStructureId);
                if (mainCompany != null)
                {
                  TrainingActionPlan.WorkUnit = mainCompany.Name;
                }

                var providerlist = db.Companies.FirstOrDefault(x => x.MainCompanyId == trainingPlan.MainCompanyId);
                if (providerlist != null)
                {
                    TrainingActionPlan.Provider = providerlist.TradeName;
                }
                TrainingActionPlan.Year = trainingPlan.GlFiscalYear.Name;
                TrainingActionPlan.TraingPeriod = trainingPlan.GlFiscalYear.Name;
                TrainingActionPlan.TotalTrainee = trainingPlan.NoOfStaff;
                TrainingActionPlan.TrainingNeedRequests = new List<TrainingNeedRequestDetailVM>();
                var trainingNeedRequests = db.TrainingNeedRequests.Where(x => x.AssesmentId == trainingPlan.AssesmentId).ToList();
                foreach(var trainingNeedRequest in trainingNeedRequests)
                {
                    var needRequest = new TrainingNeedRequestDetailVM();
                    var employee = db.Person_Employee.Find(trainingNeedRequest.EmployeeId);
                    if(employee != null)
                    {
                        needRequest.Employee = employee.EmployeeCode + "-" + employee.FirstNameEnglish +" "+ employee.MiddleNameEnglish + " " + employee.LastNameEnglish;
                    }
                    needRequest.EmployeePerformanceGap = trainingNeedRequest.EmployeePerformanceGap;
                    needRequest.Prerequest = trainingNeedRequest.Prerequest;
                    needRequest.TrainingNeedForEmployeePerformanceGap = trainingNeedRequest.TrainingNeedForEmployeePerformanceGap;
                    needRequest.TheNeedForTrainingRelatedToInistitution = trainingNeedRequest.TheNeedForTrainingRelatedToInistitution;
                    needRequest.ExpectedResultFromTraining = trainingNeedRequest.ExpectedResultFromTraining;
                    TrainingActionPlan.TrainingNeedRequests.Add(needRequest);
                }



                Lists.Add(TrainingActionPlan);

            }
            
            return Lists;
        }
    }
}
