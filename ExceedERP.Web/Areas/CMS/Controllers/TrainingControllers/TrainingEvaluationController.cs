using ExceedERP.Core.Domain.CMS;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CMS.Controllers.TrainingControllers
{
    public class TrainingEvaluationController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: CMS/TrainingEvaluation
        public ActionResult Index()
        {
            ViewData["Programs"] = db.CMSPrograms.Select(p => new { p.CMSProgramId, p.Name }).ToList();
            ViewData["Courses"] = db.CMSCourses.Select(p => new { p.CMSCourseId, p.Name }).ToList();
            ViewData["Cities"] = db.Cities.Select(c => new { c.Name, c.CityId }).ToList();

            return View();
        }

        public ActionResult Trainees_Read([DataSourceRequest]DataSourceRequest request, TraineeType traineeType)
        {
            IQueryable<CmsTrainee> trainees = db.CmsTrainees.Where(t => t.Type == traineeType && t.Status == TraineeStatusConstants.OnExamination);
            DataSourceResult result = trainees.ToDataSourceResult(request, trainee => new CmsTrainee
            {
                CmsTraineeId = trainee.CmsTraineeId,
                Type = trainee.Type,
                RegistrationDate = trainee.RegistrationDate,
                RegistrationNumber = trainee.RegistrationNumber,
                CMSProgramId = trainee.CMSProgramId,
                CMSCourseId = trainee.CMSCourseId,
                FirstName = trainee.FirstName,
                LastName = trainee.LastName,
                MiddleName = trainee.MiddleName,
                FullName = trainee.FullName,
                Nationality = trainee.Nationality,
                CompanyName = trainee.CompanyName,
                Region = trainee.Region,
                Address = trainee.Address,
                CityId = trainee.CityId,
                DateOfBirth = trainee.DateOfBirth,
                PhoneNumber = trainee.PhoneNumber,
                MobilePhoneNumber = trainee.MobilePhoneNumber,
                Email = trainee.Email,
                PreferedDate = trainee.PreferedDate,
                TrainingVenue = trainee.TrainingVenue,
                ProfilePicFileName = string.IsNullOrEmpty(trainee.ProfilePicFileName) ?
                "user-avatar.png" : trainee.ProfilePicFileName,
                BarcodeImageName = $"barcode-{trainee.CmsTraineeId}.png",
                Status = trainee.Status,

                DateCreated = trainee.DateCreated,
                LastModified = trainee.LastModified,
                CreatedBy = trainee.CreatedBy,
                ModifiedBy = trainee.ModifiedBy
            });

            return Json(result);
        }
        public JsonResult SetAsPassedExamination(int id)
        {
            var record = db.CmsTrainees.Find(id);
            if ((record != null && record.Status == TraineeStatusConstants.OnExamination))
            {
                #region validate line items
                var lineItems = db.TraineePrograms.Where(li => li.CmsTraineeId == record.CmsTraineeId).ToList();
                if (!lineItems.Any())
                {
                    var d = new
                    {
                        Message = "No Training Program Assigned for this Trainee. Please add Programs first",
                        Status = "NOT_OK"
                    };
                    return Json(d, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    foreach (var program in lineItems)
                    {
                        if (program.TrainingStatus != TrainingProgramStatus.Completed)
                        {
                            var d = new
                            {
                                Message = "All Programs have to be Copleted before setting this Training as 'Paassed Examination'!",
                                Status = "NOT_OK"
                            };
                            return Json(d, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                #endregion

                record.Status = TraineeStatusConstants.PassedExamination;
                //record.IsSendForApproval = true;
                //record.SendForApprovalBy = User.Identity.Name;
                //record.SendForApprovalTime = DateTime.Now;
                db.CmsTrainees.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { Message = "Successfully set as 'Passed Examination'.", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var d = new
                {
                    Message = "Trainee record not found!",
                    Status = "OK"
                };
                return Json(d, JsonRequestBehavior.AllowGet);
            }
            if (record.Status != TraineeStatusConstants.OnExamination)
            {
                var formattedData = new
                {
                    Message = "This Trainee  is not in examination.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record.Status == TraineeStatusConstants.PassedExamination)
            {
                var formattedData = new
                {
                    Message = "This Trainee  is already set as 'Passed Examination'.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

           
            return null;
        }
        public JsonResult SetAsFailedExamination(int id)
        {
            var record = db.CmsTrainees.Find(id);
            if ((record != null && record.Status == TraineeStatusConstants.OnExamination))
            {
                #region validate line items
                var lineItems = db.TraineePrograms.Where(li => li.CmsTraineeId == record.CmsTraineeId).ToList();
                if (!lineItems.Any())
                {
                    var d = new
                    {
                        Message = "No Training Program Assigned for this Trainee. Please add Programs first",
                        Status = "NOT_OK"
                    };
                    return Json(d, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    
                }
                #endregion

                record.Status = TraineeStatusConstants.FailedExamination;
                //record.IsSendForApproval = true;
                //record.SendForApprovalBy = User.Identity.Name;
                //record.SendForApprovalTime = DateTime.Now;
                db.CmsTrainees.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { Message = "Successfully set as 'Failed Examination'.", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var d = new
                {
                    Message = "Trainee record not found!",
                    Status = "OK"
                };
                return Json(d, JsonRequestBehavior.AllowGet);
            }
            if (record.Status != TraineeStatusConstants.OnExamination)
            {
                var formattedData = new
                {
                    Message = "This Trainee  is not in examination.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record.Status == TraineeStatusConstants.FailedExamination)
            {
                var formattedData = new
                {
                    Message = "This Trainee  is already set as 'Failed Examination'.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            return null;
        }
    }
}