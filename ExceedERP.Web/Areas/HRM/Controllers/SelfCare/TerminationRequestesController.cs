using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    [AuthorizeRoles(HrmConstants.HRSelfServiceManager, HrmConstants.HRTerminationUser, HrmConstants.HRDepartmentHead, HrmConstants.HRAdmin)]
    public class TerminationRequestesController : BaseController
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
                                  EmployeeId = e.EmployeeId,
                                  FullNameAmharic = e.FirstName + " " + e.MiddleName + " " + e.LastName,
                                  FullNameaEnglish = e.FirstNameEnglish + " " + e.MiddleNameEnglish + " " + e.LastNameEnglish
                              }).AsQueryable();
            // Assign to ViewData for Kendo Grid ForeignKey binding
            ViewData["EmployeeList"] = employees; // For EmployeeId column
            ViewData["EmployeeName"] = employees;    // For ApproverId column
            ViewData["Department"] = db.MainCompanyStructures.AsQueryable();
            ViewData["TerminationTypes"] = db.Lookups.Where(l => l.LookupType == LookUpTypes.TERMINATIONTYPE).AsQueryable();
            ViewData["isEthiopicDate"] = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate;

        }
        

        public ActionResult Terminations_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Core.Domain.HRM.EmployeeDetail.Termination> terminations = db.Terminations.Where(x=>x.IsSendForApproval).OrderByDescending(x => x.ApplicationDate);
            
            DataSourceResult result = terminations.ToDataSourceResult(request, termination => new
            {
                TerminationId = termination.TerminationId,
                EmployeeId = termination.EmployeeId,
                LookupId = termination.LookupId,
                Reason = termination.Reason,
                TerminationDate = termination.TerminationDate,
                ApplicationDate = termination.ApplicationDate,
                Remark = termination.Remark,
                EmployeeCode = termination.EmployeeCode,
                TerminationDateAm = termination.TerminationDateAm,
                ApplicationDateAm = termination.ApplicationDateAm,
                DepartmentSendForApprovalBy = termination.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = termination.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = termination.IsDepartmentSendForApproval,
                DepartmentOnlineApprovedBy = termination.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = termination.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = termination.IsDepartmentOnlineApproved,
                OnlineApprovedBy = termination.OnlineApprovedBy,
                OnlineApprovedTime = termination.OnlineApprovedTime,
                IsOnlineApproved = termination.IsOnlineApproved,
                IsForciblyTerminated = termination.IsForciblyTerminated
            });

            return Json(result);
        }

        public void SetEmployeeId(string fullName)
        {
            List<EmployeeIdView> EmployeeIdViews = new List<EmployeeIdView>();
            EmployeeIdView EmployeeIdView = new EmployeeIdView();
            var employeeList = db.Person_Employee.AsQueryable();
            if (employeeList.Count() > 0)
            {
                employeeList = employeeList.Where(l => l.IsActive).AsQueryable();
            }
            foreach (var emp in employeeList)
            {
                EmployeeIdView = new EmployeeIdView();
                EmployeeIdView.PersonId = emp.EmployeeId;
                EmployeeIdView.FullName = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
                EmployeeIdViews.Add(EmployeeIdView);
            }
            var employees = EmployeeIdViews.Where(p => p.FullName == fullName).FirstOrDefault();
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
            if (ModelState.IsValid)
            {
                var entity = new Core.Domain.HRM.EmployeeDetail.Termination
                {
                    EmployeeId = termination.EmployeeId,
                    LookupId = termination.LookupId,
                    Reason = termination.Reason,
                    ApplicationDate = termination.ApplicationDate,
                    TerminationDate = new DateTime(termination.TerminationDate.Year, termination.TerminationDate.Month, termination.TerminationDate.Day),
                    Remark = termination.Remark
                };

                db.Terminations.Add(entity);
                db.SaveChanges();
                db = new ExceedDbContext();
                termination.TerminationId = entity.TerminationId;
                var id = db.Terminations.Max(x => x.TerminationId);
                var requestentity = new RequestesStatus
                {
                    RequestId = id,
                    ReferenceType = "Termination",
                    Status = RequestStatus.APPROVE,
                    Date = DateTime.Now,
                    Remark = "Direct"
                };
                db.RequestesStatus.Add(requestentity);
                db.SaveChanges();
                requestentity.RequestStatusId = requestentity.RequestStatusId;

            }

            return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Terminations_Update([DataSourceRequest]DataSourceRequest request, Core.Domain.HRM.EmployeeDetail.Termination termination)
        {
            termination.EmployeeId = HRMInputs.EmployeeId;
            if (ModelState.IsValid)
            {
                var entity = new Core.Domain.HRM.EmployeeDetail.Termination
                {
                    TerminationId = termination.TerminationId,
                    EmployeeId = termination.EmployeeId,
                    LookupId = termination.LookupId,
                    Reason = termination.Reason,
                    TerminationDate = new DateTime(termination.TerminationDate.Year, termination.TerminationDate.Month, termination.TerminationDate.Day),
                    ApplicationDate = termination.ApplicationDate,
                    Remark = termination.Remark
                };

                db.Terminations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Terminations_Destroy([DataSourceRequest]DataSourceRequest request, Core.Domain.HRM.EmployeeDetail.Termination termination)
        {
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
                    Remark = termination.Remark
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
