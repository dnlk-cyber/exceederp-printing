using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.FleetEquipmentRent
{
    public class RentFromExternalCustomerChangeController : Controller
    {
        // GET: FleetManagement/RentedEquipmentAdvancePayment
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read([DataSourceRequest]DataSourceRequest request,Guid id)
        {
            var changedCustomers = db.RentFromExternalCustomerChange.Where(x => x.RentFromExternalId == id);
            DataSourceResult result = changedCustomers.ToDataSourceResult(request, x => new
            {
                RentFromExternalCustomerChangeId = x.RentFromExternalCustomerChangeId,
                RentFromExternalId = x.RentFromExternalId,
                ProcurementSupplierID = x.ProcurementSupplierID,
                IsCurent = x.IsCurent,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                LastModified =x.LastModified,
                ModifiedBy= x.ModifiedBy,
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create([DataSourceRequest]DataSourceRequest request, RentFromExternalCustomerChange changedCustomer, Guid id)
        {
            ModelState.Remove("RentFromExternalId");
           
            var agreement = db.RentFromExternal.FirstOrDefault(x => x.RentFromExternalID == id);
                if(ModelState.IsValid)
            {
                var entity = new RentFromExternalCustomerChange()
                {
                    RentFromExternalId = id,
                    ProcurementSupplierID = agreement.ProcurementSupplierID,
                    IsCurent = true,                   
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now
                };
                db.RentFromExternalCustomerChange.Add(entity);
                db.SaveChanges();
                agreement.ProcurementSupplierID = changedCustomer.ProcurementSupplierID;
                db.Entry(agreement).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                //var fleets = db.Fleets.Where(x => x.RentFromExternalNum == agreement.Num).ToList();
                //foreach(var fleet in fleets)
                //{
                //    fleet.
                //}
               
            }
            return Json(new[] { changedCustomer }.ToDataSourceResult(request, ModelState));

        }
        public ActionResult Update([DataSourceRequest]DataSourceRequest request, RentFromExternalCustomerChange changedCustomer)
        {
            ModelState.Remove("RentFromExternalId");
            var agreement = db.RentFromExternal.FirstOrDefault(x => x.RentFromExternalID == changedCustomer.RentFromExternalId);
            if (ModelState.IsValid)
            {
                agreement.ProcurementSupplierID = changedCustomer.ProcurementSupplierID;
                db.Entry(agreement).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { changedCustomer }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Destroy([DataSourceRequest]DataSourceRequest request, RentFromExternalCustomerChange changedCustomer)
        {
            ModelState.Remove("RentFromExternalId");
            var agreement = db.RentFromExternal.FirstOrDefault(x => x.RentFromExternalID == changedCustomer.RentFromExternalId);
            if (ModelState.IsValid)
            {
                var oldCustomer = db.RentFromExternalCustomerChange.Where(x => x.RentFromExternalId == changedCustomer.RentFromExternalId && x.IsCurent == true).OrderByDescending(x=>x.RentFromExternalCustomerChangeId).FirstOrDefault();
                agreement.ProcurementSupplierID = oldCustomer.ProcurementSupplierID;
                db.RentFromExternalCustomerChange.Remove(oldCustomer);
                db.SaveChanges();
                db.Entry(agreement).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new[] { changedCustomer }.ToDataSourceResult(request, ModelState));

            }
            return Json(new[] { changedCustomer }.ToDataSourceResult(request, ModelState));

        }
    }
}