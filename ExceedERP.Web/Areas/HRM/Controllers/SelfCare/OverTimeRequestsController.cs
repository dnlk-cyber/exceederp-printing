﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
 using ExceedERP.Core.Domain;
 using ExceedERP.Core.Domain.Common;
﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
 using ExceedERP.Core.Domain.HRM.SelfCare;
 using ExceedERP.DataAccess.Context;
﻿using ExceedERP.Web.Areas.Common.Controllers;
﻿using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using ExceedERP.Web.Controllers;
 using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    [AuthorizeRoles(HrmConstants.HROverTimeRequestApprover)]
    public class OverTimeRequestsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
           PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            var structres = db.MainCompanyStructures;
            ViewData["organization"] = structres;
            HRMInputs inputs = new HRMInputs();
            var list = inputs.GetPayrollPlacmentEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            var employeName = db.Person_Employee.ToList();
            ViewData["EmployeeName"] = employeName;
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
                        //  return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                        ModelState.AddModelError("xx", messages);
                        return null;
                    }
                    if (s.Status == RequestStatus.WAITING_FOR_APPROVAL)
                    {
                        messages = "The Request is already send for approval!!";
                        // return  ModelState.AddModelError("xx", messages );
                        return null;//new JavaScriptResult() { Script = "alert('" + messages + "');" };
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
                    Date = DateTime.Now
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
                    Date = DateTime.Now
                };

                db.RequestesStatus.Add(entityy);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
       
        public ActionResult OverTimeRequest_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<OverTimeRequest> overtimerequest = db.OverTimeRequest;
            List<OverTimeRequest> requests = new List<OverTimeRequest>();
            foreach (var m in overtimerequest)
            {
                db = new ExceedDbContext();
                var status = db.RequestesStatus.Where(x => x.Status == RequestStatus.DEPARTMENT_APPROVAL && x.RequestId == m.OverTimeRequestId && x.ReferenceType == "OverTime").FirstOrDefault();
                if (status != null)
                {
                    requests.Add(m);
                }

            }

            overtimerequest = requests.AsQueryable().OrderBy(x => x.FromDate);
            DataSourceResult result = overtimerequest.ToDataSourceResult(request, overTimeRequest => new {
                OverTimeRequestId = overTimeRequest.OverTimeRequestId,
                FromDate = overTimeRequest.FromDate,
                ToDate = overTimeRequest.ToDate,
                FromTime = overTimeRequest.FromTime,
                ToTime = overTimeRequest.ToTime,
                OrgStructureId = overTimeRequest.OrgStructureId,
                Description = overTimeRequest.Description
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OverTimeRequest_Create([DataSourceRequest]DataSourceRequest request, OverTimeRequest overTimeRequest)
        {
         
            if (ModelState.IsValid)
            {
                var entity = new OverTimeRequest
                {
                    FromDate = overTimeRequest.FromDate,
                    ToDate = overTimeRequest.ToDate,
                    FromTime = overTimeRequest.FromTime,
                    ToTime = overTimeRequest.ToTime,//.Date,
                    OrgStructureId = overTimeRequest.OrgStructureId,
                    Description = overTimeRequest.Description
                };

                db.OverTimeRequest.Add(entity);
                db.SaveChanges();
                overTimeRequest.OverTimeRequestId = entity.OverTimeRequestId;
            }

            return Json(new[] { overTimeRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OverTimeRequest_Update([DataSourceRequest]DataSourceRequest request, OverTimeRequest overTimeRequest)
        {
            if (ModelState.IsValid)
            {
                var entity = new OverTimeRequest
                {
                    OverTimeRequestId = overTimeRequest.OverTimeRequestId,
                    FromDate = overTimeRequest.FromDate,
                    ToDate = overTimeRequest.ToDate,
                    FromTime = overTimeRequest.FromTime,
                    ToTime = overTimeRequest.ToTime,
                    OrgStructureId = overTimeRequest.OrgStructureId,
                    Description = overTimeRequest.Description
                };

                db.OverTimeRequest.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { overTimeRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OverTimeRequest_Destroy([DataSourceRequest]DataSourceRequest request, OverTimeRequest overTimeRequest)
        {
            if (ModelState.IsValid)
            {
                var entity = new OverTimeRequest
                {
                    OverTimeRequestId = overTimeRequest.OverTimeRequestId,
                    FromDate = overTimeRequest.FromDate,
                    ToDate = overTimeRequest.ToDate,
                    FromTime = overTimeRequest.FromTime,
                    ToTime = overTimeRequest.ToTime,
                    OrgStructureId = overTimeRequest.OrgStructureId,
                    Description = overTimeRequest.Description
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted over time request of work unit" + overTimeRequest.OrgStructureId);
                #endregion
                db.OverTimeRequest.Attach(entity);
                db.OverTimeRequest.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { overTimeRequest }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
