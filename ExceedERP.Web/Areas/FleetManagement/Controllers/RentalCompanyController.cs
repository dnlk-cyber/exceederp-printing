using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class RentalCompanyController : Controller
    {

        ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }



        public ActionResult RentalCompanies_Read([DataSourceRequest] DataSourceRequest request)
        {
            var rentalCompanies = db.RentalCompany;

            DataSourceResult result = rentalCompanies.ToDataSourceResult(request, rentalCompany => new
            {
                RentalCompanyId = rentalCompany.RentalCompanyId,
                RentalCompanyName = rentalCompany.RentalCompanyName,
                Address = rentalCompany.Address,
                Phone = rentalCompany.Phone,
                Email = rentalCompany.Email,
                DateCreated = rentalCompany.DateCreated,
                CreatedBy = rentalCompany.CreatedBy,
                ModifiedBy = rentalCompany.ModifiedBy,
                LastModified = rentalCompany.LastModified


            });

            return Json(result);
        }



        public ActionResult RentalCompanies_Create([DataSourceRequest] DataSourceRequest request, RentalCompany rentalCompany)
        {
            var exists = db.RentalCompany.Where(x => x.RentalCompanyName.Trim().ToLower() == rentalCompany.RentalCompanyName.Trim().ToLower()).Any();
            if (exists)
            {
                ModelState.AddModelError("exists", "Company name is already registered");
            }
            if (ModelState.IsValid)
            {
                var entity = new RentalCompany
                {
                    RentalCompanyId = rentalCompany.RentalCompanyId,
                    RentalCompanyName = rentalCompany.RentalCompanyName,
                    Address = rentalCompany.Address,
                    Phone = rentalCompany.Phone,
                    Email = rentalCompany.Email,
                    DateCreated = DateTime.Now.Date,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = rentalCompany.ModifiedBy,
                    LastModified = rentalCompany.LastModified
                };

                db.RentalCompany.Add(entity);
                db.SaveChanges();
                rentalCompany.RentalCompanyId = entity.RentalCompanyId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Rental company registration", "Success", User.Identity.Name, $"created rental company reg {entity.RentalCompanyId}", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { rentalCompany }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RentalCompanies_Update([DataSourceRequest] DataSourceRequest request, RentalCompany rentalCompany)
        {
            if (ModelState.IsValid)
            {
                var entity = new RentalCompany
                {
                    RentalCompanyId = rentalCompany.RentalCompanyId,
                    RentalCompanyName = rentalCompany.RentalCompanyName,
                    Address = rentalCompany.Address,
                    Phone = rentalCompany.Phone,
                    Email = rentalCompany.Email,
                    DateCreated = rentalCompany.DateCreated,
                    CreatedBy = rentalCompany.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now.Date


                };

                db.RentalCompany.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Rental company registration", "Success", User.Identity.Name, $"updated rental company reg {entity.RentalCompanyId}", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { rentalCompany }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult RentalCompanies_Destroy([DataSourceRequest] DataSourceRequest request, RentalCompany rentalCompany)
        {
            if (ModelState.IsValid)
            {
                var entity = new RentalCompany
                {
                    RentalCompanyId = rentalCompany.RentalCompanyId,
                    RentalCompanyName = rentalCompany.RentalCompanyName,
                    Address = rentalCompany.Address,
                    Phone = rentalCompany.Phone,
                    Email = rentalCompany.Email,
                    DateCreated = rentalCompany.DateCreated,
                    CreatedBy = rentalCompany.CreatedBy,
                    ModifiedBy = rentalCompany.ModifiedBy,
                    LastModified = rentalCompany.LastModified


                };

                db.RentalCompany.Attach(entity);
                db.RentalCompany.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted asset registration number {entity.RentalCompanyId }");
                #endregion
            }

            return Json(new[] { rentalCompany }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}