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
    public class DespensaryStoreController : Controller
    {
        // GET: APTS/DespensaryStore
        ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        public void PopulateViewData()
        {
            var drug = db.DrugLists.Where(x => x.IsActive);
            ViewData["druglists"] = drug;
            ViewData["Unit"] = db.DrugUnits.ToList();
        }
        public ActionResult DespensaryStore_Read([DataSourceRequest]DataSourceRequest request)
        {
            var DespensaryStore = db.DespensaryStores.ToList();
            DataSourceResult result = DespensaryStore.ToDataSourceResult(request, store => new
            {
                DespensaryStoreId = store.DespensaryStoreId,
                DrugListsId = store.DrugListsId,
                DrugUnitId = store.DrugUnitId,
                BegStock = store.BegStock,
                NewReceived = store.NewReceived,
                Balance = store.Balance,
                LastUpdateDate = store.LastUpdateDate,
                Remark = store.Remark
            });

            return Json(result);
        }
        public ActionResult DespensaryStore_Create([DataSourceRequest]DataSourceRequest request, DespensaryStore despensaryStore)
        {
            //var checkDrug = db.DrugStores.FirstOrDefault(x => x.DrugListsId == despensaryStore.DrugListsId);
            //if (checkDrug != null)
            //{
            //    ModelState.AddModelError("", "This Drug is already registered ");
            //}
            if (ModelState.IsValid)
            {
                var entity = new DespensaryStore
                {
                    DrugListsId = despensaryStore.DrugListsId,
                    BegStock = despensaryStore.BegStock,
                    NewReceived = despensaryStore.NewReceived,
                    Balance = despensaryStore.BegStock + despensaryStore.NewReceived,
                    DrugUnitId = despensaryStore.DrugUnitId,
                    LastUpdateDate = DateTime.Now,
                    Remark = despensaryStore.Remark,
                };
                db.DespensaryStores.Add(entity);
                db.SaveChanges();
                despensaryStore.DespensaryStoreId = entity.DespensaryStoreId;
            }

            return Json(new[] { despensaryStore }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DespensaryStore_Update([DataSourceRequest]DataSourceRequest request, DespensaryStore despensaryStore)
        {

            if (ModelState.IsValid)
            {
                var entity = new DespensaryStore
                {
                    DespensaryStoreId = despensaryStore.DespensaryStoreId,
                    DrugListsId = despensaryStore.DrugListsId,
                    DrugUnitId = despensaryStore.DrugUnitId,
                    BegStock = despensaryStore.BegStock,
                    NewReceived = despensaryStore.NewReceived,
                    Balance = despensaryStore.Balance,
                    LastUpdateDate = DateTime.Now,
                    Remark = despensaryStore.Remark,

                };

                db.DespensaryStores.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();


            }

            return Json(new[] { despensaryStore }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DespensaryStore_Destroy([DataSourceRequest]DataSourceRequest request, DespensaryStore despensaryStore)
        {
            if (ModelState.IsValid)
            {
                var entity = new DespensaryStore
                {
                    DespensaryStoreId = despensaryStore.DespensaryStoreId,
                    DrugListsId = despensaryStore.DrugListsId,
                    DrugUnitId = despensaryStore.DrugUnitId,
                    BegStock = despensaryStore.BegStock,
                    NewReceived = despensaryStore.NewReceived,
                    Balance = despensaryStore.Balance,
                    LastUpdateDate = despensaryStore.LastUpdateDate,
                    Remark = despensaryStore.Remark

                };

                db.DespensaryStores.Attach(entity);
                db.DespensaryStores.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { despensaryStore }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}