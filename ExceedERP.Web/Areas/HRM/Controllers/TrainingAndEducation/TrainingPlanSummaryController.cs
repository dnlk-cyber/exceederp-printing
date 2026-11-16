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
using ExceedERP.Web.Hubs;

namespace ExceedERP.Web.Areas.HRM.Controllers.TrainingAndEducation
{
    [AuthorizeRoles(HrmConstants.HRTrainingUser)]
    public class TrainingPlanSummaryController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private IQueryable<NeedAssessmentSummary> ApprovedSummaries =>
            db.NeedAssessmentSummary.Where(x => x.IsThirdSendForApproval && x.IsApproved && !x.IsRejected);

        public ActionResult Index()
        {
            ViewData["EvaluationCategories"] = db.TrainingEvaluationCriteriaCategories.OrderBy(x => x.Name).ToList();
            ViewData["EmployeeList"] = db.Person_Employee.Select(x => new
            {
                PersonId=x.EmployeeId,
                FullNameWithOutCode = x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish
            }).ToList();
            GetProvider();
            GetStructures();
            //GetDepartments();
            GetCourse();
            GetTargetGroup();
            GetPeriod();
            PopulateViewData();
            // TraininigPlans();
            return View();
        }

        public ActionResult EvaluationCriteria_Read([DataSourceRequest] DataSourceRequest request, int summaryId)
        {
            var courses = ApprovedSummaries.Where(x => x.NeedAssessmentSummaryId == summaryId).Select(x => x.LookupId);
            var criteria = db.TrainingEvaluationCriterias
                .Where(x => x.TrainingCourseId.HasValue && courses.Contains(x.TrainingCourseId.Value))
                .Select(x => new { x.TrainingEvaluationCriteriaId, x.TrainingEvaluationSettingCategoryId, x.Criteria, x.Weight });
            return Json(criteria.ToDataSourceResult(request));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EvaluationCriteria_Create([DataSourceRequest] DataSourceRequest request, int summaryId,
            TrainingEvaluationCriteria input)
        {
            var summary = ApprovedSummaries.SingleOrDefault(x => x.NeedAssessmentSummaryId == summaryId);
            ValidateEvaluationCriterion(summary, input);
            if (ModelState.IsValid)
            {
                var criterion = new TrainingEvaluationCriteria
                {
                    TrainingCourseId = summary.LookupId,
                    TrainingEvaluationSettingCategoryId = input.TrainingEvaluationSettingCategoryId,
                    Criteria = input.Criteria.Trim(),
                    Weight = input.Weight
                };
                db.TrainingEvaluationCriterias.Add(criterion);
                db.SaveChanges();
                input.TrainingEvaluationCriteriaId = criterion.TrainingEvaluationCriteriaId;
                input.Criteria = criterion.Criteria;
            }
            return Json(new[] { input }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EvaluationCriteria_Update([DataSourceRequest] DataSourceRequest request, int summaryId,
            TrainingEvaluationCriteria input)
        {
            var summary = ApprovedSummaries.SingleOrDefault(x => x.NeedAssessmentSummaryId == summaryId);
            ValidateEvaluationCriterion(summary, input);
            var criterion = FindCourseCriterion(summary, input.TrainingEvaluationCriteriaId);
            if (criterion == null) ModelState.AddModelError("", "This criterion does not belong to the course.");
            if (ModelState.IsValid)
            {
                criterion.TrainingEvaluationSettingCategoryId = input.TrainingEvaluationSettingCategoryId;
                criterion.Criteria = input.Criteria.Trim();
                criterion.Weight = input.Weight;
                db.SaveChanges();
                input.Criteria = criterion.Criteria;
            }
            return Json(new[] { input }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EvaluationCriteria_Destroy([DataSourceRequest] DataSourceRequest request, int summaryId,
            TrainingEvaluationCriteria input)
        {
            using (var transaction = db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                var summary = ApprovedSummaries.SingleOrDefault(x => x.NeedAssessmentSummaryId == summaryId);
                var criterion = FindCourseCriterion(summary, input.TrainingEvaluationCriteriaId);
                if (criterion == null)
                    ModelState.AddModelError("", "Select a criterion from an approved training summary.");
                else if (db.TrainingEvaluationBefores.Any(x => x.TrainingEvaluationCriteriaId == criterion.TrainingEvaluationCriteriaId)
                    || db.TrainingEvaluationAfters.Any(x => x.TrainingEvaluationCriteriaId == criterion.TrainingEvaluationCriteriaId))
                    ModelState.AddModelError("", "This criterion is already used by an evaluation.");
                if (ModelState.IsValid)
                {
                    db.TrainingEvaluationCriterias.Remove(criterion);
                    db.SaveChanges();
                    transaction.Commit();
                }
            }
            return Json(new[] { input }.ToDataSourceResult(request, ModelState));
        }

        private TrainingEvaluationCriteria FindCourseCriterion(NeedAssessmentSummary summary, int criteriaId)
        {
            return summary == null ? null : db.TrainingEvaluationCriterias.SingleOrDefault(x =>
                x.TrainingEvaluationCriteriaId == criteriaId && x.TrainingCourseId == summary.LookupId);
        }

        private void ValidateEvaluationCriterion(NeedAssessmentSummary summary, TrainingEvaluationCriteria input)
        {
            if (summary == null) ModelState.AddModelError("", "Select an approved training summary.");
            if (string.IsNullOrWhiteSpace(input.Criteria)) ModelState.AddModelError("Criteria", "Enter the evaluation criterion.");
            if (input.Weight < 0) ModelState.AddModelError("Weight", "Weight cannot be negative.");
            if (!db.TrainingEvaluationCriteriaCategories.Any(x => x.TrainingEvaluationSettingCategoryId == input.TrainingEvaluationSettingCategoryId))
                ModelState.AddModelError("TrainingEvaluationSettingCategoryId", "Select an evaluation category.");
        }
       
        private void PopulateViewData()
        {
            var employeName = db.Person_Employee.ToList();
            ViewData["EmployeeName"] = employeName;
            var structres = db.MainCompanyStructures.AsQueryable();
            ViewData["organization"] = structres;
            GetDepartments();
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

       
       
        
        public JsonResult TrainingType()
        {
            var traTypeList = db.Lookups.AsQueryable();
            if (traTypeList.Count() > 0)
            {
                traTypeList = traTypeList.Where(l => l.LookupType == LookUpTypes.TRAINING_TYPE).OrderBy(o => o.LookupType);

            }
            ViewData["TraTypeList"] = traTypeList != null ? traTypeList.ToList() : new List<Lookup>();
            return Json(traTypeList.Select(p => new { LookupId = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStructures()
        {
            var courseList = db.MainCompanyStructures.AsQueryable().ToList();
            ViewData["Departments"] = courseList.ToList();
            return Json(courseList.Select(p => new
            {
                OrgStructureId = p.OrgStructureId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);
        }
        public void GetDepartments()
        {
            List<MainCompanyStructure> allDepartment = new List<MainCompanyStructure>();
            var departments = db.MainCompanyStructures.FirstOrDefault(x => x.Parent == 0);
            if (departments != null)
            {
                var deptlevelOne = db.MainCompanyStructures.Where(x => x.Parent == departments.OrgStructureId).ToList();
                if (deptlevelOne.Any())
                {
                    foreach (var a in deptlevelOne)
                    {
                        var departmentLevelTwo =
                            db.MainCompanyStructures.Where(x => x.Parent == a.OrgStructureId).ToList();
                        if (departmentLevelTwo.Any())
                        {
                            foreach (var b in departmentLevelTwo)
                            {
                                allDepartment.Add(b);
                            }
                        }
                        allDepartment.Add(a);
                    }
                }
            }
            ViewData["Departments"] = allDepartment;
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

       
        public ActionResult TrainingPlanSummary_Read([DataSourceRequest]DataSourceRequest request)
        {

            IQueryable<NeedAssessmentSummary> assessmentSummaries = db.NeedAssessmentSummary.Where(x=>x.IsThirdSendForApproval&x.IsApproved||x.IsRejected);//.Where(x => mybraches.Contains(x.OrgBranchName) && orgId.Contains(x.WorkUnit));
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


        [HttpPost]
        public JsonResult SendCourseTrackingNotifications(int needAssessmentSummaryId)
        {
            var needAssessmentSummary = db.NeedAssessmentSummary
                .FirstOrDefault(x => x.NeedAssessmentSummaryId == needAssessmentSummaryId);

            if (needAssessmentSummary == null)
            {
                return Json(new
                {
                    Success = false,
                    Message = "The selected Training Plan Summary was not found."
                }, JsonRequestBehavior.AllowGet);
            }

            // Get assessment IDs related to this summary
            var assessmentIds = db.NeedAssessmentSummaryDetails
                .Where(x => x.AssesmentSummaryId == needAssessmentSummaryId)
                .Select(x => x.AssesmentId)
                .ToList();

            // Find the required course tracking
            var courseTrackings = db.CourseTrackings
                .Where(x => x.CourseTrackingId == needAssessmentSummary.RequiredTraining);

            //if (!courseTrackings.Any())
            //{
            //    return Json(new
            //    {
            //        Success = false,
            //        Message = "The required course was not found."
            //    }, JsonRequestBehavior.AllowGet);
            //}
            var trackid = courseTrackings.Select(x => x.CourseTrackingId);
            //// Don't notify for courses that have already ended
            //if (courseTracking.To < DateTime.Now)
            //{
            //    return Json(new
            //    {
            //        Success = false,
            //        Message = "The selected course has already ended."
            //    }, JsonRequestBehavior.AllowGet);
            //}

            // Employees already assigned as trainees
            var traineeEmployeeIds = db.Trainee
                .Where(x => trackid.Contains( x.CourseTrackingId))
                .Select(x => x.EmployeeId)
                .Distinct()
                .ToList();

            // Employees coming from training need requests
            var requestedEmployeeIds = db.TrainingNeedRequests
                .Where(x => assessmentIds.Contains(x.AssesmentId))
                .Select(x => x.EmployeeId)
                .Distinct()
                .ToList();

            // Combine both lists
            var employeeIds = traineeEmployeeIds
                .Union(requestedEmployeeIds)
                .Distinct()
                .ToList();

            if (!employeeIds.Any())
            {
                return Json(new
                {
                    Success = false,
                    Message = "No employees are assigned to this course. Please add trainees first."
                }, JsonRequestBehavior.AllowGet);
            }

            int successCount = 0;

            foreach (var empId in employeeIds)
            {
                var userId = UserInformationHelper.GetEmployeeUserId(empId);

                if (userId == null)
                    continue;

                var fullName = UserInformationHelper.GetEmployeeFullNameById(empId);

                var notif = new NotifModels
                {
                    UserId = userId,
                    Title = "📘 Course Assignment Notification",
                    Message = $"Dear {fullName}, you have been assigned a course. Please review and complete it on time.",
                    URL = "/HRM/CourseTracking"
                };

                Notify.NotifyUser(notif);

                successCount++;
            }

            // Update notification information
            needAssessmentSummary.IsNotified = true;
            needAssessmentSummary.NotifiedBy = User.Identity.Name;
            needAssessmentSummary.NotifiedDate = DateTime.Now;

            db.Entry(needAssessmentSummary).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new
            {
                Success = true,
                Message = $"{successCount} employee(s) have been successfully notified."
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported employee placment");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported employee placment");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
