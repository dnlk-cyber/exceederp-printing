﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.Payroll;
 using ExceedERP.Core.Domain.HRM.Payroll.Setting;
 using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
      [Authorize(Roles = Payroll_Admin)]
    public class PerdimeSettingController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string Payroll_Admin = "Payroll_Admin";
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PerdimeSetting_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PerdimeSetting> perdimesetting = db.PerdimeSetting;
            DataSourceResult result = perdimesetting.ToDataSourceResult(request, perdimeSetting => new {
                PerdimeSettingId = perdimeSetting.PerdimeSettingId,
                Minmum = perdimeSetting.Minmum,
                Maximum = perdimeSetting.Maximum,
                Amount = perdimeSetting.Amount,
                Remark = perdimeSetting.Remark,
                 DateCreated = perdimeSetting.DateCreated,
                LastModified = perdimeSetting.LastModified,

                CreatedBy = perdimeSetting.CreatedBy,

                ModifiedBy = perdimeSetting.ModifiedBy 
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PerdimeSetting_Create([DataSourceRequest]DataSourceRequest request, PerdimeSetting perdimeSetting)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new PerdimeSetting
                {
                    Minmum = perdimeSetting.Minmum,
                    Maximum = perdimeSetting.Maximum,
                    Amount = perdimeSetting.Amount,
                    Remark = perdimeSetting.Remark,
                    DateCreated =DateTime .Now ,
                    LastModified = perdimeSetting.LastModified,

                    CreatedBy = User.Identity .Name ,

                    ModifiedBy = perdimeSetting.ModifiedBy
                };

                db.PerdimeSetting.Add(entity);
                db.SaveChanges();
                perdimeSetting.PerdimeSettingId = entity.PerdimeSettingId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll settings", "Success", User.Identity.Name, $"added perdiem min {entity.Minmum} max {entity.Maximum} amount {entity.Amount}", Modules.Payroll);
                #endregion
            }

            return Json(new[] { perdimeSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PerdimeSetting_Update([DataSourceRequest]DataSourceRequest request, PerdimeSetting perdimeSetting)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new PerdimeSetting
                {
                    PerdimeSettingId = perdimeSetting.PerdimeSettingId,
                    Minmum = perdimeSetting.Minmum,
                    Maximum = perdimeSetting.Maximum,
                    Amount = perdimeSetting.Amount,
                    Remark = perdimeSetting.Remark,
                    DateCreated = perdimeSetting.DateCreated ,
                    LastModified = DateTime.Now,

                    CreatedBy = perdimeSetting.CreatedBy ,

                    ModifiedBy = User.Identity.Name
                };

                db.PerdimeSetting.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll settings", "Success", User.Identity.Name, $"updated perdiem min {entity.Minmum} max {entity.Maximum} amount {entity.Amount}", Modules.Payroll);
                #endregion
            }

            return Json(new[] { perdimeSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PerdimeSetting_Destroy([DataSourceRequest]DataSourceRequest request, PerdimeSetting perdimeSetting)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new PerdimeSetting
                {
                    PerdimeSettingId = perdimeSetting.PerdimeSettingId,
                    Minmum = perdimeSetting.Minmum,
                    Maximum = perdimeSetting.Maximum,
                    Amount = perdimeSetting.Amount,
                    Remark = perdimeSetting.Remark,
                    DateCreated = perdimeSetting.DateCreated,
                    LastModified = perdimeSetting.LastModified,
                    CreatedBy = perdimeSetting.CreatedBy,
                    ModifiedBy = perdimeSetting.ModifiedBy 
                };
                #region delete log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  perdime setting information of setting " + perdimeSetting.Amount,Modules.Payroll);
                #endregion
                db.PerdimeSetting.Attach(entity);
                db.PerdimeSetting.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { perdimeSetting }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
