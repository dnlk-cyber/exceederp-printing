using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.TrainingEducation;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Helpers.HR;
using ExceedERP.Web.Hubs;

namespace ExceedERP.Web.Areas.HRM.Controllers.TrainingAndEducation
{

    [AuthorizeRoles(HrmConstants.HRTrainingUser)]
    public class CourseTrackingController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();

    
        public ActionResult Index()
        {
            PopulateViewData();
          

            return View();
        }
        private void PopulateViewData()
        {
            ViewData["PeriodList"] = CacheData.GetFiscalYearCaches().Select(
                             i => new
                             {
                                 Name = i.Name,
                                 GlFiscalYearId = i.Id,
                             }).AsQueryable();
            ViewData["EmployeeList"] = CacheData.GetEmployeeCaches().Select(
          i => new
          {
              FullNameAmharic = i.AmharicName,
              FullNameaEnglish = i.Name,
              FullName = i.Name,
              FullNameCode = i.Code + " " + i.Name,
              EmployeeCode = i.Code,
              EmployeeId = i.Id,
              PersonId = i.Id,
          }).AsQueryable();
            ViewData["Branches"] = CacheData.GetBranchCaches().Select(
             i => new
             {
                 Name = i.Name,
                 BranchId = i.Id,
             }).AsQueryable(); ;

            ViewData["businessUnits"] = CacheData.GetBusinessUnitCaches().Select(
               i => new
               {
                   Name = i.Name,
                   BusinessUnitId = i.Id,
               }).AsQueryable();
            ViewData["Departments"] = CacheData.GetWorkUnitCaches().Select(
               i => new
               {
                   Name = i.Name,
                   OrgStructureId = i.Id,
               }).AsQueryable();
            ViewData["CourseList"] = db.Lookups.Where(l => l.LookupType == LookUpTypes.COURSE && l.IsActive).AsQueryable();
            //var branch = db.Branch.ToList();
            //ViewData["Branches"] = branch;
            var plans = db.TrainingPlans.ToList();
            ViewData["plan"] = plans;
            ViewData["ProviderList"] = db.Companies.AsQueryable();
            var lookups = db.Lookups.Where(l => l.LookupType == LookUpTypes.EDUCATION_LEVEL);
            ViewData["EducationLevel"] = lookups;
            ViewData["Criterias"] = db.TrainingEvaluationCriterias.Where(x => !x.TrainingCourseId.HasValue).ToList();
            ViewData["TraTypeList"] = db.Lookups.Where(l => l.LookupType == LookUpTypes.COURSE && l.IsActive).AsQueryable();
            ViewData["isEthiopicDate"] = HrHelper.GetDatePreference();

        }

        public void SetDate(string name, string value)
        {
            if (name == "FromAmharic")
            {
         
                var From = HrHelper.ParseEthiopicDate(value);
            }
            else if (name == "ToAmharic")
            {
                var To = HrHelper.ParseEthiopicDate(value);
            }
        }
        public JsonResult GetCourse()
        {
            var courseList = db.Lookups.AsQueryable();
            if (courseList.Count() > 0)
            {
                courseList = courseList.Where(l => l.LookupType == LookUpTypes.COURSE && l.IsActive).AsQueryable();
            }
            ViewData["CourseList"] = courseList != null ? courseList.ToList() : new List<Lookup>();
            return Json(courseList.Select(p => new { LookupId = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployee()
        {
            HRMInputs inputs = new HRMInputs();

            var list = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            return Json(list.Select(p => new { PersonId = p.PersonId, FullName = p.FullName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProvider()
        {
            var providerlist = db.Companies.AsQueryable();
            if (providerlist.Count() > 0)
            {
                providerlist = providerlist.Where(c => c.Category == "1" && c.IsActive).AsQueryable();
            }
            ViewData["ProviderList"] = providerlist != null ? providerlist.ToList() : new List<MainCompany>();
            return Json(providerlist.Select(p => new { MainCompanyId = p.MainCompanyId, TradeName = p.TradeName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPeriod()
        {
            var periodList = db.GLFiscalYears;

            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();
            return Json(periodList.Select(p => new { GlFiscalYearId = p.GlFiscalYearId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult TrainingType()
        {
            var traTypeList = db.Lookups.AsQueryable().ToList();
            if (traTypeList.Any())
            {
                traTypeList = traTypeList.Where(l => l.LookupType == LookUpTypes.TRAINING_TYPE).OrderBy(o => o.LookupType).ToList();

            }
            ViewData["TraTypeList"] = traTypeList.Count > 0 ? traTypeList.ToList() : new List<Lookup>();
            return Json(traTypeList.Select(p => new { LookupId = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);
        }

        [AuthorizeRoles(HrmConstants.HRTrainingUser, HrmConstants.HRAdmin)]
        public ActionResult CourseTrackings_Read([DataSourceRequest]DataSourceRequest request)
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            IQueryable<CourseTracking> coursetrackings = db.CourseTrackings.AsQueryable();//.Where(x => mybraches.Contains(x.OrgBranchName));
            DataSourceResult result = coursetrackings.ToDataSourceResult(request, courseTracking => new
            {
                CourseTrackingId = courseTracking.CourseTrackingId,
                GlFiscalYearId = courseTracking.GlFiscalYearId,
                NoOfTrainee = courseTracking.NoOfTrainee,
                TrainingPlanId = courseTracking.TrainingPlanId,
                LookupId = courseTracking.LookupId,
                From = courseTracking.From,
                To = courseTracking.To,
                Cost = courseTracking.Cost,
                TrainingType = courseTracking.TrainingType,
                MainCompanyId = courseTracking.MainCompanyId,
                Remark = courseTracking.Remark,
                FromAmharic = courseTracking.FromAmharic,
                ToAmharic = courseTracking.ToAmharic,
                DateCreated = courseTracking.DateCreated,
                LastModified = courseTracking.LastModified,
                CreatedBy = courseTracking.CreatedBy,
                ModifiedBy = courseTracking.ModifiedBy,
                OrgBranchName = courseTracking.OrgBranchName,
                TrainingLocation = courseTracking.TrainingLocation,
                TrainingTerm = courseTracking.TrainingTerm,
                IsOnJob = courseTracking.IsOnJob,
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRTrainingUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CourseTrackings_Create([DataSourceRequest]DataSourceRequest request, CourseTracking courseTracking)
        {
            ModelState.Clear();

            var noOfTrainee = db.TrainingPlans.Where(x => x.Course == courseTracking.LookupId &&
                          x.GlFiscalYearId == courseTracking.GlFiscalYearId);
            int no = 0;
            if (noOfTrainee.Any())
            {
                no = noOfTrainee.Sum(x => x.NoofTrainee);
            }
            if (no == 0)
            {
                no = courseTracking.NoOfTrainee;
            }
            bool useEthiopian = HrHelper.GetDatePreference();
            if (useEthiopian)
            {
                // User inputs Ethiopian dates → must validate
                if (string.IsNullOrWhiteSpace(courseTracking.FromAmharic) ||
                    string.IsNullOrWhiteSpace(courseTracking.ToAmharic))
                {
                    ModelState.AddModelError("", "Both From and To Date fields are mandatory.");
                    return Json(new[] { courseTracking }.ToDataSourceResult(request, ModelState));
                }
                // Convert Ethiopic → Gregorian
                courseTracking.From = HrHelper.ParseEthiopicDate(courseTracking.FromAmharic);
                courseTracking.To = HrHelper.ParseEthiopicDate(courseTracking.ToAmharic);
            }
            else
            {
                // User inputs Gregorian dates → must validate
                if (courseTracking.From == default(DateTime) || courseTracking.To == default(DateTime))
                {
                    ModelState.AddModelError("", "Both From and To Date fields are mandatory.");
                    return Json(new[] { courseTracking }.ToDataSourceResult(request, ModelState));
                }
                // Convert Gregorian → Ethiopic for storage
                courseTracking.FromAmharic = HrHelper.GetECDate(courseTracking.From);
                courseTracking.ToAmharic = HrHelper.GetECDate(courseTracking.To);

            }
            if (courseTracking.From.Date > courseTracking.To.Date)
            {
                ModelState.AddModelError("", "Please check the date range.");
                return Json(new[] { courseTracking }.ToDataSourceResult(request, ModelState));

            }

            if (courseTracking.TrainingType == Core.Domain.Common.TrainingType.EDUCATION)
            {
                courseTracking.TrainingTerm = TrainingTerm.LONG;
            }
            else
            {
                courseTracking.TrainingTerm = TrainingTerm.SHORT;
            }
            if (ModelState.IsValid)
            {
                var entity = new CourseTracking
                {
                    GlFiscalYearId = courseTracking.GlFiscalYearId,
                    TrainingPlanId = courseTracking.TrainingPlanId,
                    NoOfTrainee = no,
                    LookupId = courseTracking.LookupId,
                    From = courseTracking.From,
                    To = courseTracking.To,
                    Cost = courseTracking.Cost,
                    TrainingType = courseTracking.TrainingType,
                    MainCompanyId = courseTracking.MainCompanyId,
                    Remark = courseTracking.Remark,
                    FromAmharic = courseTracking.FromAmharic,
                    ToAmharic = courseTracking.ToAmharic,
                    DateCreated = DateTime.Now,
                    LastModified = courseTracking.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = courseTracking.ModifiedBy,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    TrainingLocation = courseTracking.TrainingLocation,
                    TrainingTerm = courseTracking.TrainingTerm,
                    IsOnJob = courseTracking.IsOnJob,
                };

                //var trainee = db.Trainee.Where(x => x.CourseTrackingId == entity.CourseTrackingId).ToList();
                //foreach (var trainee1 in trainee)
                //{
                //    decimal cost=trainee1.
                //}

                db.CourseTrackings.Add(entity);
                db.SaveChanges();
                courseTracking.CourseTrackingId = entity.CourseTrackingId;

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Course Tracking ", "Success", User.Identity.Name, User.Identity.Name + " add new Course Tracking adding date " + DateTime.Today.ToShortDateString(), Modules.HRM);

                #endregion

            }

            return Json(new[] { courseTracking }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRTrainingUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CourseTrackings_Update([DataSourceRequest]DataSourceRequest request, CourseTracking courseTracking)
        {
            ModelState.Clear();

            bool useEthiopian = HrHelper.GetDatePreference();
            if (useEthiopian)
            {
                // User inputs Ethiopian dates → must validate
                if (string.IsNullOrWhiteSpace(courseTracking.FromAmharic) ||
                    string.IsNullOrWhiteSpace(courseTracking.ToAmharic))
                {
                    ModelState.AddModelError("", "Both From and To Date fields are mandatory.");
                    return Json(new[] { courseTracking }.ToDataSourceResult(request, ModelState));
                }
                // Convert Ethiopic → Gregorian
                courseTracking.From = HrHelper.ParseEthiopicDate(courseTracking.FromAmharic);
                courseTracking.To = HrHelper.ParseEthiopicDate(courseTracking.ToAmharic);
            }
            else
            {
                // User inputs Gregorian dates → must validate
                if (courseTracking.From == default(DateTime) || courseTracking.To == default(DateTime))
                {
                    ModelState.AddModelError("", "Both From and To Date fields are mandatory.");
                    return Json(new[] { courseTracking }.ToDataSourceResult(request, ModelState));
                }
                // Convert Gregorian → Ethiopic for storage
                courseTracking.FromAmharic = HrHelper.GetECDate(courseTracking.From);
                courseTracking.ToAmharic = HrHelper.GetECDate(courseTracking.To);

            }
            if (courseTracking.From.Date > courseTracking.To.Date)
            {
                ModelState.AddModelError("", "Please check the date range.");
                return Json(new[] { courseTracking }.ToDataSourceResult(request, ModelState));

            }
          
            if (ModelState.IsValid)
            {
                if (courseTracking.TrainingType == Core.Domain.Common.TrainingType.EDUCATION)
                {
                    courseTracking.TrainingTerm = TrainingTerm.LONG;
                }
                else
                {
                    courseTracking.TrainingTerm = TrainingTerm.SHORT;
                }
                var entity = new CourseTracking
                {
                    CourseTrackingId = courseTracking.CourseTrackingId,
                    GlFiscalYearId = courseTracking.GlFiscalYearId,
                    NoOfTrainee = courseTracking.NoOfTrainee,
                    TrainingPlanId = courseTracking.TrainingPlanId,
                    LookupId = courseTracking.LookupId,
                    From = courseTracking.From,
                    To = courseTracking.To,
                    Cost = courseTracking.Cost,
                    TrainingType = courseTracking.TrainingType,
                    MainCompanyId = courseTracking.MainCompanyId,
                    Remark = courseTracking.Remark,
                    FromAmharic = courseTracking.FromAmharic,
                    DateCreated = courseTracking.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = courseTracking.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    OrgBranchName = courseTracking.OrgBranchName,
                    ToAmharic = courseTracking.ToAmharic,
                    TrainingLocation = courseTracking.TrainingLocation,
                    TrainingTerm = courseTracking.TrainingTerm,
                    IsOnJob = courseTracking.IsOnJob,

                };

                db.CourseTrackings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Course Tracking ", "Success", User.Identity.Name, User.Identity.Name + " update Course Tracking updated date " + DateTime.Today.ToShortDateString(), Modules.HRM);

                #endregion
            }


            return Json(new[] { courseTracking }.ToDataSourceResult(request, ModelState));
        }
        string ErrorMessage = "";
        private bool CheckCourseDetailForDelete(int id)
        {
            var trainee = db.Trainee.Where(x => x.CourseTrackingId == id).FirstOrDefault();
            if (trainee != null)
            {
                ErrorMessage = "You cannot delete course traking infomation. please delete Trainee information First!!";
                return false;
            }
            //var trainingoutc= db.trainin
            return true;
        }

        [AuthorizeRoles(HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CourseTrackings_Destroy([DataSourceRequest]DataSourceRequest request, CourseTracking courseTracking)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (!CheckCourseDetailForDelete(courseTracking.CourseTrackingId))
                { ModelState.AddModelError("", ErrorMessage); }
                else
                {
                    var entity = new CourseTracking
                    {
                        CourseTrackingId = courseTracking.CourseTrackingId,
                        GlFiscalYearId = courseTracking.GlFiscalYearId,
                        TrainingPlanId = courseTracking.TrainingPlanId,
                        NoOfTrainee = courseTracking.NoOfTrainee,
                        LookupId = courseTracking.LookupId,
                        From = courseTracking.From,
                        To = courseTracking.To,
                        Cost = courseTracking.Cost,
                        TrainingType = courseTracking.TrainingType,
                        MainCompanyId = courseTracking.MainCompanyId,
                        Remark = courseTracking.Remark,
                        DateCreated = courseTracking.DateCreated,
                        LastModified = courseTracking.LastModified,
                        CreatedBy = courseTracking.CreatedBy,
                        ModifiedBy = courseTracking.ModifiedBy,
                        OrgBranchName = courseTracking.OrgBranchName,
                        TrainingLocation = courseTracking.TrainingLocation,
                        TrainingTerm = courseTracking.TrainingTerm,
                        IsOnJob = courseTracking.IsOnJob,
                    };
                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted clearance information of training provider " + courseTracking.MainCompanyId);
                    #endregion
                    db.CourseTrackings.Attach(entity);
                    db.CourseTrackings.Remove(entity);
                    db.SaveChanges();
                }
            }

            return Json(new[] { courseTracking }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult SendCourseTrackingNotifications(int courseTrackingId)
        {
            var courseTracking = db.CourseTrackings
                .FirstOrDefault(x => x.CourseTrackingId == courseTrackingId);

            if (courseTracking == null)
            {
                return Json(new
                {
                    Success = false,
                    Message = "The selected course schedule was not found."
                }, JsonRequestBehavior.AllowGet);
            }

            if (courseTracking.To < DateTime.Now)
            {
                return Json(new
                {
                    Success = false,
                    Message = "This course has already expired. Please update the schedule before sending notifications."
                }, JsonRequestBehavior.AllowGet);
            }

            var trainees = db.Trainee
                .Where(x => x.CourseTrackingId == courseTrackingId)
                .Select(x => x.EmployeeId)
                .Distinct()
                .ToList();

            if (!trainees.Any())
            {
                return Json(new
                {
                    Success = false,
                    Message = "No employees are assigned to this course. Please add trainees first."
                }, JsonRequestBehavior.AllowGet);
            }

            int successCount = 0;

            foreach (var empId in trainees)
            {
                var userId = UserInformationHelper.GetEmployeeUserId(empId);
                var fullName = UserInformationHelper.GetEmployeeFullNameById(empId);

                if (userId == null) continue;

                NotifModels notif = new NotifModels
                {
                    UserId = userId,
                    Title = "📘 Course Assignment Notification",
                    Message = $"Dear {fullName}, you have been assigned a course. Please review and complete it on time.",
                    URL = "/HRM/CourseTracking"
                };

                Notify.NotifyUser(notif);
                successCount++;
            }

            return Json(new
            {
                Success = true,
                Message = $"{successCount} employee(s) have been successfully notified."
            }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
