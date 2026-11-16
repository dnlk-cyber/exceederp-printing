using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using System;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.HRMPlan;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.HRM.TrainingEducation;

namespace ExceedERP.Web.Areas.HRM.Controllers.TrainingAndEducation
{
    [AuthorizeRoles(HrmConstants.HRTrainingUser)]
    public class NeedAssessmentSummaryController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            HRMInputs inputs = new HRMInputs();
            ViewData["EmployeeList"] = inputs.GetEmployees(User.Identity.Name);
            GetProvider();
            GetCourse();
            GetTargetGroup();
            GetPeriod();
            PopulateViewData();
            TraininigPlans();
            return View();
        }
        private void PopulateViewData()
        {
            var structres = db.MainCompanyStructures;
            ViewData["organization"] = structres;
            var employeName = db.Person_Employee.ToList();
            ViewData["EmployeeName"] = employeName;
            var branch = db.Branch.ToList();
            ViewData["Branches"] = branch;
            var periodList = db.GLFiscalYears;
            ViewData["PeriodList"] = periodList;
            ViewData["allDepartments"] = db.MainCompanyStructures.ToList();
            ViewData["TargetGroup"] = db.Lookups.Where(x => x.LookupType == LookUpTypes.TARGET_GROUP);
            var businessUnits = db.BusinessUnits.Select(x => new { x.BusinessUnitId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") });
            ViewData["businessUnits"] = businessUnits;
            ViewData["isEthiopicDate"] = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate;


        }

        public JsonResult GetStructures()
        {
            var courseList = db.MainCompanyStructures.AsQueryable().ToList();

            return Json(courseList.Select(p => new
            {
                OrgStructureId = p.OrgStructureId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCourse()
        {
            var courseList = db.Lookups.AsQueryable().ToList();
            if (courseList.Any())
            {
                courseList = courseList.Where(l => l.LookupType == LookUpTypes.COURSE && l.IsActive).ToList();
            }
            ViewData["CourseList"] = courseList.Count > 0 ? courseList.ToList() : new List<Lookup>();
            return Json(courseList.Select(p => new { LookupId = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTargetGroup()
        {
            var targetGroupList = db.Lookups.AsQueryable().ToList();
            if (targetGroupList.Count > 0)
            {
                targetGroupList = targetGroupList.Where(l => l.LookupType == LookUpTypes.TARGET_GROUP && l.IsActive).ToList();
            }
            ViewData["TargetGroupList"] = targetGroupList.Count > 0 ? targetGroupList.ToList() : new List<Lookup>();
            return Json(targetGroupList.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProvider()
        {
            var providerlist = db.Companies.AsQueryable().ToList();
            if (providerlist.Any())
            {
                providerlist = providerlist.Where(c => c.Category == "1" && c.IsActive).ToList();
            }
            ViewData["Companies"] = providerlist.Count > 0 ? providerlist.ToList() : new List<MainCompany>();
            return Json(providerlist.Select(p => new
            {
                MainCompanyId = p.MainCompanyId,
                TradeName = p.TradeName
            }), JsonRequestBehavior.AllowGet);
        }
        private void TraininigPlans()
        {
            var assessment = db.NeedAssessments.ToList();

            if (assessment.Any())
            {
                foreach (var need in assessment)
                {
                    var plan = db.TrainingPlans.Where(x => x.LookupId == need.LookupId).FirstOrDefault();
                    if (plan == null)
                    {
                        var level = db.Lookups.Where(x => x.LookupId == need.LookupId).FirstOrDefault();
                        var entity = new TrainingPlan
                        {
                            GlFiscalYearId = need.GlFiscalYearId,
                            NoofTrainee = need.NoOfStaff,
                            LookupId = need.LookupId,
                            MainCompanyId = need.MainCompanyId,
                            BusinessUnitId = need.BusinessUnitId,
                            WorkUnit = need.OrgStructureId??0,
                           // WorkUnit=Convert.ToInt32(level.Description),
                            Remark = need.Remark,
                            DateCreated = DateTime.Now,
                            LastModified = need.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = need.ModifiedBy,
                            OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                        };
                        db.TrainingPlans.Add(entity);
                        db.SaveChanges();
                    }
                    else
                    {
                        var entity = new TrainingPlan
                        {
                            TrainingPlanId = plan.TrainingPlanId,
                            GlFiscalYearId = plan.GlFiscalYearId,
                            WorkUnit = plan.WorkUnit,
                            BusinessUnitId = plan.BusinessUnitId,
                            Course = plan.Course,
                            NoofTrainee = plan.NoofTrainee,
                            LookupId = plan.LookupId,
                            MainCompanyId = plan.MainCompanyId,
                            CostPerPerson = plan.CostPerPerson,
                            TotalCost = plan.TotalCost,
                            Remark = plan.Remark,
                            DateCreated = plan.DateCreated,
                            LastModified = DateTime.Now,
                            CreatedBy = plan.CreatedBy,
                            ModifiedBy = User.Identity.Name,
                            OrgBranchName = plan.OrgBranchName,

                        };
                        db.TrainingPlans.Attach(entity);
                        db.Entry(entity).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
        }
        private JsonResult GetPeriod()
        {
            var periodList = db.GLFiscalYears.AsQueryable();



            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

            return Json(periodList.Select(p => new { GlFiscalYearId = p.GlFiscalYearId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ApproveNeedSummary(int[] needAssessmenntSummaryIds)
        {
            foreach(var needSummaryId in needAssessmenntSummaryIds)
            {
                var needSummary = db.NeedAssessmentSummary.Find(needSummaryId);
                if(needSummary != null)
                {
                    needSummary.IsSendForApproval = true;
                    needSummary.IsSecondSendForApproval = true;
                    needSummary.IsThirdSendForApproval = true;
                    needSummary.IsApproved = true;
                    needSummary.IsTransfered = true;
                    needSummary.Remark = "Approved";
                    db.NeedAssessmentSummary.Attach(needSummary);
                    db.Entry(needSummary).State = EntityState.Modified;

                    // The department Training Plan page builds its plans from assessments flagged here.
                    var approvedAssessmentIds = db.NeedAssessmentSummaryDetails.Where(x => x.AssesmentSummaryId == needSummaryId).Select(x => x.AssesmentId).Distinct();
                    foreach (var assessment in db.NeedAssessments.Where(x => approvedAssessmentIds.Contains(x.AssesmentId)))
                    {
                        assessment.IsThirdSendForApproval = true;
                        db.Entry(assessment).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
            }
            return Json("successfyully sent");
        }
        public ActionResult RejectNeedSummary(int[] needAssessmenntSummaryRejectsIds)
        {
            foreach(var needSummaryId in needAssessmenntSummaryRejectsIds)
            {
                var needSummary = db.NeedAssessmentSummary.Find(needSummaryId);
                if(needSummary != null)
                {
                    needSummary.IsSendForApproval = true;
                    needSummary.IsSecondSendForApproval = false;
                    needSummary.IsTransfered = false;
                    needSummary.IsRejected = true;
                    needSummary.Remark = "Rejected";
                    db.NeedAssessmentSummary.Attach(needSummary);
                    db.Entry(needSummary).State = EntityState.Modified;
                    //db.SaveChanges();
                    var assessmentIds = db.NeedAssessmentSummaryDetails.Where(x => x.AssesmentSummaryId == needSummaryId).Select(x=>x.AssesmentId).Distinct();
                    var assessments = db.NeedAssessments.Where(x => assessmentIds.Contains(x.AssesmentId));
                    foreach(var assessment in assessments)
                    {
                        assessment.IsSecondSendForApproval = false;
                        assessment.IsSecondSendForApproval = false;
                        assessment.IsSummarized = false;
                        assessment.IsSendForApproval = false;
                        assessment.Remark = "Rejected";
                        db.Entry(assessment).State = EntityState.Modified;

                    }
                    db.SaveChanges();

                }
            }
            return Json("successfyully sent");
        }
        public ActionResult SendNeedSummaryForApproval(int[] needAssessSummaryIds)
        {
            foreach(var needSummaryId in needAssessSummaryIds)
            {
                var needSummary = db.NeedAssessmentSummary.Find(needSummaryId);
                if(needSummary != null)
                {
                    needSummary.IsSendForApproval = true;
                    needSummary.IsSecondSendForApproval = true;
                    needSummary.IsTransfered = true;
                    db.NeedAssessmentSummary.Attach(needSummary);
                    db.Entry(needSummary).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return Json("successfyully sent");
        }

        public ActionResult TrainingNeedSummaryRequests_Read([DataSourceRequest]DataSourceRequest request,int assessmentSummaryId, int looKupId,int fiscalYearId)
        {
            List<int> assessmentIds = db.NeedAssessmentSummaryDetails.Where(x => x.AssesmentSummaryId == assessmentSummaryId).Select(x => x.AssesmentId).ToList();
            var  trainingNeedRequests = db.TrainingNeedRequests.Where(x => assessmentIds.Contains(x.AssesmentId) /*x.LookupId == looKupId && x.GlFiscalYearId == fiscalYearId*/).ToList()/*.Where(x => mybraches.Contains(x.OrgBranchName) && x.IsTransfered == false)*/;
            DataSourceResult result = trainingNeedRequests.ToDataSourceResult(request, trainingNeedRequest => new
            {
                TrainingNeedRequestId = trainingNeedRequest.TrainingNeedRequestId,
                AssesmentId = trainingNeedRequest.AssesmentId,
                EmployeeId = trainingNeedRequest.EmployeeId,
                OrgStructureId = trainingNeedRequest.OrgStructureId,
                GlFiscalYearId = trainingNeedRequest.GlFiscalYearId,
                LookupId = trainingNeedRequest.LookupId,
                Prerequest = trainingNeedRequest.Prerequest,
                EmployeePerformanceGap = trainingNeedRequest.EmployeePerformanceGap,
                TrainingNeedForEmployeePerformanceGap = trainingNeedRequest.TrainingNeedForEmployeePerformanceGap,
                TheNeedForTrainingRelatedToInistitution = trainingNeedRequest.TheNeedForTrainingRelatedToInistitution,
                ExpectedResultFromTraining = trainingNeedRequest.ExpectedResultFromTraining,

            });

            return Json(result);
        }

        public ActionResult NeedAssessmentSummaryDetail_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            HashSet<int> assesmentIds = new HashSet<int>(db.NeedAssessmentSummaryDetails.Where(x => x.AssesmentSummaryId == id).Select(x => x.AssesmentId));
            IQueryable<NeedAssessment> needassessments = db.NeedAssessments.Where(x => assesmentIds.Contains(x.AssesmentId)); ;

            DataSourceResult result = needassessments.ToDataSourceResult(request, needAssessment => new
            {
                AssesmentId = needAssessment.AssesmentId,
                OrgStructureId = needAssessment.OrgStructureId,
                BusinessUnitId = needAssessment.BusinessUnitId,
                NoOfStaff = needAssessment.NoOfStaff,
                GlFiscalYearId = needAssessment.GlFiscalYearId,
                RequiredTraining = needAssessment.RequiredTraining,
                IsTransfered = needAssessment.IsTransfered,
                LookupId = needAssessment.LookupId,
                Description = needAssessment.Description,
                Type = needAssessment.Type,
                From = needAssessment.From,
                FromAmharic = needAssessment.FromAmharic,
                To = needAssessment.To,
                ToAmharic = needAssessment.ToAmharic,
                TotalDays = needAssessment.TotalDays,
                IsSecondSendForApproval = needAssessment.IsSecondSendForApproval,
                IsSendForApproval = needAssessment.IsSendForApproval,
                IsSummarized = needAssessment.IsSummarized,
                IsThirdSendForApproval = needAssessment.IsThirdSendForApproval,
                Check = needAssessment.Check,
                MainCompanyId = needAssessment.MainCompanyId,
                DevelopmentRequired = needAssessment.DevelopmentRequired,
                DevelopmentBy = needAssessment.DevelopmentBy,
                Remark = needAssessment.Remark,
                DateCreated = needAssessment.DateCreated,
                LastModified = needAssessment.LastModified,
                CreatedBy = needAssessment.CreatedBy,
                ModifiedBy = needAssessment.ModifiedBy,
                OrgBranchName = needAssessment.OrgBranchName,
            });
            return Json(result);
        }

        public ActionResult NeedAssessmentSummaryForApproval_Read([DataSourceRequest]DataSourceRequest request)
        {

            IQueryable<NeedAssessmentSummary> assessmentSummaries = db.NeedAssessmentSummary.Where(x=>x.IsSendForApproval&&x.IsTransfered&&!x.IsApproved&&!x.IsRejected);//.Where(x => mybraches.Contains(x.OrgBranchName) && orgId.Contains(x.WorkUnit));
            DataSourceResult result = assessmentSummaries.ToDataSourceResult(request, assessmentSummary => new
            {
                NeedAssessmentSummaryId = assessmentSummary.NeedAssessmentSummaryId,
                GlFiscalYearId = assessmentSummary.GlFiscalYearId,
                NoOfStaff = assessmentSummary.NoOfStaff,
                LookupId = assessmentSummary.LookupId,
                MainCompanyId = assessmentSummary.MainCompanyId,
                CostPerPerson = assessmentSummary.CostPerPerson,
                TotalCost = assessmentSummary.TotalCost,
                Remark = assessmentSummary.Remark,
                DateCreated = assessmentSummary.DateCreated,
                LastModified = assessmentSummary.LastModified,
                CreatedBy = assessmentSummary.CreatedBy,
                ModifiedBy = assessmentSummary.ModifiedBy,
                OrgBranchName = assessmentSummary.OrgBranchName,
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRTrainingUser, HrmConstants.HRAdmin)]
        public ActionResult NeedAssessmentSummary_Read([DataSourceRequest]DataSourceRequest request)
        {


            IQueryable<NeedAssessmentSummary> assessmentSummaries = db.NeedAssessmentSummary.Where(x => !x.IsTransfered & !x.IsSendForApproval);//.Where(x => mybraches.Contains(x.OrgBranchName) && orgId.Contains(x.WorkUnit));
            DataSourceResult result = assessmentSummaries.ToDataSourceResult(request, assessmentSummary => new
            {
                NeedAssessmentSummaryId = assessmentSummary.NeedAssessmentSummaryId,
                GlFiscalYearId = assessmentSummary.GlFiscalYearId,
                NoOfStaff = assessmentSummary.NoOfStaff,
                LookupId = assessmentSummary.LookupId,
                MainCompanyId = assessmentSummary.MainCompanyId,
                CostPerPerson = assessmentSummary.CostPerPerson,
                TotalCost = assessmentSummary.TotalCost,
                Remark = assessmentSummary.Remark,
                DateCreated = assessmentSummary.DateCreated,
                LastModified = assessmentSummary.LastModified,
                CreatedBy = assessmentSummary.CreatedBy,
                ModifiedBy = assessmentSummary.ModifiedBy,
                OrgBranchName = assessmentSummary.OrgBranchName,
            });

            return Json(result);
        }
        [AuthorizeRoles(HrmConstants.HRTrainingUser)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NeedAssessmentSummary_Create([DataSourceRequest]DataSourceRequest request, NeedAssessmentSummary assessmentSummary)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                assessmentSummary.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                assessmentSummary.TotalCost = assessmentSummary.NoOfStaff * assessmentSummary.CostPerPerson;
                var entity = GetMappedObject(assessmentSummary);
                db.NeedAssessmentSummary.Add(entity);
                db.SaveChanges();
                assessmentSummary.NeedAssessmentSummaryId = entity.NeedAssessmentSummaryId;
            }

            return Json(new[] { assessmentSummary }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRTrainingUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NeedAssessmentSummary_Update([DataSourceRequest]DataSourceRequest request, NeedAssessmentSummary assessmentSummary)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                assessmentSummary.TotalCost = assessmentSummary.NoOfStaff * assessmentSummary.CostPerPerson;
                var entity = GetMappedObject(assessmentSummary);
                db.NeedAssessmentSummary.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { assessmentSummary }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRTrainingUser, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NeedAssessmentSummary_Destroy([DataSourceRequest]DataSourceRequest request, NeedAssessmentSummary assessmentSummary)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(assessmentSummary);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted training plan information of fiscal year" + assessmentSummary.GlFiscalYear.Name + " and training provider " + assessmentSummary.MainCompany.TradeName);
                #endregion
                db.NeedAssessmentSummary.Attach(entity);
                db.NeedAssessmentSummary.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { assessmentSummary }.ToDataSourceResult(request, ModelState));
        }
        private NeedAssessmentSummary GetMappedObject(NeedAssessmentSummary assessmentSummary)
        {
            var entity = new NeedAssessmentSummary
            {
                NeedAssessmentSummaryId = assessmentSummary.NeedAssessmentSummaryId,
                GlFiscalYearId = assessmentSummary.GlFiscalYearId,
                NoOfStaff = assessmentSummary.NoOfStaff,
                LookupId = assessmentSummary.LookupId,
                MainCompanyId = assessmentSummary.MainCompanyId,
                CostPerPerson = assessmentSummary.CostPerPerson,
                TotalCost = assessmentSummary.TotalCost,
                Remark = assessmentSummary.Remark,
                DateCreated = assessmentSummary.DateCreated,
                LastModified = assessmentSummary.LastModified,
                CreatedBy = assessmentSummary.CreatedBy,
                ModifiedBy = assessmentSummary.ModifiedBy,
                OrgBranchName = assessmentSummary.OrgBranchName,

            };
            return entity;
        }
      

     
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
