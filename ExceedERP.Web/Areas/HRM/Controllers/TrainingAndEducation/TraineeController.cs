using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.TrainingEducation;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.TrainingAndEducation
{

    [AuthorizeRoles(HrmConstants.HRTrainingUser, HrmConstants.HRAdmin)]
    public class TraineeController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            HRMInputs inputs = new HRMInputs();
            var list = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            var lookups = db.Lookups.Where(l => l.LookupType == LookUpTypes.EDUCATION_LEVEL);
            ViewData["EducationLevel"] = lookups;
        }

        [AuthorizeRoles(HrmConstants.HRTrainingUser, HrmConstants.HRAdmin)]
        public ActionResult Trainee_Read([DataSourceRequest]DataSourceRequest request, int id)
        {

            IQueryable<Trainee> trainees = db.Trainee.Where(x => x.CourseTrackingId == id);
            DataSourceResult result = trainees.ToDataSourceResult(request, trainee => new
            {
                TraineeId = trainee.TraineeId,
                CourseTrackingId = trainee.CourseTrackingId,
                EducationLevel = trainee.EducationLevel,
                EmployeeId = trainee.EmployeeId,
                IsCompleteTraining = trainee.IsCompleteTraining,
                Remark = trainee.Remark
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRTrainingUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Trainee_Create([DataSourceRequest]DataSourceRequest request, Trainee trainee, int id)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                int num = 0;
                var con = db.CourseTrackings.FirstOrDefault(x => x.CourseTrackingId == id);
                if (con != null)
                {
                    var traineeNum = db.TrainingPlans
                        .FirstOrDefault(x => x.TrainingPlanId == con.TrainingPlanId);
                    num = traineeNum.NoofTrainee;


                    var traineeCount = db.Trainee.Where(x => x.CourseTrackingId == id).ToList().Count();


                    var tra = db.Trainee.FirstOrDefault(x =>
                        x.EmployeeId == trainee.EmployeeId && x.CourseTrackingId == id);
                    if (tra != null)
                    {
                        ModelState.AddModelError("", "You are already Register, Cannot register again");
                    }

                    else
                    {

                        var entity = new Trainee
                        {
                            CourseTrackingId = id,
                            EmployeeId = trainee.EmployeeId,
                            EducationLevel = trainee.EducationLevel,
                            IsCompleteTraining = trainee.IsCompleteTraining,
                            Remark = trainee.Remark
                        };

                        db.Trainee.Add(entity);
                        db.SaveChanges();
                        trainee.TraineeId = entity.TraineeId;
                        trainee.CourseTrackingId = id;
                        if (trainee.IsCompleteTraining)
                        {
                            SaveTraining(trainee);
                        }
                        if (traineeCount >= num)
                        {
                            string messages = "Maximum Number of Trainee Registered You are Out of Plan Amount";
                            return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                        }

                        #region operation log
                        var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                        if (person != null)
                        {
                            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                            var name = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId);
                            if (name != null)
                            {
                                UserHelper.OperationLog(Request.UserHostAddress, "Trainee ", "Success", User.Identity.Name, name.EmployeeCode + " add new Trainee name " + person.EmployeeCode,Core.Domain.Global.Modules.HRM);

                            }
                        }
                        #endregion

                    }

                }
            }
            else
            {
                ModelState.AddModelError("", "No such Plan Cannot Register");
            }


            return Json(new[] { trainee }.ToDataSourceResult(request, ModelState));
        }
        private void SaveTraining(Trainee traine)
        {
            string oneAddress = "";
            ExceedDbContext dbs = new ExceedDbContext();
            var courseTracking =
                dbs.CourseTrackings.Where(x => x.CourseTrackingId == traine.CourseTrackingId).FirstOrDefault();
            if (courseTracking != null)
            {
                dbs = new ExceedDbContext();
                var address =
                    dbs.Addresses.Where(
                        x =>
                            x.Reference == courseTracking.MainCompanyId.ToString() &&
                            x.ReferenceType == "TrainingProvider").ToList();


                foreach (var a in address)
                {
                    oneAddress += " " + a.Value;
                }
                if (oneAddress.Length == 0)
                    oneAddress = "N/A";
                var entityy = new Training
                {
                    OrganizationName = courseTracking.MainCompany.TradeName,
                    EmployeeId = traine.EmployeeId,
                    OrgAddress = oneAddress,
                    IsTraining = true,
                    From = courseTracking.From,
                    To = courseTracking.To,
                    Score = 0,
                    TotalCost = Convert.ToDouble(courseTracking.Cost),
                    TrainingName = courseTracking.Lookup.Description,
                    PercentCoverdByOrg = 0,
                    Remark = courseTracking.Remark,
                    DateCreated = DateTime.Now,
                    LastModified = null,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = null,
                    FromTrAm = courseTracking.FromAmharic,
                    ToTraAm = courseTracking.ToAmharic,
                    EmpEduTrainId = 0,


                };
                //dbs = new ExceedDbContext();
                dbs.EmpTrainingEducations.Add(entityy);
                dbs.SaveChanges();
            }
        }

        [AuthorizeRoles(HrmConstants.HRTrainingUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Trainee_Update([DataSourceRequest]DataSourceRequest request, Trainee trainee)
        {
            if (ModelState.IsValid)
            {
                var entity = new Trainee
                {
                    TraineeId = trainee.TraineeId,
                    CourseTrackingId = trainee.CourseTrackingId,
                    EmployeeId = trainee.EmployeeId,
                    IsCompleteTraining = trainee.IsCompleteTraining,
                    Remark = trainee.Remark
                };

                db.Trainee.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                    var name = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId);
                    if (name != null)
                    {
                        UserHelper.OperationLog(Request.UserHostAddress, "Trainee ", "Success", User.Identity.Name, name.EmployeeCode + " update Trainee info of " + person.EmployeeCode,Core.Domain.Global.Modules.HRM);

                    }
                }
                #endregion

                if (trainee.IsCompleteTraining)
                {
                    db = new ExceedDbContext();
                    var courseTracking =
                        db.CourseTrackings.Where(x => x.CourseTrackingId == trainee.CourseTrackingId).FirstOrDefault();
                    if (courseTracking != null)
                    {
                        var address =
                            db.Addresses.Where(
                                x =>
                                    x.Reference == courseTracking.MainCompanyId.ToString() &&
                                    x.ReferenceType == "Company").ToList();
                        string oneAddress = "";
                        foreach (var a in address)
                        {
                            oneAddress += " " + a.Value;
                        }
                        if (oneAddress.Length == 0)
                            oneAddress = "N/A";
                        var entityy = new Training
                        {
                            OrganizationName = courseTracking.MainCompany.TradeName,
                            EmployeeId = trainee.EmployeeId,
                            OrgAddress = oneAddress,
                            IsTraining = true,
                            From = courseTracking.From,
                            To = courseTracking.To,
                            Score = 0,
                            TotalCost = Convert.ToDouble(courseTracking.Cost),
                            TrainingName = courseTracking.Lookup.Description,
                            PercentCoverdByOrg = 0,
                            Remark = courseTracking.Remark,
                            DateCreated = DateTime.Now,
                            LastModified = null,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = null,
                            FromTrAm = courseTracking.FromAmharic,
                            ToTraAm = courseTracking.ToAmharic
                        };
                        db = new ExceedDbContext();
                        db.EmpTrainingEducations.Add(entityy);
                        db.SaveChanges();
                    }
                }
            }
            return Json(new[] { trainee }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Trainee_Destroy([DataSourceRequest]DataSourceRequest request, Trainee trainee)
        {
            if (ModelState.IsValid)
            {
                var entity = new Trainee
                {
                    TraineeId = trainee.TraineeId,
                    CourseTrackingId = trainee.CourseTrackingId,
                    EmployeeId = trainee.EmployeeId,
                    IsCompleteTraining = trainee.IsCompleteTraining,
                    Remark = trainee.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Trainee information of employee " + trainee.EmployeeId);
                #endregion
                db.Trainee.Attach(entity);
                db.Trainee.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { trainee }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
