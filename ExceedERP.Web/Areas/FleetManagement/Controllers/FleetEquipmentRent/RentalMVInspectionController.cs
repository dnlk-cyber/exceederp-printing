using System.Data.Entity;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Helpers.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetInsurances;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using ExceedERP.Core.Domain.FleetManagement.EquipmentInspection;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Setting
{
    //[AuthorizeRoles(FleetManagementRoles.FleetTimeSheet)]
    public class RentalMVInspectionController : Controller
    {

        ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
           //ViewData["RentalMvInspection"] = db.RentalInspectionCriteria.Select(x => new { x.RentalInspectionCriteriaId, x.CriteriaCategory }).ToList();
            return View();
        }



        public ActionResult RentalMVInspection_Read([DataSourceRequest] DataSourceRequest request)
        {
            var rentalMVInspections = db.RentalMVInspections;

            DataSourceResult result = rentalMVInspections.ToDataSourceResult(request, rentalMVInspection => new
            {
                Index = rentalMVInspection.Index,
                RentalMVInspectionId = rentalMVInspection.RentalMVInspectionId,
                RentalInspectionCriteriaId=rentalMVInspection.RentalInspectionCriteriaId,
                HeadOfficeGoodCondtion = rentalMVInspection.HeadOfficeGoodCondtion,
                HeadOfficePoorCondtion = rentalMVInspection.HeadOfficePoorCondtion,
                BranchOfficeGoodCondtion = rentalMVInspection.BranchOfficeGoodCondtion,
                BranchOfficePoorCondtion = rentalMVInspection.BranchOfficePoorCondtion

            });

            return Json(result);
        }



        public ActionResult RentalMVInspection_Create([DataSourceRequest] DataSourceRequest request, RentalMVInspection rentalMVInspection)
        {
            if (ModelState.IsValid)
            {
                var entity = new RentalMVInspection
                {
                    RentalMVInspectionId = rentalMVInspection.RentalMVInspectionId,
                    RentalInspectionCriteriaId = rentalMVInspection.RentalInspectionCriteriaId,
                    Index = rentalMVInspection.Index,
                    HeadOfficeGoodCondtion = rentalMVInspection.HeadOfficeGoodCondtion,
                    HeadOfficePoorCondtion = rentalMVInspection.HeadOfficePoorCondtion,
                    BranchOfficeGoodCondtion = rentalMVInspection.BranchOfficeGoodCondtion,
                    BranchOfficePoorCondtion = rentalMVInspection.BranchOfficePoorCondtion

                };

                db.RentalMVInspections.Add(entity);
                db.SaveChanges();
                rentalMVInspection.RentalInspectionCriteriaId = entity.RentalInspectionCriteriaId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Fleet insurance registration", "Success", User.Identity.Name, $"created insurance registration {entity.RentalMVInspectionId} ", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { rentalMVInspection }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RentalMVInspection_Update([DataSourceRequest] DataSourceRequest request, RentalMVInspection rentalMVInspection)
        {
            if (ModelState.IsValid)
            {
                var entity = new RentalMVInspection
                {
                    RentalMVInspectionId = rentalMVInspection.RentalMVInspectionId,
                    RentalInspectionCriteriaId = rentalMVInspection.RentalInspectionCriteriaId,
                    Index = rentalMVInspection.Index,
                    HeadOfficeGoodCondtion = rentalMVInspection.HeadOfficeGoodCondtion,
                    HeadOfficePoorCondtion = rentalMVInspection.HeadOfficePoorCondtion,
                    BranchOfficeGoodCondtion = rentalMVInspection.BranchOfficeGoodCondtion,
                    BranchOfficePoorCondtion = rentalMVInspection.BranchOfficePoorCondtion,
                    RentalInspectionCriteriaDetailId = rentalMVInspection.RentalInspectionCriteriaDetailId,
                    RentalInspectionId = rentalMVInspection.RentalInspectionId,
                    
                };

                db.RentalMVInspections.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Fleet insurance update", "Success", User.Identity.Name, $"updated insurance registration number {entity.RentalMVInspectionId} ", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { rentalMVInspection }.ToDataSourceResult(request, ModelState));
        }


        public ActionResult RentalMVInspection_Destroy([DataSourceRequest] DataSourceRequest request, RentalMVInspection rentalMVInspection)
        {
            if (ModelState.IsValid)
            {
                var entity = new RentalMVInspection
                {
                    RentalMVInspectionId = rentalMVInspection.RentalMVInspectionId,
                    RentalInspectionCriteriaId = rentalMVInspection.RentalInspectionCriteriaId,
                    Index = rentalMVInspection.Index,
                    HeadOfficeGoodCondtion = rentalMVInspection.HeadOfficeGoodCondtion,
                    HeadOfficePoorCondtion = rentalMVInspection.HeadOfficePoorCondtion,
                    BranchOfficeGoodCondtion = rentalMVInspection.BranchOfficeGoodCondtion,
                    BranchOfficePoorCondtion = rentalMVInspection.BranchOfficePoorCondtion,
                    RentalInspectionCriteriaDetailId = rentalMVInspection.RentalInspectionCriteriaDetailId,
                    RentalInspectionId = rentalMVInspection.RentalInspectionId,
                };
                db.RentalMVInspections.Attach(entity);
                db.RentalMVInspections.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted insurance registration number {entity.RentalMVInspectionId }");
                #endregion
            }

            return Json(new[] { rentalMVInspection }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}