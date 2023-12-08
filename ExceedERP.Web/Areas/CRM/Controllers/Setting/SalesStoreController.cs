using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;
using System.Collections.Generic;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CRMAdmin)]
    public class SalesStoreController : Controller
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {

            ViewData["InventoryStore"] = db.InventoryStores.ToList();
            return View();
        }
        public ActionResult SalesPersonAssignment()
        {

            ViewData["Shops"] = db.SalesStores.Select(s => new { s.Description, s.SalesStoreId }).ToList();
            ViewData["PaymentMethod"] = db.PaymentMethods;
            return View();
        }
        public ActionResult SalesStores_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<SalesStore> salesStores = db.SalesStores.ToList();
            //List<SalesStore> listToReturn = new List<SalesStore>();
            //foreach (var store in salesStores)
            //{
            //    store.HasReference = HasReference(store);
            //    listToReturn.Add(store);
            //}
            DataSourceResult result = salesStores.ToDataSourceResult(request, salesStore => new SalesStore
            {
                SalesStoreId = salesStore.SalesStoreId,
                Location = salesStore.Location,
                StoreCode = salesStore.StoreCode,
                Description = salesStore.Description,
                Type = salesStore.Type,
                PosId = salesStore.PosId,
                PosServiceBaseUrl = salesStore.PosServiceBaseUrl,
                HasReference = HasReference(salesStore),

                CreatedBy = salesStore.CreatedBy,
                DateCreated = salesStore.DateCreated,
                LastModified = salesStore.LastModified,
                ModifiedBy = salesStore.ModifiedBy,
                OrgBranchName = salesStore.OrgBranchName,

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalesStores_Create([DataSourceRequest]DataSourceRequest request, SalesStore SalesStore)
        {
            var MappedStore = db.SalesStores.Where(x => x.Type == SalesStore.Type && SalesStore.Type == SalesStoreType.Store && x.StoreCode == SalesStore.StoreCode).ToList();
            if (MappedStore.Any())
            {
                ModelState.AddModelError("AlreadyMapped", " Store is Already Mapped !");
                return Json(new[] { SalesStore }.ToDataSourceResult(request, ModelState));
            }
            var existingMachineIdMapping = db.SalesStores.FirstOrDefault(s => !string.IsNullOrEmpty(s.PosId) && s.PosId.Trim() == SalesStore.PosId.Trim());
            if (existingMachineIdMapping != null )
            {
                ModelState.AddModelError("", $"{existingMachineIdMapping.PosId} is mapped to {existingMachineIdMapping.Description}. You cannot map it to {SalesStore.Description}");
            }
            if (ModelState.IsValid)
            {
                var Store = db.InventoryStores.Where(x => x.StoreCode == SalesStore.StoreCode).FirstOrDefault();
                if (Store != null)
                {
                    SalesStore.Location = Store.Branch.Name;

                    var entity = new SalesStore
                    {

                        SalesStoreId = SalesStore.SalesStoreId,
                        Location = SalesStore.Location,
                        StoreCode = SalesStore.StoreCode,
                        Description = SalesStore.Description,
                        Type = SalesStore.Type,
                        PosId = SalesStore.PosId,
                        PosServiceBaseUrl = SalesStore.PosServiceBaseUrl,

                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        InventoryStore = Store
                    };

                    db.SalesStores.Add(entity);
                    db.SaveChanges();
                    SalesStore.SalesStoreId = entity.SalesStoreId;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, $"CRM {entity.Type.ToString() } Create ", "Success", User.Identity.Name, $"CRM {entity.Type.ToString()} {entity.Description} [{entity.StoreCode}] created.", Modules.CRM);
                    #endregion
                }
            }

            return Json(new[] { SalesStore }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalesStores_Update([DataSourceRequest]DataSourceRequest request, SalesStore SalesStore)
        {
            var MappedStore = db.SalesStores.Where(x => x.SalesStoreId != SalesStore.SalesStoreId && x.Type == SalesStore.Type && SalesStore.Type == SalesStoreType.Store && x.StoreCode == SalesStore.StoreCode).ToList();
            if (MappedStore.Any())
            {
                ModelState.AddModelError("AlreadyMapped", " Store is Already Mapped !");
                return Json(new[] { SalesStore }.ToDataSourceResult(request, ModelState));
            }

            var existingMachineIdMapping = db.SalesStores.FirstOrDefault(s => s.SalesStoreId != SalesStore.SalesStoreId && !string.IsNullOrEmpty(s.PosId) && s.PosId.Trim() == SalesStore.PosId.Trim());
            if (existingMachineIdMapping != null)
            {
                ModelState.AddModelError("", $"{existingMachineIdMapping.PosId} is mapped to {existingMachineIdMapping.Description}. You cannot map it to {SalesStore.Description}");
            }
            if (ModelState.IsValid)
            {
              
                var Store = db.InventoryStores.Where(x => x.StoreCode == SalesStore.StoreCode).FirstOrDefault();
                if (Store != null)
                {
                    SalesStore.Location = Store.Branch.Name;
                }
                var entity = new SalesStore
                {
                    SalesStoreId = SalesStore.SalesStoreId,
                    Location = SalesStore.Location,
                    StoreCode = SalesStore.StoreCode,
                    Description = SalesStore.Description,
                    Type = SalesStore.Type,
                    PosId = SalesStore.PosId,
                    PosServiceBaseUrl = SalesStore.PosServiceBaseUrl,

                    CreatedBy = SalesStore.CreatedBy,
                    DateCreated = SalesStore.DateCreated,
                    LastModified = SalesStore.LastModified,
                    ModifiedBy = SalesStore.ModifiedBy,
                    OrgBranchName = SalesStore.OrgBranchName,
                };

                db.SalesStores.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, $"CRM {entity.Type.ToString() } Edit ", "Success", User.Identity.Name, $"CRM {entity.Type.ToString() } {entity.Description} [{entity.StoreCode}] updated.", Modules.CRM);
                #endregion
            }

            return Json(new[] { SalesStore }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalesStores_Destroy([DataSourceRequest]DataSourceRequest request, SalesStore SalesStore)
        {
            if (ModelState.IsValid)
            {
                var entity = new SalesStore
                {
                    SalesStoreId = SalesStore.SalesStoreId,
                    Location = SalesStore.Location,
                    StoreCode = SalesStore.StoreCode,
                    Description = SalesStore.Description,
                    Type = SalesStore.Type,
                    PosId = SalesStore.PosId,
                    PosServiceBaseUrl = SalesStore.PosServiceBaseUrl,

                    CreatedBy = SalesStore.CreatedBy,
                    DateCreated = SalesStore.DateCreated,
                    LastModified = SalesStore.LastModified,
                    ModifiedBy = SalesStore.ModifiedBy,
                    OrgBranchName = SalesStore.OrgBranchName,
                };

                db.SalesStores.Attach(entity);
                db.SalesStores.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, $"CRM {entity.Type.ToString() } Delete", "Success", User.Identity.Name, $"CRM {entity.Type.ToString() }  {entity.Description} [{entity.StoreCode} deleted.");

            }

            return Json(new[] { SalesStore }.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        private bool HasReference(SalesStore store)
        {
            // sales quote, sales order
            var hasLineItem = db.LineItems.Any(l => l.StoreID == store.SalesStoreId);
            var hashipment = db.SalesShipmentLineItem.Any(l => l.SalesStoreId == store.SalesStoreId);

            return (hashipment || hasLineItem);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}