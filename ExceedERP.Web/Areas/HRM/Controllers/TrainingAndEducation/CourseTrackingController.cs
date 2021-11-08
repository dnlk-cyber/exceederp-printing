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

namespace ExceedERP.Web.Areas.HRM.Controllers.TrainingAndEducation
{

    [AuthorizeRoles(HrmConstants.HRTrainingUser, HrmConstants.HRAdmin)]
    public class CourseTrackingController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static DateTime From { get; set; }
        public static DateTime To { get; set; }
        public static string FromAmharic { get; set; }
        public static string ToAmharic { get; set; }
        public ActionResult Index()
        {
            PopulateViewData();
            GetPeriod();
            GetProvider();
            GetCourse();
            TrainingType();
            GetEmployee();

            return View();
        }
        private void PopulateViewData()
        {
            var branch = db.Branch.ToList();
            ViewData["Branches"] = branch;
            var plans = db.TrainingPlans.ToList();
            ViewData["plan"] = plans;
            var lookups = db.Lookups.Where(l => l.LookupType == LookUpTypes.EDUCATION_LEVEL);
            ViewData["EducationLevel"] = lookups;
            ViewData["Criterias"] = db.TrainingEvaluationCriterias.ToList();
        }
        public void SetDate(string name, string value)
        {
            if (name == "FromAmharic")
            {
                FromAmharic = value;
                var splitDate = FromAmharic.Split('/');
                From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else if (name == "ToAmharic")
            {
                ToAmharic = value;
                var splitDate = ToAmharic.Split('/');
                To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
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
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            IQueryable<CourseTracking> coursetrackings = db.CourseTrackings;//.Where(x => mybraches.Contains(x.OrgBranchName));
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
            if (From.Date > To.Date)
            {
                ModelState.AddModelError("", "Please check the date range.");
            }

            if (FromAmharic == null || ToAmharic == null)
            {
                ModelState.AddModelError("", "Date form and to values are mandatory!!");
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
                    From = From,
                    To = To,
                    Cost = courseTracking.Cost,
                    TrainingType = courseTracking.TrainingType,
                    MainCompanyId = courseTracking.MainCompanyId,
                    Remark = courseTracking.Remark,
                    FromAmharic = FromAmharic,
                    ToAmharic = ToAmharic,
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

            //var noOfTrainee = db.TrainingPlans.Where(x => x.LookupId == courseTracking.LookupId)?.ToList().Sum(x => x.NoofTrainee);
            if (courseTracking.FromAmharic != null)
            {
                var splitDate = courseTracking.FromAmharic.Split('/');
                courseTracking.From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else
            {
                ModelState.AddModelError("","please select initial date ");
            }
            if (courseTracking.ToAmharic != null)
            {
                var splitDate = courseTracking.ToAmharic.Split('/');
                courseTracking.To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            if (courseTracking.From.Date > courseTracking.To.Date)
            {
                ModelState.AddModelError("", "Please check the date range.");
            }
            //else if (From.Date > DateTime.Now.Date)
            //{

            //    ModelState.AddModelError("", "Start Date should be less or equal to the current date!!");
            //}
            //else if (To.Date > DateTime.Now.Date)
            //{

            //    ModelState.AddModelError("", "End Date should be less or equal to the current date!!");
            //}
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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
