using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.APTS;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Inventory;

namespace ExceedERP.Web.Areas.APTS.Controllers
{
    public class DrugReceivalDetailController : Controller
    {
        // GET: APTS/DrugReceivalDetail
        public static DateTime expDate;
        public static DateTime receiveDate;

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
            ViewData["Store"] = db.DrugStores.ToList();
            ViewData["StoreLists"] = db.InventoryStores.ToList();
        }
        public ActionResult DrugReceivalDetail_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var drugReceivalDetails = db.DrugReceivalDetails.Where(x=>x.DrugReceivalModel19Id == id).ToList();
            DataSourceResult result = drugReceivalDetails.ToDataSourceResult(request, drugReceivalDetail => new
            {
                DrugReceivalDetailId = drugReceivalDetail.DrugReceivalDetailId,
                DrugReceivalModel19Id = drugReceivalDetail.DrugReceivalModel19Id,
                DrugListsId = drugReceivalDetail.DrugListsId,
                DrugUnitId = drugReceivalDetail.DrugUnitId,
                QtyReceived = drugReceivalDetail.QtyReceived,
                UnitCost = drugReceivalDetail.UnitCost,
                TotalCost = drugReceivalDetail.TotalCost,
                BatchNo = drugReceivalDetail.BatchNo,
                ExpiryDate = drugReceivalDetail.ExpiryDate,
                DateCreated = drugReceivalDetail.DateCreated,
                ExpiryDateAmh = drugReceivalDetail.ExpiryDateAmh,
                Balance = drugReceivalDetail.Balance
            });

            return Json(result);
        }
        public ActionResult DrugReceivalDetail_Create([DataSourceRequest]DataSourceRequest request, DrugReceivalDetail drugReceivalDetail,int id)
        {

            ModelState.Clear();

            if (drugReceivalDetail.ExpiryDateAmh != null)
            {
                var splitDateExp = drugReceivalDetail.ExpiryDateAmh.Split('/');
                expDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateExp[0]), Convert.ToInt32(splitDateExp[1]), Convert.ToInt32(splitDateExp[2]));

            }
           


            if (ModelState.IsValid)
            {
                var entity = new DrugReceivalDetail
                {
                    
                    DrugReceivalModel19Id = id,
                    DrugListsId = drugReceivalDetail.DrugListsId,
                    DrugUnitId = drugReceivalDetail.DrugUnitId,
                    QtyReceived = drugReceivalDetail.QtyReceived,
                    UnitCost = drugReceivalDetail.UnitCost,
                    TotalCost = drugReceivalDetail.UnitCost * drugReceivalDetail.QtyReceived,
                    BatchNo = drugReceivalDetail.BatchNo,
                    ExpiryDate = expDate,
                    DateCreated = DateTime.Now,
                    ExpiryDateAmh = drugReceivalDetail.ExpiryDateAmh,
                    Balance = drugReceivalDetail.QtyReceived
                };

                db.DrugReceivalDetails.Add(entity);
                db.SaveChanges();
                drugReceivalDetail.DrugReceivalModel19Id = entity.DrugReceivalModel19Id;

                #region Update Balance
                //var drugList = db.DrugLists.FirstOrDefault(x => x.DrugListsId == drugReceivalDetail.DrugListsId);
                //if (drugList != null)
                //{
                //    var drugReceival = db.DrugReceivalModel19s.FirstOrDefault(x => x.DrugReceivalModel19Id == id);
                //    if (drugReceival != null)
                //    {

                //        var drugSt = db.DrugStores.FirstOrDefault(x => x.DrugStoreId == drugReceival.DrugStoreId);
                //        if (drugSt != null)
                //        {
                //            var receival = db.DrugStoreItems.FirstOrDefault(x => x.DrugListsId == drugReceivalDetail.DrugListsId && x.DrugUnitId == drugReceivalDetail.DrugUnitId && x.DrugStoreId == drugSt.DrugStoreId);
                //            if (receival != null)
                //            {

                //                receival.Balance = receival.Balance + drugReceivalDetail.QtyReceived;

                //                db.Entry(receival).State = EntityState.Modified;
                //                db.SaveChanges();

                //                db.DrugReceivalDetails.Add(entity);
                //                db.SaveChanges();
                //                drugReceivalDetail.DrugReceivalModel19Id = entity.DrugReceivalModel19Id;

                //            }
                //            else
                //            {
                //                var drugStore = new DrugStoreItem
                //                {
                //                    DrugListsId = drugReceivalDetail.DrugListsId,
                //                    ItemCode = drugList.ItemCode,
                //                    DrugStoreId = drugSt.DrugStoreId,
                //                    Balance = drugReceivalDetail.QtyReceived,
                //                    DrugUnitId = drugReceivalDetail.DrugUnitId,
                //                };
                //                db.DrugStoreItems.Add(drugStore);
                //                db.SaveChanges();

                //                db.DrugReceivalDetails.Add(entity);
                //                db.SaveChanges();
                //                drugReceivalDetail.DrugReceivalModel19Id = entity.DrugReceivalModel19Id;
                //            }

                //        }

                //        else
                //        {
                //            ModelState.AddModelError("", "No such Drug Store");
                //        }
                //    }

                //}
                #endregion




            }

            return Json(new[] { drugReceivalDetail }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DrugReceivalDetail_Update([DataSourceRequest]DataSourceRequest request, DrugReceivalDetail drugReceivalDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new DrugReceivalDetail
                {

                    DrugReceivalDetailId = drugReceivalDetail.DrugReceivalDetailId,
                    DrugReceivalModel19Id = drugReceivalDetail.DrugReceivalModel19Id,
                    DrugListsId = drugReceivalDetail.DrugListsId,
                    DrugUnitId = drugReceivalDetail.DrugUnitId,
                    QtyReceived = drugReceivalDetail.QtyReceived,
                    UnitCost = drugReceivalDetail.UnitCost,
                    TotalCost = drugReceivalDetail.TotalCost,
                    BatchNo = drugReceivalDetail.BatchNo,
                    ExpiryDate = drugReceivalDetail.ExpiryDate,
                    DateCreated = drugReceivalDetail.DateCreated,
                    ExpiryDateAmh = drugReceivalDetail.ExpiryDateAmh,
                    Balance = drugReceivalDetail.Balance
                };

                db.DrugReceivalDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();


            }

            return Json(new[] { drugReceivalDetail }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DrugReceivalDetail_Destroy([DataSourceRequest]DataSourceRequest request, DrugReceivalDetail drugReceivalDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new DrugReceivalDetail
                {

                    DrugReceivalDetailId = drugReceivalDetail.DrugReceivalDetailId,
                    DrugReceivalModel19Id = drugReceivalDetail.DrugReceivalModel19Id,
                    DrugListsId = drugReceivalDetail.DrugListsId,
                    DrugUnitId = drugReceivalDetail.DrugUnitId,
                    QtyReceived = drugReceivalDetail.QtyReceived,
                    UnitCost = drugReceivalDetail.UnitCost,
                    TotalCost = drugReceivalDetail.TotalCost,
                    BatchNo = drugReceivalDetail.BatchNo,
                    ExpiryDate = drugReceivalDetail.ExpiryDate,
                    DateCreated = drugReceivalDetail.DateCreated,
                    ExpiryDateAmh = drugReceivalDetail.ExpiryDateAmh,
                    Balance = drugReceivalDetail.Balance
                };


                db.DrugReceivalDetails.Attach(entity);
                db.DrugReceivalDetails.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { drugReceivalDetail }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}