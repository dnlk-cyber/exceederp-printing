using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
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
using ExceedERP.Helpers.HR;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{


    [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser)]
    public class EmployeeCosignController : BaseController
    {
        // GET: HRM/EmployeeExperience
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {

            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            ViewData["isEthiopicDate"] = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate;
            var cosignTypes = db.Lookups.Where(x => x.LookupType == LookUpTypes.COSIGN_TYPE);
            ViewData["CosignType"] = cosignTypes;

            var employeeIds = db.Placements.Select(x => x.EmployeeId).ToList();
            ViewData["EmployeeList"] = db.Person_Employee
                                        .Where(x => employeeIds.Contains(x.EmployeeId))
                                        .Select(x => new {
                                            EmployeeId = x.EmployeeId,
                                            HeadId = x.EmployeeId,
                                            EmployeeCode = x.EmployeeCode,
                                            FullNameAmharic = x.EmployeeCode + " - " + x.FirstName + " " + x.MiddleName + " " + x.LastName,
                                            FullNameaEnglish = x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish
                                        }).ToList();

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
                var AgreementDateAm = value;
                var splitDate = AgreementDateAm.Split('/');
                var AgreementDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }


        }
        public ActionResult EmployeeCosigns_Read([DataSourceRequest]DataSourceRequest request, int employeeCosignId)
        {
            IQueryable<Cosign> cosigns = db.Cosigns.Where(x => x.EmployeeId == employeeCosignId);
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
                Salary = cosign.Salary,
                IsActive = cosign.IsActive,
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
        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser, HrmConstants.HRAdmin)]
        public ActionResult Cosigns_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            IQueryable<Cosign> cosigns = db.Cosigns/*.Where(x => mybraches.Contains(x.OrgBranchName))*/;
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
                Salary = cosign.Salary,
                IsActive = cosign.IsActive,
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
            bool datePreference = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;
            if (datePreference)
            {
                if (string.IsNullOrEmpty(cosign.AgreementDateAm))
                {
                    ModelState.AddModelError("", "Agreement Date Date field is mandatory");
                    return Json(new[] { cosign }.ToDataSourceResult(request, ModelState));
                }


                cosign.AgreementDate = HrHelper.ParseEthiopicDate(cosign.AgreementDateAm);
             

            }
            else
            {

                cosign.AgreementDateAm = XAPI.EthiopicDateTime.GetEthiopicDate(cosign.AgreementDate.Day, cosign.AgreementDate.Month, cosign.AgreementDate.Year);

            }
            if (cosign.AgreementDate.Date > DateTime.Now.Date)
            {

                ModelState.AddModelError("", "Agreement date should be less or equal to the current date!!");
            }
            if (ModelState.IsValid)
            {

                var employee = db.Person_Employee.Find(cosign.EmployeeId);
                var entity = new Cosign
                {
                    EmployeeId = cosign.EmployeeId,
                    AgreementDate = cosign.AgreementDate,
                    FullName = cosign.FullName,
                    LookupId = cosign.LookupId,
                    Occupation = cosign.Occupation,
                    Workorganization = cosign.Workorganization,
                    Salary = cosign.Salary,
                    Remark = cosign.Remark,
                    AgreementDateAm = cosign.AgreementDateAm,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    ModifiedBy = cosign.ModifiedBy,
                    LastModified = cosign.LastModified,
                    EmployeeCode = employee?.EmployeeCode??string.Empty,
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

            return Json(new[] { cosign }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeCosigns_Create([DataSourceRequest]DataSourceRequest request, Cosign cosign,int employeeCosignId)
        {
            ModelState.Clear();
            bool datePreference = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;
            if (datePreference)
            {
                if (string.IsNullOrEmpty(cosign.AgreementDateAm))
                {
                    ModelState.AddModelError("", "Agreement Date Date field is mandatory");
                    return Json(new[] { cosign }.ToDataSourceResult(request, ModelState));
                }


                cosign.AgreementDate = HrHelper.ParseEthiopicDate(cosign.AgreementDateAm);
             

            }
            else
            {

                cosign.AgreementDateAm = XAPI.EthiopicDateTime.GetEthiopicDate(cosign.AgreementDate.Day, cosign.AgreementDate.Month, cosign.AgreementDate.Year);

            }
            if (cosign.AgreementDate.Date > DateTime.Now.Date)
            {

                ModelState.AddModelError("", "Agreement date should be less or equal to the current date!!");
            }
            cosign.EmployeeId = employeeCosignId;
            if (ModelState.IsValid)
            {

                var employee = db.Person_Employee.Find(cosign.EmployeeId);
                var entity = new Cosign
                {
                    EmployeeId = cosign.EmployeeId,
                    AgreementDate = cosign.AgreementDate,
                    FullName = cosign.FullName,
                    LookupId = cosign.LookupId,
                    Occupation = cosign.Occupation,
                    Workorganization = cosign.Workorganization,
                    Salary = cosign.Salary,
                    Remark = cosign.Remark,
                    AgreementDateAm = cosign.AgreementDateAm,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    ModifiedBy = cosign.ModifiedBy,
                    LastModified = cosign.LastModified,
                    EmployeeCode = employee?.EmployeeCode??string.Empty,
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

            return Json(new[] { cosign }.ToDataSourceResult(request, ModelState));
        }
     
        [AuthorizeRoles( HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Cosigns_Update([DataSourceRequest]DataSourceRequest request, Cosign cosign)
        {
            ModelState.Clear();
            bool datePreference = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;
            if (datePreference)
            {
                if (string.IsNullOrEmpty(cosign.AgreementDateAm))
                {
                    ModelState.AddModelError("", "Agreement Date Date field is mandatory");
                    return Json(new[] { cosign }.ToDataSourceResult(request, ModelState));
                }


                cosign.AgreementDate = HrHelper.ParseEthiopicDate(cosign.AgreementDateAm);


            }
            else
            {

                cosign.AgreementDateAm = XAPI.EthiopicDateTime.GetEthiopicDate(cosign.AgreementDate.Day, cosign.AgreementDate.Month, cosign.AgreementDate.Year);

            }
            if (cosign.AgreementDate.Date > DateTime.Now.Date)
            {

                ModelState.AddModelError("", "Agreement date should be less or equal to the current date!!");
            }
            if (ModelState.IsValid)
            {
                var employee = db.Person_Employee.Find(cosign.EmployeeId);
                var entity = new Cosign
                {
                    CosignId = cosign.CosignId,
                    EmployeeId = cosign.EmployeeId,
                    AgreementDate =cosign.AgreementDate,
                    FullName = cosign.FullName,
                    Occupation = cosign.Occupation,
                    LookupId = cosign.LookupId,
                    Workorganization = cosign.Workorganization,
                    Salary = cosign.Salary,
                    Remark = cosign.Remark,
                    AgreementDateAm = cosign.AgreementDateAm,
                    CreatedBy = cosign.CreatedBy,
                    DateCreated = cosign.DateCreated,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                    EmployeeCode =  employee?.EmployeeCode ?? string.Empty,
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
                    Salary = cosign.Salary,
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

        public ActionResult EnableOrDisable(int id)
        {
            var cosign = db.Cosigns.Find(id);

            if (cosign == null)
            {
                // Record not found case
                return Json(new
                {
                    Message = "Record not found.",
                    Status = "Error"
                }, JsonRequestBehavior.AllowGet);
            }

            // Toggle active/inactive status
            cosign.IsActive = !cosign.IsActive;
            db.Entry(cosign).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new
            {
                Message = cosign.IsActive
                    ? "You successfully enabled this Cosign."
                    : "You successfully disabled this Cosign.",
                Status = "OK"
            }, JsonRequestBehavior.AllowGet);
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}