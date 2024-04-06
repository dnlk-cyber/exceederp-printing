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
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Helpers.HR;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{

    [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRProfileUser, HrmConstants.HRAdmin)]
    public class EmployeeExperienceController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/EmployeeExperience
        public static int EmployeeId { get; set; }
        public static string FromAmharic { get; set; }
        public static string ToAmharic { get; set; }
        public static DateTime From { get; set; }
        public static DateTime To { get; set; }
        public static DateTime ClearanceDateTime { get; set; }
        public static string CleDateTimeAm { get; set; }
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
        public void SetDate(string name, string value)
        {
            if (name == "FromExAmharic")
            {
                FromAmharic = value;
                var splitDate = FromAmharic.Split('/');
                From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else if (name == "ToExAmharic")
            {
                ToAmharic = value;
                var splitDate = ToAmharic.Split('/');
                To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else if (name == "ClearanceDateTimeAm")
            {
                CleDateTimeAm = value;
                var splitDate = CleDateTimeAm.Split('/');
                ClearanceDateTime = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
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
                       db.Attachments.Where(x => x.Reference == filename && x.ReferenceType == AttachmentType.EXPERIENCE && x.Remark == HRMInputs.Owner && x.AttachmentId == HRMInputs.SelectedFile)
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
                    byte[] datas = req.DownloadData(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.EXPERIENCE + "/" + attachment.Description));
                    response.BinaryWrite(datas);
                    response.End();
                    HRMInputs.SelectedFile = 0;
                    return File(filename, attachment.FileType);
                }
            }
            catch (System.Exception)
            {

                //throw;
            }
            HRMInputs.SelectedFile = 0;
            return null;
        }
        public string GetTotalExp(int empId, int expId)
        {
            var empExp = db.Experiences.FirstOrDefault(x=>x.EmployeeId == empId && x.ExperienceId == expId);
            if(empExp != null)
            {
                return XAPI.Common.getDaystoWords((empExp.To - empExp.From).Days);
            }
            return null;
        }

        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRProfileUser, HrmConstants.HRAdmin)]
        public ActionResult Experiences_Read([DataSourceRequest] DataSourceRequest request, int idEx)
        {
            IQueryable<Experience> experiences = db.Experiences.Where(x => x.EmployeeId == idEx);

            DataSourceResult result = experiences.ToDataSourceResult(request, experience => new
            {
                EmployeeId = experience.EmployeeId,
                ExperienceId = experience.ExperienceId,
                Organozation = experience.Organozation,
                OrgFullAddress = experience.OrgFullAddress,
                EmploymentType = experience.EmploymentType,
                From = experience.From,
                To = experience.To,
                TotalDuration = experience.TotalDuration,
                Responsibility = experience.Responsibility,
                ClearanceDateTime = experience.ClearanceDateTime,
                CleranceReason = experience.CleranceReason,
                IncomeTaxPaymentRefNo = experience.IncomeTaxPaymentRefNo,
                Remark = experience.Remark,
                Salary = experience.Salary,
                EmployeeCode = experience.Employee.EmployeeCode,
                FromExAmharic = experience.FromExAmharic,
                ToExAmharic = experience.ToExAmharic,
                TotalExp = GetTotalExp(experience.EmployeeId, experience.ExperienceId),
                ClearanceDateTimeAm = experience.ClearanceDateTimeAm,
                IsPrivate = experience.IsPrivate,
                //JobTitleId = experience.JobTitleId,

            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRProfileUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Experiences_Create([DataSourceRequest] DataSourceRequest request, Experience experience, int idExTwo)
        {
            ModelState.Clear();
            if (experience.FromExAmharic != null)
            {
                var splitDate = experience.FromExAmharic.Split('/');
                experience.From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

            }
            else
            {
                ModelState.AddModelError("", "Please Enter Start Date");
            }
            if (experience.ToExAmharic != null)
            {
                var splitDate = experience.ToExAmharic.Split('/');
                experience.To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

            }
            else
            {
                ModelState.AddModelError("", "Please Enter End Date");
            }

            if (experience.ClearanceDateTimeAm != null)
            {
                var splitDate = experience.ClearanceDateTimeAm.Split('/');
                experience.ClearanceDateTime = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

            }
            //else
            //{
            //    ModelState.AddModelError("", "Please Enter Clearance Date");
            //}

            if (experience.From > experience.To)
            {
                ModelState.AddModelError("", "Please check the date range, The year of experience should be non negative!!");
            }
            if (experience.From > DateTime.Now)
            {

                ModelState.AddModelError("", "Start date should be less or equal to the current date!!");
            }
            if (experience.To > DateTime.Now)
            {
                ModelState.AddModelError("", "End date should be less or equal to the current date!!");
            }

            var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == idExTwo);
            if (person != null)
            {
                if (experience.From.Date >= person.HiredDate || experience.To.Date >= person.HiredDate)
                {
                    ModelState.AddModelError("", "external experience date should be lessthan hire date");
                }
            }

            var exps = db.Experiences.Where(x => x.EmployeeId == idExTwo);

            if (exps.Any())
            {
                foreach (var exp in exps)
                {
                    if ((experience.From >= exp.From && experience.From <= exp.To) || (experience.To >= exp.From && experience.To <= exp.To))
                    {
                        ModelState.AddModelError("", "experience can't overlap");
                        break;
                    }
                }
            }

            if (ModelState.IsValid)
            {

                int days = (experience.To - experience.From).Days;

                //double month = days / 30;

                decimal year = Convert.ToDecimal(days / 365);

                //var y = (int)month / 12;

                //var ym = month % 12;

                //var ex = y + " አመት ከ " + ym + " ወር";

                experience.TotalExp = EmployeeHelper.GetDaystoWordsAmharic(days);

                var entity = new Experience
                {
                    EmployeeId = idExTwo,
                    Organozation = experience.Organozation,
                    OrgFullAddress = experience.OrgFullAddress,
                    EmploymentType = experience.EmploymentType,
                    From = experience.From,
                    To = experience.To,
                    TotalDuration = year,
                    TotalExp = experience.TotalExp,
                    Responsibility = experience.Responsibility,
                    ClearanceDateTime = experience.ClearanceDateTime,
                    CleranceReason = experience.CleranceReason,
                    IncomeTaxPaymentRefNo = experience.IncomeTaxPaymentRefNo,
                    Remark = experience.Remark,
                    Salary = experience.Salary,
                    EmployeeCode = experience.EmployeeCode,
                    FromExAmharic = experience.FromExAmharic,
                    ToExAmharic = experience.ToExAmharic,
                    ClearanceDateTimeAm = experience.ClearanceDateTimeAm,
                    IsPrivate = experience.IsPrivate,
                    //JobTitleId = experience.JobTitleId,
                };

                db.Experiences.Add(entity);
                db.SaveChanges();
                experience.ExperienceId = entity.ExperienceId;

                #region operation log
                var person1 = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person1 != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Employee Experience", "Success", User.Identity.Name, "Experience Info for " + person1.EmployeeCode + " is Created By  " + User.Identity.Name, Modules.HRM);

                }
                #endregion

            }

            return Json(new[] { experience }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRProfileUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Experiences_Update([DataSourceRequest] DataSourceRequest request, Experience experience)
        {
            ModelState.Clear();

            if (experience.FromExAmharic != null)
            {
                var splitDate = experience.FromExAmharic.Split('/');
                experience.From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

            }
            else
            {
                ModelState.AddModelError("", "Please Enter Start Date");
            }
            if (experience.ToExAmharic != null)
            {
                var splitDate = experience.ToExAmharic.Split('/');
                experience.To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

            }
            else
            {
                ModelState.AddModelError("", "Please Enter End Date");
            }

            if (experience.ClearanceDateTimeAm != null)
            {
                var splitDate = experience.ClearanceDateTimeAm.Split('/');
                experience.ClearanceDateTime = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

            }
            else
            {
                ModelState.AddModelError("", "Please Enter Clearance Date");
            }

            if (experience.From.Date > experience.To.Date)
            {
                ModelState.AddModelError("", "Please check the date range, The year of experience should be non negative!!");
            }
            else if (experience.From.Date > DateTime.Now.Date)
            {

                ModelState.AddModelError("", "Start date should be less or equal to the current date!!");
            }
            else if (experience.To.Date > DateTime.Now.Date)
            {

                ModelState.AddModelError("", "End date should be less or equal to the current date!!");
            }

            if (ModelState.IsValid)
            {
                int days = (To - From).Days;

                // double month = (To - From).Days / 30;

                decimal year = Convert.ToDecimal(days / 365);

                //var y = (int)month / 12;

                //var ym = month % 12;

                //var ex = y + " አመት ከ " + ym + " ወር";

                experience.TotalExp = EmployeeHelper.GetDaystoWordsAmharic(days);

                var entity = new Experience
                {
                    EmployeeId = experience.EmployeeId,
                    ExperienceId = experience.ExperienceId,
                    Organozation = experience.Organozation,
                    OrgFullAddress = experience.OrgFullAddress,
                    EmploymentType = experience.EmploymentType,
                    From = experience.From,
                    To = experience.To,
                    TotalDuration = year,
                    TotalExp = experience.TotalExp,
                    Responsibility = experience.Responsibility,
                    ClearanceDateTime = experience.ClearanceDateTime,
                    CleranceReason = experience.CleranceReason,
                    IncomeTaxPaymentRefNo = experience.IncomeTaxPaymentRefNo,
                    Remark = experience.Remark,
                    Salary = experience.Salary,
                    EmployeeCode = experience.EmployeeCode,
                    FromExAmharic = experience.FromExAmharic,
                    ToExAmharic = experience.ToExAmharic,
                    ClearanceDateTimeAm = experience.ClearanceDateTimeAm,
                    IsPrivate = experience.IsPrivate,
                    //JobTitleId = experience.JobTitleId,
                };

                db.Experiences.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Employee Experience", "Success", User.Identity.Name, "Experience Info of " + person.EmployeeCode + " is Updated By  " + User.Identity.Name, Modules.HRM);

                }
                #endregion

            }

            return Json(new[] { experience }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRAdmin)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Experiences_Destroy([DataSourceRequest] DataSourceRequest request, Experience experience)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Experience
                {
                    EmployeeId = experience.EmployeeId,
                    ExperienceId = experience.ExperienceId,
                    Organozation = experience.Organozation,
                    OrgFullAddress = experience.OrgFullAddress,
                    EmploymentType = experience.EmploymentType,
                    From = experience.From,
                    To = experience.To,
                    TotalDuration = experience.TotalDuration,
                    Responsibility = experience.Responsibility,
                    ClearanceDateTime = experience.ClearanceDateTime,
                    CleranceReason = experience.CleranceReason,
                    IncomeTaxPaymentRefNo = experience.IncomeTaxPaymentRefNo,
                    Remark = experience.Remark,
                    Salary = experience.Salary,
                    EmployeeCode = experience.EmployeeCode,
                    FromExAmharic = experience.FromExAmharic,
                    ToExAmharic = experience.ToExAmharic,
                    TotalExp = experience.TotalExp,
                    ClearanceDateTimeAm = experience.ClearanceDateTimeAm,
                    IsPrivate = experience.IsPrivate,
                    //JobTitleId = experience.JobTitleId,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Experience information of employee Id" + experience.EmployeeCode);
                #endregion
                db.Experiences.Attach(entity);
                db.Experiences.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { experience }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Upload(HttpPostedFileBase file)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.EXPERIENCE), fileName);
                    var mainPath =
                        Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.EXPERIENCE));
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
                        ReferenceType = AttachmentType.EXPERIENCE,
                        Index = 0,
                        URL = path,
                        Remark = HRMInputs.Owner


                    };
                    db.Attachments.Add(entity);

                    db.SaveChanges();
                    ViewBag.Message = "Upload successful";
                }
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "Upload failed";
                return RedirectToAction("Upload");
            }
        }

        public void Download(int id)
        {
            HRMInputs.SelectedFile = id;

        }

        public string GetContentType(string FileName)
        {
            string path = "~/Content/Images/" + FileName;
            string ext = Path.GetExtension(path); //get file extension
            string type = "";

            // set known types based on file extension
            if (ext != null)
            {
                switch (ext.ToLower())
                {
                    case ".htm":
                    case ".html":
                        type = "text/HTML";
                        break;

                    case ".txt":
                        type = "text/plain";
                        break;

                    case ".GIF":
                        type = "image/GIF";
                        break;

                    case ".pdf":
                        type = "Application/pdf";
                        break;

                    case ".doc":
                    case ".docx":
                    case ".rtf":

                        type = "Application/msword";
                        break;
                    case ".xlsx":
                        type = "Application/x-msexcel";
                        break;
                }
            }
            return type;
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