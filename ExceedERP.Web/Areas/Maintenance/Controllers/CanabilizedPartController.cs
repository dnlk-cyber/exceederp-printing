using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    public class CanabilizedPartController : Controller
    {
        // GET: Maintenance/Canibalization
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read([DataSourceRequest]DataSourceRequest request,string id)
        {
            var FetchCanibalization = db.CanibalizedParts.Where(x => x.CanibalizationId.ToString() == id).AsQueryable();
            DataSourceResult Result = FetchCanibalization.ToDataSourceResult(request, CanibalizationRecord => new
            {
                CanibalizationId = CanibalizationRecord.CanibalizationId,
                CanibalizedPartsId = CanibalizationRecord.CanibalizedPartsId,
                Unit = CanibalizationRecord.Unit,
                Description = CanibalizationRecord.Description,
                VehiclePartId = CanibalizationRecord.VehiclePartId,
                Quantity = CanibalizationRecord.Quantity,
                CreatedBy = CanibalizationRecord.CreatedBy,
                DateCreated = CanibalizationRecord.DateCreated,
                ModifiedBy = CanibalizationRecord.ModifiedBy,
                LastModified = CanibalizationRecord.LastModified,             
            });

            return Json(Result);
        }

        public ActionResult Create([DataSourceRequest]DataSourceRequest request, CanibalizedParts canibalization,int id)
        {
            canibalization.CreatedBy = User.Identity.Name;
            canibalization.DateCreated = DateTime.Now;
            if (ModelState.IsValid)
            {
                var entity = (CanibalizedParts)this.GetMappedCanibalization(canibalization, id);
                db.CanibalizedParts.Add(entity);
                db.SaveChanges();
            }

            return Json(new[] { canibalization }.ToDataSourceResult(request, ModelState));
        }
        private object GetMappedCanibalization(CanibalizedParts CanibalizationRecord, int id)
        {
            var entity = new CanibalizedParts
            {
                CanibalizationId =id,
                CanibalizedPartsId = CanibalizationRecord.CanibalizedPartsId,
                Unit = CanibalizationRecord.Unit,
                Description = CanibalizationRecord.Description,
                VehiclePartId = CanibalizationRecord.VehiclePartId,
                Quantity = CanibalizationRecord.Quantity,
                CreatedBy = CanibalizationRecord.CreatedBy,
                DateCreated = CanibalizationRecord.DateCreated,
                ModifiedBy = CanibalizationRecord.ModifiedBy,
                LastModified = CanibalizationRecord.LastModified,
            };

            return entity;
        }
        public ActionResult Update([DataSourceRequest]DataSourceRequest request, CanibalizedParts canibalization)
        {
            var FetchCanablizedPart = db.CanibalizedParts.FirstOrDefault(x => x.CanibalizedPartsId == canibalization.CanibalizedPartsId);
            if(FetchCanablizedPart==null)
            {
                ModelState.AddModelError("No record found", "No record found");
            }
            FetchCanablizedPart.VehiclePartId = canibalization.VehiclePartId;
            FetchCanablizedPart.Unit = canibalization.Unit;
            FetchCanablizedPart.Description = canibalization.Description;
            FetchCanablizedPart.Quantity = canibalization.Quantity;
            FetchCanablizedPart.ModifiedBy = User.Identity.Name;
            FetchCanablizedPart.LastModified = DateTime.Now;
            if(ModelState.IsValid)
            {
                db.Entry(FetchCanablizedPart).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { canibalization }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Destroy([DataSourceRequest]DataSourceRequest request, CanibalizedParts canibalization)
        {
            var FetchCanablizedPart = db.CanibalizedParts.FirstOrDefault(x => x.CanibalizedPartsId == canibalization.CanibalizedPartsId);
            if (FetchCanablizedPart == null)
            {
                ModelState.AddModelError("No record found", "No record found");
            }
           
            if (ModelState.IsValid)
            {
                db.CanibalizedParts.Remove(FetchCanablizedPart);
                db.SaveChanges();
            }
            return Json(new[] { canibalization }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}