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
﻿using ExceedERP.DataAccess.Context;
 using ExceedERP.Core.Domain.Common;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
       [AuthorizeRoles(CRMRoles.CRMAdmin)] 
    public class RelationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Relations_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Relation> relations = db.Relations;
            DataSourceResult result = relations.ToDataSourceResult(request, relation => new
            {
                RelationId = relation.RelationId,
                Refereing = relation.Refereing,
                Reference = relation.Reference,
                /*  CreatedBy = relation.CreatedBy,
                  DateCreated = relation.DateCreated,
                  LastModified = relation.LastModified,
                  ModifiedBy = relation.ModifiedBy,
                  OrgBranchName = relation.OrgBranchName,*/
                Remark = relation.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Relations_Create([DataSourceRequest]DataSourceRequest request, Relation relation)
        {
            if (ModelState.IsValid)
            {
                var entity = new Relation
                {
                    Refereing = relation.Refereing,
                    Reference = relation.Reference,
                    Remark = relation.Remark
                };

                db.Relations.Add(entity);
                db.SaveChanges();
                relation.RelationId = entity.RelationId;
            }

            return Json(new[] { relation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Relations_Update([DataSourceRequest]DataSourceRequest request, Relation relation)
        {
            if (ModelState.IsValid)
            {
                var entity = new Relation
                {
                    RelationId = relation.RelationId,
                    Refereing = relation.Refereing,
                    Reference = relation.Reference,
                    Remark = relation.Remark
                };

                db.Relations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { relation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Relations_Destroy([DataSourceRequest]DataSourceRequest request, Relation relation)
        {
            if (ModelState.IsValid)
            {
                var entity = new Relation
                {
                    RelationId = relation.RelationId,
                    Refereing = relation.Refereing,
                    Reference = relation.Reference,
                    Remark = relation.Remark
                };

                db.Relations.Attach(entity);
                db.Relations.Remove(entity);
                db.SaveChanges();

                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  Relation number " + relation.RelationId);
      
            }

            return Json(new[] { relation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
