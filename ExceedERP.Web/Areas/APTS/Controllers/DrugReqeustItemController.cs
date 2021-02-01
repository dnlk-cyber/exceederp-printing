using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.APTS;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.HRM.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.APTS.Controllers
{
    public class DrugReqeustItemController : Controller
    {
        // GET: APTS/DrugReqeustItem
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        public void PopulateViewData()
        {
            ViewData["UnitsList"] = db.DrugUnits.ToList();
            ViewData["DrugLists"] = db.DrugLists.ToList();
        
        }
        public bool GetApprovalStatus(int id)
        {
            var drugRequest = db.DrugRequests.FirstOrDefault(x=>x.DrugRequestId == id);
            if(drugRequest != null && drugRequest.IsOnlineApproved)
            {
                return true;
            }
            return false;
        }
        public ActionResult DrugReqeustItem_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            var requests = db.DrugRequestItems.Where(x=>x.DrugRequestId == id).ToList();
            DataSourceResult result = requests.ToDataSourceResult(request, drugRequestItem => new
            {
                DrugListsId = drugRequestItem.DrugListsId,
                DrugRequestId = drugRequestItem.DrugRequestId,
                DrugRequestItemId = drugRequestItem.DrugRequestItemId,
                DrugUnitId = drugRequestItem.DrugUnitId,
                BegStock = drugRequestItem.BegStock,
                Loss = drugRequestItem.Loss,
                EndStock = drugRequestItem.EndStock,
                QtyRequested = drugRequestItem.QtyRequested,
                QtyApproved = drugRequestItem.QtyApproved,
                QtyAllowed = drugRequestItem.QtyAllowed,
                IsOnlineApproved = GetApprovalStatus(drugRequestItem.DrugRequestId),
                ReceivedQty = drugRequestItem.ReceivedQty

            });

            return Json(result);
        }
        public ActionResult DrugReqeustItem_Create([DataSourceRequest]DataSourceRequest request, DrugRequestItem drugRequestItem,int idTwo)
        {
            decimal begStock = 0;
            decimal receivedQty = 0;
            decimal soldItem = 0;
            //decimal endStock = 0;
            if (ModelState.IsValid)
            {
                var checkNum = db.DespensaryStores.FirstOrDefault(x=>x.DrugListsId == drugRequestItem.DrugListsId && x.DrugUnitId == drugRequestItem.DrugUnitId);
                if(checkNum != null)
                {
                    begStock = checkNum.NewReceived + checkNum.BegStock;
                    receivedQty = checkNum.NewReceived;
                    //endStock = checkNum.Balance;

                     var patientRegisters = db.PatientRegisters.Where(x=>x.RegisterDate >= checkNum.LastUpdateDate.Date).ToList();
                    foreach(var patientRegister in patientRegisters)
                    {
                      var num = db.PatientRegisterDetails.Where(x => x.PatientRegisterId == patientRegister.PatientRegisterId && x.DrugListsId == drugRequestItem.DrugListsId
                                                                && x.DrugUnitId == drugRequestItem.DrugUnitId).ToList().Sum(x => x.TotalQty);
                        soldItem = soldItem + num;
                    }
                }
                var entity = new DrugRequestItem
                {
                    DrugListsId = drugRequestItem.DrugListsId,
                    DrugRequestId = idTwo,
                    DrugRequestItemId = drugRequestItem.DrugRequestItemId,
                    DrugUnitId = drugRequestItem.DrugUnitId,
                    BegStock = begStock,
                    ReceivedQty = receivedQty,
                    Loss = drugRequestItem.Loss,
                    EndStock = (begStock) - (drugRequestItem.Loss + soldItem),
                    QtyRequested = drugRequestItem.QtyRequested,
                    QtyAllowed = drugRequestItem.QtyRequested,
                    QtyApproved = drugRequestItem.QtyRequested,
                    


                };
                db.DrugRequestItems.Add(entity);
                db.SaveChanges();
                drugRequestItem.DrugRequestItemId = entity.DrugRequestItemId;



            }

            return Json(new[] { drugRequestItem }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DrugReqeustItem_Update([DataSourceRequest]DataSourceRequest request, DrugRequestItem drugRequestItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new DrugRequestItem
                {
                    DrugListsId = drugRequestItem.DrugListsId,
                    DrugRequestId = drugRequestItem.DrugRequestId,
                    DrugRequestItemId = drugRequestItem.DrugRequestItemId,
                    DrugUnitId = drugRequestItem.DrugUnitId,
                    BegStock = drugRequestItem.BegStock,
                    Loss = drugRequestItem.Loss,
                    EndStock = drugRequestItem.EndStock,
                    QtyRequested = drugRequestItem.QtyRequested,
                    QtyApproved = drugRequestItem.QtyApproved,
                    QtyAllowed = drugRequestItem.QtyAllowed,
                    ReceivedQty = drugRequestItem.ReceivedQty

                };

                db.DrugRequestItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { drugRequestItem }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DrugReqeustItem_Destroy([DataSourceRequest]DataSourceRequest request, DrugRequestItem drugRequestItem)
        {
            if (ModelState.IsValid)
            {

                var entity = new DrugRequestItem
                {
                    DrugListsId = drugRequestItem.DrugListsId,
                    DrugRequestId = drugRequestItem.DrugRequestId,
                    DrugRequestItemId = drugRequestItem.DrugRequestItemId,
                    DrugUnitId = drugRequestItem.DrugUnitId,
                    BegStock = drugRequestItem.BegStock,
                    Loss = drugRequestItem.Loss,
                    EndStock = drugRequestItem.EndStock,
                    QtyRequested = drugRequestItem.QtyRequested,
                    QtyApproved = drugRequestItem.QtyApproved,
                    QtyAllowed = drugRequestItem.QtyAllowed,
                    ReceivedQty = drugRequestItem.ReceivedQty

                };
                db.DrugRequestItems.Attach(entity);
                db.DrugRequestItems.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { drugRequestItem }.ToDataSourceResult(request, ModelState));
        }

        public string GetBalance(int drugList)
        {
            var stockBalance = "";
            var stock = db.DespensaryStores.FirstOrDefault(x => x.DrugListsId == drugList);
            var begBalance = stock.BegStock;
            var receivedQuantity = stock.NewReceived;

            stockBalance = begBalance + "-" + receivedQuantity;
            //  var endStock = begBalance + receivedQuantity + stock
            return stockBalance;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}