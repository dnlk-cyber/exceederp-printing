using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using ExceedERP.Core.Domain.Global;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Web.Models;
using System.Net;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    [Authorize]
    public class ManPowerPlanningRequestController : Controller
    {
        // GET: HRM/ManPowerPlanningRequest
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            List<SelectListItem> requestStatusList = new List<SelectListItem>();

            var requests = Enum.GetValues(typeof(RequestStatus))
                         .Cast<RequestStatus>()
                         .Select(v => v)
                         .ToList();           

            foreach (var req in requests)
            {
                var selectedList = new SelectListItem
                {
                    Text = req.ToString(),
                    Value = req.ToString()
                };
                requestStatusList.Add(selectedList);
            }

            ViewBag.RequestStatus = requests;

            PopulateViewData();
           

            return View();
        }
        private void PopulateViewData()
        {
            var employeName = db.Person_Employee.ToList();
            ViewData["EmployeeName"] = employeName;
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            var structres = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).AsQueryable();
            ViewData["jobTitles"] = structres;
            ViewData["branches"] = db.Branch;//.Where(x => mybraches.Contains(x.BranchId.ToString()));


            HRMInputs inputs = new HRMInputs();
            var list = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);

            //List<MainCompanyStructure> allDepartment = new List<MainCompanyStructure>();
            var departments = db.MainCompanyStructures.ToList();
            //allDepartment.AddRange(departments);
            ViewData["Departments"] = departments;
            var periodList = db.GLFiscalYears;
            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

        }
        public ActionResult SendForApproval()
        {
            var status =
                     db.RequestesStatus.Where(x => x.RequestId == HRMInputs.RequestId && x.ReferenceType == HRMInputs.ReferenceType).ToList();
            string messages = "";
            if (status.Any())
            {
                foreach (var s in status)
                {
                    if (s.Status == RequestStatus.APPROVE)
                    {
                        messages = "The Request is already approved!!";
                        return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                    }
                    if (s.Status == RequestStatus.WAITING_FOR_APPROVAL)
                    {
                        messages = "The Request is already send for approval!!";
                        return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                    }

                }
            }
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
                    Remark = requestesStatus.Remark,
                    DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(requestesStatus.Date.Day, requestesStatus.Date.Month, requestesStatus.Date.Year),
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
        public ActionResult ManPowerPlanningRequest_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ManPowerPlanningRequest> manPowerPlanningRequests = db.ManPowerPlanningRequests;
            DataSourceResult result = manPowerPlanningRequests.ToDataSourceResult(request, manPowerPlanningRequest => new
            {
                ManPowerPlanningRequestId = manPowerPlanningRequest.ManPowerPlanningRequestId,
                OrgStructureId = manPowerPlanningRequest.OrgStructureId,
                GlFiscalYearId = manPowerPlanningRequest.GlFiscalYearId,
                EmploymentType = manPowerPlanningRequest.EmploymentType,
                LookupId = manPowerPlanningRequest.LookupId,
                NoOfCurrentStaff = manPowerPlanningRequest.NoOfCurrentStaff,
                NoOfShouldBeStaff = manPowerPlanningRequest.NoOfShouldBeStaff,
                NumberOfStaff = manPowerPlanningRequest.NumberOfStaff,
                PlanType = manPowerPlanningRequest.PlanType,
                Date = manPowerPlanningRequest.Date,
                CreatedBy = manPowerPlanningRequest.CreatedBy,
                DateCreated = manPowerPlanningRequest.DateCreated,
                ModifiedBy = manPowerPlanningRequest.ModifiedBy,
                LastModified = manPowerPlanningRequest.LastModified,
                OrgBranchName = manPowerPlanningRequest.OrgBranchName,
                BranchId = manPowerPlanningRequest.BranchId,
                IsNewJobTitle = manPowerPlanningRequest.IsNewJobTitle,
                Reason = manPowerPlanningRequest.Reason,
                Experience = manPowerPlanningRequest.Experience,
                Education = manPowerPlanningRequest.Education,
                JobTitle = manPowerPlanningRequest.JobTitle,
                Remark = manPowerPlanningRequest.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ManPowerPlanningRequest_Create([DataSourceRequest]DataSourceRequest request, ManPowerPlanningRequest planningRequest)
        {
            ApplicationDbContext adb =new ApplicationDbContext();
            ModelState.Clear();

            var username = User.Identity.Name;
            var user = adb.Users.FirstOrDefault(x=>x.UserName == username);
            if(user != null)
            {
                var placement = db.Placements.FirstOrDefault(x=>x.EmployeeId == user.EmployeeId && x.isCurrent);
                planningRequest.OrgStructureId = placement.OrgStructureId;
            }

            if(planningRequest.IsNewJobTitle)
            {
                planningRequest.LookupId = 0;
                if(planningRequest.JobTitle == null || planningRequest.JobTitle.Trim() == "")
                {
                    ModelState.AddModelError("","If you select is new job title you have to add jobtitle");
                }
            }
            else
            {
                if(planningRequest.LookupId == null)
                {
                    ModelState.AddModelError("","You have to select job title");
                }
            }
            planningRequest.Date = DateTime.Today;
            planningRequest.DateCreated = DateTime.Today;
            planningRequest.CreatedBy = User.Identity.Name;
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(planningRequest);

                db.ManPowerPlanningRequests.Add(entity);
                db.SaveChanges();
                planningRequest.ManPowerPlanningRequestId = entity.ManPowerPlanningRequestId;

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "ManPowerPlanningReqeust ", "Success", User.Identity.Name, User.Identity.Name + " add new Manpower planning " + DateTime.Today.ToShortDateString(), Modules.HRM);
                
                #endregion

            }

            return Json(new[] { planningRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ManPowerPlanningRequest_Update([DataSourceRequest]DataSourceRequest request, ManPowerPlanningRequest planningRequest)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (planningRequest.IsNewJobTitle)
                {
                    planningRequest.LookupId = 0;
                }
                planningRequest.Date = DateTime.Today;
                planningRequest.LastModified = DateTime.Today;
                planningRequest.ModifiedBy = User.Identity.Name;

                var entity = GetMappedObject(planningRequest);
                db.ManPowerPlanningRequests.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "ManPowerPlanningReqeust ", "Success", User.Identity.Name, User.Identity.Name + " update Manpower planning " + DateTime.Today.ToShortDateString(), Modules.HRM);

                #endregion

            }

            return Json(new[] { planningRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ManPowerPlanningRequest_Destroy([DataSourceRequest]DataSourceRequest request, ManPowerPlanningRequest planningRequest)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(planningRequest);
                db.ManPowerPlanningRequests.Attach(entity);
                db.ManPowerPlanningRequests.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { planningRequest }.ToDataSourceResult(request, ModelState));
        }

        private ManPowerPlanningRequest GetMappedObject(ManPowerPlanningRequest manPowerPlanningRequest)
        {
            var entity = new ManPowerPlanningRequest
            {
                ManPowerPlanningRequestId = manPowerPlanningRequest.ManPowerPlanningRequestId,
                OrgStructureId = manPowerPlanningRequest.OrgStructureId,
                GlFiscalYearId = manPowerPlanningRequest.GlFiscalYearId,
                EmploymentType = manPowerPlanningRequest.EmploymentType,
                LookupId = manPowerPlanningRequest.LookupId,
                NoOfCurrentStaff = manPowerPlanningRequest.NoOfCurrentStaff,
                NoOfShouldBeStaff = manPowerPlanningRequest.NoOfShouldBeStaff,
                NumberOfStaff = manPowerPlanningRequest.NumberOfStaff,
                PlanType = manPowerPlanningRequest.PlanType,
                Date = manPowerPlanningRequest.Date,
                CreatedBy = manPowerPlanningRequest.CreatedBy,
                DateCreated = manPowerPlanningRequest.DateCreated,
                ModifiedBy = manPowerPlanningRequest.ModifiedBy,
                LastModified = manPowerPlanningRequest.LastModified,
                OrgBranchName = manPowerPlanningRequest.OrgBranchName,
                BranchId = manPowerPlanningRequest.BranchId,
                IsNewJobTitle = manPowerPlanningRequest.IsNewJobTitle,
                Reason = manPowerPlanningRequest.Reason,
                Experience = manPowerPlanningRequest.Experience,
                Education = manPowerPlanningRequest.Education,
                JobTitle = manPowerPlanningRequest.JobTitle,
                Remark = manPowerPlanningRequest.Remark
            };
            return entity;

        }
        public string CheckStatus(int masterId)
        {
            var reques = db.RequestesStatus.FirstOrDefault(x => x.RequestId == masterId && x.ReferenceType == "ManPowerPlanning");
            if (reques != null)
            {
                var approve = db.RequestesStatus.FirstOrDefault(x => x.RequestId == masterId && x.ReferenceType == "ManPowerPlanning" && x.Status == RequestStatus.APPROVE);

                if (approve != null)
                {
                    return "approved";
                }
                var deptApprove = db.RequestesStatus.FirstOrDefault(x => x.RequestId == masterId && x.ReferenceType == "ManPowerPlanning" && x.Status == RequestStatus.DEPARTMENT_APPROVAL);
                if(deptApprove != null)
                {
                    return "dept_approved";
                }
                var reject = db.RequestesStatus.FirstOrDefault(x => x.RequestId == masterId && x.ReferenceType == "ManPowerPlanning" && x.Status == RequestStatus.REJECT);
                if (reject != null)
                {
                    return "reject";
                }
                var waiting = db.RequestesStatus.FirstOrDefault(x => x.RequestId == masterId && x.ReferenceType == "ManPowerPlanning" && x.Status == RequestStatus.WAITING_FOR_APPROVAL);
                if (waiting != null)
                {
                    return "waiting";
                }
                var pend = db.RequestesStatus.FirstOrDefault(x => x.RequestId == masterId && x.ReferenceType == "ManPowerPlanning" && x.Status == RequestStatus.PEND);
                if (pend != null)
                {
                    return "pend";
                }
            }
            return "notsent";
        }
        public void Download(int id)
        {
            HRMInputs.SelectedFile = id;

        }
        //public ActionResult DownloadDoument(string filename, string attachId)
        //{
        //    if (attachId == "0")
        //    {
        //        string messages = "Please select the row first!!";
        //        return new JavaScriptResult() { Script = "alert('" + messages + "');" };
        //    }

        //    db = new ExceedDbContext();
        //    //try
        //    //{
        //    int.TryParse(attachId, out int attachmentId);
        //    var attachment =
        //            db.Attachments.FirstOrDefault(x => x.Reference == filename && x.ReferenceType == AttachmentType.DOCUMENT && x.AttachmentId == attachmentId);

        //    if (attachment != null)
        //    {
        //        WebClient req = new WebClient();
        //        HttpResponse response = System.Web.HttpContext.Current.Response;
        //        response.Clear();
        //        response.ClearContent();
        //        response.ClearHeaders();
        //        response.Buffer = true;
        //        response.AddHeader("Content-Disposition",
        //            "attachment;filename=\"" + attachment.Description + "\"");
        //        byte[] datas = req.DownloadData(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.DOCUMENT + "/" + attachment.Description));

        //        // byte[] datas11 = req.DownloadData(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.PHOTO + "/" + attachment.Description));

        //        response.BinaryWrite(datas);
        //        response.End();
        //        HRMInputs.SelectedFile = 0;
        //        return File(filename, attachment.FileType);
        //    }
        //    //}
        //    //catch (Exception)
        //    //{

        //    //    // throw;
        //    //}
        //    HRMInputs.SelectedFile = 0;
        //    return null;
        //}
        public ActionResult DownloadDoument(string filename, string attachId)
        {
            if (attachId == "0")
            {
                string messages = "Please select the row first!!";
                return new JavaScriptResult() { Script = "alert('" + messages + "');" };
            }

            db = new ExceedDbContext();
            //try
            //{
            int.TryParse(attachId, out int attachmentId);
            var attachment =
                    db.Attachments.FirstOrDefault(x => x.Reference == filename && x.ReferenceType == AttachmentType.DOCUMENT && x.AttachmentId == attachmentId);

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
                byte[] datas = req.DownloadData(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.DOCUMENT + "/" + attachment.Description));

                // byte[] datas11 = req.DownloadData(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.PHOTO + "/" + attachment.Description));

                response.BinaryWrite(datas);
                response.End();
                HRMInputs.SelectedFile = 0;
                return File(filename, attachment.FileType);
            }
            //}
            //catch (Exception)
            //{

            //    // throw;
            //}
            HRMInputs.SelectedFile = 0;
            return null;
        }

        public ActionResult UploadDocument(HttpPostedFileBase file, string txtValue)
        {
            try
            {
                if (file.ContentLength > 0)
                {

                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.DOCUMENT), HRMInputs.EmployeeId.ToString() + "-" + txtValue + "-" + fileName);

                    var mainPath = Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.DOCUMENT));

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
                            Description = HRMInputs.EmployeeId.ToString() + "-" + txtValue + "-" + fileName,
                            FileName = txtValue,
                            FileType = file.ContentType,
                            ReferenceType = AttachmentType.DOCUMENT,
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
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}