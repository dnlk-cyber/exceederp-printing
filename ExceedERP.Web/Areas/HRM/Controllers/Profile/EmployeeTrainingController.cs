using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.Common.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.TrainingEducation;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{

    [AuthorizeRoles(HrmConstants.HRProfileUser, HrmConstants.HRProfileAdmin, HrmConstants.HRAdmin)]
    public class EmployeeTrainingController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public static string FromAmharic { get; set; }
        public static string ToAmharic { get; set; }
        public static DateTime From { get; set; }
        public static DateTime To { get; set; }
        // GET: HRM/EmployeeEducation
        public ActionResult Index()
        {

            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            HRMInputs inputs = new HRMInputs();
            ViewData["EmployeeList"] = inputs.GetEmployees(User.Identity.Name);
        }
        public ActionResult DownloadFile(string filename)
        {
            if (HRMInputs.SelectedFile == 0)
            {
                string messages = "Please select the row first!!";
                return new JavaScriptResult() { Script = "alert('" + messages + "');" };
            }
            db = new ExceedDbContext();
            try
            {
                var attachment =
                       db.Attachments.Where(x => x.Reference == filename && x.ReferenceType == AttachmentType.TRAINING && x.Remark == HRMInputs.Owner && x.AttachmentId == HRMInputs.SelectedFile)
                           .FirstOrDefault();
                if (attachment != null)
                {
                    WebClient req = new WebClient();
                    HttpResponse response = System.Web.HttpContext.Current.Response;
                    response.Clear();
                    response.ClearContent();
                    response.ClearHeaders();
                    response.Buffer = true;
                    response.AddHeader("Content-Disposition",
                        "attachment;filename=\"" + attachment.Description + "\"");
                    byte[] datas = req.DownloadData(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.TRAINING + "/" + attachment.Description));
                    response.BinaryWrite(datas);
                    response.End();
                    return File(filename, attachment.FileType);
                }
            }
            catch (System.Exception)
            {

                //throw;
            }
            return null;
        }
        public void SetDate(string name, string value)
        {
            if (name == "FromTrAm")
            {
                FromAmharic = value;
                var splitDate = FromAmharic.Split('/');
                From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else if (name == "ToTraAm")
            {
                ToAmharic = value;
                var splitDate = ToAmharic.Split('/');
                To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }

        }

        [AuthorizeRoles(HrmConstants.HRProfileUser, HrmConstants.HRProfileAdmin, HrmConstants.HRAdmin)]
        public ActionResult EmpTrainingEducations_Read([DataSourceRequest]DataSourceRequest request, int idTr)
        {
            IQueryable<Training> emptrainingeducations = db.EmpTrainingEducations.Where(t => t.IsTraining == true && t.EmployeeId == idTr);
            DataSourceResult result = emptrainingeducations.ToDataSourceResult(request, empTrainingEducation => new
            {
                EmpEduTrainId = empTrainingEducation.EmpEduTrainId,
                EmployeeId = empTrainingEducation.EmployeeId,
                OrganizationName = empTrainingEducation.OrganizationName,
                OrgAddress = empTrainingEducation.OrgAddress,
                IsTraining = true,
                From = empTrainingEducation.From,
                TrainingName = empTrainingEducation.TrainingName,
                To = empTrainingEducation.To,
                Score = empTrainingEducation.Score,
                TotalCost = empTrainingEducation.TotalCost,
                PercentCoverdByOrg = empTrainingEducation.PercentCoverdByOrg,
                Remark = empTrainingEducation.Remark,
                DateCreated = empTrainingEducation.DateCreated,
                LastModified = empTrainingEducation.LastModified,
                CreatedBy = empTrainingEducation.CreatedBy,
                ModifiedBy = empTrainingEducation.ModifiedBy,
                FromTrAm = empTrainingEducation.FromTrAm,
                ToTraAm = empTrainingEducation.ToTraAm
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRProfileUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmpTrainingEducations_Create([DataSourceRequest]DataSourceRequest request, Training empTrainingEducation, int idTrTwo)
        {
            ModelState.Clear();

            if (empTrainingEducation.ToTraAm != null)
            {
                var splitDate = empTrainingEducation.ToTraAm.Split('/');
                empTrainingEducation.To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

            }
            else
            {
                ModelState.AddModelError("", "Please Enter End Date");
            }
            if (empTrainingEducation.FromTrAm != null)
            {
                var splitDate = empTrainingEducation.FromTrAm.Split('/');
                empTrainingEducation.From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

            }

            if (empTrainingEducation.From > DateTime.Now.Date)
            {

                ModelState.AddModelError("", "Start date should be less or equal to the current date!!");
            }
            else if (empTrainingEducation.From > empTrainingEducation.To)
            {

                ModelState.AddModelError("", "Start date should be less or equal to end date!!");
            }


            if (ModelState.IsValid)
            {
                var entity = new Training
                {
                    OrganizationName = empTrainingEducation.OrganizationName,
                    EmployeeId = idTrTwo,
                    OrgAddress = empTrainingEducation.OrgAddress,
                    IsTraining = true,
                    From = empTrainingEducation.From,
                    To = empTrainingEducation.To,
                    Score = empTrainingEducation.Score,
                    TotalCost = empTrainingEducation.TotalCost,
                    TrainingName = empTrainingEducation.TrainingName,
                    PercentCoverdByOrg = empTrainingEducation.PercentCoverdByOrg,
                    Remark = empTrainingEducation.Remark,
                    DateCreated = DateTime.Now,
                    LastModified = empTrainingEducation.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = empTrainingEducation.ModifiedBy,
                    FromTrAm = empTrainingEducation.FromTrAm,
                    ToTraAm = empTrainingEducation.ToTraAm
                };

               
                db.EmpTrainingEducations.Add(entity);
                db.SaveChanges();
                empTrainingEducation.EmpEduTrainId = entity.EmpEduTrainId;

                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Employee Training", "Success", User.Identity.Name, "Employee Training for " + person.EmployeeCode + " is Created By  " + User.Identity.Name, Modules.HRM);

                }
                #endregion

            }

            return Json(new[] { empTrainingEducation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRProfileUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmpTrainingEducations_Update([DataSourceRequest]DataSourceRequest request, Training empTrainingEducation)
        {
            ModelState.Clear();

            if (empTrainingEducation.ToTraAm != null)
            {
                var splitDate = empTrainingEducation.ToTraAm.Split('/');
                empTrainingEducation.To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

            }
            else
            {
                ModelState.AddModelError("", "Please Enter End Date");
            }
            if (empTrainingEducation.FromTrAm != null)
            {
                var splitDate = empTrainingEducation.FromTrAm.Split('/');
                empTrainingEducation.From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

            }

            if (empTrainingEducation.From > DateTime.Now.Date)
            {

                ModelState.AddModelError("", "Start date should be less or equal to the current date!!");
            }
            else if (empTrainingEducation.From > empTrainingEducation.To)
            {

                ModelState.AddModelError("", "Start date should be less or equal to end date!!");
            }


            if (ModelState.IsValid)
            {

                var entity = new Training
                {
                    EmpEduTrainId = empTrainingEducation.EmpEduTrainId,
                    EmployeeId = empTrainingEducation.EmployeeId,
                    OrganizationName = empTrainingEducation.OrganizationName,
                    OrgAddress = empTrainingEducation.OrgAddress,
                    IsTraining = true,
                    From = empTrainingEducation.From,
                    To = empTrainingEducation.To,
                    TrainingName = empTrainingEducation.TrainingName,
                    Score = empTrainingEducation.Score,
                    TotalCost = empTrainingEducation.TotalCost,
                    PercentCoverdByOrg = empTrainingEducation.PercentCoverdByOrg,
                    Remark = empTrainingEducation.Remark,
                    DateCreated = empTrainingEducation.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = empTrainingEducation.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    FromTrAm = empTrainingEducation.FromTrAm,
                    ToTraAm = empTrainingEducation.ToTraAm
                };

                db.EmpTrainingEducations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Employee Training", "Success", User.Identity.Name, "Training Info of " + person.EmployeeCode + " is Updated By  " + User.Identity.Name, Modules.HRM);

                }

                #endregion

            }

            return Json(new[] { empTrainingEducation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmpTrainingEducations_Destroy([DataSourceRequest]DataSourceRequest request, Training empTrainingEducation)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Training
                {
                    EmpEduTrainId = empTrainingEducation.EmpEduTrainId,
                    OrganizationName = empTrainingEducation.OrganizationName,
                    EmployeeId = empTrainingEducation.EmployeeId,
                    OrgAddress = empTrainingEducation.OrgAddress,
                    IsTraining = true,
                    From = empTrainingEducation.From,
                    To = empTrainingEducation.To,
                    Score = empTrainingEducation.Score,
                    TrainingName = empTrainingEducation.TrainingName,
                    TotalCost = empTrainingEducation.TotalCost,
                    PercentCoverdByOrg = empTrainingEducation.PercentCoverdByOrg,
                    Remark = empTrainingEducation.Remark,
                    DateCreated = empTrainingEducation.DateCreated,
                    LastModified = empTrainingEducation.LastModified,
                    CreatedBy = empTrainingEducation.CreatedBy,
                    ModifiedBy = empTrainingEducation.ModifiedBy,
                    FromTrAm = empTrainingEducation.FromTrAm,
                    ToTraAm = empTrainingEducation.ToTraAm
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee information of employee Id" + empTrainingEducation.EmployeeId);
                #endregion
                db.EmpTrainingEducations.Attach(entity);
                db.EmpTrainingEducations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { empTrainingEducation }.ToDataSourceResult(request, ModelState));
        }



        public ActionResult Upload(HttpPostedFileBase file)
        {
            try
            {

                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.TRAINING.ToString()), fileName);
                var mainPath =
                    Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.TRAINING.ToString()));
                if (!Directory.Exists(mainPath))
                {
                    Directory.CreateDirectory(mainPath);
                }
                file.SaveAs(path);

                var entity = new Attachment
                {

                    Reference = HRMInputs.EmployeeId.ToString(),
                    Description = file.FileName,
                    FileType = file.ContentType,
                    ReferenceType = AttachmentType.TRAINING,
                    Index = 0,
                    URL = path,
                    Remark = HRMInputs.Owner

                };
                db.Attachments.Add(entity);

                db.SaveChanges();
                ViewBag.Message = "Upload successful";


            }
            catch
            {
                ViewBag.Message = "Upload failed";
                return RedirectToAction("Upload");
            }
            return RedirectToAction("Index");
        }

        public void Download(int id)
        {
            HRMInputs.SelectedFile = id;

        }
        public ActionResult Attachments_Destroy([DataSourceRequest]DataSourceRequest request, Attachment attachment)
        {
            if (ModelState.IsValid)
            {
                var entity = new Attachment
                {
                    AttachmentId = attachment.AttachmentId,
                    ReferenceType = attachment.ReferenceType,
                    Reference = attachment.Reference,
                    Description = attachment.Description,
                    URL = attachment.URL,
                    BinaryFile = attachment.BinaryFile,
                    FileType = attachment.FileType,
                    Index = attachment.Index,
                    Remark = attachment.Remark
                };

                db.Attachments.Attach(entity);
                db.Attachments.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
