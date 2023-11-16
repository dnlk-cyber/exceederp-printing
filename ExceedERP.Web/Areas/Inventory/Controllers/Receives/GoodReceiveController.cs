using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender.Lot;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Areas.Inventory.Models;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using EntityState = System.Data.Entity.EntityState;


namespace ExceedERP.Web.Areas.Inventory.Controllers.Receives
{



    public class GoodReceiveController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        private readonly DistributionHelper _distributionHelper = new DistributionHelper();
        private InventoryTransferHelper _transferHelper;

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover, InventoryConstants.PropertyInspector)]
        public ActionResult Index()
        {

            PopulateViewData();
            return View();

        }

        private void PopulateViewData()
        {
            var categories = _db.ItemCategorys;
            var items = _db.InventoryItems;
            var branches = _db.Branch;
            var tax = _db.GLTaxRates;
            var stores = _db.InventoryStores;
            var receivetype = _db.ReceiveTypes;

            var inventoryPeriods = _db.InventoryPeriods;
            var selectedinventoryPeriods = inventoryPeriods.Select(
             i => new
             {
                 Name = i.Name,
                 GLPeriodId = i.InventoryPeriodId
             }).AsQueryable();
            ViewData["inventoryPeriods"] = selectedinventoryPeriods;



            ViewData["stores"] = stores;
            ViewData["tax"] = tax;
            ViewData["ReeceiveType"] = receivetype;
            ViewData["Branches"] = branches;
            var selecteditems = items.Select(
                i => new
                {
                    Name = i.ItemCode+"_"+i.Name,
                    ItemCode = i.ItemCode
                }).ToList();
             
            ViewData["Items"] = selecteditems;
            ViewData["Category"] = categories;


            var employee = _db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;

        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover, InventoryConstants.PropertyInspector)]
        public ActionResult GoodReceives_Read([DataSourceRequest]DataSourceRequest request)
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            //IQueryable<GoodReceive> goodreceives = db.GoodReceives.Where(x => mybraches.Contains(x.OrgBranchName));

            List<string> userStores = UserInformationHelper.GetUserStores(User.Identity.Name);
            IQueryable<GoodReceive> goodreceives = _db.GoodReceives.Where(x => x.IsManual==false).Where(x => userStores.Contains(x.StoreCode));

            DataSourceResult result = goodreceives.ToDataSourceResult(request, goodReceive => new
            {
                ReceiveRef = goodReceive.ReceiveRef,
                IsManual = goodReceive.IsManual,
                BranchId = goodReceive.BranchId,
                BusinessUnitId = goodReceive.BusinessUnitId,
                ReceiveTypeId = goodReceive.ReceiveTypeId,
                GoodReceiveId = goodReceive.GoodReceiveId,
                TenderLot = goodReceive.TenderLot,
                StoreCode = goodReceive.StoreCode,
                SupplierName = goodReceive.SupplierName,
                SupplierInvoiceNo = goodReceive.SupplierInvoiceNo,
                Reference = goodReceive.Reference,
                IsFreshBazaar = goodReceive.IsFreshBazaar,
                ReceiveDate = goodReceive.ReceiveDate,
                DateCreated = goodReceive.DateCreated,
                CreatedBy = goodReceive.CreatedBy,
                Status = goodReceive.Status,
                Remark = goodReceive.Remark,
                GLPeriodId = goodReceive.GLPeriodId,
                IsVoid = goodReceive.IsVoid,
                VoidBy = goodReceive.VoidBy,
                VoidTime = goodReceive.VoidTime,

                IsOnlineApproved = goodReceive.IsOnlineApproved,
                OnlineApprovedBy = goodReceive.OnlineApprovedBy,
                OnlineApprovedTime = goodReceive.OnlineApprovedTime,

                IsSendForApproval = goodReceive.IsSendForApproval,
                SendForApprovalBy = goodReceive.SendForApprovalBy,
                SendForApprovalTime = goodReceive.SendForApprovalTime,

                IsOnlineTransferred = goodReceive.IsOnlineTransferred,
                IsOnlineTransferredBy = goodReceive.IsOnlineTransferredBy,
                OnlineTransferredTime = goodReceive.OnlineTransferredTime,
            });

            return Json(result);

        }

     

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GoodReceives_Create([DataSourceRequest]DataSourceRequest request, GoodReceive goodReceive)
        {
            #region validation

            ReceiveType type = _db.ReceiveTypes.Find(goodReceive.ReceiveTypeId);
            if (type != null)
            {
                switch (type.Type)
                {
                    case GoodReceiveType.PurchaseOrder:
                        int pid = int.Parse(goodReceive.ReceiveRef);
                        PurchaseOrder purchaseOrder = _db.PurchaseOrders.Find(pid);
                        if (purchaseOrder != null)
                        {
                            goodReceive.SupplierId = purchaseOrder.SupplierId;
                            goodReceive.SupplierName = purchaseOrder.SupplierName;

                            if(String.IsNullOrWhiteSpace(purchaseOrder.SupplierName))
                                ModelState.AddModelError("supplierid","Supplier is not found in selected purchase order");
                        }
                        break;
                    case GoodReceiveType.Tender:
                        int awardid = int.Parse(goodReceive.ReceiveRef);
                        BidAwardedSupplier award = _db.BidAwardedSuppliers.Find(awardid);
                        if (award != null)
                        {
                            goodReceive.SupplierId = award.SupplierId;
                            goodReceive.SupplierName = award.SupplierName;

                            if (String.IsNullOrWhiteSpace(award.SupplierName))
                                ModelState.AddModelError("supplierid", "Supplier is not found in selected award");
                        }
                        break;
                    default:
                        ModelState.AddModelError("auto", "you can not register manual receive with this form, please use auto receive form");
                        break;
                }
            }
            else
            {
                ModelState.AddModelError("type", "Receive Type is not found.");
            }
            //to allow multiple receive

            //if (!String.IsNullOrEmpty(goodReceive.ReceiveRef))
            //{
            //    var exists = db.GoodReceives.Where(x => x.ReceiveRef == goodReceive.ReceiveRef && x.IsVoid == false).ToList();

            //    if (exists.Any())
            //    {
            //        ModelState.AddModelError("Reference", goodReceive.ReceiveRef + "  already exists in the system.");
            //    }
            //}

            if (!String.IsNullOrEmpty(goodReceive.Reference))
            {
                var exists = _db.GoodReceives.Where(x => x.Reference == goodReceive.Reference && x.IsVoid == false).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("Reference", $"{goodReceive.Reference}  already exists in the system.");
                }
            }

            if (goodReceive.GLPeriodId != 0)
            {
                var inventoryPeriod = _db.InventoryPeriods.Find(goodReceive.GLPeriodId);
                if (inventoryPeriod != null)
                {
                    if (
                        !((goodReceive.ReceiveDate.Date >= inventoryPeriod.DateFrom) &&
                          (goodReceive.ReceiveDate.Date <= inventoryPeriod.DateTo)))
                        ModelState.AddModelError("",
                            $"Receive Date should be within the period: " + inventoryPeriod.Name + "(From " +
                            inventoryPeriod.DateFrom.ToShortDateString() + " To " +
                            inventoryPeriod.DateTo.ToShortDateString() + ")");

                }
                else
                {
                    ModelState.AddModelError("", "The selected Period is not defined.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Please select period.");
            }

            if (goodReceive.ReceiveDate.Date > DateTime.Now)
            {
                ModelState.AddModelError("future", "You are recording in future period,please correct the date.");
            }

            #endregion

            if (ModelState.IsValid)
            {


                var entity = new GoodReceive
                    {
                        BranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(goodReceive.StoreCode),
                    BusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name),
                    IsManual = false,
                        ReceiveRef = goodReceive.ReceiveRef,
                        ReceiveTypeId = goodReceive.ReceiveTypeId,
                        StoreCode = goodReceive.StoreCode,
                        SupplierName = goodReceive.SupplierName,
                        SupplierId = goodReceive.SupplierId,
                        SupplierInvoiceNo = goodReceive.SupplierInvoiceNo,
                        TenderLot = goodReceive.TenderLot,
                        Reference = goodReceive.Reference,
                        IsFreshBazaar = goodReceive.IsFreshBazaar,
                        ReceiveDate = goodReceive.ReceiveDate.Date,
                        Status = Enum.GetName(typeof(OperationStatuses), 5),
                        Remark = goodReceive.Remark,
                        GLPeriodId=goodReceive.GLPeriodId,
                        IsVoid = goodReceive.IsVoid,
                        VoidBy = goodReceive.VoidBy,
                        VoidTime = goodReceive.VoidTime,
                        IsOnlineApproved = goodReceive.IsOnlineApproved,
                        OnlineApprovedBy = goodReceive.OnlineApprovedBy,
                        OnlineApprovedTime = goodReceive.OnlineApprovedTime,
                        IsOnlineTransferred = goodReceive.IsOnlineTransferred,
                        IsOnlineTransferredBy = goodReceive.IsOnlineTransferredBy,
                        OnlineTransferredTime = goodReceive.OnlineTransferredTime,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        ModifiedBy = goodReceive.ModifiedBy,
                        LastModified = goodReceive.LastModified,
                        OrgBranchName = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(goodReceive.StoreCode).ToString()
                    };

                _db.GoodReceives.Add(entity);
                _db.SaveChanges();
                goodReceive.GoodReceiveId = entity.GoodReceiveId;

                GetGoodReceiveItems(entity);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "good receive", "Success", User.Identity.Name, $"created good receive {entity.GoodReceiveId} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { goodReceive }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GoodReceives_Update([DataSourceRequest]DataSourceRequest request, GoodReceive goodReceive)
        {
            #region validation

            ReceiveType type = _db.ReceiveTypes.Find(goodReceive.ReceiveTypeId);
            if (type != null)
            {
                switch (type.Type)
                {
                    case GoodReceiveType.PurchaseOrder:
                        int pid = int.Parse(goodReceive.ReceiveRef);
                        PurchaseOrder purchaseOrder = _db.PurchaseOrders.Find(pid);
                        if (purchaseOrder != null)
                        {
                            goodReceive.SupplierId = purchaseOrder.SupplierId;
                            goodReceive.SupplierName = purchaseOrder.SupplierName;

                            if (String.IsNullOrWhiteSpace(purchaseOrder.SupplierName))
                                ModelState.AddModelError("supplierid", "Supplier is not found in selected purchase order");
                        }
                        break;
                    case GoodReceiveType.Tender:
                        int awardid = int.Parse(goodReceive.ReceiveRef);
                        BidAwardedSupplier award = _db.BidAwardedSuppliers.Find(awardid);
                        if (award != null)
                        {
                            goodReceive.SupplierId = award.SupplierId;
                            goodReceive.SupplierName = award.SupplierName;

                            if (String.IsNullOrWhiteSpace(award.SupplierName))
                                ModelState.AddModelError("supplierid", "Supplier is not found in selected award");
                        }
                        break;
                    default:
                        ModelState.AddModelError("auto", "you can not register manual receive with this form, please use manual receive form");
                        break;
                }
            }
            else
            {
                ModelState.AddModelError("type", "Receive Type is not found.");
            }
            //to allow multiple receive
            //if (!String.IsNullOrEmpty(goodReceive.ReceiveRef))
            //{
            //    var exists = db.GoodReceives.Where(x => x.GoodReceiveId != goodReceive.GoodReceiveId).Where(x => x.ReceiveRef == goodReceive.ReceiveRef && x.IsVoid == false).ToList();

            //    if (exists.Any())
            //    {
            //        ModelState.AddModelError("Reference", goodReceive.ReceiveRef + "  already exists in the system.");
            //    }
            //}
            if (!String.IsNullOrEmpty(goodReceive.Reference))
            {
                var exists = _db.GoodReceives.Where(x => x.GoodReceiveId != goodReceive.GoodReceiveId).Where(x => x.Reference == goodReceive.Reference && x.IsVoid == false).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("Reference", goodReceive.Reference + "  already exists in the system.");
                }
            }

            if (goodReceive.GLPeriodId != 0)
            {
                var inventoryPeriod = _db.InventoryPeriods.Find(goodReceive.GLPeriodId);
                if (inventoryPeriod != null)
                {
                    if (
                        !((goodReceive.ReceiveDate.Date >= inventoryPeriod.DateFrom) &&
                          (goodReceive.ReceiveDate.Date <= inventoryPeriod.DateTo)))
                        ModelState.AddModelError("",
                            "Receive Date should be within the period: " + inventoryPeriod.Name + "(From " +
                            inventoryPeriod.DateFrom.ToShortDateString() + " To " +
                            inventoryPeriod.DateTo.ToShortDateString() + ")");

                }
                else
                {
                    ModelState.AddModelError("", "The selected Period is not defined.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Please select period.");
            }

            if (goodReceive.ReceiveDate > DateTime.Today)
            {
                ModelState.AddModelError("future", "You are recording in future period,please correct the date.");
            }

            #endregion

            if (ModelState.IsValid)
            {
                var entity = new GoodReceive
                {
                    BranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(goodReceive.StoreCode),
                    BusinessUnitId = goodReceive.BusinessUnitId,
                    IsManual = goodReceive.IsManual,
                    GoodReceiveId = goodReceive.GoodReceiveId,
                    ReceiveRef = goodReceive.ReceiveRef,
                    StoreCode = goodReceive.StoreCode,
                    SupplierName = goodReceive.SupplierName,
                    SupplierInvoiceNo = goodReceive.SupplierInvoiceNo,
                    Reference = goodReceive.Reference,
                    IsFreshBazaar = goodReceive.IsFreshBazaar,
                    ReceiveDate = goodReceive.ReceiveDate,
                    Status = goodReceive.Status,
                    ReceiveTypeId = goodReceive.ReceiveTypeId,
                    Remark = goodReceive.Remark,
                    GLPeriodId=goodReceive.GLPeriodId,
                    IsVoid = goodReceive.IsVoid,
                    VoidBy = goodReceive.VoidBy,
                    VoidTime = goodReceive.VoidTime,
                    IsOnlineApproved = goodReceive.IsOnlineApproved,
                    OnlineApprovedBy = goodReceive.OnlineApprovedBy,
                    OnlineApprovedTime = goodReceive.OnlineApprovedTime,
                    IsOnlineTransferred = goodReceive.IsOnlineTransferred,
                    IsOnlineTransferredBy = goodReceive.IsOnlineTransferredBy,
                    OnlineTransferredTime = goodReceive.OnlineTransferredTime,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                    OrgBranchName =ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(goodReceive.StoreCode).ToString(),
                };

                _db.GoodReceives.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                GetGoodReceiveItems(entity);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "good receive", "Success", User.Identity.Name, $"updated good receive {entity.GoodReceiveId} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { goodReceive }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GoodReceives_Destroy([DataSourceRequest]DataSourceRequest request, GoodReceive goodReceive)
        {

            ModelState.Remove("PurchaseOrderID");

            if (ModelState.IsValid)
            {
                var entity = new GoodReceive
                {
                    BranchId = goodReceive.BranchId,
                    BusinessUnitId = goodReceive.BusinessUnitId,
                    GoodReceiveId = goodReceive.GoodReceiveId,
                    StoreCode = goodReceive.StoreCode,
                    SupplierName = goodReceive.SupplierName,
                    SupplierInvoiceNo = goodReceive.SupplierInvoiceNo,
                    Reference = goodReceive.Reference,
                    ReceiveDate = goodReceive.ReceiveDate,
                    Status = goodReceive.Status,
                    Remark = goodReceive.Remark,
                    IsVoid = goodReceive.IsVoid,
                    ReceiveTypeId = goodReceive.ReceiveTypeId,
                    VoidBy = goodReceive.VoidBy,
                    VoidTime = goodReceive.VoidTime,
                    IsOnlineApproved = goodReceive.IsOnlineApproved,
                    OnlineApprovedBy = goodReceive.OnlineApprovedBy,
                    OnlineApprovedTime = goodReceive.OnlineApprovedTime,
                    IsOnlineTransferred = goodReceive.IsOnlineTransferred,
                    IsOnlineTransferredBy = goodReceive.IsOnlineTransferredBy,
                    OnlineTransferredTime = goodReceive.OnlineTransferredTime,
                };

                _db.GoodReceives.Attach(entity);
                _db.GoodReceives.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted good receive number " + goodReceive.GoodReceiveId + " " + goodReceive.Reference);
                #endregion
            }

            return Json(new[] { goodReceive }.ToDataSourceResult(request, ModelState));
        }

        private void GetGoodReceiveItems(GoodReceive record)
        {
            #region remove previous receive

            var receiveItems = _db.GoodReceiveItems.Where(x => x.GoodReceiveId == record.GoodReceiveId).ToList();
            _db.GoodReceiveItems.RemoveRange(receiveItems);
            _db.SaveChanges();

            #endregion

            decimal prevReceivedQty = 0;
            decimal unReceivedQty = 0;
            ReceiveType type = record.ReceiveType;

            if (type.Type == GoodReceiveType.PurchaseOrder)
            {
                #region get prev receive

                var previousReceiveIds = _db.GoodReceives
                                        .Where(x => x.ReceiveType.Type == GoodReceiveType.PurchaseOrder)
                                        .Where(x => x.ReceiveRef == record.ReceiveRef).Where(x => x.IsOnlineApproved)
                                        .Where(x => x.IsVoid == false).Select(x => x.GoodReceiveId)
                                        .ToList();

                var prevReceiveItems = _db.GoodReceiveItems
                                        .Where(x => previousReceiveIds.Contains(x.GoodReceiveId))
                                        .ToList();

                #endregion
                #region Purchase Order Items
                int pid = int.Parse(record.ReceiveRef);
                List<PurchaseOrderItem> purchaseOrderItems = _db.PurchaseOrderItems
                    .Where(pi => pi.PurchaseOrderID == pid)
                    .Where(pi => pi.ItemType == PurchaseItemType.Stock)
                    .ToList();

                foreach (var orderItem in purchaseOrderItems)
                {
                    var prevReceiveItem = prevReceiveItems
                        .Where(x => x.ItemCode == orderItem.ItmCode)
                         .Where(x => x.Specification == orderItem.ItemSpecification)//added
                        .ToList();
                    if(prevReceiveItem.Any())
                         prevReceivedQty = prevReceiveItem.Sum(x => x.Quantity);

                    if (prevReceivedQty < orderItem.Quantity)
                        unReceivedQty = orderItem.Quantity - prevReceivedQty;

                 
                        var tax = _db.GLTaxRates.Find(orderItem.GLTaxRateID);

                        var receiveItem = new GoodReceiveItem
                        {
                            GoodReceiveId = record.GoodReceiveId,
                            ItemCategoryCode = orderItem.CatCode,
                            ItemCode = orderItem.ItmCode,
                            Quantity = unReceivedQty,
                            UnitPrice = orderItem.UnitPrice,
                            Balance = unReceivedQty,
                            UOM=orderItem.UoM,
                            GLTaxRateID = orderItem.GLTaxRateID
                        };

                        if (tax != null)
                        {
                            receiveItem.TaxAmount = orderItem.UnitPrice * unReceivedQty * tax.CalculatedRate;
                            receiveItem.SubTotal = orderItem.UnitPrice * unReceivedQty;
                            receiveItem.Total = orderItem.UnitPrice * unReceivedQty +
                                                (orderItem.UnitPrice * unReceivedQty * tax.CalculatedRate);
                        }

                        _db.GoodReceiveItems.Add(receiveItem);
                        _db.SaveChanges();
                }

                #endregion
            }

            else if (type.Type == GoodReceiveType.Tender)
            {
                #region get prev receive

                var previousReceiveIds = _db.GoodReceives.Where(x => x.ReceiveType.Type == GoodReceiveType.Tender)
                                            .Where(x => x.ReceiveRef == record.ReceiveRef).Where(x => x.IsOnlineApproved)
                                            .Where(x => x.IsVoid == false).Select(x => x.GoodReceiveId).ToList();

                var prevReceiveItems = _db.GoodReceiveItems
                                            .Where(x => previousReceiveIds.Contains(x.GoodReceiveId))
                                            .ToList();

                #endregion
                #region tender
                int awardid = int.Parse(record.ReceiveRef);
                var awarditems = _db.BidAwardedSupplierItems
                                        .Where(x => x.ItemType == PurchaseItemType.Stock)
                                        .Where(x => x.BidAwardedSupplierID == awardid)
                                        .ToList();
                foreach (var awardItem in awarditems)
                {
                    decimal unitprice = awardItem.FinalUPrice;
                    #region check foreign purchase

                    var award = _db.BidAwardedSuppliers.Find(awardid);
                    if (award != null)
                    {
                        var lot = _db.Lots.Find(award.LotID);
                        if (lot != null)
                        {
                            var tender = _db.Tenders.Find(lot.TenderID);
                            if (tender != null && tender.PurchaseCategory == PurchaseCategory.ForeignPurchase)
                            {
                                var foreignpurchase = _db.ForeignPurchases.FirstOrDefault(x => x.PurchaseRef == awardItem.BidAwardedSupplierID);
                                if (foreignpurchase != null)
                                {
                                    var item = _db.ForeignPurchaseFollowUpItems
                                        .Where(x => x.ItmCode == awardItem.ItmCode)
                                        .FirstOrDefault(x => x.ForeignPurchaseId == foreignpurchase.ForeignPurchaseId);
                                    if (item != null)
                                        unitprice = item.FinalUnitPrice;
                                }
                            }
                        }
                    }

                    #endregion

                    var prevReceiveItem = prevReceiveItems
                        .Where(x => x.ItemCode == awardItem.ItmCode)
                         .Where(x => x.Specification == awardItem.ItemSpecification)//added
                        .ToList();
                    if (prevReceiveItem.Any())
                        prevReceivedQty = prevReceiveItem.Sum(x => x.Quantity);

                    if (prevReceivedQty < awardItem.Quanity)
                        unReceivedQty = awardItem.Quanity - prevReceivedQty;

                    var tax = _db.GLTaxRates.Find(awardItem.GLTaxRateID);
                    var receiveItem = new GoodReceiveItem
                    {
                        GoodReceiveId = record.GoodReceiveId,
                        ItemCategoryCode = awardItem.CatCode,
                        ItemCode = awardItem.ItmCode,
                        Quantity = unReceivedQty,
                        UnitPrice = unitprice,
                        Balance = unReceivedQty,
                        UOM = awardItem.UoM,
                        GLTaxRateID = awardItem.GLTaxRateID,

                        ModifiedBy = User.Identity.Name,
                        LastModified = DateTime.Today,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Today
                    };
                    if (tax != null)
                    {
                        receiveItem.TaxAmount = unitprice * unReceivedQty * tax.CalculatedRate;
                        receiveItem.SubTotal = unitprice * unReceivedQty;
                        receiveItem.Total = unitprice * unReceivedQty + (unitprice * unReceivedQty * tax.CalculatedRate);
                    }
                    _db.GoodReceiveItems.Add(receiveItem);
                    _db.SaveChanges();
                }

                #endregion

            }
            _distributionHelper.UpdateGoodReceiveDistribution(record);

        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        public ActionResult ReciveImport([DataSourceRequest]DataSourceRequest request, IEnumerable<HttpPostedFileBase> file)
        {

            #region Import

            DataSet ds = new DataSet();
            Core.Domain.Inventory.Issue issuee = _db.Issues.FirstOrDefault();
            List<GoodReceive> issues = new List<GoodReceive>();
            foreach (HttpPostedFileBase fileBase in file)
            {
                string fileExtension = System.IO.Path.GetExtension(Request.Files["file"].FileName);
                if (fileExtension == ".xls" || fileExtension == ".xlsx" || fileExtension == ".xlsm")
                {
                    string fileLocation = Server.MapPath("~/Content/Import/") + DateTime.Now.Year.ToString() +
                                          DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() +
                                          DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() +
                                          DateTime.Now.Second.ToString() + Request.Files["file"].FileName;
                    if (System.IO.File.Exists(fileLocation))
                    {
                        System.IO.File.Delete(fileLocation);
                    }
                    Request.Files["file"].SaveAs(fileLocation);
                    string excelConnectionString = string.Empty;
                    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                            ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";

                    if (fileExtension == ".xls")
                    {
                        excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation +
                                                ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                    }
                    //connection String for xlsx file format.
                    else if (fileExtension == ".xlsx")
                    {

                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    }
                    else if (fileExtension == ".xlsm")
                    {

                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    }
                    //Create Connection to Excel work book and add oledb namespace
                    OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                    excelConnection.Open();
                    DataTable dt = new DataTable();

                    dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dt == null)
                    {
                        return null;
                    }

                    String[] excelSheets = new String[dt.Rows.Count];
                    int t = 0;
                    //excel data saves in temp file here.
                    foreach (DataRow row in dt.Rows)
                    {
                        excelSheets[t] = row["TABLE_NAME"].ToString();
                        t++;
                    }
                    OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                    string query = string.Format("Select * from [{0}]", excelSheets[0]);
                    using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                    {
                        dataAdapter.Fill(ds);
                    }

                }

            }
            List<GoodReciveImport> goodReciveImports = new List<GoodReciveImport>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int j = 0; j <= ds.Tables[0].Rows.Count - 1; j++)
                {
                    GoodReciveImport import = new GoodReciveImport();
                    import.ReciveNo = ds.Tables[0].Rows[j].ItemArray[0].ToString();
                    import.ReceiveType = int.Parse(ds.Tables[0].Rows[j].ItemArray[1].ToString());
                    import.ReferenceId = ds.Tables[0].Rows[j].ItemArray[2].ToString();

                    import.Store = ds.Tables[0].Rows[j].ItemArray[3].ToString();
                    import.SupplierCode = ds.Tables[0].Rows[j].ItemArray[4].ToString();
                    import.SupplierInvoice = ds.Tables[0].Rows[j].ItemArray[5].ToString();
                    import.ReciveDate = DateTime.Parse(ds.Tables[0].Rows[j].ItemArray[6].ToString());
                    import.ItemCode = ds.Tables[0].Rows[j].ItemArray[7].ToString();
                    import.Quantity = decimal.Parse(ds.Tables[0].Rows[j].ItemArray[8].ToString());




                    goodReciveImports.Add(import);



                }
            }
            var imports = goodReciveImports.GroupBy(x => x.ReciveNo).Select(x => x.FirstOrDefault());
            foreach (var goodReciveImport in goodReciveImports)
            {
                var inventoryItem = _db.InventoryItems.Find(goodReciveImport.ItemCode);
                if (inventoryItem == null)
                {
                    ModelState.AddModelError("unknown", "Unknown Item " + goodReciveImport.ItemCode);
                    ViewBag.Message = "Unkown Item " + goodReciveImport.ItemCode;
                }
            }
            if (ModelState.IsValid)
            {
                foreach (var imp in imports)
                {
                    if (imp != null)
                    {
                        ReceiveType type = _db.ReceiveTypes.Find(imp.ReceiveType);
                        if (type != null)
                        {
                            switch (type.Type)
                            {
                                case GoodReceiveType.PurchaseOrder:
                                    int pid = int.Parse(imp.ReferenceId);
                                    PurchaseOrder purchaseOrder = _db.PurchaseOrders.Find(pid);
                                    if (purchaseOrder != null) imp.SupplierCode = purchaseOrder.SupplierName;

                                    break;
                            }

                            GoodReceive goodReceive = new GoodReceive
                            {
                                StoreCode = imp.Store,
                                ReceiveDate = imp.ReciveDate,
                                ReceiveRef = imp.ReferenceId,
                                Reference = imp.ReferenceDocument,
                                Status = Enum.GetName(typeof(OperationStatuses), 5)
                            };

                            List<IssueItem> issueItems = new List<IssueItem>();
                            //foreach (var source in goodReciveImports.Where(x => x.IssueNo == imp.IssueNo))
                            //{

                            //    InventoryItem inventoryItem = db.InventoryItems.Find(source.ItemCode);
                            //    StoreRequisitionItem requestedItem =
                            //        storeRequisition.StoreRequisitionItem.FirstOrDefault(x => x.ItemCode == source.ItemCode);

                            //    IssueItem issueItem;


                            //}
                            //goodReceive.IssueItems = issueItems;
                            //goodReceive.CreatedBy = User.Identity.Name;
                            //goodReceive.DateCreated = DateTime.Now;
                            //db.Issues.Add(goodReceive);
                            //db.SaveChanges(); 
                        }
                    }

            #endregion

                    //#region Distribution

                    //IssueDistribution distribution;
                    //Dictionary<string, string> checkAccount = new Dictionary<string, string>();
                    //List<IssueDistribution> distributions = new List<IssueDistribution>();
                    //#region debit

                    //foreach (var line in goodReceive.IssueItems)
                    //{
                    //    var Item = db.InventoryItems.FirstOrDefault(it => it.ItemCode == line.ItemCode);
                    //    ItemCategory itemCategory = db.ItemCategorys.Find(Item.ItemCategoryCode);

                    //    string acc = itemCategory.Description;
                    //    string[] account = acc.Split('-');
                    //    string newacc = account[0] + "-" + account[1] + "-" + goodReceive.StoreRequisition.Department + "-" + account[3];


                    //    if (line.Total != 0 && checkAccount.ContainsKey(acc))
                    //    {
                    //        IssueDistribution iDistribution = distributions.FirstOrDefault(x => x.Description == newacc);

                    //        iDistribution.Description = newacc;
                    //        iDistribution.Debit += line.Total;
                    //        iDistribution.Credit = 0;
                    //        iDistribution.IssueID = line.IssueID;
                    //        iDistribution.IssueItemIDTrackBack = line.IssueItemID;

                    //    }
                    //    else if (line.Total != 0 && !checkAccount.ContainsKey(acc))
                    //    {
                    //        distribution = new IssueDistribution();
                    //        distribution.Description = newacc;
                    //        distribution.Debit = line.Total;
                    //        distribution.Credit = 0;
                    //        distribution.IssueID = line.IssueID;
                    //        distribution.IssueItemIDTrackBack = line.IssueItemID;
                    //        distributions.Add(distribution);
                    //        checkAccount.Add(acc, line.ItemCode);
                    //    }
                    //}
                    //foreach (IssueDistribution issueDistribution in distributions)
                    //{
                    //    db.IssueDistributions.Add(issueDistribution);
                    //    db.SaveChanges();
                    //}
                    //distributions.Clear();
                    //checkAccount.Clear();
                    //#endregion

                    //#region Credit

                    //if (goodReceive.IssueItems.Sum(x => x.Total) != 0)
                    //{
                    //    foreach (IssueItem items in goodReceive.IssueItems)
                    //    {
                    //        InventoryItem inventory = db.InventoryItems.Find(items.ItemCode);
                    //        string AccCode = inventory.ItemCategory.IssueAccountCode;

                    //        if (items.Total != 0 && checkAccount.ContainsKey(AccCode))
                    //        {
                    //            IssueDistribution iDistribution = distributions.FirstOrDefault(x => x.Description == AccCode);

                    //            iDistribution.Description = AccCode;
                    //            iDistribution.Debit = 0;
                    //            iDistribution.Credit += items.Total;
                    //            iDistribution.IssueID = items.IssueID;
                    //            iDistribution.IssueItemIDTrackBack = items.IssueItemID;

                    //        }
                    //        else if (items.Total != 0 && !checkAccount.ContainsKey(AccCode))
                    //        {
                    //            distribution = new IssueDistribution();
                    //            distribution.Description = AccCode;
                    //            distribution.Debit = 0;
                    //            distribution.Credit = items.Total;
                    //            distribution.IssueID = items.IssueID;
                    //            distribution.IssueItemIDTrackBack = items.IssueItemID;
                    //            distributions.Add(distribution);
                    //            checkAccount.Add(AccCode, items.ItemCode);
                    //        }

                    //    }
                    //    foreach (var issueDistribution in distributions)
                    //    {
                    //        db.IssueDistributions.Add(issueDistribution);
                    //        db.SaveChanges();
                    //    }
                    //    distributions.Clear();
                    //    checkAccount.Clear();

                    //}
                    //issues.Add(goodReceive);
                    //#endregion

                    //#endregion

                }

            }
            return Json(new[] { issuee }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        public JsonResult SendForApproval(int id)
        {
            var record = _db.GoodReceives.Find(id);
            var items = _db.GoodReceiveItems.Where(x => x.GoodReceiveId == id).ToList();
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && items.Any())
            {
                #region validate qty,price and tax

                foreach (var item in record.GoodReceiveItem)
                {
                    if (item.UnitPrice <= 0 || item.Quantity <= 0 || item.GLTaxRateID == 0)
                    {
                        var formattedData3 = new
                        {
                            Message = "Please fill quantity, unit price and tax information for all items.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData3, JsonRequestBehavior.AllowGet);
                    }
                }
                #endregion
                #region validate total receive
                if (!record.IsManual)
                {
                    string error = InventoryValidationHelper.ValidateGrN(record.GoodReceiveId);
                    if (!string.IsNullOrEmpty(error))
                    {
                        var formattedData1 = new
                        {
                            Message = error,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData1, JsonRequestBehavior.AllowGet);
                    }
                }
                #endregion 

                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.GoodReceives.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "good receive", "Success", User.Identity.Name, $"send for approval good receive {id} ", Modules.Inventory);
                #endregion
                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            else if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            else if (!items.Any())
            {
                var formattedData = new
                {
                    Message = "Please add line items first.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            else if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            else if (record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is already approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [AuthorizeRoles(InventoryConstants.PropertyVoid)]
        public JsonResult Void(int id)
        {
            var record = _db.GoodReceives.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred && record.GoodReceiveItem.Count > 0)
            {
                bool valid = true;
                #region validate

                foreach (var i in record.GoodReceiveItem)
                {
                    #region stock

                    var storeItem = _db.StoreItemAssignments.Where(x => x.ItemCode == i.ItemCode).FirstOrDefault(x => x.StoreCode == record.StoreCode);
                    if (storeItem == null)
                    {
                        valid = false;
                        string message = "Item: " + i.InventoryItem.Name + " is not available in store: " + record.StoreCode;
                        ModelState.AddModelError("store", message);

                        var formattedData2 = new
                        {
                            Message = message,
                            Status = "OK"
                        };
                        return Json(formattedData2, JsonRequestBehavior.AllowGet);

                    }
                    if (storeItem.Balance - i.Quantity < 0)
                    {
                        valid = false;
                        string message ="There is no balance in system to void for item " + i.ItemCode +"-"+i.InventoryItem.Name;
                        var formattedData2 = new
                        {
                            Message = message,
                            Status = "OK"
                        };
                        return Json(formattedData2, JsonRequestBehavior.AllowGet);
                    }

                    #endregion
                    
                }
                #endregion

                if (valid)
                {
                    record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                    record.IsVoid = true;
                    record.VoidBy = User.Identity.Name;
                    record.VoidTime = DateTime.Now;
                    _db.GoodReceives.Attach(record);
                    _db.Entry(record).State = EntityState.Modified;
                    //_db.SaveChanges();

                    #region update stock

                    string storecode = record.StoreCode;

                    foreach (GoodReceiveItem i in record.GoodReceiveItem)
                    {
                        string itemCode = i.ItemCode;

                        InventoryItem stockItem = _db.InventoryItems.Find(itemCode);
                        int storeBranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(storecode);

                        #region stock

                        var branchItem =
                            _db.BranchItemAssignments.Where(x => x.BranchId == storeBranchId)
                                .FirstOrDefault(x => x.ItemCode == i.ItemCode);
                        var storeItem =
                            _db.StoreItemAssignments.FirstOrDefault(
                                x => x.StoreCode == storecode && x.ItemCode == itemCode);

                        if (storeItem != null)
                        {
                            storeItem.Balance = storeItem.Balance - i.Quantity;
                            _db.Entry(storeItem).State = EntityState.Modified;
                            //_db.SaveChanges();

                            #region update bin card

                            #region bin card line

                            BinCardLine binCardLine = new BinCardLine
                            {
                                StoreItemAssignment = storeItem,
                                ReferenceDoc = "Void Receive " + record.GoodReceiveId,
                                Date = record.ReceiveDate,
                                IssuedQTY = i.Quantity,
                                IssuedUPrice = i.UnitPrice,
                                IssuedTPrice = i.Total,
                                BalanceQTY = storeItem.Balance,
                                BalanceUPrice = i.UnitPrice,
                                BalanceTPrice = i.UnitPrice * storeItem.Balance,
                                //new    
                                StoreCode = record.StoreCode,
                                GLPeriodId = record.GLPeriodId,
                                TransactionType = BinCardTransactionTypes.Receive,
                                StatusType = BinCardTransactionStatus.Regular,
                                TransactionOrder = BinCardTransactionOrder.VoidReceive
                            };
                            _db.BinCardLines.Add(binCardLine);
                           // _db.SaveChanges();

                            #endregion

                            #endregion

                        }
                        if (branchItem != null)
                        {
                            branchItem.Balance = branchItem.Balance - i.Quantity;
                            _db.Entry(branchItem).State = EntityState.Modified;
                            //_db.SaveChanges();

                        }
                        if (stockItem != null)
                        {
                            stockItem.Balance = stockItem.Balance - i.Quantity;
                            _db.Entry(stockItem).State = EntityState.Modified;
                            //_db.SaveChanges();
                        }



                        #endregion

                    }
                    _db.SaveChanges();

                    #endregion

                    #region update  po status
                    var receiveType = _db.ReceiveTypes.Where(x => x.ReceiveTypeId == record.ReceiveTypeId).FirstOrDefault();
                    if(receiveType!=null)
                    {
                        if(receiveType.Type==GoodReceiveType.PurchaseOrder)
                        {
                            int did = int.Parse(record.ReceiveRef);
                            var po = _db.PurchaseOrders.Find(did);
                            if(po!=null)
                            {
                                po.IsFullfilled = false;
                                _db.PurchaseOrders.Attach(po);
                                _db.Entry(po).State = EntityState.Modified;
                                _db.SaveChanges();
                            }
                        }
                    }
                    #endregion

                    //copy record
                    CopyRecord(id);

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "good receive", "Success", User.Identity.Name, $"void good receive {id} ", Modules.Inventory);
                    #endregion

                    var formattedData = new
                    {
                        Message = "Void Successful.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
            }
            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.GoodReceiveItem.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add line items first.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record.IsVoid)
            {
                var formattedData = new
                {
                    Message = "The record is already void.You can not void again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record.IsOnlineTransferred)
            {
                var formattedData = new
                {
                    Message = "The record is transferred to GL.You can not void .",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (!record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not approved hence you can modify by editing.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            return null;
        }

        private void CopyRecord(int id)
        {
            var record = _db.GoodReceives.Find(id);
            if (record!=null)
            {
                #region header
                 var entity = new GoodReceive
                    {
                        BranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(record.StoreCode),
                        IsManual = record.IsManual,
                        ReceiveRef = record.ReceiveRef,
                        ReceiveTypeId = record.ReceiveTypeId,
                        StoreCode = record.StoreCode,
                        SupplierName = record.SupplierName,
                        SupplierInvoiceNo = record.SupplierInvoiceNo,
                        TenderLot = record.TenderLot,
                        Reference = record.Reference,
                        ReceiveDate = record.ReceiveDate,
                        Status = Enum.GetName(typeof(OperationStatuses), 5),
                        Remark = record.Remark,
                        GLPeriodId=record.GLPeriodId,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        ModifiedBy = record.ModifiedBy,
                        LastModified = record.LastModified,
                        OrgBranchName = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(record.StoreCode).ToString()
                    };

                _db.GoodReceives.Add(entity);
                _db.SaveChanges();
                #endregion
                foreach (var i in record.GoodReceiveItem)
                {
                    #region lines
                    var line = new GoodReceiveItem
                    {
                        GoodReceiveId = entity.GoodReceiveId,
                        ItemCategoryCode = i.ItemCategoryCode,
                        ItemCode = i.ItemCode,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        SubTotal = i.SubTotal,
                        GLTaxRateID = i.GLTaxRateID,
                        TaxAmount = i.TaxAmount,
                        Total = i.Total,
                        IsExtra = i.IsExtra,
                        ExtraQuantity = i.ExtraQuantity,
                        ExtraAmount = i.ExtraAmount,
                        WarrantyInformation = i.WarrantyInformation,
                        Specification = i.Specification,
                        Expire = i.Expire,
                        ExpireDate = i.ExpireDate,
                        Balance = i.Balance,
                       
                        OrgBranchName = i.OrgBranchName,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                    };
                    _db.GoodReceiveItems.Add(line);
                    _db.SaveChanges();
                 
                    #endregion

                }
               
            }
           
        }

        [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
        public JsonResult TransferDistribution(string id, string description)
        {
            int myid = int.Parse(id);
            var record = _db.GoodReceives.Find(myid);

            if (record != null && record.IsOnlineApproved && !record.IsOnlineTransferred)
            {
                _transferHelper = new InventoryTransferHelper();
                string response= _transferHelper.GoodReceive(myid, description, User?.Identity?.Name ?? "");
                if (response != "SUCCESS")
                {
                    var formattedData = new
                    {
                        Message = response,
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var formattedData = new
                    {
                        Message = "Successfuly Transferred.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }


            }
            if (record != null && !record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record cannot be transfered before validation.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record != null && record.IsOnlineTransferred)
            {
                var formattedData = new
                {
                    Message = "You cannot transfer this transaction, it is already transfered.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            return null;
        }       

        [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
        public JsonResult GenerateDistribution(int id)
        {

            var record = _db.GoodReceives.Find(id);
            if (record != null && !record.IsOnlineTransferred)
            {
                _distributionHelper.UpdateGoodReceiveDistribution(record);
                var formattedData1 = new
                {
                    Message = "Complete.",
                    Status = "OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);
            }

            var formattedData = new
            {
                Message = "Record not found.",
                Status = "NOT_OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

    }
}
