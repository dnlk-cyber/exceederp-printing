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
﻿using ExceedERP.Web.Areas.HRM.Controllers.Profile;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    [Authorize(Roles = HR_Manager_SelfService)]
    public class PromotionRequestStatusController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string HR_Manager_SelfService = "HR_Manager_SelfService";
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {

            var structres = db.Lookups.AsQueryable();
            ViewData["jobTitles"] = structres;
            GetDepartments();
            GetStatusType();
            HRMInputs inputs = new HRMInputs();
            var list = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
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
            List<PromotionRequest> requests = new List<PromotionRequest>();
            foreach (var m in Promotionrequest)
            {
                db = new ExceedDbContext();
                var status = db.RequestesStatus.Where(x => x.Status == RequestStatus.WAITING_FOR_APPROVAL).FirstOrDefault();
                if (status != null)
                {
                    requests.Add(m);
                }

            }
            Promotionrequest = requests.AsQueryable().OrderBy(x => x.Date);
            DataSourceResult result = Promotionrequest.ToDataSourceResult(request, PromotionRequest => new
            {
                PromotionRequestId = PromotionRequest.PromotionRequestId,
                OrgStructureId = PromotionRequest.OrgStructureId,

                LookupId = PromotionRequest.LookupId,
                NoOfPostion = PromotionRequest.NoOfPostion,

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

                };

                db.PromotionRequest.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { PromotionRequest }.ToDataSourceResult(request, ModelState));
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

                };

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
