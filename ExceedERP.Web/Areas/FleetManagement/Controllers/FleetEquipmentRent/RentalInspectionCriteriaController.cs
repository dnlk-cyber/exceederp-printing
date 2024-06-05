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

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Setting
{
    //[AuthorizeRoles(FleetManagementRoles.FleetTimeSheet)]
    public class RentalInspectionCriteriaController : Controller
    {

        ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
           return View();
        }



        public ActionResult RentalInspectionCriteria_Read([DataSourceRequest] DataSourceRequest request)
        {
            var rentalInspectionCriterias = db.RentalInspectionCriteria;
            DataSourceResult result = rentalInspectionCriterias.ToDataSourceResult(request, rentalInspectionCriteria => new
            {
                RentalInspectionCriteriaId = rentalInspectionCriteria.RentalInspectionCriteriaId,
                CriteriaCategory = rentalInspectionCriteria.CriteriaCategory,
                Index = rentalInspectionCriteria.Index
            });
            return Json(result);
        }
        public ActionResult RentalInspectionCriteria_Create([DataSourceRequest] DataSourceRequest request, RentalInspectionCriteria rentalInspectionCriteria)
        {
            if (ModelState.IsValid)
            {
                var entity = new RentalInspectionCriteria
                {
                    RentalInspectionCriteriaId = rentalInspectionCriteria.RentalInspectionCriteriaId,
                    CriteriaCategory = rentalInspectionCriteria.CriteriaCategory,
                    //CriteriaName = rentalInspectionCriteria.CriteriaName,
                    Index = rentalInspectionCriteria.Index

                };

                db.RentalInspectionCriteria.Add(entity);
                db.SaveChanges();
                rentalInspectionCriteria.RentalInspectionCriteriaId = entity.RentalInspectionCriteriaId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Fleet insurance registration", "Success", User.Identity.Name, $"created insurance registration {entity.RentalInspectionCriteriaId} ", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { rentalInspectionCriteria }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RentalInspectionCriteria_Update([DataSourceRequest] DataSourceRequest request, RentalInspectionCriteria rentalInspectionCriteria)
        {
            if (ModelState.IsValid)
            {
                var entity = new RentalInspectionCriteria
                {
                    RentalInspectionCriteriaId = rentalInspectionCriteria.RentalInspectionCriteriaId,
                    CriteriaCategory = rentalInspectionCriteria.CriteriaCategory,
                    //CriteriaName = rentalInspectionCriteria.CriteriaName,
                    Index = rentalInspectionCriteria.Index
                };

                db.RentalInspectionCriteria.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Fleet insurance update", "Success", User.Identity.Name, $"updated insurance registration number {entity.RentalInspectionCriteriaId} ", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { rentalInspectionCriteria }.ToDataSourceResult(request, ModelState));
        }


        public ActionResult RentalInspectionCriteria_Destroy([DataSourceRequest] DataSourceRequest request, RentalInspectionCriteria rentalInspectionCriteria)
        {
            if (ModelState.IsValid)
            {
                var entity = new RentalInspectionCriteria
                {
                    RentalInspectionCriteriaId = rentalInspectionCriteria.RentalInspectionCriteriaId,
                    CriteriaCategory = rentalInspectionCriteria.CriteriaCategory,
                    Index = rentalInspectionCriteria.Index


                };

                db.RentalInspectionCriteria.Attach(entity);
                db.RentalInspectionCriteria.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted insurance registration number {entity.RentalInspectionCriteriaId }");
                #endregion
            }

            return Json(new[] { rentalInspectionCriteria }.ToDataSourceResult(request, ModelState));
        }

        //Detail Section
        public ActionResult RentalInspectionCriteriaDetail_Read([DataSourceRequest] DataSourceRequest request,int id)
        {
            var rentalInspectionCriteriaDetails = db.RentalInspectionCriteriaDetails.Where(x=>x.RentalInspectionCriteriaId ==id);

            DataSourceResult result = rentalInspectionCriteriaDetails.ToDataSourceResult(request, rentalInspectionCriteriaDetail => new
            {
                RentalInspectionCriteriaDetailId =rentalInspectionCriteriaDetail.RentalInspectionCriteriaDetailId,
                RentalInspectionCriteriaId = rentalInspectionCriteriaDetail.RentalInspectionCriteriaId,
                //SerialNumber =GetFormattedIndex (rentalInspectionCriteriaDetail.RentalInspectionCriteriaDetailId),
                CriteriaName = rentalInspectionCriteriaDetail.CriteriaName,
                Index = rentalInspectionCriteriaDetail.Index

            });

            return Json(result);
        }
        //private string GetFormattedIndex(int id)
        //{
        //    var index2 = db.RentalInspectionCriteriaDetails.Find(id);
        //    if(index2 != null)
        //    {
        //        var index1 = db.RentalInspectionCriteria.Find(index2.RentalInspectionCriteriaId);
        //        if(index1 != null)
        //        {
        //            return index1.Index.ToString() + "." + index2.Index.ToString();
        //        }
            
        //    }
        //    return "";
        //}

        public ActionResult RentalInspectionCriteriaDetail_Create([DataSourceRequest] DataSourceRequest request, RentalInspectionCriteriaDetail rentalInspectionCriteriaDetail, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new RentalInspectionCriteriaDetail
                {
                    RentalInspectionCriteriaDetailId = rentalInspectionCriteriaDetail.RentalInspectionCriteriaDetailId,
                    RentalInspectionCriteriaId = id,
                    CriteriaName = rentalInspectionCriteriaDetail.CriteriaName,
                    Index = rentalInspectionCriteriaDetail.Index

                };

                db.RentalInspectionCriteriaDetails.Add(entity);
                db.SaveChanges();
                rentalInspectionCriteriaDetail.RentalInspectionCriteriaId = entity.RentalInspectionCriteriaId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Fleet insurance registration", "Success", User.Identity.Name, $"created insurance registration {entity.RentalInspectionCriteriaId} ", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { rentalInspectionCriteriaDetail }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RentalInspectionCriteriaDetail_Update([DataSourceRequest] DataSourceRequest request, RentalInspectionCriteriaDetail rentalInspectionCriteriaDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new RentalInspectionCriteriaDetail
                {
                    RentalInspectionCriteriaDetailId = rentalInspectionCriteriaDetail.RentalInspectionCriteriaDetailId,
                    RentalInspectionCriteriaId = rentalInspectionCriteriaDetail.RentalInspectionCriteriaId,
                    CriteriaName = rentalInspectionCriteriaDetail.CriteriaName,
                    Index = rentalInspectionCriteriaDetail.Index
                };

                db.RentalInspectionCriteriaDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Fleet insurance update", "Success", User.Identity.Name, $"updated insurance registration number {entity.RentalInspectionCriteriaId} ", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { rentalInspectionCriteriaDetail }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult RentalInspectionCriteriaDetail_Destroy([DataSourceRequest] DataSourceRequest request, RentalInspectionCriteriaDetail rentalInspectionCriteriaDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new RentalInspectionCriteriaDetail
                {
                    RentalInspectionCriteriaDetailId = rentalInspectionCriteriaDetail.RentalInspectionCriteriaDetailId,
                    RentalInspectionCriteriaId = rentalInspectionCriteriaDetail.RentalInspectionCriteriaId,
                    CriteriaName = rentalInspectionCriteriaDetail.CriteriaName,
                    Index = rentalInspectionCriteriaDetail.Index


                };

                db.RentalInspectionCriteriaDetails.Attach(entity);
                //db.RentalInspectionCriteriaDetails.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted insurance registration number {entity.RentalInspectionCriteriaId }");
                #endregion
            }

            return Json(new[] { rentalInspectionCriteriaDetail }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}