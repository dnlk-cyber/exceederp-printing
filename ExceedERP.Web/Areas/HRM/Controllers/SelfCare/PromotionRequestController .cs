﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
﻿using ExceedERP.Core.Domain.Common;
﻿using ExceedERP.Core.Domain.Common.HRM;
﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
﻿using ExceedERP.Web.Areas.Common.Controllers;
﻿using ExceedERP.Web.Areas.HRM.Controllers.Profile;
﻿using Microsoft.Ajax.Utilities;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    [Authorize(Roles = HR_Employee_SelfService)]
    public class PromotionRequestController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string HR_Employee_SelfService = "HR_Employee_SelfService";
        public ActionResult Index()
        {
            PopulateViewData();
            GetDepartments();
            GetStatusType();

            return View();
        }
        private void PopulateViewData()
        {
            HRMInputs inputs = new HRMInputs();
            var list = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            var structres = db.Lookups.AsQueryable();
            ViewData["jobTitles"] = structres;
            var levels = db.Lookups.Where(l => l.LookupType == LookUpTypes.EDUCATION_LEVEL);
            ViewData["EduLevelList"] = levels;
        }
        private void GetStatusType()
        {
            var ststus = Enum.GetValues(typeof(RequestStatus))
                .Cast<RequestStatus>().ToList();

            var ststuses = ststus.Select(s => new { Text = s.ToString(), Value = s.ToString() });
            ViewData["RequestStatus"] = ststuses;
        }
        public void GetDepartments()
        {
            List<MainCompanyStructure> allDepartment = new List<MainCompanyStructure>();
            var departments = db.MainCompanyStructures.Where(x => x.Parent == 0).FirstOrDefault();
            if (departments != null)
            {
                var deptlevelOne = db.MainCompanyStructures.Where(x => x.Parent == departments.OrgStructureId).ToList();
                if (deptlevelOne.Any())
                {
                    foreach (var a in deptlevelOne)
                    {
                        var departmentLevelTwo =
                            db.MainCompanyStructures.Where(x => x.Parent == a.OrgStructureId).ToList();
                        if (departmentLevelTwo.Any())
                        {
                            foreach (var b in departmentLevelTwo)
                            {
                                allDepartment.Add(b);
                            }
                        }
                        allDepartment.Add(a);
                    }
                }
            }
            ViewData["Departments"] = allDepartment;
        }
        public ActionResult PromotionRequest_Read([DataSourceRequest]DataSourceRequest request)
        {

            IQueryable<PromotionRequest> Promotionrequest = db.PromotionRequest.OrderBy(x => x.Date);

            //var employeeid = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            //List<int> employeeids = EmployeeHelper.GetSubordinates(employeeid);
            //IQueryable<LeaveRequest> leaverequest = db.PromotionRequest.Where(x => employeeids.Contains(x.EmployeeId)).OrderBy(x => x.RequestDate);

            DataSourceResult result = Promotionrequest.ToDataSourceResult(request, PromotionRequest => new
            {
                PromotionRequestId = PromotionRequest.PromotionRequestId,
                OrgStructureId = PromotionRequest.OrgStructureId,
                NoOfPostion = PromotionRequest.NoOfPostion,
                LookupId = PromotionRequest.LookupId,
                EduLevel = PromotionRequest.EduLevel,
                RelevantExp = PromotionRequest.RelevantExp,
                SpecialSkill = PromotionRequest.SpecialSkill,
                PlannedSalary = PromotionRequest.PlannedSalary,

                RankId = PromotionRequest.RankId,
                Date = PromotionRequest.Date
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PromotionRequest_Create([DataSourceRequest]DataSourceRequest request, PromotionRequest PromotionRequest)
        {
            if (ModelState.IsValid)
            {

                var entity = new PromotionRequest
                {
                    OrgStructureId = PromotionRequest.OrgStructureId,
                    NoOfPostion = PromotionRequest.NoOfPostion,
                    LookupId = PromotionRequest.LookupId,
                    EduLevel = PromotionRequest.EduLevel,
                    RelevantExp = PromotionRequest.RelevantExp,
                    SpecialSkill = PromotionRequest.SpecialSkill,
                    PlannedSalary = PromotionRequest.PlannedSalary,

                    RankId = PromotionRequest.RankId,
                    Date = PromotionRequest.Date
                };

                db.PromotionRequest.Add(entity);
                db.SaveChanges();
                PromotionRequest.PromotionRequestId = entity.PromotionRequestId;
            }

            return Json(new[] { PromotionRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PromotionRequest_Update([DataSourceRequest]DataSourceRequest request, PromotionRequest PromotionRequest)
        {
            if (ModelState.IsValid)
            {
                var entity = new PromotionRequest
                {
                    PromotionRequestId = PromotionRequest.PromotionRequestId,
                    OrgStructureId = PromotionRequest.OrgStructureId,
                    NoOfPostion = PromotionRequest.NoOfPostion,
                    LookupId = PromotionRequest.LookupId,
                    EduLevel = PromotionRequest.EduLevel,
                    RelevantExp = PromotionRequest.RelevantExp,
                    SpecialSkill = PromotionRequest.SpecialSkill,
                    PlannedSalary = PromotionRequest.PlannedSalary,

                    RankId = PromotionRequest.RankId,
                    Date = PromotionRequest.Date
                };

                db.PromotionRequest.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { PromotionRequest }.ToDataSourceResult(request, ModelState));
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
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PromotionRequest_Destroy([DataSourceRequest]DataSourceRequest request, PromotionRequest PromotionRequest)
        {
            if (ModelState.IsValid)
            {
                var entity = new PromotionRequest
                {
                    PromotionRequestId = PromotionRequest.PromotionRequestId,
                    OrgStructureId = PromotionRequest.OrgStructureId,
                    NoOfPostion = PromotionRequest.NoOfPostion,
                    LookupId = PromotionRequest.LookupId,
                    EduLevel = PromotionRequest.EduLevel,
                    RelevantExp = PromotionRequest.RelevantExp,
                    SpecialSkill = PromotionRequest.SpecialSkill,
                    PlannedSalary = PromotionRequest.PlannedSalary,

                    RankId = PromotionRequest.RankId,
                    Date = PromotionRequest.Date

                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted promotion request of work unit" + PromotionRequest.MainCompanyStructure.Name);
                #endregion
                db.PromotionRequest.Attach(entity);
                db.PromotionRequest.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { PromotionRequest }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
