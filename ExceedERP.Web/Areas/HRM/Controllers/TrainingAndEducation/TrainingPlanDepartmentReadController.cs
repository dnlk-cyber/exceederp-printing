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
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.HRMPlan;
using ExceedERP.Helpers.Common;
namespace ExceedERP.Web.Areas.HRM.Controllers.TrainingAndEducation
{
    public class TrainingPlanDepartmentReadController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            GetProvider();
            GetCourse();
            GetTargetGroup();
            GetPeriod();
            PopulateViewData();
            TraininigPlans();
            // TraininigPlans();
            return View();
        }
        private void PopulateViewData()
        {
            var structres = db.MainCompanyStructures;
            ViewData["organization"] = structres;

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
            // Approved summaries count even when the assessment flag was never set (approvals made before it was).
            var assessments = db.NeedAssessments
                .Where(x => x.IsSummarized
                    && (x.IsThirdSendForApproval
                        || db.NeedAssessmentSummaryDetails.Any(d => d.AssesmentId == x.AssesmentId && d.Summary.IsApproved)))
                .ToList();
            if (!assessments.Any()) return;

            foreach (var need in assessments)
            {
                var trainigRequest = db.TrainingNeedRequests.Where(x => x.AssesmentId == need.AssesmentId);
                var existingPlan = db.TrainingPlans
                                .FirstOrDefault(x => x.LookupId == need.LookupId &&
                                x.GlFiscalYearId==need.GlFiscalYearId);
                var noOfPeople = trainigRequest.Select(x => x.EmployeeId).Distinct().Count();
                var costPerPerson = db.NeedAssessmentSummaryDetails.Where(x => x.AssesmentId==need.AssesmentId).Distinct().FirstOrDefault()?.Summary.CostPerPerson??0;
                if (existingPlan == null)
                {
                    var entity = new TrainingPlan
                    {
                        GlFiscalYearId = need.GlFiscalYearId,
                        NoofTrainee = noOfPeople,
                        CostPerPerson = costPerPerson,
                        LookupId = need.LookupId,
                        BusinessUnitId = need.BusinessUnitId,
                        WorkUnit = need.OrgStructureId ?? 0,
                        MainCompanyId = need.MainCompanyId,
                        TotalCost = (double)costPerPerson * noOfPeople,
                        Remark = need.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = need.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = need.ModifiedBy,
                        OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    };

                    db.TrainingPlans.Add(entity);
                }
                else
                {
                    existingPlan.NoofTrainee = noOfPeople;
                    existingPlan.CostPerPerson = costPerPerson;
                    existingPlan.TotalCost = (double) costPerPerson * noOfPeople;
                    existingPlan.LastModified = DateTime.Now;
                    existingPlan.ModifiedBy = User.Identity.Name;
                    db.Entry(existingPlan).State = EntityState.Modified;
                }

                db.SaveChanges();
            }
        }

        private JsonResult GetPeriod()
        {
            var periodList = db.GLFiscalYears.AsQueryable();



            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

            return Json(periodList.Select(p => new { GlFiscalYearId = p.GlFiscalYearId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult TrainingPlanDepartment_Read([DataSourceRequest]DataSourceRequest request)
        {

            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            IQueryable<TrainingPlan> trainingplans = db.TrainingPlans/*.Where(x => mybraches.Contains(x.b))*/;// && orgId.Contains(x.WorkUnit));
            DataSourceResult result = trainingplans.ToDataSourceResult(request, trainingPlan => new
            {
                TrainingPlanId = trainingPlan.TrainingPlanId,
                GlFiscalYearId = trainingPlan.GlFiscalYearId,
                Course = trainingPlan.Course,
                WorkUnit = trainingPlan.WorkUnit,
                NoofTrainee = trainingPlan.NoofTrainee,
                LookupId = trainingPlan.LookupId,
                MainCompanyId = trainingPlan.MainCompanyId,
                CostPerPerson = trainingPlan.CostPerPerson,
                TotalCost = trainingPlan.TotalCost,
                Remark = trainingPlan.Remark,
                DateCreated = trainingPlan.DateCreated,
                LastModified = trainingPlan.LastModified,
                CreatedBy = trainingPlan.CreatedBy,
                ModifiedBy = trainingPlan.ModifiedBy,
                OrgBranchName = trainingPlan.OrgBranchName,
            });

            return Json(result);
        }

        public ActionResult TrainingPlanDepartmentRead_Read([DataSourceRequest]DataSourceRequest request)
        {

            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            IQueryable<TrainingPlan> trainingplans = db.TrainingPlans/*.Where(x => mybraches.Contains(x.OrgBranchName))*/;// && orgId.Contains(x.WorkUnit));
            DataSourceResult result = trainingplans.ToDataSourceResult(request, trainingPlan => new
            {
                TrainingPlanId = trainingPlan.TrainingPlanId,
                GlFiscalYearId = trainingPlan.GlFiscalYearId,
                Course = trainingPlan.Course,
                WorkUnit = trainingPlan.WorkUnit,
                NoofTrainee = trainingPlan.NoofTrainee,
                LookupId = trainingPlan.LookupId,
                MainCompanyId = trainingPlan.MainCompanyId,
                CostPerPerson = trainingPlan.CostPerPerson,
                TotalCost = trainingPlan.TotalCost,
                Remark = trainingPlan.Remark,
                DateCreated = trainingPlan.DateCreated,
                LastModified = trainingPlan.LastModified,
                CreatedBy = trainingPlan.CreatedBy,
                ModifiedBy = trainingPlan.ModifiedBy,
                OrgBranchName = trainingPlan.OrgBranchName,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingPlanDepartmentRead_Create([DataSourceRequest]DataSourceRequest request, TrainingPlan trainingPlan)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new TrainingPlan
                {

                    GlFiscalYearId = trainingPlan.GlFiscalYearId,
                    Course = trainingPlan.Course,
                    NoofTrainee = trainingPlan.NoofTrainee,
                    LookupId = trainingPlan.LookupId,
                    MainCompanyId = trainingPlan.MainCompanyId,
                    WorkUnit = trainingPlan.WorkUnit,
                    CostPerPerson = trainingPlan.CostPerPerson,
                    TotalCost = trainingPlan.TotalCost,
                    Remark = trainingPlan.Remark,
                    DateCreated = DateTime.Now,
                    LastModified = trainingPlan.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = trainingPlan.ModifiedBy,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                };

                db.TrainingPlans.Add(entity);
                db.SaveChanges();
                trainingPlan.TrainingPlanId = entity.TrainingPlanId;
            }

            return Json(new[] { trainingPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingPlanDepartmentRead_Update([DataSourceRequest]DataSourceRequest request, TrainingPlan trainingPlan)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                double totalCost = trainingPlan.NoofTrainee * trainingPlan.CostPerPerson;
                var entity = new TrainingPlan
                {

                    TrainingPlanId = trainingPlan.TrainingPlanId,
                    GlFiscalYearId = trainingPlan.GlFiscalYearId,
                    WorkUnit = trainingPlan.WorkUnit,
                    Course = trainingPlan.Course,
                    NoofTrainee = trainingPlan.NoofTrainee,
                    LookupId = trainingPlan.LookupId,
                    MainCompanyId = trainingPlan.MainCompanyId,
                    CostPerPerson = trainingPlan.CostPerPerson,
                    TotalCost = totalCost,//trainingPlan.TotalCost,
                    Remark = trainingPlan.Remark,
                    DateCreated = trainingPlan.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = trainingPlan.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    OrgBranchName = trainingPlan.OrgBranchName,

                };

                db.TrainingPlans.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { trainingPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingPlanDepartmentRead_Destroy([DataSourceRequest]DataSourceRequest request, TrainingPlan trainingPlan)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new TrainingPlan
                {
                    TrainingPlanId = trainingPlan.TrainingPlanId,

                    GlFiscalYearId = trainingPlan.GlFiscalYearId,
                    Course = trainingPlan.Course,
                    WorkUnit = trainingPlan.WorkUnit,
                    NoofTrainee = trainingPlan.NoofTrainee,
                    LookupId = trainingPlan.LookupId,
                    MainCompanyId = trainingPlan.MainCompanyId,
                    CostPerPerson = trainingPlan.CostPerPerson,
                    TotalCost = trainingPlan.TotalCost,
                    Remark = trainingPlan.Remark,
                    DateCreated = trainingPlan.DateCreated,
                    LastModified = trainingPlan.LastModified,
                    CreatedBy = trainingPlan.CreatedBy,
                    ModifiedBy = trainingPlan.ModifiedBy,
                    OrgBranchName = trainingPlan.OrgBranchName,

                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted training plan information of fiscal year" + trainingPlan.GlFiscalYear.Name + " and training provider " + trainingPlan.Course);
                #endregion
                db.TrainingPlans.Attach(entity);
                db.TrainingPlans.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { trainingPlan }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
