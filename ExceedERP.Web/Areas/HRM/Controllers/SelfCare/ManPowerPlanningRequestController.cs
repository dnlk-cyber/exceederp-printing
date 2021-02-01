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

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    public class ManPowerPlanningRequestController : Controller
    {
        // GET: HRM/ManPowerPlanningRequest
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
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
            IQueryable<ManPowerPlanningRequest> manpowerplannings = db.ManPowerPlanningRequests;
            DataSourceResult result = manpowerplannings.ToDataSourceResult(request, req => new
            {
                ManPowerPlanningRequestId = req.ManPowerPlanningRequestId,
                OrgStructureId = req.OrgStructureId,
                GlFiscalYearId = req.GlFiscalYearId,
                EmploymentType = req.EmploymentType,
                LookupId = req.LookupId,
                NoOfCurrentStaff = req.NoOfCurrentStaff,
                NoOfShouldBeStaff = req.NoOfShouldBeStaff,
                NoOfVaccantPostion = req.NoOfVaccantPostion,
                PlanType = req.PlanType,
                Date = req.Date,
                CreatedBy = req.CreatedBy,
                DateCreated = req.DateCreated,
                ModifiedBy = req.ModifiedBy,
                LastModified = req.LastModified,
                OrgBranchName = req.OrgBranchName,
                BranchId = req.BranchId,
                IsNewJobTitle = req.IsNewJobTitle,
                JobTitle = req.JobTitle
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ManPowerPlanningRequest_Create([DataSourceRequest]DataSourceRequest request, ManPowerPlanningRequest req)
        {
            ModelState.Clear();

           
            if(req.IsNewJobTitle)
            {
                req.LookupId = 0;
            }

            if (ModelState.IsValid)
            {
                var entity = new ManPowerPlanningRequest
                {
                    OrgStructureId = req.OrgStructureId,
                    GlFiscalYearId = req.GlFiscalYearId,
                    EmploymentType = req.EmploymentType,
                    LookupId = req.LookupId,
                    NoOfCurrentStaff = req.NoOfCurrentStaff,
                    NoOfShouldBeStaff = req.NoOfShouldBeStaff,
                    NoOfVaccantPostion = req.NoOfVaccantPostion,
                    PlanType = req.PlanType,
                    Date = DateTime.Today,
                    CreatedBy = req.CreatedBy,
                    DateCreated = DateTime.Today,
                    ModifiedBy = req.ModifiedBy,
                    LastModified = req.LastModified,
                    OrgBranchName = req.OrgBranchName,
                    BranchId = req.BranchId,
                    IsNewJobTitle = req.IsNewJobTitle,
                    JobTitle = req.JobTitle
                };

                db.ManPowerPlanningRequests.Add(entity);
                db.SaveChanges();
                req.ManPowerPlanningRequestId = entity.ManPowerPlanningRequestId;

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "ManPowerPlanningReqeust ", "Success", User.Identity.Name, User.Identity.Name + " add new Manpower planning " + DateTime.Today.ToShortDateString(), Modules.HRM);
                
                #endregion

            }

            return Json(new[] { req }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ManPowerPlanningRequest_Update([DataSourceRequest]DataSourceRequest request, ManPowerPlanningRequest req)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (req.IsNewJobTitle)
                {
                    req.LookupId = 0;
                }

                var entity = new ManPowerPlanningRequest
                {
                    ManPowerPlanningRequestId = req.ManPowerPlanningRequestId,
                    OrgStructureId = req.OrgStructureId,
                    GlFiscalYearId = req.GlFiscalYearId,
                    EmploymentType = req.EmploymentType,
                    LookupId = req.LookupId,
                    NoOfCurrentStaff = req.NoOfCurrentStaff,
                    NoOfShouldBeStaff = req.NoOfShouldBeStaff,
                    NoOfVaccantPostion = req.NoOfVaccantPostion,
                    PlanType = req.PlanType,
                    Date = DateTime.Today,
                    CreatedBy = req.CreatedBy,
                    DateCreated = req.DateCreated,
                    ModifiedBy = req.ModifiedBy,
                    LastModified = DateTime.Today,
                    OrgBranchName = req.OrgBranchName,
                    BranchId = req.BranchId,
                    IsNewJobTitle = req.IsNewJobTitle,
                    JobTitle = req.JobTitle
                };


                db.ManPowerPlanningRequests.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "ManPowerPlanningReqeust ", "Success", User.Identity.Name, User.Identity.Name + " update Manpower planning " + DateTime.Today.ToShortDateString(), Modules.HRM);

                #endregion

            }

            return Json(new[] { req }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ManPowerPlanningRequest_Destroy([DataSourceRequest]DataSourceRequest request, ManPowerPlanningRequest req)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new ManPowerPlanningRequest
                {
                    ManPowerPlanningRequestId = req.ManPowerPlanningRequestId,
                    OrgStructureId = req.OrgStructureId,
                    GlFiscalYearId = req.GlFiscalYearId,
                    EmploymentType = req.EmploymentType,
                    LookupId = req.LookupId,
                    NoOfCurrentStaff = req.NoOfCurrentStaff,
                    NoOfShouldBeStaff = req.NoOfShouldBeStaff,
                    NoOfVaccantPostion = req.NoOfVaccantPostion,
                    PlanType = req.PlanType,
                    Date = req.Date,
                    CreatedBy = req.CreatedBy,
                    DateCreated = req.DateCreated,
                    ModifiedBy = req.ModifiedBy,
                    LastModified = req.LastModified,
                    OrgBranchName = req.OrgBranchName,
                    BranchId = req.BranchId,
                    IsNewJobTitle = req.IsNewJobTitle,
                    JobTitle = req.JobTitle
                };


                db.ManPowerPlanningRequests.Attach(entity);
                db.ManPowerPlanningRequests.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { req }.ToDataSourceResult(request, ModelState));
        }

        public void Download(int id)
        {
            HRMInputs.SelectedFile = id;

        }
        public ActionResult UploadDocument(HttpPostedFileBase file, string txtValue)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path =
                        Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" +
                                           AttachmentType.DOCUMENT), txtValue);
                    var mainPath =
                        Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" +
                                           AttachmentType.DOCUMENT));
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