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
using ExceedERP.Helpers.HR;
using ExceedERP.Web.Hubs;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
    public class TerminationRequestController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {

           
            PopulateViewData();

            return View();
        }
        private void PopulateViewData()
        {

            // Employees for dropdown (used by ApproverId column)
            var employees = db.Person_Employee
                              .Where(e => e.IsActive)
                              .Select(e => new
                              {
                                  PersonId = e.EmployeeId,
                                  EmployeeCode = e.EmployeeCode,
                                  EmployeeId = e.EmployeeId,
                                  FullNameAmharic = e.FirstName + " " + e.MiddleName + " " + e.LastName,
                                  FullNameWithOutCode = e.FirstNameEnglish + " " + e.MiddleNameEnglish + " " + e.LastNameEnglish
                              }).AsQueryable();
            // Assign to ViewData for Kendo Grid ForeignKey binding
            ViewData["EmployeeList"] = employees; // For EmployeeId column
            ViewData["EmployeeName"] = employees;    // For ApproverId column
            ViewData["Department"] = db.MainCompanyStructures.AsQueryable();
            ViewData["TerminationTypes"] = db.Lookups.Where(l => l.LookupType == LookUpTypes.TERMINATIONTYPE).AsQueryable();
            ViewData["isEthiopicDate"] = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate;

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
       
        private bool CheckStatusForBackColor(int termId)
        {
            var reques = db.RequestesStatus.Where(x => x.RequestId == termId && x.Status == RequestStatus.APPROVE && x.ReferenceType == "Termination").FirstOrDefault();
            if (reques != null)
            {
                return true;
            }
            return false;
        }
        private bool CheckAgreeRetunSalary(DateTime terminationDate, DateTime applicationDate)
        {
            var diffDays = (terminationDate - applicationDate).TotalDays;
            return diffDays < 30;
        }
        public ActionResult Terminations_Read([DataSourceRequest]DataSourceRequest request)
        {
            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            List<int> employeeids = EmployeeHelper.GetSubordinates_FilteredByHead(employeeId);
            IQueryable<Core.Domain.HRM.EmployeeDetail.Termination> terminations = db.Terminations.Where(x => employeeids.Contains(x.EmployeeId)||x.EmployeeId==employeeId);
            DataSourceResult result = terminations.ToDataSourceResult(request, termination => new
            {
                TerminationId = termination.TerminationId,
                EmployeeId = termination.EmployeeId,
                IsForOther = termination.IsForOther,
                LookupId = termination.LookupId,
                Reason = termination.Reason,
                TerminationDate = termination.TerminationDate,
                ApplicationDate = termination.ApplicationDate,
                TerminationDateAm = termination.TerminationDateAm,
                ApplicationDateAm = termination.ApplicationDateAm,
                Remark = termination.Remark,
                EmployeeCode = termination.EmployeeCode,
                DateCreated = termination.DateCreated,
                LastModified = termination.LastModified,
                CreatedBy = termination.CreatedBy,
                ModifiedBy = termination.ModifiedBy,
                OrgBranchName = termination.OrgBranchName,
                IsApproved = CheckStatusForBackColor(termination.TerminationId),
                IsAgreeRetunSalary = CheckAgreeRetunSalary(termination.TerminationDate.Value,termination.ApplicationDate),
                DepartmentSendForApprovalBy = termination.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = termination.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = termination.IsDepartmentSendForApproval,
                DepartmentOnlineApprovedBy = termination.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = termination.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = termination.IsDepartmentOnlineApproved,
                IsSendForApproval = termination.IsSendForApproval,
                OnlineApprovedBy = termination.OnlineApprovedBy,
                OnlineApprovedTime = termination.OnlineApprovedTime,
                IsOnlineApproved = termination.IsOnlineApproved,
                IsEarlyRetirement = termination.IsEarlyRetirement,
            });

            return Json(result);
        }

  
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Terminations_Create([DataSourceRequest]DataSourceRequest request, Core.Domain.HRM.EmployeeDetail.Termination termination)
        {
            ModelState.Clear();
            var employeeId = termination.IsForOther? termination.EmployeeId : UserInformationHelper.GetEmployeeId(User.Identity.Name);
            // 1. Date Handling
            bool isEthiopic = HrHelper.GetDatePreference();

            if (isEthiopic)
            {
                if (string.IsNullOrWhiteSpace(termination.ApplicationDateAm))
                {
                    ModelState.AddModelError("", "Application date is required.");
                    return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
                }

                termination.ApplicationDate = HrHelper.ParseEthiopicDate(termination.ApplicationDateAm);

                if (!string.IsNullOrWhiteSpace(termination.TerminationDateAm))
                {
                    termination.TerminationDate = HrHelper.ParseEthiopicDate(termination.TerminationDateAm);
                }
            }
            else
            {
               

                termination.ApplicationDateAm = HrHelper.GetECDate(termination.ApplicationDate);

                if (termination.TerminationDate.HasValue)
                {
                    termination.TerminationDateAm = HrHelper.GetECDate(termination.TerminationDate.Value);
                }
            }

            // 2. Basic Validation
            if (termination.ApplicationDate > DateTime.Now.Date)
            {
                ModelState.AddModelError("", "Application date must be today or earlier.");
                return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
            }

            // Get fiscal year first
            var fiscalYear = db.GLFiscalYears
                .Where(x => x.DateFrom <= termination.ApplicationDate
                         && x.DateTo >= termination.ApplicationDate)
                .Select(x => new { x.DateFrom, x.DateTo })
                .FirstOrDefault();

            if (fiscalYear == null)
            {
                ModelState.AddModelError("", "No fiscal year found for the selected application date.");
                return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
            }

            // Check if termination already exists in SAME fiscal year
            bool exists = db.Terminations.Any(x =>
                x.EmployeeId == employeeId &&
                x.ApplicationDate >= fiscalYear.DateFrom &&
                x.ApplicationDate <= fiscalYear.DateTo
            );

            if (exists)
            {
                ModelState.AddModelError("", "Termination request already exists in this fiscal year.");
                return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
            }

            // --------------------
            // 3. Placement + Notice Period
            // --------------------
            var placement = db.Placements
                .Where(x => x.EmployeeId == employeeId && x.isCurrent)
                .Select(x => new { x.ResponsibilityLevel })
                .FirstOrDefault();

            if (placement == null)
            {
                ModelState.AddModelError("", "No active placement found.");
                return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
            }

            int noticeDays = db.TerminationSettings
                .Where(x => x.IsActive && x.ResponsibilityLevel == placement.ResponsibilityLevel)
                .Select(x => (int?)x.NoticePeriodDays)
                .FirstOrDefault() ?? 30; // default fallback

            // --------------------
            // 4. Termination Date Logic
            // --------------------
            if (!termination.TerminationDate.HasValue)
            {
                // Auto-calculate if not provided
                termination.TerminationDate = termination.ApplicationDate.AddDays(noticeDays);
            }
            else
            {
                if (termination.TerminationDate.Value.Date < termination.ApplicationDate.Date)
                {
                    ModelState.AddModelError("", "Termination date must be after application date.");
                    return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
                }
            }

            // Sync Ethiopic date
            termination.TerminationDateAm = HrHelper.GetECDate(termination.TerminationDate.Value);

            // --------------------
            // 5. Business Rule (Minimum 30 days)
            // --------------------
            var diffDays = (termination.TerminationDate.Value - termination.ApplicationDate).TotalDays;

            if (diffDays < 30 && !termination.IsAgreeRetunSalary)
            {
                ModelState.AddModelError("", "Less than 30 days requires salary agreement.");
                termination.Remark = "Notice period less than 30 days.";
                return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
            }
            var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == termination.EmployeeId);

            if (termination.IsEarlyRetirement && employee != null)
            {
                var age = DateTime.Now.Year - employee.DateOfBirth.Year;

                if (employee.DateOfBirth > DateTime.Now.AddYears(-age))
                {
                    age--;
                }

                if (age < 55)
                {
                    ModelState.AddModelError("", "Employee is not eligible for early retirement before age 55.");
                    return Json(new[] { termination }.ToDataSourceResult(request, ModelState));

                }
            }
            if (ModelState.IsValid)
            {
               

                var entity = new Core.Domain.HRM.EmployeeDetail.Termination
                {
                    EmployeeId = employeeId,
                    LookupId = termination.LookupId,
                    Reason = termination.Reason,
                    ApplicationDate = termination.ApplicationDate,
                    TerminationDate = termination.TerminationDate,
                    Remark = termination.Remark,
                    EmployeeCode = employee?.EmployeeCode??null,
                    TerminationDateAm = termination.TerminationDateAm,
                    ApplicationDateAm = termination.ApplicationDateAm,
                    DateCreated = DateTime.Now,
                    LastModified = termination.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = termination.ModifiedBy,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    IsForOther = termination.IsForOther,
                    IsEarlyRetirement = termination.IsEarlyRetirement,

                };

                db.Terminations.Add(entity);
                db.SaveChanges();
                termination.TerminationId = entity.TerminationId;
                if (diffDays < 30)
                {
                    string messages = "Your termination request date length is less than 30. You need to return one month salary to finance.";
                    termination.Remark = "The Date Length is less than 30.";
                    return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                }
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "TerminationRequest ", "Success", User.Identity.Name, User.Identity.Name + " add new Termination Request at date " + DateTime.Today.ToShortDateString(), Modules.HRM);
                #endregion

                
            }
            return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Terminations_Update([DataSourceRequest]DataSourceRequest request, Core.Domain.HRM.EmployeeDetail.Termination termination)
        {
            ModelState.Clear();
            termination.EmployeeId = termination.IsForOther ? termination.EmployeeId : UserInformationHelper.GetEmployeeId(User.Identity.Name);
            // 1. Date Handling
            bool isEthiopic = HrHelper.GetDatePreference();

            if (isEthiopic)
            {
                if (string.IsNullOrWhiteSpace(termination.ApplicationDateAm))
                {
                    ModelState.AddModelError("", "Application date is required.");
                    return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
                }

                termination.ApplicationDate = HrHelper.ParseEthiopicDate(termination.ApplicationDateAm);

                if (!string.IsNullOrWhiteSpace(termination.TerminationDateAm))
                {
                    termination.TerminationDate = HrHelper.ParseEthiopicDate(termination.TerminationDateAm);
                }
            }
            else
            {
                if (termination.ApplicationDate == null)
                {
                    ModelState.AddModelError("", "Application date is required.");
                    return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
                }

                termination.ApplicationDateAm = HrHelper.GetECDate(termination.ApplicationDate);

                if (termination.TerminationDate != null)
                {
                    termination.TerminationDateAm = HrHelper.GetECDate(termination.TerminationDate.Value);
                }
            }

            // 2. Basic Validations
            if (termination.ApplicationDate > DateTime.Now.Date)
            {
                ModelState.AddModelError("", "Application date must be today or earlier.");
                return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
            }

            // Get fiscal year first
            var fiscalYear = db.GLFiscalYears
                .Where(x => x.DateFrom <= termination.ApplicationDate
                         && x.DateTo >= termination.ApplicationDate)
                .Select(x => new { x.DateFrom, x.DateTo })
                .FirstOrDefault();

            if (fiscalYear == null)
            {
                ModelState.AddModelError("", "No fiscal year found for the selected application date.");
                return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
            }

            // Check if termination already exists in SAME fiscal year
            bool exists = db.Terminations.Any(x =>
                x.EmployeeId == termination.EmployeeId &&
                x.TerminationId != termination.TerminationId &&
                x.ApplicationDate >= fiscalYear.DateFrom &&
                x.ApplicationDate <= fiscalYear.DateTo
            );

            // 3. Get Placement + Notice Period
            var placement = db.Placements
                .Where(x => x.EmployeeId == termination.EmployeeId && x.isCurrent)
                .Select(x => new { x.ResponsibilityLevel })
                .FirstOrDefault();

            if (placement == null)
            {
                ModelState.AddModelError("", "No active placement found.");
                return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
            }

            int noticeDays = db.TerminationSettings
                .Where(x => x.IsActive && x.ResponsibilityLevel == placement.ResponsibilityLevel)
                .Select(x => (int?)x.NoticePeriodDays)
                .FirstOrDefault() ?? 30; // default = 30 days

            // 4. Termination Date Logic
            if (string.IsNullOrWhiteSpace(termination.TerminationDateAm)&& termination.TerminationDate == null)
            {
                // Auto-calculate if not provided
                termination.TerminationDate = termination.ApplicationDate.AddDays(noticeDays);
            }
            else
            {
                // Validate user input
                if (termination.TerminationDate < termination.ApplicationDate)
                {
                    ModelState.AddModelError("", "Termination date must be after application date.");
                    return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
                }
            }

            // Sync Ethiopic date
            termination.TerminationDateAm = HrHelper.GetECDate(termination.TerminationDate.Value);

            // 5. Business Rule (Minimum 30 days)
            var diffDays = (termination.TerminationDate.Value - termination.ApplicationDate).TotalDays;

            if (diffDays < 30 && !termination.IsAgreeRetunSalary)
            {
                ModelState.AddModelError("", "Less than 30 days requires salary compensation.");
                termination.Remark = "Notice period less than 30 days.";
                return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
            }

            var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == termination.EmployeeId);

            if (termination.IsEarlyRetirement && employee != null)
            {
                var age = DateTime.Now.Year - employee.DateOfBirth.Year;

                if (employee.DateOfBirth > DateTime.Now.AddYears(-age))
                {
                    age--;
                }

                if (age < 55)
                {
                    ModelState.AddModelError("", "Employee is not eligible for early retirement before age 55.");
                    return Json(new[] { termination }.ToDataSourceResult(request, ModelState));

                }
            }
            if (ModelState.IsValid)
            {

                var entity = new Core.Domain.HRM.EmployeeDetail.Termination
                {
                    TerminationId = termination.TerminationId,
                    EmployeeId = termination.EmployeeId,
                    LookupId = termination.LookupId,
                    Reason = termination.Reason,
                    TerminationDate = termination.TerminationDate,
                    ApplicationDate = termination.ApplicationDate,
                    EmployeeCode =  employee?.EmployeeCode ?? null,
                    Remark = termination.Remark,
                    TerminationDateAm = termination.TerminationDateAm,
                    ApplicationDateAm = termination.ApplicationDateAm,
                    DateCreated = termination.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = termination.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    OrgBranchName = termination.OrgBranchName,
                    IsForOther = termination.IsForOther,
                    IsEarlyRetirement = termination.IsEarlyRetirement,

                };
                db.Terminations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "TerminationRequest ", "Success", User.Identity.Name, User.Identity.Name + " update Termination Request at date " + DateTime.Today.ToShortDateString(), Modules.HRM);

                #endregion

                
            }
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
                        ApplicationDate = new DateTime(termination.TerminationDate.Value.Year, termination.TerminationDate.Value.Month, termination.TerminationDate.Value.Day),
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
                        IsEarlyRetirement = termination.IsEarlyRetirement,

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
       
        public JsonResult SendForApproval(int id)
        {
            var termination = db.Terminations.Find(id);
            if (termination == null)
            {
                return Json(new { Message = "Termination request not found.", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);
            }
            bool isAlreadyWaitingForApproval = db.RequestesStatus.Any(x => x.RequestId == id && x.Status == RequestStatus.WAITING_FOR_APPROVAL);
            
            if (termination.IsSendForApproval
                && isAlreadyWaitingForApproval)
            {
                return Json(new { Message = "The termination Request is already sent for approval.", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);
            }
            var requestStatus = new RequestesStatus
            {
                RequestId = id,
                ReferenceType = "Termination",
                Status = RequestStatus.WAITING_FOR_APPROVAL,
                Date = DateTime.Now,
                DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year)
            };

            termination.IsSendForApproval = true;
            termination.IsDepartmentSendForApproval = true;
            termination.SendForApprovalBy = User.Identity.Name;
            termination.Status = RequestStatus.WAITING_FOR_APPROVAL.ToString();
            termination.SendForApprovalTime = DateTime.Now;
            db.RequestesStatus.Add(requestStatus);
            db.Entry(termination).State = EntityState.Modified;
            db.SaveChanges();
            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var headId = EmployeeHelper.GetHeadId(empId);
            if (headId != 0)
            {
                NotifModels notif = new NotifModels
                {
                    UserId = UserInformationHelper.GetEmployeeUserId(headId),
                    Title = "Termination Request",
                    Message = $"{User.Identity.Name} has submitted a Employee Transfer Requests that requires your approval.",
                    URL = "~/HRM/TerminationRequestes"
                };

                Notify.NotifyUser(notif);
            }
            return Json(new { Message = "Request successfully sent for approval.", Status = "Success" }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
   
}
