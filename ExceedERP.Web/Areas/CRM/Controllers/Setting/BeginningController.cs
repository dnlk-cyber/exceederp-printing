using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles( CRMRoles.CRMAdmin)] 
    public class BeginningController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Beginnings_Read([DataSourceRequest]DataSourceRequest request, int id, string beginningReferenceType)
        {
            IQueryable<Beginning> beginnings = db.Beginnings.Where(b => b.Reference == id.ToString() && b.ReferenceType == beginningReferenceType);
            DataSourceResult result = beginnings.ToDataSourceResult(request, beginning => new
            {
                BeginningID = beginning.BeginningID,
                Reference = beginning.Reference,
                Period = beginning.Period,
                Value = beginning.Value,
                IsProvisional = beginning.IsProvisional,
                ReferenceType = beginning.ReferenceType,
                CreatedBy = beginning.CreatedBy,
                DateCreated = beginning.DateCreated,
                ModifiedBy = beginning.ModifiedBy,
                LastModified = beginning.LastModified,
                Remark = beginning.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Beginnings_Create([DataSourceRequest]DataSourceRequest request, Beginning beginning, int id, string beginningReferenceType)
        {
            List<Beginning> beginnings = db.Beginnings.Where(tc => tc.Period == beginning.Period && tc.BeginningID == beginning.BeginningID).ToList();

            if (beginnings.Any())
            {
                ModelState.AddModelError("DuplicateIndexEntry", "Period Name already exist !");
            }
            else
            {

                if (ModelState.IsValid)
                {
                    var entity = new Beginning
                    {
                        Reference = id.ToString(),
                        ReferenceType = beginningReferenceType,
                        Period = beginning.Period,
                        Value = beginning.Value,
                        IsProvisional = beginning.IsProvisional,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        Remark = beginning.Remark
                    };

                    db.Beginnings.Add(entity);
                    db.SaveChanges();
                    beginning.BeginningID = entity.BeginningID;
                }
            }
            return Json(new[] { beginning }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Beginnings_Update([DataSourceRequest]DataSourceRequest request, Beginning beginning)
        {
            
            if (ModelState.IsValid)
            {
                var entity = new Beginning
                {
                    BeginningID = beginning.BeginningID,
                    Reference = beginning.Reference,
                    Period = beginning.Period,
                    Value = beginning.Value,
                    IsProvisional = beginning.IsProvisional,
                    Remark = beginning.Remark
                };

                db.Beginnings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                //}
            }
            return Json(new[] { beginning }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Beginnings_Destroy([DataSourceRequest]DataSourceRequest request, Beginning beginning)
        {
            if (ModelState.IsValid)
            {
                var entity = new Beginning
                {
                    BeginningID = beginning.BeginningID,
                    Reference = beginning.Reference,
                    Period = beginning.Period,
                    Value = beginning.Value,
                    IsProvisional = beginning.IsProvisional,
                    Remark = beginning.Remark
                };

                db.Beginnings.Attach(entity);
                db.Beginnings.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Beginning number " + beginning.BeginningID);
		
     
            }

            return Json(new[] { beginning }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
