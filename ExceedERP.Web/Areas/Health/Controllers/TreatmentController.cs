﻿using System;
using System.Collections.Generic;
﻿using System.Data.Entity;
﻿using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
 using ExceedERP.Core.Domain;
 using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
﻿using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
 using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Health;
using ExceedERP.Web.Areas.HRM.Controllers;

namespace ExceedERP.Web.Areas.Health.Controllers
{
    [AuthorizeRoles(HrmConstants.HRHealthUser, HrmConstants.HRHealthAdmin)]
    public class TreatmentController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            PopulateViewData();
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            HRMInputs inputs = new HRMInputs();
            var list = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
        }
        public ActionResult Treatment_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<Treatment> treatments = db.Treatment.Where(x => x.PrescriptionId == id);
            DataSourceResult result = treatments.ToDataSourceResult(request, treatment => new
            {
                TreatmentId = treatment.TreatmentId,
                PrescriptionId = treatment.PrescriptionId,
                Measure = treatment.Measure,
                Quntity = treatment.Quntity
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Treatment_Create([DataSourceRequest]DataSourceRequest request, Treatment treatment, int id)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Treatment
                {
                    PrescriptionId = id,
                    Measure = treatment.Measure,
                    Quntity = treatment.Quntity
                };

                db.Treatment.Add(entity);
                db.SaveChanges();
                treatment.TreatmentId = entity.TreatmentId;
            }

            return Json(new[] { treatment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Treatment_Update([DataSourceRequest]DataSourceRequest request, Treatment treatment)
        {
            if (ModelState.IsValid)
            {
                var entity = new Treatment
                {
                    TreatmentId = treatment.TreatmentId,
                    PrescriptionId = treatment.PrescriptionId,
                    Measure = treatment.Measure,
                    Quntity = treatment.Quntity
                };

                db.Treatment.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { treatment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Treatment_Destroy([DataSourceRequest]DataSourceRequest request, Treatment treatment)
        {
            if (ModelState.IsValid)
            {
                var entity = new Treatment
                {
                    PrescriptionId = treatment.PrescriptionId,
                    TreatmentId = treatment.TreatmentId,
                    Measure = treatment.Measure,
                    Quntity = treatment.Quntity
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted treatment information of Prescription id " + treatment.PrescriptionId);
                #endregion
                db.Treatment.Attach(entity);
                db.Treatment.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { treatment }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
