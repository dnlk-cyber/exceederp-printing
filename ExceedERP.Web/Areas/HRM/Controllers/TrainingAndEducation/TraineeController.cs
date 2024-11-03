using System;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.TrainingEducation;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.TrainingAndEducation
{

    [AuthorizeRoles(HrmConstants.HRTrainingUser)]
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

            int traineeCount = 0;
            var tracking = db.CourseTrackings.FirstOrDefault(x => x.CourseTrackingId == id);
            if (tracking != null)
            {
                traineeCount = db.Trainee.Where(x => x.CourseTrackingId == tracking.CourseTrackingId).ToList().Count();

                var tra = db.Trainee.FirstOrDefault(x => x.EmployeeId == trainee.EmployeeId && x.CourseTrackingId == tracking.CourseTrackingId);
                if (tra != null)
                {
                    ModelState.AddModelError("", "You are already Register, Cannot register again");
                }

                if (ModelState.IsValid)
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
                    if (traineeCount >= tracking.NoOfTrainee)
                    {
                        string messages = "Maximum Number of Trainee Registered";
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
                            UserHelper.OperationLog(Request.UserHostAddress, "Trainee ", "Success", User.Identity.Name, name.EmployeeCode + " add new Trainee name " + person.EmployeeCode, Core.Domain.Global.Modules.HRM);

                        }
                    }
                }
                #endregion

            }


            return Json(new[] { trainee }.ToDataSourceResult(request, ModelState));
        }
        private void SaveTraining(Trainee traine)
        {
            string oneAddress = "";

            var courseTracking = db.CourseTrackings.FirstOrDefault(x => x.CourseTrackingId == traine.CourseTrackingId);

            if (courseTracking != null)
            {
                var address = db.Addresses.Where(x => x.Reference == courseTracking.MainCompanyId.ToString() && x.ReferenceType == "TrainingProvider").ToList();

                foreach (var a in address)
                {
                    oneAddress += " " + a.Value;
                }
                if (oneAddress.Length == 0)
                {
                    oneAddress = "N/A";
                }
                string organizationName = "";
                var mainCompany = db.Companies.FirstOrDefault(x => x.MainCompanyId == courseTracking.MainCompanyId);
                if (mainCompany != null)
                {
                    organizationName = mainCompany.TradeName;
                }
                string trainingName = "";
                var lookUp = db.Lookups.FirstOrDefault(x => x.LookupId == courseTracking.LookupId);
                if (lookUp != null)
                {
                    trainingName = lookUp.Description;
                }

                var entityy = new Training
                {
                    OrganizationName = organizationName,
                    EmployeeId = traine.EmployeeId,
                    OrgAddress = oneAddress,
                    IsTraining = true,
                    From = courseTracking.From,
                    To = courseTracking.To,
                    Score = 0,
                    TrainingName = trainingName,
                    PercentCoverdByOrg = 0,
                    Remark = courseTracking.Remark,
                    DateCreated = DateTime.Now,
                    LastModified = null,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = null,
                    FromTrAm = courseTracking.FromAmharic,
                    ToTraAm = courseTracking.ToAmharic,
                    EmpEduTrainId = 0,
                    CourseTrackingId = courseTracking.CourseTrackingId,
                };

                int noOfTrainee = db.Trainee.Where(x => x.CourseTrackingId == traine.CourseTrackingId).Count();

                if (noOfTrainee > 0)
                {
                    entityy.TotalCost = Convert.ToDouble(courseTracking.Cost / noOfTrainee);
                }
                else
                {
                    entityy.TotalCost = 0;
                }
              
                db.EmpTrainingEducations.Add(entityy);
                db.SaveChanges();
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

                if (trainee.IsCompleteTraining)
                {
                    var trainingInProfile = db.EmpTrainingEducations.FirstOrDefault(x=>x.CourseTrackingId == trainee.CourseTrackingId);
                    if(trainingInProfile == null)
                    {
                        SaveTraining(trainee);
                    }                   
                }

                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                    var name = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId);
                    if (name != null)
                    {
                        UserHelper.OperationLog(Request.UserHostAddress, "Trainee ", "Success", User.Identity.Name, name.EmployeeCode + " update Trainee info of " + person.EmployeeCode, Core.Domain.Global.Modules.HRM);

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
                        {
                            oneAddress = "N/A";
                        }

                        string organizationName = "";
                        var mainCompany = db.Companies.FirstOrDefault(x => x.MainCompanyId == courseTracking.MainCompanyId);
                        if (mainCompany != null)
                        {
                            organizationName = mainCompany.TradeName;
                        }
                        string trainingName = "";
                        var lookUp = db.Lookups.FirstOrDefault(x => x.LookupId == courseTracking.LookupId);
                        if (lookUp != null)
                        {
                            trainingName = lookUp.Description;
                        }
                        var entityy = new Training
                        {
                            OrganizationName = organizationName,
                            EmployeeId = trainee.EmployeeId,
                            OrgAddress = oneAddress,
                            IsTraining = true,
                            From = courseTracking.From,
                            To = courseTracking.To,
                            Score = 0,
                            TotalCost = Convert.ToDouble(courseTracking.Cost),
                            TrainingName = trainingName,
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
