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

namespace ExceedERP.Web.Areas.HRM.Controllers.TrainingAndEducation
{
    [AuthorizeRoles(HrmConstants.HRTrainingUser)]
    public class TrainingPlanController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            GetProvider();
            GetCourse();
            GetTargetGroup();
            GetPeriod();
            PopulateViewData();
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
                            GlFiscalYearId = 111,
                            NoofTrainee = need.NoOfStaff,
                            LookupId = need.LookupId,
                            MainCompanyId = need.MainCompanyId,
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
        [AuthorizeRoles(HrmConstants.HRTrainingUser, HrmConstants.HRAdmin)]
        public ActionResult TrainingPlans_Read([DataSourceRequest]DataSourceRequest request)
        {

            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            //var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

            //List<int> orgId = new List<int>();
            //var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empId && x.IsHead);
            //if (placement != null)
            //{
            //    var mainCompany = db.MainCompanyStructures.Where(x => x.Parent == placement.OrgStructureId || x.OrgStructureId == placement.OrgStructureId);
            //    if (mainCompany.Any())
            //    {
            //        var org = mainCompany.Select(x => x.OrgStructureId);
            //        if (org.Any())
            //        {
            //            orgId.AddRange(org);
            //        }
            //    }
            //}
            IQueryable<TrainingPlan> trainingplans = db.TrainingPlans;//.Where(x => mybraches.Contains(x.OrgBranchName) && orgId.Contains(x.WorkUnit));
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

        [AuthorizeRoles(HrmConstants.HRTrainingUser)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingPlans_Create([DataSourceRequest]DataSourceRequest request, TrainingPlan trainingPlan)
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

        [AuthorizeRoles(HrmConstants.HRTrainingUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingPlans_Update([DataSourceRequest]DataSourceRequest request, TrainingPlan trainingPlan)
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

        [AuthorizeRoles(HrmConstants.HRTrainingUser, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingPlans_Destroy([DataSourceRequest]DataSourceRequest request, TrainingPlan trainingPlan)
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
