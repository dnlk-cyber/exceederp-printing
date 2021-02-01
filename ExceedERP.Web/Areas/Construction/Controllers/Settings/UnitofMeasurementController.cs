﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Construction;
﻿using ExceedERP.DataAccess.Context;
 using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Construction.Controllers.Settings
{
    [AuthorizeRoles(ConstructionConstants.ConstructionSettingAdmin)]
    public class UnitOfMeasurementController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UnitOfMeasurement_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<UnitOfMeasurement> UnitOfMeasurements = db.UnitOfMeasurement;
            DataSourceResult result = UnitOfMeasurements.ToDataSourceResult(request, UnitOfMeasurement => new
            {
                UnitOfMeasurementId = UnitOfMeasurement.UnitOfMeasurementId,
                Name = UnitOfMeasurement.Name,
                Abbreviation = UnitOfMeasurement.Abbreviation,
                TakeOffSheetType = UnitOfMeasurement.TakeOffSheetType,
                Remark = UnitOfMeasurement.Remark
            });

            return Json(result);
        }
        private bool CheckExistance(string Name, string abb)
        {
            var lookup = db.UnitOfMeasurement.Where(x => x.Name == Name&& x.Abbreviation == abb).FirstOrDefault();
            if (lookup != null)
            {
                return true;
            }
            return false;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UnitOfMeasurement_Create([DataSourceRequest]DataSourceRequest request, UnitOfMeasurement UnitOfMeasurement)
        {
            if (ModelState.IsValid)
            {
                if (CheckExistance(UnitOfMeasurement.Name, UnitOfMeasurement.Abbreviation ))
                {
                    ModelState.AddModelError("", "This unit of measurment is already registered!!");
                }
                else
                {
                    var entity = new UnitOfMeasurement
                    {
                        Name = UnitOfMeasurement.Name,
                        Abbreviation = UnitOfMeasurement.Abbreviation,
                        TakeOffSheetType = UnitOfMeasurement.TakeOffSheetType,
                        Remark = UnitOfMeasurement.Remark
                    };

                    db.UnitOfMeasurement.Add(entity);
                    db.SaveChanges();
                    UnitOfMeasurement.UnitOfMeasurementId = entity.UnitOfMeasurementId;
                }
            }

            return Json(new[] { UnitOfMeasurement }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UnitOfMeasurement_Update([DataSourceRequest]DataSourceRequest request, UnitOfMeasurement UnitOfMeasurement)
        {
            if (ModelState.IsValid)
            {
                var entity = new UnitOfMeasurement
                {
                    UnitOfMeasurementId = UnitOfMeasurement.UnitOfMeasurementId,
                    Name = UnitOfMeasurement.Name,
                    Abbreviation = UnitOfMeasurement.Abbreviation,
                    TakeOffSheetType = UnitOfMeasurement.TakeOffSheetType,
                    Remark = UnitOfMeasurement.Remark
                };

                db.UnitOfMeasurement.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { UnitOfMeasurement }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UnitOfMeasurement_Destroy([DataSourceRequest]DataSourceRequest request, UnitOfMeasurement UnitOfMeasurement)
        {
            if (ModelState.IsValid)
            {
                var entity = new UnitOfMeasurement
                {
                    UnitOfMeasurementId = UnitOfMeasurement.UnitOfMeasurementId,
                    Name = UnitOfMeasurement.Name,
                    Abbreviation = UnitOfMeasurement.Abbreviation,
                    TakeOffSheetType = UnitOfMeasurement.TakeOffSheetType,
                    Remark = UnitOfMeasurement.Remark
                };

                db.UnitOfMeasurement.Attach(entity);
                db.UnitOfMeasurement.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { UnitOfMeasurement }.ToDataSourceResult(request, ModelState));
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
