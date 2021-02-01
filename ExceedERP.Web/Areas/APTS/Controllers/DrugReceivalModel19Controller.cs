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
    public class DrugReceivalModel19Controller : Controller
    {
        // GET: APTS/DrugReceivalModel19
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
        public ActionResult DrugReceivalModel19_Read([DataSourceRequest]DataSourceRequest request)
        {
            var medisince = db.DrugReceivalModel19s.ToList();
            DataSourceResult result = medisince.ToDataSourceResult(request, drugReceival => new
            {
                DrugReceivalModel19Id = drugReceival.DrugReceivalModel19Id,
                DrugStoreId = drugReceival.DrugStoreId,
                DateCreated = drugReceival.DateCreated,
                ReceivingDate = drugReceival.ReceivingDate,
                ReceivingDateAmh = drugReceival.ReceivingDateAmh,
                ReceiptNo = drugReceival.ReceiptNo,
                ReceiverName = drugReceival.ReceiverName,
                Remark = drugReceival.Remark,
                IsCurrent = drugReceival.IsCurrent,
                IsOnlineApproved = drugReceival.IsOnlineApproved,
                IsOnlineTransferred = drugReceival.IsOnlineTransferred,
                IsOnlineTransferredBy = drugReceival.IsOnlineTransferredBy,
                IsVoid = drugReceival.IsVoid,
                LastModified = drugReceival.LastModified,
                ModifiedBy = drugReceival.ModifiedBy,
                Name = drugReceival.Name,
                OnlineApprovedBy = drugReceival.OnlineApprovedBy,
                OnlineApprovedTime = drugReceival.OnlineApprovedTime,
                OnlineTransferredTime = drugReceival.OnlineTransferredTime,
                Status = drugReceival.Status,
                StoreCode = drugReceival.StoreCode,
                VoidBy = drugReceival.VoidBy,
                VoidTime = drugReceival.VoidTime,
                CreatedBy = drugReceival.CreatedBy,
                SendForApprovalBy = drugReceival.SendForApprovalBy,
                SendForApprovalTime = drugReceival.SendForApprovalTime,
                IsSendForApproval = drugReceival.IsSendForApproval
            });

            return Json(result);
        }
        public ActionResult DrugReceivalModel19_Create([DataSourceRequest]DataSourceRequest request, DrugReceivalModel19 drugReceival)
        {

            ModelState.Clear();

            
            if (drugReceival.ReceivingDateAmh != null)
            {
                var splitDateRec = drugReceival.ReceivingDateAmh.Split('/');
                receiveDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateRec[0]), Convert.ToInt32(splitDateRec[1]), Convert.ToInt32(splitDateRec[2]));

            }
            var storeCode = "";
            var store = db.DrugStores.FirstOrDefault(x=>x.DrugStoreId == drugReceival.DrugStoreId);
            if(store != null)
            {
                storeCode = store.StoreCode;
            }

            if (ModelState.IsValid)
            {
                var entity = new DrugReceivalModel19
                {
                   
                    ReceivingDate =  receiveDate,
                    ReceivingDateAmh = drugReceival.ReceivingDateAmh,
                    DrugStoreId = drugReceival.DrugStoreId,
                    DateCreated = DateTime.Now,
                    ReceiptNo = drugReceival.ReceiptNo,
                    ReceiverName = User.Identity.Name,
                    Remark = drugReceival.Remark,
                    DrugReceivalModel19Id = drugReceival.DrugReceivalModel19Id,
                    IsCurrent = drugReceival.IsCurrent,
                    IsOnlineApproved = drugReceival.IsOnlineApproved,
                    IsOnlineTransferred = drugReceival.IsOnlineTransferred,
                    IsOnlineTransferredBy = drugReceival.IsOnlineTransferredBy,
                    IsVoid = drugReceival.IsVoid,
                    LastModified = drugReceival.LastModified,
                    ModifiedBy = drugReceival.ModifiedBy,
                    Name = drugReceival.Name,
                    OnlineApprovedBy = drugReceival.OnlineApprovedBy,
                    OnlineApprovedTime = drugReceival.OnlineApprovedTime,
                    OnlineTransferredTime = drugReceival.OnlineTransferredTime,
                    Status = drugReceival.Status,
                    StoreCode = storeCode,
                    VoidBy = drugReceival.VoidBy,
                    VoidTime = drugReceival.VoidTime,
                    CreatedBy = drugReceival.CreatedBy,
                    SendForApprovalBy = drugReceival.SendForApprovalBy,
                    SendForApprovalTime = DateTime.Now,
                    IsSendForApproval = drugReceival.IsSendForApproval
                };

                db.DrugReceivalModel19s.Add(entity);
                db.SaveChanges();
                drugReceival.DrugReceivalModel19Id = entity.DrugReceivalModel19Id;

               

               
            }

            return Json(new[] { drugReceival }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DrugReceivalModel19_Update([DataSourceRequest]DataSourceRequest request, DrugReceivalModel19 drugReceival)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (drugReceival.ReceivingDateAmh != null)
                {
                    var splitDateRec = drugReceival.ReceivingDateAmh.Split('/');
                    receiveDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateRec[0]), Convert.ToInt32(splitDateRec[1]), Convert.ToInt32(splitDateRec[2]));

                }
                var storeCode = "";
                var store = db.DrugStores.FirstOrDefault(x => x.DrugStoreId == drugReceival.DrugStoreId);
                if (store != null)
                {
                    storeCode = store.StoreCode;
                }
                var entity = new DrugReceivalModel19
                {
                    ReceivingDate = receiveDate,
                    ReceivingDateAmh = drugReceival.ReceivingDateAmh,
                    DrugStoreId = drugReceival.DrugStoreId,
                    DateCreated = DateTime.Now,
                    ReceiptNo = drugReceival.ReceiptNo,
                    ReceiverName = User.Identity.Name,
                    Remark = drugReceival.Remark,
                    DrugReceivalModel19Id = drugReceival.DrugReceivalModel19Id,
                    IsCurrent = drugReceival.IsCurrent,
                    IsOnlineApproved = drugReceival.IsOnlineApproved,
                    IsOnlineTransferred = drugReceival.IsOnlineTransferred,
                    IsOnlineTransferredBy = drugReceival.IsOnlineTransferredBy,
                    IsVoid = drugReceival.IsVoid,
                    LastModified = drugReceival.LastModified,
                    ModifiedBy = drugReceival.ModifiedBy,
                    Name = drugReceival.Name,
                    OnlineApprovedBy = drugReceival.OnlineApprovedBy,
                    OnlineApprovedTime = drugReceival.OnlineApprovedTime,
                    OnlineTransferredTime = drugReceival.OnlineTransferredTime,
                    Status = drugReceival.Status,
                    StoreCode = storeCode,
                    VoidBy = drugReceival.VoidBy,
                    VoidTime = drugReceival.VoidTime,
                    CreatedBy = drugReceival.CreatedBy,
                    SendForApprovalBy = drugReceival.SendForApprovalBy,
                    SendForApprovalTime = DateTime.Now,
                    IsSendForApproval = drugReceival.IsSendForApproval
                };

                db.DrugReceivalModel19s.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();


            }

            return Json(new[] { drugReceival }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DrugReceivalModel19_Destroy([DataSourceRequest]DataSourceRequest request, DrugReceivalModel19 drugReceival)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new DrugReceivalModel19
                {
                    ReceivingDate = receiveDate,
                    ReceivingDateAmh = drugReceival.ReceivingDateAmh,
                    DrugStoreId = drugReceival.DrugStoreId,
                    DateCreated = drugReceival.DateCreated,
                    ReceiptNo = drugReceival.ReceiptNo,
                    ReceiverName = User.Identity.Name,
                    Remark = drugReceival.Remark,
                    DrugReceivalModel19Id = drugReceival.DrugReceivalModel19Id,
                    IsCurrent = drugReceival.IsCurrent,
                    IsOnlineApproved = drugReceival.IsOnlineApproved,
                    IsOnlineTransferred = drugReceival.IsOnlineTransferred,
                    IsOnlineTransferredBy = drugReceival.IsOnlineTransferredBy,
                    IsVoid = drugReceival.IsVoid,
                    LastModified = drugReceival.LastModified,
                    ModifiedBy = drugReceival.ModifiedBy,
                    Name = drugReceival.Name,
                    OnlineApprovedBy = drugReceival.OnlineApprovedBy,
                    OnlineApprovedTime = drugReceival.OnlineApprovedTime,
                    OnlineTransferredTime = drugReceival.OnlineTransferredTime,
                    Status = drugReceival.Status,
                    StoreCode = drugReceival.StoreCode,
                    VoidBy = drugReceival.VoidBy,
                    VoidTime = drugReceival.VoidTime,
                    CreatedBy = drugReceival.CreatedBy,
                    SendForApprovalBy = drugReceival.SendForApprovalBy,
                    SendForApprovalTime = drugReceival.SendForApprovalTime,
                    IsSendForApproval = drugReceival.IsSendForApproval
                };

                db.DrugReceivalModel19s.Attach(entity);
                db.DrugReceivalModel19s.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { drugReceival }.ToDataSourceResult(request, ModelState));
        }

        public string CheckStatus(int masterId)
        {
            var approve = db.APTSValidations.Where(x => x.DrugReceivalModel19Id == masterId && x.Status == Core.Domain.Common.ApprovalStatuses.Approve).FirstOrDefault();
            if (approve != null)
            {
                return "approved";
            }
            var reject = db.APTSValidations.Where(x => x.DrugReceivalModel19Id == masterId && x.Status == Core.Domain.Common.ApprovalStatuses.Reject).FirstOrDefault();

            if (reject != null)
            {
                return "Dapproved";               
            }
            return "false";
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}