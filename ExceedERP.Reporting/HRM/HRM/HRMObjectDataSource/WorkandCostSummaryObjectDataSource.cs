using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{


    [DataObject]
    public class WorkandCostSummaryObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<WorkandCostSummary> GetWorkandCostSummaries(string fisicalYear, string workunit,string trainingType,string branch)
        {
            List<WorkandCostSummary> summaryList = new List<WorkandCostSummary> { };
            string term = TrainingTerm.LONG.ToString();
            if (trainingType == TrainingType .TRAINING .ToString ())
            {
                term = TrainingTerm.SHORT.ToString();
            }

            // include case teams under that department
            List<int> departments = new List<int>();
            if(workunit != null)
            {
                int.TryParse(workunit, out int orgId);
                 departments = db.MainCompanyStructures.Where(x => x.OrgStructureId == orgId || x.Parent == orgId).Select(x=>x.OrgStructureId).ToList();
            }
          

            if (fisicalYear != null && workunit == null && trainingType==null)
            {
                var courseTracking = db.CourseTrackings.Where(x => x.GlFiscalYearId.ToString() == fisicalYear).ToList();
                if(branch != null)
                {
                    courseTracking = courseTracking.Where(x=>x.OrgBranchName.ToString() == branch).ToList();
                }
                foreach (var tracking in courseTracking)
                {
                    var workandCostSummary = new WorkandCostSummary();
                    // workandCostSummary.TrainingType = tracking.TrainingType;
                    //if (tracking.TrainingType==TrainingType.TRAINING)
                    //{
                    //    workandCostSummary.TrainingType = "ስልጠና";
                    //}
                    //else if(tracking.TrainingType == TrainingType.EDUCATION)
                    //{
                    //    workandCostSummary.TrainingType = "ትምህርት";
                    //}
                    var course = db.Lookups.FirstOrDefault(x=>x.LookupId == tracking.LookupId && x.LookupType == LookUpTypes.COURSE);

                    if (course != null)
                    {
                        workandCostSummary.TrainingType = course.Description.ToString();
                    }
                    workandCostSummary.TotalCost = tracking.Cost;

                    var trainingPlan =
                        db.TrainingPlans.FirstOrDefault(x => x.TrainingPlanId == tracking.TrainingPlanId);
                    if (trainingPlan != null)
                    {

                        var mainCompanyStructure =
                            db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == trainingPlan.WorkUnit);
                        if (mainCompanyStructure != null)
                        {
                            workandCostSummary.WorkUnit = mainCompanyStructure.Name;
                        }
                    }

                    var trainee = db.Trainee.Where(x => x.CourseTrackingId == tracking.CourseTrackingId);
                    if (trainee != null)
                    {
                        workandCostSummary.TraineeQuantity = trainee.Count();
                    }

                    var year = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == tracking.GlFiscalYearId);
                    if (year != null)
                    {
                        workandCostSummary.TrainingTime = year.Name;
                    }

                    var mainCompany = db.Companies.FirstOrDefault(x => x.MainCompanyId == tracking.MainCompanyId);
                    if (mainCompany != null)
                    {
                        workandCostSummary.Provider = mainCompany.TradeName;
                    }

                    summaryList.Add(workandCostSummary);
                }
            }
            else if (fisicalYear != null && workunit!=null && trainingType == null)
            {

                var courseTracking = db.CourseTrackings.Where(x => x.GlFiscalYearId.ToString() == fisicalYear ).ToList();
                if (branch != null)
                {
                    courseTracking = courseTracking.Where(x => x.OrgBranchName.ToString() == branch).ToList();
                }
                foreach (var tracking in courseTracking)
                {
                    var workandCostSummary = new WorkandCostSummary();

                    var trainingPlan =
                        db.TrainingPlans.FirstOrDefault(x => x.TrainingPlanId == tracking.TrainingPlanId && departments.Contains(x.WorkUnit));
                    if (trainingPlan != null)
                    {

                        // workandCostSummary.TrainingType = tracking.TrainingType;
                        //if (tracking.TrainingType == TrainingType.TRAINING)
                        //{
                        //    workandCostSummary.TrainingType = "ስልጠና";
                        //}
                        //else if (tracking.TrainingType == TrainingType.EDUCATION)
                        //{
                        //    workandCostSummary.TrainingType = "ትምህርት";
                        //}
                        var course = db.Lookups.FirstOrDefault(x => x.LookupId == tracking.LookupId && x.LookupType == LookUpTypes.COURSE);

                        if (course != null)
                        {
                            workandCostSummary.TrainingType = course.Description.ToString();
                        }
                        workandCostSummary.TotalCost = tracking.Cost;

                        var mainCompanyStructure =
                            db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == trainingPlan.WorkUnit);
                        if (mainCompanyStructure != null)
                        {
                            workandCostSummary.WorkUnit = mainCompanyStructure.Name;
                        }

                        var trainee = db.Trainee.Where(x => x.CourseTrackingId == tracking.CourseTrackingId).ToList();
                        if (trainee.Any())
                        {
                            workandCostSummary.TraineeQuantity = trainee.Count();
                        }

                        var year = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == tracking.GlFiscalYearId);
                        if (year != null)
                        {
                            workandCostSummary.TrainingTime = year.Name;
                        }

                        var mainCompany = db.Companies.FirstOrDefault(x => x.MainCompanyId == tracking.MainCompanyId);
                        if (mainCompany != null)
                        {
                            workandCostSummary.Provider = mainCompany.TradeName;
                        }
                        summaryList.Add(workandCostSummary);
                    }

                  
                }
            }
            else if (fisicalYear != null && workunit==null && trainingType != null)
            {
                var courseTracking = db.CourseTrackings.Where(x => x.GlFiscalYearId.ToString() == fisicalYear && x.TrainingType.ToString()==trainingType&& x.TrainingTerm.ToString () ==term ).ToList();
                if (branch != null)
                {
                    courseTracking = courseTracking.Where(x => x.OrgBranchName.ToString() == branch).ToList();
                }
                foreach (var tracking in courseTracking)
                {
                    var workandCostSummary = new WorkandCostSummary();

                    var trainingPlan =
                        db.TrainingPlans.FirstOrDefault(x => x.TrainingPlanId == tracking.TrainingPlanId );
                    if (trainingPlan != null)
                    {

                        // workandCostSummary.TrainingType = tracking.TrainingType;
                        //if (tracking.TrainingType == TrainingType.TRAINING)
                        //{
                        //    workandCostSummary.TrainingType = "ስልጠና";
                        //}
                        //else if (tracking.TrainingType == TrainingType.EDUCATION)
                        //{
                        //    workandCostSummary.TrainingType = "ትምህርት";
                        //}
                        var course = db.Lookups.FirstOrDefault(x => x.LookupId == tracking.LookupId && x.LookupType == LookUpTypes.COURSE);

                        if (course != null)
                        {
                            workandCostSummary.TrainingType = course.Description.ToString();
                        }
                      

                        workandCostSummary.TotalCost = tracking.Cost;

                        var mainCompanyStructure =
                            db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == trainingPlan.WorkUnit);
                        if (mainCompanyStructure != null)
                        {
                            workandCostSummary.WorkUnit = mainCompanyStructure.Name;
                        }

                        var trainee = db.Trainee.Where(x => x.CourseTrackingId == tracking.CourseTrackingId).ToList();
                        if (trainee.Any())
                        {
                            workandCostSummary.TraineeQuantity = trainee.Count();
                        }

                        var year = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == tracking.GlFiscalYearId);
                        if (year != null)
                        {
                            workandCostSummary.TrainingTime = year.Name;
                        }

                        var mainCompany = db.Companies.FirstOrDefault(x => x.MainCompanyId == tracking.MainCompanyId);
                        if (mainCompany != null)
                        {
                            workandCostSummary.Provider = mainCompany.TradeName;
                        }
                        summaryList.Add(workandCostSummary);
                    }


                }
            }
            else if (fisicalYear != null && workunit != null && trainingType != null)
            {
                var courseTracking = db.CourseTrackings.Where(x => x.GlFiscalYearId.ToString() == fisicalYear && x.TrainingType.ToString() == trainingType && x.TrainingTerm.ToString() == term).ToList();
                if (branch != null)
                {
                    courseTracking = courseTracking.Where(x => x.OrgBranchName.ToString() == branch).ToList();
                }
                foreach (var tracking in courseTracking)
                {
                    var workandCostSummary = new WorkandCostSummary();

                    var trainingPlan =
                        db.TrainingPlans.FirstOrDefault(x => x.TrainingPlanId == tracking.TrainingPlanId && departments.Contains(x.WorkUnit));
                    if (trainingPlan != null)
                    {

                        // workandCostSummary.TrainingType = tracking.TrainingType;
                        //if (tracking.TrainingType == TrainingType.TRAINING)
                        //{
                        //    workandCostSummary.TrainingType = "ስልጠና";
                        //}
                        //else if (tracking.TrainingType == TrainingType.EDUCATION)
                        //{
                        //    workandCostSummary.TrainingType = "ትምህርት";
                        //}

                        var course = db.Lookups.FirstOrDefault(x => x.LookupId == tracking.LookupId && x.LookupType == LookUpTypes.COURSE);

                        if (course != null)
                        {
                            workandCostSummary.TrainingType = course.Description.ToString();
                        }
                        workandCostSummary.TotalCost = tracking.Cost;

                        var mainCompanyStructure =
                            db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == trainingPlan.WorkUnit);
                        if (mainCompanyStructure != null)
                        {
                            workandCostSummary.WorkUnit = mainCompanyStructure.Name;
                        }

                        var trainee = db.Trainee.Where(x => x.CourseTrackingId == tracking.CourseTrackingId).ToList();
                        if (trainee.Any())
                        {
                            workandCostSummary.TraineeQuantity = trainee.Count();
                        }

                        var year = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == tracking.GlFiscalYearId);
                        if (year != null)
                        {
                            workandCostSummary.TrainingTime = year.Name;
                        }

                        var mainCompany = db.Companies.FirstOrDefault(x => x.MainCompanyId == tracking.MainCompanyId);
                        if (mainCompany != null)
                        {
                            workandCostSummary.Provider = mainCompany.TradeName;
                        }
                        summaryList.Add(workandCostSummary);
                    }


                }
            }

            return summaryList;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}
