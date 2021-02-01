﻿using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core;
﻿using ExceedERP.DataAccess;
﻿using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;  

namespace ExceedERP.Web.Areas.CRM.Controllers
{
           [AuthorizeRoles( CRMRoles.CRMAdmin)] 
    public class LifeSpanController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LifeSpans_Read([DataSourceRequest]DataSourceRequest request,int lineitemID)
        {
            IQueryable<LifeSpan> lifespans = db.LifeSpans.Where(ls=>ls.Reference==lineitemID.ToString());
            DataSourceResult result = lifespans.ToDataSourceResult(request, lifeSpan => new {
                LifeSpanID = lifeSpan.LifeSpanID,
                Reference = lifeSpan.Reference,
                ProductionDate = lifeSpan.ProductionDate,
                ExpiryDate = lifeSpan.ExpiryDate,
                CreatedBy = lifeSpan.CreatedBy,
                DateCreated = lifeSpan.DateCreated,
                LastModified = lifeSpan.LastModified,
                ModifiedBy = lifeSpan.ModifiedBy,
                OrgBranchName = lifeSpan.OrgBranchName,
                Remark = lifeSpan.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LifeSpans_Create([DataSourceRequest]DataSourceRequest request, LifeSpan lifeSpan, int lineitemID)
        {
            if (ModelState.IsValid)
            {
                var entity = new LifeSpan
                {
                    Reference = lineitemID.ToString(),
                    ProductionDate = lifeSpan.ProductionDate,
                    ExpiryDate = lifeSpan.ExpiryDate,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Remark = lifeSpan.Remark
                };

                db.LifeSpans.Add(entity);
                db.SaveChanges();
                lifeSpan.LifeSpanID = entity.LifeSpanID;
            }

            return Json(new[] { lifeSpan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LifeSpans_Update([DataSourceRequest]DataSourceRequest request, LifeSpan lifeSpan)
        {
            if (ModelState.IsValid)
            {
                var entity = new LifeSpan
                {
                    LifeSpanID = lifeSpan.LifeSpanID,
                    Reference = lifeSpan.Reference,
                    ProductionDate = lifeSpan.ProductionDate,
                    ExpiryDate = lifeSpan.ExpiryDate,
                    Remark = lifeSpan.Remark
                };

                db.LifeSpans.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { lifeSpan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LifeSpans_Destroy([DataSourceRequest]DataSourceRequest request, LifeSpan lifeSpan)
        {
            if (ModelState.IsValid)
            {
                var entity = new LifeSpan
                {
                    LifeSpanID = lifeSpan.LifeSpanID,
                    Reference = lifeSpan.Reference,
                    ProductionDate = lifeSpan.ProductionDate,
                    ExpiryDate = lifeSpan.ExpiryDate,
                    Remark = lifeSpan.Remark
                };

                db.LifeSpans.Attach(entity);
                db.LifeSpans.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Life Span number " + lifeSpan.LifeSpanID);
      
            }

            return Json(new[] { lifeSpan }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
