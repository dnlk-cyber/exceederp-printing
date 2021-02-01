using ExceedERP.Core.Domain.APTS;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.APTS.Controllers
{
    public class DrugInnerTransferFromStoreController : Controller
    {
        // GET: APTS/StoreInnerTransfer
        ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        public void PopulateViewData()
        {
            ViewData["Branch"] = db.Branch.ToList();
            ViewData["DrugStore"] = db.InventoryStores.ToList(); //db.DrugStores.ToList();
            ViewData["Store"] = db.InventoryStores.ToList();
            ViewData["Unit"] = db.DrugUnits.ToList();
            ViewData["DrugList"] = db.DrugLists.ToList();
            ViewData["Items"] = db.InventoryItems.ToList();
            ViewData["ItemCatagory"] = db.ItemCategorys.ToList();
            ViewData["DrugStores"] = db.DrugStores.ToList();
        }
        public ActionResult DrugInnerTransferFromStore_Read([DataSourceRequest]DataSourceRequest request)
        {
            var drugStoreTransfers = db.DrugStoreTransfers.ToList();
            DataSourceResult result = drugStoreTransfers.ToDataSourceResult(request, drugStoreTransfer => new
            {
                DrugStoreTransferId = drugStoreTransfer.DrugStoreTransferId,
                FromDrugStoreId = drugStoreTransfer.FromDrugStoreId,
                ToDrugStoreId = drugStoreTransfer.ToDrugStoreId,
                BranchId = drugStoreTransfer.BranchId,

            });

            return Json(result);
        }
        public ActionResult DrugInnerTransferFromStore_Create([DataSourceRequest]DataSourceRequest request, DrugStoreTransfer drugStoreTransfer)
        {
            if (ModelState.IsValid)
            {
                var entity = new DrugStoreTransfer
                {
                    FromDrugStoreId = drugStoreTransfer.FromDrugStoreId,
                    ToDrugStoreId = drugStoreTransfer.ToDrugStoreId,
                    BranchId = drugStoreTransfer.BranchId,

                };
                db.DrugStoreTransfers.Add(entity);
                db.SaveChanges();
            }

            return Json(new[] { drugStoreTransfer }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DrugInnerTransferFromStore_Update([DataSourceRequest]DataSourceRequest request, DrugStoreTransfer drugStoreTransfer)
        {

            if (ModelState.IsValid)
            {


                var entity = new DrugStoreTransfer
                {
                    DrugStoreTransferId = drugStoreTransfer.DrugStoreTransferId,
                    FromDrugStoreId = drugStoreTransfer.FromDrugStoreId,
                    ToDrugStoreId = drugStoreTransfer.ToDrugStoreId,
                    BranchId = drugStoreTransfer.BranchId,

                };

                db.DrugStoreTransfers.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { drugStoreTransfer }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DrugInnerTransferFromStore_Destroy([DataSourceRequest]DataSourceRequest request, DrugStoreTransfer drugStoreTransfer)
        {
            if (ModelState.IsValid)
            {
                var entity = new DrugStoreTransfer
                {
                    DrugStoreTransferId = drugStoreTransfer.DrugStoreTransferId,
                    FromDrugStoreId = drugStoreTransfer.FromDrugStoreId,
                    ToDrugStoreId = drugStoreTransfer.ToDrugStoreId,
                    BranchId = drugStoreTransfer.BranchId,

                };

                db.DrugStoreTransfers.Attach(entity);
                db.DrugStoreTransfers.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { drugStoreTransfer }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult GetDrugList()
        {
            var drugList = db.DrugLists.ToList();
            return Json(drugList.Select(p => new
            {
                DrugListsId = p.DrugListsId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}