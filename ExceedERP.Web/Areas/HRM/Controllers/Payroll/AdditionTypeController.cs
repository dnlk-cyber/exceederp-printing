using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
    [Authorize(Roles = Payroll_Admin)]
    public class AdditionTypeController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string Payroll_Admin = "Payroll_Admin";
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Lookups_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Lookup> lookups = db.Lookups.Where (x=> x.LookupType ==LookUpTypes .ADDITION_TYPE );
            DataSourceResult result = lookups.ToDataSourceResult(request, lookup => new {
                LookupId = lookup.LookupId,
                Code = lookup.Code,
                Category = lookup.Category,
                Index = lookup.Index,
                LookupType = lookup.LookupType,
                Description = lookup.Description,
                IsDefault = lookup.IsDefault,
                IsActive = lookup.IsActive,
                Remark = lookup.Remark,
                DateCreated = lookup.DateCreated,
                LastModified = lookup.LastModified,
                CreatedBy = lookup.CreatedBy,
                ModifiedBy = lookup.ModifiedBy 
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Lookups_Create([DataSourceRequest]DataSourceRequest request, Lookup lookup)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var checkExitance = db.Lookups.Where(x => x.LookupType == LookUpTypes.ADDITION_TYPE && x.Description == lookup.Description).FirstOrDefault();
                if (checkExitance != null)
                {
                    ModelState.AddModelError("", "This addition type is already exist!!");
                }
                else
                {
                    var entity = new Lookup
                    {
                        Code = lookup.Code,
                        Category = lookup.Category,
                        Index = lookup.Index,
                        LookupType = LookUpTypes.ADDITION_TYPE,
                        Description = lookup.Description,
                        IsDefault = lookup.IsDefault,
                        IsActive = true,
                        Remark = lookup.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = lookup.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = lookup.ModifiedBy
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
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                 var checkExitance = db.PayrollProcessAdditions.Where (x=> x.AdditionId!=0).FirstOrDefault();
                 if (checkExitance != null)
                 {
                     ModelState.AddModelError("", "You cannot update this addition type. It already processed!!");
                 }
                 else
                 {
                     var entity = new Lookup
                     {
                         LookupId = lookup.LookupId,
                         Code = lookup.Code,
                         Category = lookup.Category,
                         Index = lookup.Index,
                         LookupType = LookUpTypes.ADDITION_TYPE,
                         Description = lookup.Description,
                         IsDefault = lookup.IsDefault,
                         IsActive = lookup.IsActive,
                         Remark = lookup.Remark,
                         DateCreated = lookup.DateCreated,
                         LastModified = DateTime.Now,
                         CreatedBy = lookup.CreatedBy,
                         ModifiedBy = User.Identity.Name
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
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                 var checkExitance = db.PayrollProcessAdditions.Where (x=> x.AdditionId!=0).FirstOrDefault();
                 if (checkExitance != null)
                 {
                     ModelState.AddModelError("","You cannot delete this addition type. It already processed!!");
                 }
                 else
                 {
                     var entity = new Lookup
                     {
                         LookupId = lookup.LookupId,
                         Code = lookup.Code,
                         Category = lookup.Category,
                         Index = lookup.Index,
                         LookupType = LookUpTypes.ADDITION_TYPE,
                         Description = lookup.Description,
                         IsDefault = lookup.IsDefault,
                         IsActive = lookup.IsActive,
                         Remark = lookup.Remark,
                         DateCreated = lookup.DateCreated,
                         LastModified = lookup.LastModified,
                         CreatedBy = lookup.CreatedBy,
                         ModifiedBy = lookup.ModifiedBy
                     };
                     #region delete log
                     UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Addition type information of type " + lookup.Description,Core.Domain.Global.Modules.Payroll);
                     #endregion
                     db.Lookups.Attach(entity);
                     db.Lookups.Remove(entity);
                     db.SaveChanges();
                 }
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
