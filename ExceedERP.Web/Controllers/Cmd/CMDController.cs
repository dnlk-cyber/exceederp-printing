using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Hubs;
using ExceedERP.Web.Models;

namespace ExceedERP.Web.Controllers.Cmd
{
    public partial class CmdController : Controller
    {
        private ApplicationDbContext userDb = new ApplicationDbContext();
        private ExceedDbContext db = new ExceedDbContext();

        private readonly DistributionHelper _disHelper = new DistributionHelper();
        FinanceHelper _financeHelper = new FinanceHelper();
        AccountHelper _accountHelper = new AccountHelper();
        FiscalYearHelper fiscalYearHelper = new FiscalYearHelper();
        // GET: CMD
        public string Index()
        {
            //string cat = SecurityHelper.Encrypt("S", KEY);
            //string reft = SecurityHelper.Encrypt("SP", KEY);
            //string att = SecurityHelper.Encrypt("SPA", KEY);
            //string currentValue = SecurityHelper.Encrypt("exceed$ERP@is#Great^", KEY);
            return "<h1>Wrong Turn!</h1>";// + cat+ "</br>"+ reft + "</br>" + att + "</br>" + currentValue;
        }

        public ActionResult GenerateIssueDist(int id,int start)
        {
            _disHelper.GenerateIssueDistribution(id,start);
            return View("Success");
        }
        public ActionResult GenerateGrnDist(int id, int start)
        {
            _disHelper.GenerateGoodReceiveDistribution(id, start);
            return View("Success");
        }
        public ActionResult GenerateStoreReturnDist(int id, int start)
        {
            _disHelper.GenerateStoreReturnDistribution(id, start);
            return View("Success");
        }
        public ActionResult GenerateTransferSendDist(int id, int start)
        {
            _disHelper.GenerateTransferSendDistribution(id, start);
            return View("Success");
        }
        public ActionResult GenerateTransferReceiveDist(int id, int start)
        {
            _disHelper.GenerateTransferReceiveDistribution(id, start); 
            return View("Success");
        }

        public ActionResult GenPr()
        {
            var transferre = db.StoreRequisitions.Where(x => x.IsSecondOnlineApproved)
                .Where(x => x.IsVoid == false).Where(x => x.IsTransfer).ToList();
            foreach (var t in transferre)
            {
                TransferStoreRequisitionItemsToPurchaseRequisition(t.StoreRequisitionID);
            }
            return View("Success");
        }
        private void TransferStoreRequisitionItemsToPurchaseRequisition(int srid)
        {
            bool transferrable = false;
            var storerequisition = db.StoreRequisitions.Find(srid);
            var exist = db.PurchaseRequisitions.Where(x => x.StoreRequisitionID == srid)
                .AsNoTracking().ToList();
            if (!exist.Any())
            {
                //&& requestedItem.GeneratePurchaseReq
                #region check if there is anything to purchase

                var items = db.StoreRequisitionItems.Where(x =>
                    x.StoreRequisitionID == srid).ToList();
                foreach (var requestedItem in items)
                {
                    if (requestedItem.PurchaseQuantity > 0)
                    {
                        transferrable = true;
                        break;
                    }
                }

                #endregion

                if (transferrable)
                {
                    #region purchase

                    #region create purchase requisition

                    var purchaseRequisition = new PurchaseRequisition
                    {
                        StoreRequisitionID = srid,
                        BranchId = storerequisition.BranchId,
                        OrgStructureId = storerequisition.OrgStructureId,
                        EmployeeId = storerequisition.EmployeeId,
                        ReferenceNo = storerequisition.Reference,
                        ProcurementOfficer = 0,
                        RequestDate = storerequisition.RequestDate,
                        ItemType = PurchaseItemType.Stock,
                        Status = "Editing",
                        Remark = storerequisition.Remark,

                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        LastModified = storerequisition.LastModified,
                        ModifiedBy = storerequisition.ModifiedBy,
                    };

                    db.PurchaseRequisitions.Add(purchaseRequisition);
                    db.SaveChanges();

                    #endregion

                    foreach (var requestedItem in storerequisition.StoreRequisitionItem)
                    {

                        if (requestedItem.GeneratePurchaseReq && requestedItem.PurchaseQuantity > 0)
                        {
                            #region stock

                            var purchaseRequisitionItem = new PurchaseRequisitionItem
                            {
                                PurchaseRequisitionID = purchaseRequisition.PurchaseRequisitionID,
                                CatCode = requestedItem.ItemCategoryCode,
                                CategoryName = requestedItem.ItemCategory.Name,
                                ItmCode = requestedItem.ItemCode,
                                ItemName = requestedItem.InventoryItem.Name,
                                PartNo = requestedItem.PartNo,
                                UoM = requestedItem.InventoryItem.UoMName,
                                RequestedQuantity = requestedItem.PurchaseQuantity,
                                Quantity = requestedItem.PurchaseQuantity,
                                ItemSpecification = requestedItem.ItemSpecification,
                                ItemType = PurchaseItemType.Stock,
                                CreatedBy = User.Identity.Name,
                                DateCreated = DateTime.Now
                            };
                            db.PurchaseRequisitionItems.Add(purchaseRequisitionItem);
                            db.SaveChanges();

                            #endregion
                        }

                    }

                    #endregion
                }
            }

        }


    }
}