﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
 using ExceedERP.Core.Domain;
 using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.HealthAndInsurance;
using ExceedERP.DataAccess.Context;
﻿using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
 using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Health
{
    //[Authorize(Roles = Health_Admin + "," + Health_User)]
    [AuthorizeRoles(HrmConstants.HRHealthUser, HrmConstants.HRHealthAdmin)]
    public class RestitutionController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            HRMInputs inputs = new HRMInputs();
            var list = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
        }
        public ActionResult Restitutions_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<Restitution> restitutionses = db.Restitutions.Where(x => x.AccidentRecordId == id);
            DataSourceResult result = restitutionses.ToDataSourceResult(request, restitution => new
            {
                RestitutionId = restitution.RestitutionId,
                RestitutionAmount = restitution.RestitutionAmount,
                AccidentRecordId = restitution.AccidentRecordId,
                Remark = restitution.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Restitutions_Create([DataSourceRequest]DataSourceRequest request, Restitution restitution, int id)
        {

            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Restitution
                {
                    RestitutionAmount = restitution.RestitutionAmount,
                    AccidentRecordId = id,
                    Remark = restitution.Remark
                };

                db.Restitutions.Add(entity);
                db.SaveChanges();
                restitution.RestitutionId = entity.RestitutionId;
            }

            return Json(new[] { restitution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Restitutions_Update([DataSourceRequest]DataSourceRequest request, Restitution restitution)
        {
            if (ModelState.IsValid)
            {
                var entity = new Restitution
                {
                    RestitutionId = restitution.RestitutionId,
                    AccidentRecordId = restitution.AccidentRecordId,
                    RestitutionAmount = restitution.RestitutionAmount,
                    Remark = restitution.Remark
                };

                db.Restitutions.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { restitution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Restitutions_Destroy([DataSourceRequest]DataSourceRequest request, Restitution restitution)
        {

            if (ModelState.IsValid)
            {
                var entity = new Restitution
                {
                    RestitutionId = restitution.RestitutionId,
                    AccidentRecordId = restitution.AccidentRecordId,
                    RestitutionAmount = restitution.RestitutionAmount,
                    Remark = restitution.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted accident restitution  information of record id " + restitution.AccidentRecordId);
                #endregion
                db.Restitutions.Attach(entity);
                db.Restitutions.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { restitution }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
