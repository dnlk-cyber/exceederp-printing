﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Common.DocumentApproval;
 using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Web.Areas.Common.Controllers.DocumentApproval
{
    [Authorize(Roles = "Admin")]
    public class AssignApprovalGroupController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {

            var jobtitles = db.Lookups;
            var branches = db.Branch;
            var approvalGroups = db.ApprovalGroups;
            ViewData["ApprovalGroups"] = approvalGroups;
            ViewData["Branches"] = branches;
            ViewData["JobTitles"] = jobtitles;
            ViewData["EmployeeList"] = db.Person_Employee.Select(x => new { EmployeeId = x.EmployeeId,  FullNameaEnglish =x.EmployeeCode+"-"+ x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish }).ToList();
            var bizUnits = db.BusinessUnits.ToList();
            var bizUnitdisplay = bizUnits.ToList();

          
            ViewData["businesUnits"] = bizUnitdisplay.Select(
              i => new
              {
                  Name = i.Name,
                  BusinessUnitId = i.BusinessUnitId
              }).AsQueryable(); 
        }

        public ActionResult AssignApprovalGroups_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<AssignApprovalGroup> assignapprovalgroups = db.AssignApprovalGroups;
            DataSourceResult result = assignapprovalgroups.ToDataSourceResult(request, assignApprovalGroup => new {
                AssignApprovalGroupId = assignApprovalGroup.AssignApprovalGroupId,
                BusinessUnit = assignApprovalGroup.BusinessUnit,
                OpertatingUnit = assignApprovalGroup.OpertatingUnit,
                Organization = assignApprovalGroup.Organization,
                Position = assignApprovalGroup.Position,
                Job = assignApprovalGroup.Job,
                EmployeeId = assignApprovalGroup.EmployeeId,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssignApprovalGroups_Create([DataSourceRequest]DataSourceRequest request, AssignApprovalGroup assignApprovalGroup)
        {
            #region validation
            if ((assignApprovalGroup.Job==0) && (assignApprovalGroup.EmployeeId == 0))
                ModelState.AddModelError("required","Please choose either job title or employee ");

            var exisit=db.AssignApprovalGroups
                .Where(x => x.BusinessUnit == assignApprovalGroup.BusinessUnit)
                .Where(x => x.OpertatingUnit == assignApprovalGroup.OpertatingUnit)
                .Where(x => x.Job == assignApprovalGroup.Job ||x.EmployeeId== assignApprovalGroup.EmployeeId)
                .ToList();

            if (exisit.Any())
                ModelState.AddModelError("exisit", "The record you trying to add already exisits");
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new AssignApprovalGroup
                {
                    BusinessUnit = assignApprovalGroup.BusinessUnit,
                    OpertatingUnit = assignApprovalGroup.OpertatingUnit,
                    Organization = assignApprovalGroup.Organization,
                    Position = assignApprovalGroup.Position,
                    Job = assignApprovalGroup.Job,
                    EmployeeId = assignApprovalGroup.EmployeeId,
                };

                db.AssignApprovalGroups.Add(entity);
                db.SaveChanges();
                assignApprovalGroup.AssignApprovalGroupId = entity.AssignApprovalGroupId;
            }

            return Json(new[] { assignApprovalGroup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssignApprovalGroups_Update([DataSourceRequest]DataSourceRequest request, AssignApprovalGroup assignApprovalGroup)
        {
            #region validation
            if ((assignApprovalGroup.Job == 0) && (assignApprovalGroup.EmployeeId == 0))
                ModelState.AddModelError("required", "Please choose either job title or employee ");

            var exisit = db.AssignApprovalGroups
                 .Where(x => x.AssignApprovalGroupId != assignApprovalGroup.AssignApprovalGroupId)
                .Where(x => x.BusinessUnit == assignApprovalGroup.BusinessUnit)
                .Where(x => x.OpertatingUnit == assignApprovalGroup.OpertatingUnit)
                .Where(x => x.Job == assignApprovalGroup.Job || x.EmployeeId == assignApprovalGroup.EmployeeId)
                .ToList();

            if (exisit.Any())
                ModelState.AddModelError("exisit", "The record you trying to add already exisits");
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new AssignApprovalGroup
                {
                    AssignApprovalGroupId = assignApprovalGroup.AssignApprovalGroupId,
                    BusinessUnit = assignApprovalGroup.BusinessUnit,
                    OpertatingUnit = assignApprovalGroup.OpertatingUnit,
                    Organization = assignApprovalGroup.Organization,
                    Position = assignApprovalGroup.Position,
                    Job = assignApprovalGroup.Job,
                    EmployeeId = assignApprovalGroup.EmployeeId,
                };

                db.AssignApprovalGroups.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { assignApprovalGroup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssignApprovalGroups_Destroy([DataSourceRequest]DataSourceRequest request, AssignApprovalGroup assignApprovalGroup)
        {
            if (ModelState.IsValid)
            {
                var entity = new AssignApprovalGroup
                {
                    AssignApprovalGroupId = assignApprovalGroup.AssignApprovalGroupId,
                    BusinessUnit = assignApprovalGroup.BusinessUnit,
                    OpertatingUnit = assignApprovalGroup.OpertatingUnit,
                    Organization = assignApprovalGroup.Organization,
                    Position = assignApprovalGroup.Position,
                    Job = assignApprovalGroup.Job,
                    EmployeeId = assignApprovalGroup.EmployeeId,
                };

                db.AssignApprovalGroups.Attach(entity);
                db.AssignApprovalGroups.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { assignApprovalGroup }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
