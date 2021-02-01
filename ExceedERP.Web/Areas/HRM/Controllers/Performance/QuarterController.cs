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
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
 using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
 using ExceedERP.Web.Helpers;

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
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Quarter_Create([DataSourceRequest]DataSourceRequest request, Quarter quarter, int quarterLId)
        {
            
          
                if (ModelState.IsValid)
            {
               
                    var entity = new Quarter
                    {
                        QuarterLengthId = quarterLId,
                        Name = quarter.Name,
                        AmharicName = quarter.AmharicName,
                        Sequence = quarter.Sequence,
                    };

                    db.Quarters.Add(entity);
                    db.SaveChanges();
                quarter.QuarterId = entity.QuarterId;
                
            }

            return Json(new[] { quarter }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Quarter_Update([DataSourceRequest]DataSourceRequest request, Quarter quarter)
        {
            if (ModelState.IsValid)
            {
                var entity = new Quarter
                {
                    QuarterLengthId = quarter.QuarterLengthId,
                    Name = quarter.Name,
                    AmharicName = quarter.AmharicName,
                    Sequence = quarter.Sequence,
                    QuarterId = quarter.QuarterId,

                };

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
                var entity = new Quarter
                {
                    QuarterId = quarter.QuarterId,
                    QuarterLengthId = quarter.QuarterLengthId,
                    Name = quarter.Name,
                    AmharicName = quarter.AmharicName,
                    Sequence = quarter.Sequence,

                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted quarter  Id " + quarter.QuarterId);
                #endregion
                db.Quarters.Attach(entity);
                db.Quarters.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { quarter }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
