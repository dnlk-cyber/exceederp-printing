using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;

using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Setting
{
	public class InternalRentalRateController : Controller
	{
		ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();

        public ActionResult Index()
		{

            ViewData["category"] = CacheData.GetEquipmentIdentityCaches().Select(
    i => new
    {
        Text = i.Name,
        Value = i.Id
    }).AsQueryable();
            ViewData["EquipmentType"] = CacheData.GetEquipmentTypeCaches().Select(
    i => new
    {
        Text = i.Name,
        Value = i.Id
    }).AsQueryable();

            return View();
		}

		public ActionResult Rate_Read([DataSourceRequest] DataSourceRequest request)
		{
            var categoryRate = db.InternalRentalRate.ToList();
            DataSourceResult result=categoryRate.ToDataSourceResult(request,e=>new

            {
                CategoryId = e.CategoryId,
                InternalRentalRateId = e.InternalRentalRateId,
                Description = e.Description,
                Rate = e.Rate,
                RentType=e.RentType,
                MarketPrice=e.MarketPrice,
                EquipmentTypeId = e.EquipmentTypeId,
                CreatedBy=e.CreatedBy,
                DateCreated=e.DateCreated,
                ModifiedBy=e.ModifiedBy,
                LastModified = e.LastModified


				}
		);

			return Json(categoryRate.ToDataSourceResult(request));
		}
		[AcceptVerbs(HttpVerbs.Post)]
		public JsonResult Rate_Create([DataSourceRequest] DataSourceRequest request, InternalRentalRate internalRentalRate)
			{
            var duplicate = db.InternalRentalRate.FirstOrDefault(x => x.EquipmentTypeId == internalRentalRate.EquipmentTypeId);
            if(duplicate!=null)
            {
                ModelState.AddModelError("duplicate", "Rate is set for this category");
            }

            if(internalRentalRate.Rate<=0||internalRentalRate.Rate>=100)
            {
               
                    ModelState.AddModelError("invalid rate", "Rate is not allowed below zero or above 100");
            }
            if (ModelState.IsValid)
            {
                var internalRentalRt = new InternalRentalRate()
                {
                    Description = internalRentalRate.Description,
                    RentType = internalRentalRate.RentType,
                    MarketPrice = internalRentalRate.MarketPrice,
                    Rate = internalRentalRate.Rate,
                    CategoryId = internalRentalRate.CategoryId,
                    EquipmentTypeId = internalRentalRate.EquipmentTypeId,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now
                };
                db.InternalRentalRate.Add(internalRentalRt);
                db.SaveChanges();
            }
            return Json(new[] { internalRentalRate }.ToDataSourceResult(request));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public JsonResult rate_Update([DataSourceRequest] DataSourceRequest request, InternalRentalRate internalRentalRate)
		{
            var duplicate = db.InternalRentalRate.FirstOrDefault(x => x.EquipmentTypeId == internalRentalRate.EquipmentTypeId&&x.InternalRentalRateId!=internalRentalRate.InternalRentalRateId);
            if (duplicate != null)
            {
                ModelState.AddModelError("duplicate", "Rate is set for this category");
            }

            if (internalRentalRate.Rate <= 0 || internalRentalRate.Rate >= 100)
            {

                ModelState.AddModelError("invalid rate", "Rate is not allowed below zero or above 100");

            }
            var existingRate = db.InternalRentalRate.FirstOrDefault(x => x.InternalRentalRateId == internalRentalRate.InternalRentalRateId);
                if(ModelState.IsValid)
            {
                existingRate.CategoryId = internalRentalRate.CategoryId;
                existingRate.EquipmentTypeId = internalRentalRate.EquipmentTypeId;
                existingRate.Description = internalRentalRate.Description;
                existingRate.Rate = internalRentalRate.Rate;
                existingRate.MarketPrice = internalRentalRate.MarketPrice;
                existingRate.RentType = internalRentalRate.RentType;
                existingRate.LastModified = internalRentalRate.LastModified;
                existingRate.ModifiedBy = internalRentalRate.ModifiedBy;
                db.Entry(existingRate).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
			return Json(new[] { internalRentalRate }.ToDataSourceResult(request));
		}

		public JsonResult Rate_Destroy([DataSourceRequest] DataSourceRequest request, InternalRentalRate internalRentalRate)
		{
            var existingRate = db.InternalRentalRate.FirstOrDefault(x => x.InternalRentalRateId == internalRentalRate.InternalRentalRateId);
            if (ModelState.IsValid)
            {
                
                db.InternalRentalRate.Remove(existingRate);
                db.SaveChanges();
            }

            return Json(new[] { internalRentalRate }.ToDataSourceResult(request));
		}
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

