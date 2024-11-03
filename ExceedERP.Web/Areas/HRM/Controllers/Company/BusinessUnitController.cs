using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
    public class BusinessUnitController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            PopulateCategories();
            return View();
        }

        private void PopulateCategories()
        {
            
        }

        public ActionResult BusinessUnits_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<BusinessUnit> BusinessUnits = db.BusinessUnits;
            DataSourceResult result = BusinessUnits.ToDataSourceResult(request, BusinessUnit => new {
                BusinessUnitId = BusinessUnit.BusinessUnitId,
                Name = BusinessUnit.Name,
                Accountcode = BusinessUnit.Accountcode,
                DateCreated = BusinessUnit.DateCreated,
                LastModified = BusinessUnit.LastModified,
                CreatedBy = BusinessUnit.CreatedBy,
                ModifiedBy = BusinessUnit.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BusinessUnits_Create([DataSourceRequest]DataSourceRequest request, BusinessUnit BusinessUnit)
        {
            #region validation


            var exists = db.BusinessUnits.Where(x => x.Accountcode == BusinessUnit.Accountcode).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("Account Code",
                    BusinessUnit.Accountcode + " already exists in the system.");

            }

            var exists1 = db.BusinessUnits.Where(x => x.Name == BusinessUnit.Name).ToList();

            if (exists1.Any())
            {
                ModelState.AddModelError("Name",
                    BusinessUnit.Name + " already exists in the system.");

            }

            #endregion

            if (ModelState.IsValid)
            {
                var entity = new BusinessUnit
                {
                    Accountcode = BusinessUnit.Accountcode,
                    Name = BusinessUnit.Name,
                    DateCreated = DateTime.Now,
                    LastModified = BusinessUnit.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = BusinessUnit.ModifiedBy
                };

                db.BusinessUnits.Add(entity);
                db.SaveChanges();
                BusinessUnit.BusinessUnitId = entity.BusinessUnitId;
            }

            return Json(new[] { BusinessUnit }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BusinessUnits_Update([DataSourceRequest]DataSourceRequest request, BusinessUnit BusinessUnit)
        {
            #region validation
            var exists = db.BusinessUnits.Where(x => x.BusinessUnitId != BusinessUnit.BusinessUnitId)
                                                  .Where(x => x.Accountcode == BusinessUnit.Accountcode).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("Account Code",
                     BusinessUnit.Accountcode + " already exists in the system.");
            }

            var exists1 = db.BusinessUnits.Where(x => x.BusinessUnitId != BusinessUnit.BusinessUnitId)
                                                 .Where(x => x.Name == BusinessUnit.Name).ToList();

            if (exists1.Any())
            {
                ModelState.AddModelError("Name",
                     BusinessUnit.Name + " already exists in the system.");
            }

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new BusinessUnit
                {
                    Accountcode = BusinessUnit.Accountcode,
                    BusinessUnitId = BusinessUnit.BusinessUnitId,
                    Name = BusinessUnit.Name,
                    DateCreated = BusinessUnit.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = BusinessUnit.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.BusinessUnits.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { BusinessUnit }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BusinessUnits_Destroy([DataSourceRequest]DataSourceRequest request, BusinessUnit BusinessUnit)
        {
            if (ModelState.IsValid)
            {
                 #region validation

                    bool valid = true;
                    valid = false;
                    #endregion

                if (valid)
                {
                    var entity = new BusinessUnit
                    {
                        Accountcode = BusinessUnit.Accountcode,
                        BusinessUnitId = BusinessUnit.BusinessUnitId,
                        Name = BusinessUnit.Name,
                        DateCreated = BusinessUnit.DateCreated,
                        LastModified = BusinessUnit.LastModified,
                        CreatedBy = BusinessUnit.CreatedBy,
                        ModifiedBy = BusinessUnit.ModifiedBy
                    };

                    db.BusinessUnits.Attach(entity);
                    db.BusinessUnits.Remove(entity);
                    db.SaveChanges();
                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Business Unit " + BusinessUnit.Name+ " and "+BusinessUnit.Accountcode);
                    #endregion
                }
            }

            return Json(new[] { BusinessUnit }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
