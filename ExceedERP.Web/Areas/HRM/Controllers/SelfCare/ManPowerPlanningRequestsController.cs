using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    [Authorize]
    public class ManPowerPlanningRequestsController : BaseController
    {
        // GET: HRM/ManPowerPlanningRequests
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
        public ActionResult ManPowerPlanningRequests_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ManPowerPlanningRequest> manpowerrequest = db.ManPowerPlanningRequests.OrderBy(x => x.Date);
            List<ManPowerPlanningRequest> requests = new List<ManPowerPlanningRequest>();
            foreach (var m in manpowerrequest)
            {
                db = new ExceedDbContext();
                var status = db.RequestesStatus.Where(x => (x.Status == RequestStatus.WAITING_FOR_APPROVAL || x.Status == RequestStatus.DEPARTMENT_APPROVAL) && x.RequestId == m.ManPowerPlanningRequestId && x.ReferenceType == "ManPowerPlanning").FirstOrDefault();
                if (status != null)
                {
                    requests.Add(m);
                }

            }
           var manpowerPlanning = requests.AsQueryable().OrderBy(x => x.Date);
            DataSourceResult result = manpowerPlanning.ToDataSourceResult(request, manPowerPlanningRequest => new
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
        public ActionResult ManPowerPlanningRequests_Create([DataSourceRequest]DataSourceRequest request, ManPowerPlanningRequest planningRequest)
        {
            if (ModelState.IsValid)
            {

                var entity = GetMappedObject(planningRequest);
                db.ManPowerPlanningRequests.Add(entity);
                db.SaveChanges();
                planningRequest.ManPowerPlanningRequestId = entity.ManPowerPlanningRequestId;

            }

            return Json(new[] { planningRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ManPowerPlanningRequests_Update([DataSourceRequest]DataSourceRequest request, ManPowerPlanningRequest planningRequest)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = GetMappedObject(planningRequest);
                db.ManPowerPlanningRequests.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { planningRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ManPowerPlanningRequests_Destroy([DataSourceRequest]DataSourceRequest request, ManPowerPlanningRequest planningRequest)
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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}