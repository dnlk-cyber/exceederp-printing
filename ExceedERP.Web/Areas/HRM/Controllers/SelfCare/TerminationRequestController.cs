using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
    public class TerminationRequestController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        HRMInputs inputs = new HRMInputs();
        public static DateTime ApplicationDate { get; set; }
        public static string ApplicationDateAm { get; set; }
        public static DateTime TerminationDate { get; set; }
        public static string TerminationDateAm { get; set; }
        public static int EmployeeId { get; set; }
        public static bool IsForOther { get; set; }
        public static List<string> reportsToEmployee = new List<string>();
        public ActionResult Index()
        {

            SetValues();
            PopulateViewData();

            return View();
        }
        private void PopulateViewData()
        {
            EmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var list = UserInformationHelper.GetEmployeesByReportsTo(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            reportsToEmployee = list.Select(x => x.PersonId.ToString()).ToList();
            reportsToEmployee.Add(EmployeeId.ToString());

            ViewData["EmployeeName"] = db.Person_Employee.ToList();
            ViewData["Department"] = db.MainCompanyStructures.ToList();
        }
        public void SetDate(string name, string value)
        {
            if (name == "ApplicationDateAm")
            {
                ApplicationDateAm = value;
                var splitDate = ApplicationDateAm.Split('/');
                ApplicationDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else if (name == "TerminationDateAm")
            {
                TerminationDateAm = value;
                var splitDate = TerminationDateAm.Split('/');
                TerminationDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }


        }
        public string CheckStatus(int masterId)
        {
            var reques = db.RequestesStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "Termination").FirstOrDefault();
            if (reques != null)
            {
                var checkApprove = db.RequestesStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "Termination" && x.Status == RequestStatus.APPROVE).FirstOrDefault();
                if (checkApprove != null)
                {
                    return "approved";
                }
                var deptApproval = db.RequestesStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "Termination" && x.Status == RequestStatus.DEPARTMENT_APPROVAL).FirstOrDefault();
                if (deptApproval != null)
                {
                    return "Dapproved";
                }
                return "true";
            }
            return "false";
        }
        private void SetValues()
        {
            var terminationTypes = db.Lookups.Where(l => l.LookupType == LookUpTypes.TERMINATIONTYPE);
            ViewData["TerminationTypes"] = terminationTypes;

            var list = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
        }
        public int SetEmployee(string x)
        {
            IsForOther = false;
            if (x == "true")
            {
                var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                //var list = EmployeeHelper.GetSubordinates_FilteredByBranchAlso(employeeId).ToList();
                var list = EmployeeHelper.GetSubordinates_FilteredByHead(employeeId).ToList();
                IsForOther = true;
                return list.Count();
            }
            return 0;
        }
        public bool CheckStatusForBackColor(int termId)
        {
            var reques = db.RequestesStatus.Where(x => x.RequestId == termId && x.Status == RequestStatus.APPROVE && x.ReferenceType == "Termination").FirstOrDefault();
            if (reques != null)
            {
                return true;
            }
            return false;
        }
        public ActionResult Terminations_Read([DataSourceRequest]DataSourceRequest request)
        {
            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            //List<int> employeeids = EmployeeHelper.GetSubordinates_FilteredByBranchAlso(employeeid);
            List<int> employeeids = EmployeeHelper.GetSubordinates_FilteredByHead(employeeId);
            IQueryable<Core.Domain.HRM.EmployeeDetail.Termination> terminations = db.Terminations.Where(x => employeeids.Contains(x.EmployeeId));
            DataSourceResult result = terminations.ToDataSourceResult(request, termination => new
            {
                TerminationId = termination.TerminationId,
                EmployeeId = termination.EmployeeId,
                IsForOther = termination.IsForOther,
                LookupId = termination.LookupId,
                Reason = termination.Reason,
                TerminationDate = termination.TerminationDate,
                ApplicationDate = termination.ApplicationDate,
                Remark = termination.Remark,
                EmployeeCode = termination.EmployeeCode,
                TerminationDateAm = termination.TerminationDateAm,
                ApplicationDateAm = termination.ApplicationDateAm,
                DateCreated = termination.DateCreated,
                LastModified = termination.LastModified,
                CreatedBy = termination.CreatedBy,
                ModifiedBy = termination.ModifiedBy,
                OrgBranchName = termination.OrgBranchName,
                IsApproved = CheckStatusForBackColor(termination.TerminationId),
                DepartmentSendForApprovalBy = termination.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = termination.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = termination.IsDepartmentSendForApproval,
                DepartmentOnlineApprovedBy = termination.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = termination.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = termination.IsDepartmentOnlineApproved,
                OnlineApprovedBy = termination.OnlineApprovedBy,
                OnlineApprovedTime = termination.OnlineApprovedTime,
                IsOnlineApproved = termination.IsOnlineApproved,
            });

            return Json(result);
        }

        public void SetEmployeeId(string fullName)
        {
            List<EmployeeIdView> EmployeeViews = new List<EmployeeIdView>();
            EmployeeIdView EmployeeView = new EmployeeIdView();
            var employeeList = db.Person_Employee.AsQueryable();
            if (employeeList.Count() > 0)
            {
                employeeList = employeeList.Where(l => l.IsActive).AsQueryable();
            }
            foreach (var emp in employeeList)
            {
                EmployeeView = new EmployeeIdView();
                EmployeeView.PersonId = emp.EmployeeId;
                EmployeeView.FullName = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
                EmployeeViews.Add(EmployeeView);
            }
            var employees = EmployeeViews.Where(p => p.FullName == fullName).FirstOrDefault();
            if (employees != null)
            {
                HRMInputs.EmployeeId = employees.PersonId;
            }
            ;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Terminations_Create([DataSourceRequest]DataSourceRequest request, Core.Domain.HRM.EmployeeDetail.Termination termination)
        {
            ModelState.Clear();
            if (!termination.IsForOther)
            {
                EmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            }
            else
            {
                EmployeeId = termination.EmployeeId;
            }
            var checkExistance = db.Terminations.FirstOrDefault(x => x.EmployeeId == EmployeeId);
            if (checkExistance != null)
            {

                ModelState.AddModelError("", "This employee already request the termination!!");
                return Json(new[] { termination }.ToDataSourceResult(request, ModelState));

            }
            else if (TerminationDate.Date < ApplicationDate.Date)
            {
                ModelState.AddModelError("", "Please check the date, Applicaton date should be less than Termination Date!!");
                return Json(new[] { termination }.ToDataSourceResult(request, ModelState));

            }
            else if (TerminationDateAm == null || ApplicationDateAm == null)
            {
                ModelState.AddModelError("", "Termination and application Date value and is mandatory!!");
                return Json(new[] { termination }.ToDataSourceResult(request, ModelState));

            }
            else if (TerminationDate.Date < DateTime.Now.Date)
            {

                ModelState.AddModelError("", "Termination date should be greater or equal to the current date!!");
                return Json(new[] { termination }.ToDataSourceResult(request, ModelState));

            }
            else if (ApplicationDate.Date > DateTime.Now.Date)
            {

                ModelState.AddModelError("", "Application date should be less or equal to the current date!!");
                return Json(new[] { termination }.ToDataSourceResult(request, ModelState));

            }
            if (ModelState.IsValid)
            {
               
                
                var diff = (TerminationDate - ApplicationDate).TotalDays;
                   
                var entity = new Core.Domain.HRM.EmployeeDetail.Termination
                {
                    EmployeeId = EmployeeId,
                    LookupId = termination.LookupId,
                    Reason = termination.Reason,
                    ApplicationDate = ApplicationDate,
                    TerminationDate = TerminationDate,
                    Remark = termination.Remark,
                    EmployeeCode = inputs.GetEmployees().FirstOrDefault(x => x.PersonId == EmployeeId).EmployeeCode,
                    TerminationDateAm = TerminationDateAm,
                    ApplicationDateAm = ApplicationDateAm,
                    DateCreated = DateTime.Now,
                    LastModified = termination.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = termination.ModifiedBy,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    IsForOther = termination.IsForOther,
                };

                db.Terminations.Add(entity);
                db.SaveChanges();
                termination.TerminationId = entity.TerminationId;
                if (diff < 30)
                {
                    string messages = "Your termination request date length is less than 30. You need to return one month salary to finance.";
                    termination.Remark = "The Date Length is less than 30.";
                    return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                }

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "TerminationRequest ", "Success", User.Identity.Name, User.Identity.Name + " add new Termination Request at date " + DateTime.Today.ToShortDateString(), Modules.HRM);

                #endregion

                
            }
            IsForOther = false;
            return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Terminations_Update([DataSourceRequest]DataSourceRequest request, Core.Domain.HRM.EmployeeDetail.Termination termination)
        {
            ModelState.Clear();
            termination.EmployeeId = HRMInputs.EmployeeId;
            if (TerminationDateAm == null)
            {
                TerminationDateAm = termination.TerminationDateAm;
                var splitDate = TerminationDateAm.Split('/');
                TerminationDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            if (ApplicationDateAm == null)
            {
                ApplicationDateAm = termination.ApplicationDateAm;
                var splitDate = ApplicationDateAm.Split('/');
                ApplicationDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            if (TerminationDate.Date < ApplicationDate.Date)
            {
                ModelState.AddModelError("", "Please check the date range!!");
                return Json(new[] { termination }.ToDataSourceResult(request, ModelState));

            }
            else if (TerminationDate.Date < DateTime.Now.Date)
            {

                ModelState.AddModelError("", "Termination date should be greater or equal to the current date!!");
                return Json(new[] { termination }.ToDataSourceResult(request, ModelState));

            }
            else if (ApplicationDate.Date > DateTime.Now.Date)
            {

                ModelState.AddModelError("", "Application date should be less or equal to the current date!!");
                return Json(new[] { termination }.ToDataSourceResult(request, ModelState));

            }
            var diff = (TerminationDate - ApplicationDate).TotalDays;
            if (diff < 30)
            {
                ModelState.AddModelError("", "Your termination request date length is less than 30. You need to return one month salary to finance!!");
                termination.Remark = "The Date Length is less than 30.";
                return Json(new[] { termination }.ToDataSourceResult(request, ModelState));

            }
            else if (TerminationDate.Date > DateTime.Now.Date)
            {

                ModelState.AddModelError("", "Termination date should be less or equal to the current date!!");
                return Json(new[] { termination }.ToDataSourceResult(request, ModelState));

            }
            if (ModelState.IsValid)
            {
                
                var entity = new Core.Domain.HRM.EmployeeDetail.Termination
                {
                    TerminationId = termination.TerminationId,
                    EmployeeId = termination.EmployeeId,
                    LookupId = termination.LookupId,
                    Reason = termination.Reason,
                    TerminationDate = TerminationDate,
                    ApplicationDate = ApplicationDate,
                    EmployeeCode = inputs.GetEmployees().FirstOrDefault(x => x.PersonId == termination.EmployeeId)?.EmployeeCode,
                    Remark = termination.Remark,
                    TerminationDateAm = TerminationDateAm,
                    ApplicationDateAm = ApplicationDateAm,
                    DateCreated = termination.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = termination.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    OrgBranchName = termination.OrgBranchName,
                    IsForOther = termination.IsForOther,
                };

                db.Terminations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "TerminationRequest ", "Success", User.Identity.Name, User.Identity.Name + " update Termination Request at date " + DateTime.Today.ToShortDateString(), Modules.HRM);

                #endregion

                
            }
            IsForOther = false;
            return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Terminations_Destroy([DataSourceRequest]DataSourceRequest request, Core.Domain.HRM.EmployeeDetail.Termination termination)
        {
            ModelState.Clear();

            
            
            var status = db.RequestesStatus.Where(x => x.Status==RequestStatus.APPROVE && x.RequestId==termination.TerminationId && x.ReferenceType=="termination").FirstOrDefault();

           
               if (status != null)
                {
                    ModelState.AddModelError("","You are Terminated Employee You cannot Delete Termination");
                }
               if (ModelState.IsValid)
               {
             
                
                    var entity = new Core.Domain.HRM.EmployeeDetail.Termination
                    {
                        TerminationId = termination.TerminationId,
                        EmployeeId = termination.EmployeeId,
                        LookupId = termination.LookupId,
                        Reason = termination.Reason,
                        ApplicationDate = new DateTime(termination.TerminationDate.Year, termination.TerminationDate.Month, termination.TerminationDate.Day),
                        TerminationDate = termination.TerminationDate,
                        Remark = termination.Remark,
                        EmployeeCode = termination.EmployeeCode,
                        TerminationDateAm = termination.TerminationDateAm,
                        ApplicationDateAm = termination.ApplicationDateAm,
                        DateCreated = termination.DateCreated,
                        LastModified = termination.LastModified,
                        CreatedBy = termination.CreatedBy,
                        ModifiedBy = termination.ModifiedBy,
                        OrgBranchName = termination.OrgBranchName,
                        IsForOther = termination.IsForOther,
                    };

                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted termination request information of employee code " + termination.EmployeeCode);
                    #endregion
                    db.Terminations.Attach(entity);
                    db.Terminations.Remove(entity);
                    db.SaveChanges();
                }
               
            

            return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Upload(HttpPostedFileBase file)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path =
                        Path.Combine(
                            Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" +
                                           AttachmentType.CLEARANCE.ToString()), fileName);
                    var mainPath =
                        Path.Combine(
                            Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" +
                                           AttachmentType.CLEARANCE.ToString()));
                    if (!Directory.Exists(mainPath))
                    {
                        Directory.CreateDirectory(mainPath);
                    }
                    if (!System.IO.File.Exists(path))
                    {


                        file.SaveAs(path);

                        var entity = new Attachment
                        {

                            Reference = HRMInputs.EmployeeId.ToString(),
                            Description = file.FileName,
                            FileType = file.ContentType,
                            ReferenceType = AttachmentType.CLEARANCE,
                            Index = 0,
                            URL = path

                        };
                        db.Attachments.Add(entity);

                        db.SaveChanges();
                        ViewBag.Message = "Upload successful";
                    }

                }
                else
                {

                    ViewBag.Message = "The file is existed";

                }
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "Upload failed";
                return RedirectToAction("Upload");
            }
        }

        public FileResult Download(string data, int id)
        {

            try
            {
                string url = Path.Combine(
                                    Server.MapPath("~/Content/Images/" + id.ToString() + "-" +
                                                   AttachmentType.CLEARANCE.ToString()), data);
                byte[] fileBytes = System.IO.File.ReadAllBytes(url);

                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, data);
            }
            catch (Exception)
            {

                return null;
            }
            //return File(url, System.Net.Mime.MediaTypeNames.Application.Octet);

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
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted attachment of  " + attachment.ReferenceType + " and file name" + attachment.Description);
                #endregion
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
        public ActionResult SendForApproval()
        {
            RequestesStatus requestesStatus;
            db = new ExceedDbContext();
            requestesStatus =
                db.RequestesStatus.Where(
                    x =>
                        x.RequestId == HRMInputs.RequestId &&
                        x.ReferenceType == HRMInputs.ReferenceType && x.Status == RequestStatus.WAITING_FOR_APPROVAL).FirstOrDefault();
            db = new ExceedDbContext();
            if (requestesStatus == null)
            {
                var entityy = new RequestesStatus
                {
                    RequestId = HRMInputs.RequestId,
                    ReferenceType = HRMInputs.ReferenceType,
                    Status = RequestStatus.WAITING_FOR_APPROVAL,
                    Date = DateTime.Now,
                    DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year)
                };

                db.RequestesStatus.Add(entityy);
                db.SaveChanges();
            }
            else
            {
                var entity = new RequestesStatus
                {
                    RequestStatusId = requestesStatus.RequestStatusId,
                    RequestId = requestesStatus.RequestId,
                    ReferenceType = requestesStatus.ReferenceType,
                    Status = requestesStatus.Status,
                    Date = requestesStatus.Date,
                    DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(requestesStatus.Date.Day, requestesStatus.Date.Month, requestesStatus.Date.Year),
                    Remark = requestesStatus.Remark
                };

                db.RequestesStatus.Attach(entity);
                db.RequestesStatus.Remove(entity);
                db.SaveChanges();
                db = new ExceedDbContext();

                var entityy = new RequestesStatus
                {
                    RequestId = HRMInputs.RequestId,
                    ReferenceType = HRMInputs.ReferenceType,
                    Status = RequestStatus.WAITING_FOR_APPROVAL,
                    Date = DateTime.Now,
                    DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year)
                };

                db.RequestesStatus.Add(entityy);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
       
    }
   
}
