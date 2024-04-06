using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Areas.Common.Controllers;
using System.IO;
using System.Net;
using System.Web;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{
   
    [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser, HrmConstants.HRAdmin)]
    public class EmployeeCosignController : BaseController
    {
        // GET: HRM/EmployeeExperience
        private ExceedDbContext db = new ExceedDbContext();
        public static DateTime AgreementDate { get; set; }
        public static string AgreementDateAm { get; set; }
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            var cosignTypes = db.Lookups.Where(x => x.LookupType == LookUpTypes.COSIGN_TYPE);
            ViewData["CosignType"] = cosignTypes;

            var listt = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = listt.Where(x => x.IsActive);
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
                       db.Attachments.Where(x => x.Reference == filename && x.ReferenceType == AttachmentType.COSIGN && x.Remark == HRMInputs.Owner && x.AttachmentId == HRMInputs.SelectedFile)
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
                    byte[] datas = req.DownloadData(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.COSIGN + "/" + attachment.Description));
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
        public void SetDate(string name, string value)
        {
            if (name == "AgreementDateAm")
            {
                AgreementDateAm = value;
                var splitDate = AgreementDateAm.Split('/');
                AgreementDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }


        }
    
        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser, HrmConstants.HRAdmin)]
        public ActionResult Cosigns_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            IQueryable<Cosign> cosigns = db.Cosigns.Where(x => mybraches.Contains(x.OrgBranchName));
            DataSourceResult result = cosigns.ToDataSourceResult(request, cosign => new
            {
                CosignId = cosign.CosignId,
                EmployeeId = cosign.EmployeeId,
                AgreementDate = cosign.AgreementDate,
                FullName = cosign.FullName,
                Occupation = cosign.Occupation,
                LookupId = cosign.LookupId,
                Workorganization = cosign.Workorganization,
                Remark = cosign.Remark,
                AgreementDateAm = cosign.AgreementDateAm,
                CreatedBy = cosign.CreatedBy,
                DateCreated = cosign.DateCreated,
                ModifiedBy = cosign.ModifiedBy,
                LastModified = cosign.LastModified,
                EmployeeCode = cosign.EmployeeCode,
                OrgBranchName = cosign.OrgBranchName
            });

            return Json(result);
        }
    
        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Cosigns_Create([DataSourceRequest]DataSourceRequest request, Cosign cosign)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (AgreementDateAm == null)
                {
                    ModelState.AddModelError("", "Agreement date value is mandatory!!");
                }
                else if (AgreementDate.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Agreement date should be less or equal to the current date!!");
                }
                else
                {
                    var entity = new Cosign
                    {
                        EmployeeId = cosign.EmployeeId,
                        AgreementDate = AgreementDate,
                        FullName = cosign.FullName,
                        LookupId = cosign.LookupId,
                        Occupation = cosign.Occupation,
                        Workorganization = cosign.Workorganization,
                        Remark = cosign.Remark,
                        AgreementDateAm = AgreementDateAm,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        ModifiedBy = cosign.ModifiedBy,
                        LastModified = cosign.LastModified,
                        EmployeeCode = inputs.GetEmployees().Where(x => x.PersonId == cosign.EmployeeId).FirstOrDefault().EmployeeCode,
                        OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)
                    };

                    db.Cosigns.Add(entity);
                    db.SaveChanges();
                    cosign.CosignId = entity.CosignId;

                    #region operation log
                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                    if (person != null)
                    {
                            UserHelper.OperationLog(Request.UserHostAddress, "Employee Cosign", "Success", User.Identity.Name, "Employee Cosign for " + person.EmployeeCode + " is Created By  " + User.Identity.Name,Modules.HRM);
                        
                    }
                    #endregion
                }
            }

            return Json(new[] { cosign }.ToDataSourceResult(request, ModelState));
        }
     
        [AuthorizeRoles( HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Cosigns_Update([DataSourceRequest]DataSourceRequest request, Cosign cosign)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (AgreementDateAm == null)
                {
                    AgreementDateAm = cosign.AgreementDateAm;
                    var splitDate = AgreementDateAm.Split('/');
                    AgreementDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (AgreementDate.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Agreement date should be less or equal to the current date!!");
                }
                else
                {
                    var entity = new Cosign
                    {
                        CosignId = cosign.CosignId,
                        EmployeeId = cosign.EmployeeId,
                        AgreementDate = AgreementDate,
                        FullName = cosign.FullName,
                        Occupation = cosign.Occupation,
                        LookupId = cosign.LookupId,
                        Workorganization = cosign.Workorganization,
                        Remark = cosign.Remark,
                        AgreementDateAm = AgreementDateAm,
                        CreatedBy = cosign.CreatedBy,
                        DateCreated = cosign.DateCreated,
                        ModifiedBy = User.Identity.Name,
                        LastModified = DateTime.Now,
                        EmployeeCode = inputs.GetEmployees().Where(x => x.PersonId == cosign.EmployeeId).FirstOrDefault().EmployeeCode,
                        OrgBranchName = cosign.OrgBranchName
                    };

                    db.Cosigns.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    #region operation log
                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                    if (person != null)
                    {
                            UserHelper.OperationLog(Request.UserHostAddress, "Employee Cosign", "Success", User.Identity.Name, "Emoployee Cosign of " + person.EmployeeCode + " is Updated By  " + User.Identity.Name,Modules.HRM);
                        
                    }
                    #endregion
                }
            }

            return Json(new[] { cosign }.ToDataSourceResult(request, ModelState));
        }
      
        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin,HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Cosigns_Destroy([DataSourceRequest]DataSourceRequest request, Cosign cosign)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Cosign
                {
                    CosignId = cosign.CosignId,
                    EmployeeId = cosign.EmployeeId,
                    AgreementDate = cosign.AgreementDate,
                    FullName = cosign.FullName,
                    Occupation = cosign.Occupation,
                    LookupId = cosign.LookupId,
                    Workorganization = cosign.Workorganization,
                    Remark = cosign.Remark,
                    AgreementDateAm = cosign.AgreementDateAm,
                    CreatedBy = cosign.CreatedBy,
                    DateCreated = cosign.DateCreated,
                    ModifiedBy = cosign.ModifiedBy,
                    LastModified = cosign.LastModified,
                    EmployeeCode = cosign.EmployeeCode,
                    OrgBranchName = cosign.OrgBranchName
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted cosign information of employee Id" + cosign.EmployeeCode);
                #endregion
                db.Cosigns.Attach(entity);
                db.Cosigns.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { cosign }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Upload(HttpPostedFileBase file)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.CosignId.ToString() + "-" + AttachmentType.COSIGN), fileName);
                    var mainPath =
                        Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.CosignId.ToString() + "-" + AttachmentType.COSIGN));
                    if (!Directory.Exists(mainPath))
                    {
                        Directory.CreateDirectory(mainPath);
                    }
                    file.SaveAs(path);

                    var entity = new Attachment
                    {

                        Reference = HRMInputs.CosignId.ToString(),
                        Description = file.FileName,
                        FileType = file.ContentType,
                        ReferenceType = AttachmentType.COSIGN,
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