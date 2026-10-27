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
using ExceedERP.Core.Domain.Global;

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
                IsEarlyRetirement = termination.IsEarlyRetirement,

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
                    EmployeeId = termination.EmployeeId,
                    IsSendForApproval = true,
                    SendForApprovalBy = User.Identity.Name,
                    SendForApprovalTime = DateTime.Now,
                    LookupId = termination.LookupId,
                    Reason = termination.Reason,
                    ApplicationDate = termination.ApplicationDate,
                    TerminationDate = termination.TerminationDate,
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
                    TerminationDateAm = termination.TerminationDateAm,
                    ApplicationDate = termination.ApplicationDate,
                    ApplicationDateAm = termination.ApplicationDateAm,
                    IsEarlyRetirement = termination.IsEarlyRetirement,

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
                    ApplicationDate =  termination.ApplicationDate,
                    ApplicationDateAm =  termination.ApplicationDateAm,
                    TerminationDate = termination.TerminationDate,
                    TerminationDateAm = termination.TerminationDateAm,
                    IsEarlyRetirement = termination.IsEarlyRetirement,

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
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AttachmentTemplateDocumentFiles_Create([DataSourceRequest] DataSourceRequest request, IEnumerable<HttpPostedFileBase> employeeProfileDocumentFiles, string attachmentReferenceType, int? id, string fileOwner = "")
        {
            if (!id.HasValue)
            {
                id = HRMInputs.EmployeeId;
                fileOwner = HRMInputs.EmployeeId.ToString();
            }
            if (employeeProfileDocumentFiles != null)
            {
                AttachmentType value = AttachmentType.OTHER;
                if (attachmentReferenceType != null)
                {
                    switch (attachmentReferenceType.ToLower())
                    {
                        case "photo":
                            value = AttachmentType.PHOTO;
                            break;
                        case "experience":
                            value = AttachmentType.EXPERIENCE;
                            break;
                        case "education":
                            value = AttachmentType.EDUCATION;
                            break;
                        case "training":
                            value = AttachmentType.TRAINING;
                            break;
                        case "clearance":
                            value = AttachmentType.CLEARANCE;
                            break;
                        case "commitment":
                            value = AttachmentType.COMMITMENT;
                            break;
                        case "incomingletter":
                            value = AttachmentType.INCOMING_LETTER;
                            break;
                        case "outgoingletter":
                            value = AttachmentType.OUTGOING_LETTER;
                            break;
                        case "other":
                            value = AttachmentType.OTHER;
                            break;
                        case "logo":
                            value = AttachmentType.LOGO;
                            break;
                        case "cosign":
                            value = AttachmentType.COSIGN;
                            HRMInputs.CosignId = id.Value;
                            break;
                        case "letter":
                            value = AttachmentType.LETTER;
                            HRMInputs.LetterId = id.ToString();
                            break;
                        case "minute":
                            value = AttachmentType.MINUTE;
                            break;
                        case "document":
                            value = AttachmentType.DOCUMENT;
                            break;
                        case "project_document":
                            value = AttachmentType.PROJECT_DOCUMENT;
                            break;
                    }
                }
                string[] AllowedExtensions = { ".jpg", ".png", ".pdf" };
                if (value == AttachmentType.PHOTO)
                {
                    AllowedExtensions = new[] { ".jpg", ".png" };
                }
                int MaxFileSize = 2 * 1024 * 1024;

                string subPath = "~/Content/Images/";
                string shortPath = "/Content/Images/";
                bool exists = Directory.Exists(Server.MapPath("~/Content/Images/HRMFiles/"));

                if (!exists)
                    Directory.CreateDirectory(Server.MapPath("~/Content/Images/HRMFiles/"));
                string fileName = "";
                foreach (var f in employeeProfileDocumentFiles)
                {

                    if (f != null)
                    {
                        #region file validation
                        var fileExtension = Path.GetExtension(f.FileName).ToLower();
                        if (!AllowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("File", "Invalid file type.");
                            return Json(new[] { employeeProfileDocumentFiles }.ToDataSourceResult(request, ModelState));

                        }

                        if (f.ContentLength > MaxFileSize)
                        {
                            ModelState.AddModelError("File", $"The uploaded file exceeds the maximum allowed size of {MaxFileSize / (1024 * 1024)} MB.");
                            return Json(new[] { employeeProfileDocumentFiles }.ToDataSourceResult(request, ModelState));

                        }
                        #endregion
                        if (ModelState.IsValid)
                        {
                            if (value == AttachmentType.PHOTO)
                            {
                                // Validate extension and MIME type
                                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                                var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                                var extension = Path.GetExtension(f.FileName).ToLower();

                                if (!allowedExtensions.Contains(extension) || !allowedMimeTypes.Contains(f.ContentType.ToLower()))
                                {
                                    ModelState.AddModelError("File", "Invalid file type. Only images are allowed.");
                                    return Json(new List<Attachment>().ToDataSourceResult(request, ModelState));
                                }

                                const int maxFileSize = 2 * 1024 * 1024; // 2MB
                                if (f.ContentLength > maxFileSize)
                                {
                                    ModelState.AddModelError("File", $"File exceeds {maxFileSize / (1024 * 1024)} MB.");
                                    return Json(new[] { employeeProfileDocumentFiles }.ToDataSourceResult(request, ModelState));
                                }
                                var employeeId = HRMInputs.EmployeeId;
                                // Build path
                                var folderName = $"{employeeId}-{AttachmentType.PHOTO}";
                                var folderPath = Path.Combine(Server.MapPath("~/Content/Images/"), folderName);
                                if (!Directory.Exists(folderPath))
                                    Directory.CreateDirectory(folderPath);

                                var fileNamee = $"{employeeId}{extension}";
                                var path = Path.Combine(folderPath, fileNamee);

                                // Delete existing file if exists
                                if (System.IO.File.Exists(path))
                                    System.IO.File.Delete(path);

                                // Save new file
                                f.SaveAs(path);

                                // Remove existing attachment records
                                var existing = db.Attachments.Where(a => a.Reference == employeeId.ToString() && a.ReferenceType == AttachmentType.PHOTO);
                                if (existing.Any())
                                {
                                    db.Attachments.RemoveRange(existing);
                                    db.SaveChanges();
                                }

                                // Add new attachment record
                                var employee = db.Person_Employee.Find(employeeId);
                                var attachment = new Attachment
                                {
                                    Reference = employeeId.ToString(),
                                    Description = employee != null ? $"{employee.EmployeeCode}{extension}" : f.FileName,
                                    FileType = f.ContentType,
                                    ReferenceType = AttachmentType.PHOTO,
                                    Index = 0,
                                    URL = path,
                                    Remark = HRMInputs.Owner
                                };

                                db.Attachments.Add(attachment);
                                db.SaveChanges();
                            }
                            else
                            {
                                fileName = Path.GetFileName(f.FileName);
                                string physicalPath = Server.MapPath("~/Content/Images/" + fileName);
                                //   string physicalPath = Server.MapPath("~/Content/Memberimage/" + FileName);
                                f.SaveAs(physicalPath);
                                db.Attachments.Add(new Attachment()
                                {
                                    ReferenceType = value,
                                    Reference = id.ToString(),
                                    Description = value.ToString(),
                                    URL = physicalPath,
                                    BinaryFile = ConvertToBytes(f),
                                    FileType = fileExtension,
                                    Index = 0,
                                    Remark = fileOwner,
                                    FileName = Path.GetFileName(f.FileName),

                                });
                                db.SaveChanges();
                            }

                        }
                    }
                }
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "HRM Attachment template", "Success", User.Identity.Name, $"uploaded HRM Attachment template {fileName} ", Modules.HRM);
                #endregion

            }
            return Json(new[] { employeeProfileDocumentFiles }.ToDataSourceResult(request, ModelState));

        }
        private byte[] ConvertToBytes(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
                return null;

            using (var memoryStream = new MemoryStream())
            {
                file.InputStream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }

}
