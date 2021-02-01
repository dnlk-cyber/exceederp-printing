﻿using System;
using System.Collections.Generic;
﻿using System.Data.Entity;
﻿using System.IO;
﻿using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
 using ExceedERP.Core.Domain;
 using ExceedERP.Core.Domain.Common;
﻿using ExceedERP.Core.Domain.Common.HRM;
﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.TrainingEducation;
using ExceedERP.DataAccess.Context;
﻿using ExceedERP.Web.Areas.Common.Controllers;
﻿using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
 using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.TrainingAndEducation
{
    
    [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRProfileUser,HrmConstants.HRAdmin)]
    public class EducationController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static DateTime From { get; set; }
        public static DateTime To { get; set; }
        public static string FromAmharic { get; set; }
        public static string ToAmharic { get; set; }
        public ActionResult Index()
        {
            GetEducationFiled();
            GetEducationLevel();
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
            if (name == "FromEAmharic")
            {
                FromAmharic = value;
                var splitDate = FromAmharic.Split('/');
                From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else if (name == "ToEAmharic")
            {
                ToAmharic = value;
                var splitDate = ToAmharic.Split('/');
                To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
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
                       db.Attachments.Where(x => x.Reference == filename && x.ReferenceType == AttachmentType.EDUCATION && x.Remark == HRMInputs.Owner && x.AttachmentId == HRMInputs.SelectedFile)
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
                    byte[] datas = req.DownloadData(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.EDUCATION + "/" + attachment.Description));
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
        public JsonResult GetEducationFiled()
        {
            var lookups = db.Lookups.Where(l => l.LookupType == LookUpTypes.EDUCATION_FIELD).AsQueryable();
            ViewData["EduFiledList"] = lookups;
            return Json(lookups.Select(p => new { LookupId = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEducationLevel()
        {
            var lookups = db.Lookups.Where(l => l.LookupType == LookUpTypes.EDUCATION_LEVEL).AsQueryable();
            ViewData["EduLevelList"] = lookups;
            return Json(lookups.Select(p => new { LookupId = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);
        }
     
        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRProfileUser, HrmConstants.HRAdmin)]
        public ActionResult Education_Read([DataSourceRequest]DataSourceRequest request, int idEd)
        {
            // HRMInputs.EmployeeId = id;
            IQueryable<Education> educations = db.Education.Where(x => x.EmployeeId == idEd);
            DataSourceResult result = educations.ToDataSourceResult(request, education => new
            {
                EmpEduId = education.EmpEduId,
                EmployeeId = education.EmployeeId,
                OrganizationName = education.OrganizationName,
                OrgAddress = education.OrgAddress,
                From = education.From,
                To = education.To,
                Score = education.Score,
                EduLevel = education.EduLevel,
                EduField = education.EduField,
                TotalCost = education.TotalCost,
                PercentCoverdByOrg = education.PercentCoverdByOrg,
                FromEAmharic = education.FromEAmharic,
                ToEAmharic = education.ToEAmharic,
                Remark = education.Remark,
                IsHighLevel = education.IsHighLevel,
            });

            return Json(result);
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
                    ReferenceType = AttachmentType.EDUCATION,
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
       
        [AuthorizeRoles( HrmConstants.HRProfileUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Education_Create([DataSourceRequest]DataSourceRequest request, Education education, int idEdTwo)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (ToAmharic == null)
                {
                    ModelState.AddModelError("", "Date to value is mandatory!!");
                }
                else if (From.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Start Date should be less or equal to the current date!!");
                }
                else if (From.Date > To.Date)
                {

                    ModelState.AddModelError("", "Start date should be less or equal to end date!!");
                }

                else
                {
                    var entity = new Education
                    {
                        EmployeeId = idEdTwo,
                        OrganizationName = education.OrganizationName,
                        OrgAddress = education.OrgAddress,
                        From = Convert.ToDateTime(From.Date),
                        To = To,
                        Score = education.Score,
                        EduLevel = education.EduLevel,
                        EduField = education.EduField,
                        TotalCost = education.TotalCost,
                        PercentCoverdByOrg = education.PercentCoverdByOrg,
                        Remark = education.Remark,
                        FromEAmharic = FromAmharic,
                        ToEAmharic = ToAmharic,
                        IsHighLevel = education.IsHighLevel,
                    };
                    if (String.IsNullOrEmpty(entity.FromEAmharic)) //(entity.From.ToString() == "01/01/0001 12:00:00 AM")
                    {
                        entity.From = null; //new DateTime?();
                        entity.FromEAmharic = null;
                    }
                    db.Education.Add(entity);
                    db.SaveChanges();
                    education.EmpEduId = entity.EmpEduId;

                    #region operation log
                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                    if (person != null)
                    {
                            UserHelper.OperationLog(Request.UserHostAddress, "Education ", "Success", User.Identity.Name, User.Identity.Name + " add Education info for " + person.EmployeeCode,Modules.HRM);
                        
                    }
                    #endregion
                }
            }

            return Json(new[] { education }.ToDataSourceResult(request, ModelState));
        }
      
        [AuthorizeRoles( HrmConstants.HRProfileUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Education_Update([DataSourceRequest]DataSourceRequest request, Education education)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (FromAmharic == null)
                {
                    FromAmharic = education.FromEAmharic;
                    var splitDate = FromAmharic.Split('/');
                    From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                else if (ToAmharic == null)
                {
                    ToAmharic = education.ToEAmharic;
                    var splitDate = ToAmharic.Split('/');
                    To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (From.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Start Date should be less or equal to the current date!!");
                }
                else if (From.Date > To.Date)
                {

                    ModelState.AddModelError("", "From should be less or equal to To date!!");
                }
                else
                {
                    var entity = new Education
                    {
                        EmpEduId = education.EmpEduId,
                        EmployeeId = education.EmployeeId,
                        OrganizationName = education.OrganizationName,
                        OrgAddress = education.OrgAddress,
                        From = From,
                        To = To,
                        Score = education.Score,
                        EduLevel = education.EduLevel,
                        EduField = education.EduField,
                        TotalCost = education.TotalCost,
                        PercentCoverdByOrg = education.PercentCoverdByOrg,
                        Remark = education.Remark,
                        FromEAmharic = FromAmharic,
                        ToEAmharic = ToAmharic,
                        IsHighLevel = education.IsHighLevel,
                    };

                    if (String.IsNullOrEmpty(entity.FromEAmharic)) //(entity.From.ToString() == "01/01/0001 12:00:00 AM")
                    {
                        entity.From = null; //new DateTime?();
                        entity.FromEAmharic = null;
                    }
                    db.Education.Attach(entity);
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
                            UserHelper.OperationLog(Request.UserHostAddress, "Education ", "Success", User.Identity.Name, name.EmployeeCode + " update Education info of " + person.EmployeeCode,Modules.HRM);

                        }
                    }
                    #endregion
                }
            }

            return Json(new[] { education }.ToDataSourceResult(request, ModelState));
        }
    
        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Education_Destroy([DataSourceRequest]DataSourceRequest request, Education education)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Education
                {
                    EmpEduId = education.EmpEduId,
                    EmployeeId = education.EmployeeId,
                    OrganizationName = education.OrganizationName,
                    OrgAddress = education.OrgAddress,
                    From = education.From,
                    To = education.To,
                    Score = education.Score,
                    EduLevel = education.EduLevel,
                    EduField = education.EduField,
                    TotalCost = education.TotalCost,
                    PercentCoverdByOrg = education.PercentCoverdByOrg,
                    Remark = education.Remark,
                    FromEAmharic = education.FromEAmharic,
                    ToEAmharic = education.ToEAmharic,
                    IsHighLevel = education.IsHighLevel,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted educational information of employee " + education.EmployeeId);
                #endregion
                db.Education.Attach(entity);
                db.Education.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { education }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
