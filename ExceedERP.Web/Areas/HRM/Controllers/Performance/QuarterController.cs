using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
 using ExceedERP.Core.Domain;
﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
 using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
 using ExceedERP.Web.Helpers;
using System;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{
     [AuthorizeRoles(HrmConstants.HRPerformanceAdmin, HrmConstants.HRAdmin)]
    public class QuarterController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult Quarter_Read([DataSourceRequest]DataSourceRequest request, int quarterLId)
        {
            IQueryable<Quarter> Quarter = db.Quarters.Where(x=> x.QuarterLengthId == quarterLId);
            DataSourceResult result = Quarter.ToDataSourceResult(request, quarter => new {
                QuarterId = quarter.QuarterId,
                QuarterLengthId = quarter.QuarterLengthId,
                Name = quarter.Name,
                AmharicName = quarter.AmharicName,
                Sequence = quarter.Sequence,
                DateFrom = quarter.DateFrom,
                DateTo = quarter.DateTo,
                IsClosed = quarter.IsClosed,
                Remark = quarter.Remark,
                DateCreated = quarter.DateCreated,
                LastModified = quarter.LastModified,
                CreatedBy = quarter.CreatedBy,
                ModifiedBy = quarter.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Quarter_Create([DataSourceRequest]DataSourceRequest request, Quarter quarter, int quarterLId)
        {


            quarter.QuarterLengthId = quarterLId;
            quarter.CreatedBy = User.Identity.Name;
            quarter.DateCreated = DateTime.Now;
            if (ModelState.IsValid)
            {
                
                var entity = GetMappedObject(quarter);
                db.Quarters.Add(entity);
                db.SaveChanges();

                quarter.QuarterId = entity.QuarterId;
                
            }

            return Json(new[] { quarter }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Quarter_Update([DataSourceRequest]DataSourceRequest request, Quarter quarter)
        {
            quarter.ModifiedBy = User.Identity.Name;
            quarter.LastModified = DateTime.Now;
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(quarter);
                db.Quarters.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { quarter }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Quarter_Destroy([DataSourceRequest]DataSourceRequest request, Quarter quarter)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(quarter);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted quarter  Id " + quarter.QuarterId);
                #endregion
                db.Quarters.Attach(entity);
                db.Quarters.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { quarter }.ToDataSourceResult(request, ModelState));
        }

        private Quarter GetMappedObject(Quarter quarter)
        {
            var entity = new Quarter
            {
                QuarterId = quarter.QuarterId,
                QuarterLengthId = quarter.QuarterLengthId,
                Name = quarter.Name,
                AmharicName = quarter.AmharicName,
                Sequence = quarter.Sequence,
                DateFrom = quarter.DateFrom,
                DateTo = quarter.DateTo,
                IsClosed = quarter.IsClosed,
                Remark = quarter.Remark,
                DateCreated = quarter.DateCreated,
                LastModified = quarter.LastModified,
                CreatedBy = quarter.CreatedBy,
                ModifiedBy = quarter.ModifiedBy,

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
