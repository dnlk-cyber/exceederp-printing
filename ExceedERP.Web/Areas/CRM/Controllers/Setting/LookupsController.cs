using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.CRM;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CRMAdmin)] 
    public class LookupsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            SetValues();
            return View();
        }
        private void SetValues()
        {
            var LookUpTypes = Enum.GetValues(typeof(LookUpTypes))
                      .Cast<LookUpTypes>()
                      .ToList();
            var LookUpList = LookUpTypes.Select(s => new { Text = s.ToString(), Value = s.ToString() });
            ViewData["LookUpTypes"] = LookUpList;
        }
        public ActionResult Lookups_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Lookup> lookups = db.Lookups.OrderBy(l=> l.LookupType );
            DataSourceResult result = lookups.ToDataSourceResult(request, lookup => new {
                LookupID = lookup.LookupId,
                Code = lookup.Code,
                Category = lookup.Category,
                Index = lookup.Index,
                LookupType = lookup.LookupType,
                Description = lookup.Description,
                IsDefault = lookup.IsDefault,
                IsActive = lookup.IsActive,
                Remark = lookup.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Lookups_Create([DataSourceRequest]DataSourceRequest request, Lookup lookup)
        {
            List<Lookup> lookups = db.Lookups.Where(s => s.Description == lookup.Description).ToList();
            if (lookups.Any())
            {
                ModelState.AddModelError("DuplicateEntire", "Lookup Already Exist");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new Lookup
                    {
                        Code = lookup.Code,
                        Category = lookup.Category,
                        Index = lookup.Index,
                        LookupType = lookup.LookupType,
                        Description = lookup.Description,
                        IsDefault = true,
                        IsActive = true,
                       
                        Remark = lookup.Remark
                    };

                    db.Lookups.Add(entity);
                    db.SaveChanges();
                    lookup.LookupId = entity.LookupId;
                }
            }
            return Json(new[] { lookup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Lookups_Update([DataSourceRequest]DataSourceRequest request, Lookup lookup)
        {
            List<Lookup> lookups = db.Lookups.Where(s => s.Description == lookup.Description).ToList();
            if (lookups.Any())
            {
                ModelState.AddModelError("DuplicateEntire", "Lookup Already Exist");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new Lookup
                    {
                        LookupId = lookup.LookupId,
                        Code = lookup.Code,
                        Category = lookup.Category,
                        Index = lookup.Index,
                        LookupType = lookup.LookupType,
                        Description = lookup.Description,
                        IsDefault = lookup.IsDefault,
                        IsActive = lookup.IsActive,
                        Remark = lookup.Remark
                    };

                    db.Lookups.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return Json(new[] { lookup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Lookups_Destroy([DataSourceRequest]DataSourceRequest request, Lookup lookup)
        {
            if (ModelState.IsValid)
            {
                var entity = new Lookup
                {
                    LookupId = lookup.LookupId,
                    Code = lookup.Code,
                    Category = lookup.Category,
                    Index = lookup.Index,
                    LookupType = lookup.LookupType,
                    Description = lookup.Description,
                    IsDefault = lookup.IsDefault,
                    IsActive = lookup.IsActive,
                    Remark = lookup.Remark
                };

                db.Lookups.Attach(entity);
                db.Lookups.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted LookUp number " + lookup.LookupId);
      
            }

            return Json(new[] { lookup }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
