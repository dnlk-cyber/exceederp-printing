using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Hubs;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.Termination
{

    [AuthorizeRoles(HrmConstants.HRTerminationUser, HrmConstants.HRAdmin)]
    public class TerminationController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static DateTime ApplicationDate { get; set; }
        public static string ApplicationDateAm { get; set; }
        public static DateTime TerminationDate { get; set; }
        public static string TerminationDateAm { get; set; }
        HRMInputs inputs = new HRMInputs();
        string ErrorMessage = "";
        public ActionResult Index()
        {
            var periodList = db.GLFiscalYears.ToList();
            ViewData["PeriodList"] = periodList;
            SetValues();

            return View();
        }
        private bool CheckTerminationValidation(int id)
        {
            ErrorMessage = "";


            #region ExitInterviews
            var ExitInterviews = db.ExitInterviews.Where(x => x.EmployeeId == id).FirstOrDefault();
            if (ExitInterviews != null)
            {

                ErrorMessage = "You cannot delete this Termination data.Please delete Exit Interviewl information first.";
                return false;


            }
            #endregion

            return true;
        }
        public void SetDate(string name, string value)
        {
            if (name == "TerminationDateAm")
            {
                TerminationDateAm = value;
                var splitDate = TerminationDateAm.Split('/');
                TerminationDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else if (name == "ApplicationDateAm")
            {
                ApplicationDateAm = value;
                var splitDate = ApplicationDateAm.Split('/');
                ApplicationDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
        }
        private void SetValues()
        {
            var terminationTypes = db.Lookups.Where(l => l.LookupType == LookUpTypes.TERMINATIONTYPE);
            ViewData["TerminationTypes"] = terminationTypes;
            ViewData["EmployeeName"] = db.Person_Employee.ToList().Select(e => new { e.EmployeeId, FullNameAmharic = e.EmployeeCode + " " + e.FullNameAmharic, FullNameaEnglish = e.EmployeeCode + " " + e.FullNameaEnglish}).ToList();
            var list = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list;//.Where(x => x.IsActive);
        }
        public JsonResult TerminateEmployee(int id)
        {

            var termination = db.Terminations.FirstOrDefault(x => x.TerminationId == id);
            if (termination != null)
            {
                if (termination.IsOnlineApproved)
                {
                    var formatedData = new
                    {
                        Message = "This request is already Terminated",
                        Status = "FAIL"
                    };
                    return Json(formatedData, JsonRequestBehavior.AllowGet);
                }
              
                else
                {
                    termination.IsOnlineApproved = true;

                    db.Entry(termination).State = EntityState.Modified;
                    //db.SaveChanges();
                    #region change resquest status 
                    var status = new RequestesStatus
                    {
                        RequestId = termination.TerminationId,
                        Date = DateTime.Now.Date,
                        ReferenceType = "Termination",
                        Status = RequestStatus.APPROVE

                    };
                    db.RequestesStatus.Add(status);

                    #endregion

                    db.SaveChanges();

                    #region Nofify Commitees
                    var commitment = db.Commitments.FirstOrDefault(x => x.EmployeeId == termination.EmployeeId);
                    if (commitment != null)
                    {
                        var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == termination.EmployeeId);
                        if (emp != null)
                        {
                            var superviorId = EmployeeHelper.GetSupervisor(emp.EmployeeId);

                            NotifModels notif = new NotifModels
                            {

                                UserId = UserInformationHelper.GetEmployeeUserId(superviorId),
                                Title = "Terminination",
                                Message = $" {UserInformationHelper.GetEmployeeFullName(User.Identity.Name)}  has Terminate remove his Commitment",
                                URL = "~/HRM/Termination"
                            };

                            Notify.NotifyUser(notif);

                        }

                    }
                    #endregion

                    #region Notification

                    //var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == termination.EmployeeId);
                    //if (person != null)
                    //{
                    //    var empid = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                    //    var superviorId = EmployeeHelper.GetSupervisor(empid);

                    //    NotifModels notif = new NotifModels
                    //    {

                    //        UserId = UserInformationHelper.GetEmployeeUserId(superviorId),
                    //        Title = "Employee Termination",
                    //        Message = $" {person.FullNameaEnglish}  is terminated and removed from Payroll",
                    //        URL = "~/HRM/Termination"
                    //    };

                    //    Notify.NotifyUser(notif);
                    //}

                    #endregion

                    var formatedData = new
                    {
                        Message = "This employee is Forcible Terminated successfully",
                        Status = "OK"
                    };
                    return Json(formatedData, JsonRequestBehavior.AllowGet);
                }
            }
            var formatedData1 = new
            {
                Message = "Please select employee",
                Status = "Fail"
            };
            return Json(formatedData1, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ForceTerminateEmployee(int id)
        {
            var termination = db.Terminations.FirstOrDefault(x => x.TerminationId == id);
            if (termination != null)
            {
                termination.IsOnlineApproved = true;
                termination.IsForciblyTerminated = true;
                termination.Remark += "Terminated back by " + User.Identity.Name+" ";

                db.Entry(termination).State = EntityState.Modified;

                #region change resquest status 

                var statusDep = new RequestesStatus
                {
                    RequestId = termination.TerminationId,
                    Date = DateTime.Now.Date,
                    ReferenceType = "Termination",
                    Status = RequestStatus.DEPARTMENT_APPROVAL
                };
                db.RequestesStatus.Add(statusDep);

                var statusApp = new RequestesStatus
                {
                    RequestId = termination.TerminationId,
                    Date = DateTime.Now.Date,
                    ReferenceType = "Termination",
                    Status = RequestStatus.APPROVE
                };
                db.RequestesStatus.Add(statusApp);

                #endregion

                db.SaveChanges();

                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == termination.EmployeeId);
                if (person != null)
                {
                    person.IsActive = false;

                    db.Entry(person).State = EntityState.Modified;
                    db.SaveChanges();
                }

                var placements = db.Placements.Where(x => x.EmployeeId == termination.EmployeeId).ToList();

                foreach (var place in placements)
                {
                    place.isCurrent = false;

                    db.Entry(place).State = EntityState.Modified;
                    db.SaveChanges();
                }

                var formatedData = new
                {
                    Message = "This employee is successfully Terminated",
                    Status = "OK"
                };
                return Json(formatedData, JsonRequestBehavior.AllowGet);

            }
            var formatedData1 = new
            {
                Message = "Please select employee",
                Status = "Fail"
            };
            return Json(formatedData1, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UnterminateEmployee(int id)
        {
            var termination = db.Terminations.Where(x=>x.IsForciblyTerminated || x.IsOnlineApproved).FirstOrDefault(x => x.TerminationId == id);
            if (termination != null)
            {
                termination.IsOnlineApproved = false;
                termination.IsForciblyTerminated = false;
                termination.Remark += "Unterminated back by " + User.Identity.Name+",";

                db.Entry(termination).State = EntityState.Modified;

                #region change resquest status 

                var statusDep = new RequestesStatus
                {
                    RequestId = termination.TerminationId,
                    Date = DateTime.Now.Date,
                    ReferenceType = "Termination",
                    Status = RequestStatus.DEPARTMENT_APPROVAL
                };
                db.RequestesStatus.Add(statusDep);

                var statusApp = new RequestesStatus
                {
                    RequestId = termination.TerminationId,
                    Date = DateTime.Now.Date,
                    ReferenceType = "Termination",
                    Status = RequestStatus.REJECT
                };
                db.RequestesStatus.Add(statusApp);

                #endregion

                db.SaveChanges();

                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == termination.EmployeeId);
                if (person != null)
                {
                    person.IsActive = true;

                    db.Entry(person).State = EntityState.Modified;
                    db.SaveChanges();
                }

                var placements = db.Placements.Where(x => x.EmployeeId == termination.EmployeeId).ToList();

                foreach (var place in placements)
                {
                    place.isCurrent = true;
                    db.Entry(place).State = EntityState.Modified;
                    db.SaveChanges();
                }

                var formatedData = new
                {
                    Message = "This employee is successfully un-Terminated",
                    Status = "OK"
                };
                return Json(formatedData, JsonRequestBehavior.AllowGet);

            }
            var formatedData1 = new
            {
                Message = "Please select employee",
                Status = "Fail"
            };
            return Json(formatedData1, JsonRequestBehavior.AllowGet);
        }

        public JavaScriptResult UpdateTerminationStatus()
        {
            var terminations = db.Terminations.ToList();
            foreach (var term in terminations)
            {
                var requestStatus = db.RequestesStatus.FirstOrDefault(x => x.RequestId == term.TerminationId && x.ReferenceType == "Termination");
                if (requestStatus != null && requestStatus.Status == RequestStatus.APPROVE)
                {
                    term.IsDepartmentOnlineApproved = true;
                    term.IsOnlineApproved = true;

                    db.Entry(term).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            string message = "All termination status are updated successfully";
            return new JavaScriptResult() { Script = "alert('" + message + "');" };
        }
        public bool CheckIfActive(int empId)
        {
            /// this function return 'True' if the employee is active in profile or he is current in placement 
            /// and return false if the employee is InActive in profile and Not Current in placement 
            var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId);
            if (person != null && person.IsActive)
            {
                return true;
            }
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent);
            if (placement != null)
            {
                return true;
            }

            return false;
        }

        [AuthorizeRoles(HrmConstants.HRTerminationUser, HrmConstants.HRAdmin)]
        public ActionResult Terminations_Read([DataSourceRequest]DataSourceRequest request)
        {
            db.Database.CommandTimeout = 7200;//added
            //List<int> employees = inputs.GetUserBranchAndBusinessUnitEmployees(User.Identity.Name);
            IQueryable<Core.Domain.HRM.EmployeeDetail.Termination> terminations = db.Terminations/*.Where(x => employees.Contains(x.EmployeeId))*/;
            //List<Core.Domain.HRM.EmployeeDetail.Termination> terminationList = new List<Core.Domain.HRM.EmployeeDetail.Termination>();
            //foreach (var t in terminations)
            //{
            //    db = new ExceedDbContext();
            //    var requestStatus =
            //        db.RequestesStatus.Where(x => x.ReferenceType == "Termination" && x.RequestId == t.TerminationId && x.Status == RequestStatus.APPROVE).FirstOrDefault();
            //    if (requestStatus != null)
            //    {
            //        terminationList.Add(t);
            //    }
            //}
            //terminations = terminationList.AsQueryable();
            DataSourceResult result = terminations.ToDataSourceResult(request, termination => new
            {
                TerminationId = termination.TerminationId,
                EmployeeId = termination.EmployeeId,
                LookupId = termination.LookupId,
                Reason = termination.Reason,
                TerminationDate = termination.TerminationDate,
                ApplicationDate = termination.ApplicationDate,
                Remark = termination.Remark,
                TerminationDateAm = termination.TerminationDateAm,
                ApplicationDateAm = termination.ApplicationDateAm,
                DateCreated = termination.DateCreated,
                LastModified = termination.LastModified,
                CreatedBy = termination.CreatedBy,
                ModifiedBy = termination.ModifiedBy,
                OrgBranchName = termination.OrgBranchName,
                EmployeeCode = termination.EmployeeCode,
                IsOnlineApproved = termination.IsOnlineApproved,
                IsDirect = termination.IsDirect,
                IsForciblyTerminated = termination.IsForciblyTerminated,
                IsActive = CheckIfActive(termination.EmployeeId)
                //TerminationLevel = termination.TerminationLevel
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRTerminationUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Terminations_Create([DataSourceRequest]DataSourceRequest request, Core.Domain.HRM.EmployeeDetail.Termination termination)
        {
            ModelState.Clear();

            if (termination.TerminationDateAm != null)
            {
                var splitDate = termination.TerminationDateAm.Split('/');
                termination.TerminationDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else
            {
                ModelState.AddModelError("", "Termination Date Is Required");
            }

            if (termination.ApplicationDateAm != null)
            {
                var splitDate = termination.ApplicationDateAm.Split('/');
                termination.ApplicationDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else
            {
                ModelState.AddModelError("", "Termination Date Is Required");
            }

            var checkExistance = db.Terminations.FirstOrDefault(x => x.EmployeeId == termination.EmployeeId);
            if (checkExistance != null)
            {
                ModelState.AddModelError("", "This employee already request the termination!!");
            }

            if (termination.TerminationDate < termination.ApplicationDate)
            {
                ModelState.AddModelError("", "Termination Date Can't Be Below Application Date");
            }

            if (termination.ApplicationDate > DateTime.Now.Date)
            {

                ModelState.AddModelError("", "Application date should be less or equal to the current date!!");
            }

            if (ModelState.IsValid)
            {
                var diff = (termination.TerminationDate - termination.ApplicationDate).Days;

                var entity = new Core.Domain.HRM.EmployeeDetail.Termination
                {
                    EmployeeId = termination.EmployeeId,
                    LookupId = termination.LookupId,
                    Reason = termination.Reason,
                    ApplicationDate = termination.ApplicationDate,
                    TerminationDate = termination.TerminationDate,
                    Remark = termination.Remark,
                    TerminationDateAm = termination.TerminationDateAm,
                    ApplicationDateAm = termination.ApplicationDateAm,
                    DateCreated = DateTime.Now,
                    LastModified = termination.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = termination.ModifiedBy,
                    IsOnlineApproved = termination.IsOnlineApproved,
                    IsDirect = true,
                    EmployeeCode = db.Person_Employee.Find(termination.EmployeeId).EmployeeCode,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)
                    //TerminationLevel = termination.TerminationLevel

                };

                db.Terminations.Add(entity);
                db.SaveChanges();
                db = new ExceedDbContext();

                termination.TerminationId = entity.TerminationId;


                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Termination ", "Success", User.Identity.Name, User.Identity.Name + " add termination of id " + entity.TerminationId, Modules.HRM);

                #endregion

                if (diff < 30)
                {
                    string messages = "Your termination request date length is less than 30. You need to return one month salary to finance!!";
                    termination.Remark = "The Date Length is less than 30.";
                    return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                }


            }
            return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRTerminationUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Terminations_Update([DataSourceRequest]DataSourceRequest request, Core.Domain.HRM.EmployeeDetail.Termination termination)
        {
            ModelState.Clear();

            if (termination.TerminationDateAm != null)
            {
                var splitDate = termination.TerminationDateAm.Split('/');
                termination.TerminationDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            //else
            //{
            //    ModelState.AddModelError("", "Termination Date Is Required");
            //}

            if (termination.ApplicationDateAm != null)
            {
                var splitDate = termination.ApplicationDateAm.Split('/');
                termination.ApplicationDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            //else
            //{
            //    ModelState.AddModelError("", "Termination Date Is Required");
            //}

            var checkExistance = db.Terminations.FirstOrDefault(x => x.EmployeeId == termination.EmployeeId&&x.TerminationId != termination.TerminationId);
            if (checkExistance != null)
            {
                ModelState.AddModelError("", "This employee already request the termination!!");
            }

            if (termination.TerminationDate < termination.ApplicationDate)
            {
                ModelState.AddModelError("", "Termination Date Can't Be Below Application Date");
            }

            if (termination.ApplicationDate > DateTime.Now.Date)
            {

                ModelState.AddModelError("", "Application date should be less or equal to the current date!!");
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
                    Remark = termination.Remark,
                    TerminationDateAm = termination.TerminationDateAm,
                    ApplicationDateAm = termination.ApplicationDateAm,
                    DateCreated = termination.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = termination.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    OrgBranchName = termination.OrgBranchName,
                    EmployeeCode = db.Person_Employee.Find(termination.EmployeeId).EmployeeCode,
                    IsDirect = termination.IsDirect,
                    IsOnlineApproved = termination.IsOnlineApproved,
                    //TerminationLevel = termination.TerminationLevel
                };

                db.Terminations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                var diff = (TerminationDate - ApplicationDate).TotalDays;


                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Termination ", "Success", User.Identity.Name, User.Identity.Name + " update termination of id " + entity.TerminationId, Modules.HRM);

                #endregion

                if (diff < 30)
                {
                    string messages = "Your termination request date length is less than 30. You need to return one month salary to finance!!";
                    termination.Remark = "The Date Length is less than 30.";
                    return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                }

            }

            return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Terminations_Destroy([DataSourceRequest]DataSourceRequest request, Core.Domain.HRM.EmployeeDetail.Termination termination)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (!CheckTerminationValidation(termination.EmployeeId))
                {
                    ModelState.AddModelError("", ErrorMessage);
                }
                else
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
                        DateCreated = termination.DateCreated,
                        LastModified = termination.LastModified,
                        CreatedBy = termination.CreatedBy,
                        ModifiedBy = termination.ModifiedBy,
                        OrgBranchName = termination.OrgBranchName,
                        IsOnlineApproved = termination.IsOnlineApproved,
                        IsDirect = termination.IsDirect,
                        EmployeeCode = termination.EmployeeCode
                        //TerminationLevel = termination.TerminationLevel
                    };
                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted termination information of employee " + termination.EmployeeCode);
                    #endregion
                    db.Terminations.Attach(entity);
                    db.Terminations.Remove(entity);
                    db.SaveChanges();


                    #region activate employee
                    //var employee = db.Person_Employee.Find(termination.EmployeeId);
                    //if (employee != null)
                    //{
                    //    employee.IsActive = true;
                    //    db.Entry(employee).State = EntityState.Modified;
                    //    db.SaveChanges();
                    //}
                    #endregion
                }
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
                            URL = path,
                            Remark = HRMInputs.Owner

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
                        db.Attachments.Where(x => x.Reference == filename && x.ReferenceType == AttachmentType.CLEARANCE && x.Remark == HRMInputs.Owner && x.AttachmentId == HRMInputs.SelectedFile)
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
                    byte[] datas = req.DownloadData(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.CLEARANCE + "/" + attachment.Description));
                    response.BinaryWrite(datas);
                    response.End();
                    HRMInputs.SelectedFile = 0;
                    return File(filename, attachment.FileType);
                }
            }
            catch (Exception)
            {

                // throw;
            }
            HRMInputs.SelectedFile = 0;
            RedirectToAction("Index");
            return null;
        }
        public void Download(int id)
        {
            HRMInputs.SelectedFile = id;

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
