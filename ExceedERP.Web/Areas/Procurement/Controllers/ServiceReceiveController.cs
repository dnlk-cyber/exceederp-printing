using System;
using System.Collections.Generic;

using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender;
using ExceedERP.Core.Domain.Procurement.Tender.Lot;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Areas.Inventory.Models;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using EntityState = System.Data.Entity.EntityState;


namespace ExceedERP.Web.Areas.Procurement.Controllers
{



    public class ServiceReceiveController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        private readonly DistributionHelper _distributionHelper = new DistributionHelper();

        [AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentUser, ProcurementRoles.ProcurementUserDepartmentApprover)]
        public ActionResult Index()
        {

            PopulateViewData();
            return View();

        }

        private void PopulateViewData()
        {
            var categories = _db.ServiceCategories;
            var items = _db.ServiceItems;
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
            ViewData["Items"] = items;
            ViewData["Category"] = categories;
            ViewData["defaultCategory"] = categories.FirstOrDefault();

        }

        [AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentUser, ProcurementRoles.ProcurementUserDepartmentApprover)]
        public ActionResult ServiceReceives_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            IQueryable<ServiceReceive> serviceReceives = _db.ServiceReceives.Where(x => mybraches.Contains(x.BranchId));

            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                List<int> myBusinessUnits = UserInformationHelper.GetUserBusinessUnits(User.Identity.Name);
                serviceReceives = serviceReceives.Where(x => myBusinessUnits.Contains(x.BusinessUnitId));
            }

            DataSourceResult result = serviceReceives.ToDataSourceResult(request, serviceReceive => new
            {
                ReceiveRef = serviceReceive.ReceiveRef,
                BranchId = serviceReceive.BranchId,
                BusinessUnitId = serviceReceive.BusinessUnitId,
                ReceiveTypeId = serviceReceive.ReceiveTypeId,
                ServiceReceiveId = serviceReceive.ServiceReceiveId,
                TenderLot = serviceReceive.TenderLot,
                SupplierName = serviceReceive.SupplierName,
                SupplierInvoiceNo = serviceReceive.SupplierInvoiceNo,
                Reference = serviceReceive.Reference,
                ItemType = serviceReceive.ItemType,
                ReceiveDate = serviceReceive.ReceiveDate,
                DateCreated = serviceReceive.DateCreated,
                CreatedBy = serviceReceive.CreatedBy,
                Status = serviceReceive.Status,
                Remark = serviceReceive.Remark,
                GLPeriodId = serviceReceive.GLPeriodId,
                IsVoid = serviceReceive.IsVoid,
                VoidBy = serviceReceive.VoidBy,
                VoidTime = serviceReceive.VoidTime,

                IsOnlineApproved = serviceReceive.IsOnlineApproved,
                OnlineApprovedBy = serviceReceive.OnlineApprovedBy,
                OnlineApprovedTime = serviceReceive.OnlineApprovedTime,

                IsSendForApproval = serviceReceive.IsSendForApproval,
                SendForApprovalBy = serviceReceive.SendForApprovalBy,
                SendForApprovalTime = serviceReceive.SendForApprovalTime,

                IsOnlineTransferred = serviceReceive.IsOnlineTransferred,
                IsOnlineTransferredBy = serviceReceive.IsOnlineTransferredBy,
                OnlineTransferredTime = serviceReceive.OnlineTransferredTime,
            });

            return Json(result);

        }

      
        [AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceReceives_Create([DataSourceRequest]DataSourceRequest request, ServiceReceive serviceReceive)
        {
            ReceiveType type = _db.ReceiveTypes.Find(serviceReceive.ReceiveTypeId);
            if (type != null)
            {
                switch (type.Type)
                {
                    case GoodReceiveType.PurchaseOrder:
                        int pid = int.Parse(serviceReceive.ReceiveRef);
                        PurchaseOrder purchaseOrder = _db.PurchaseOrders.Find(pid);
                        if (purchaseOrder != null) serviceReceive.SupplierName = purchaseOrder.SupplierId;
                        break;
                    case GoodReceiveType.Tender:
                        int lotId = int.Parse(serviceReceive.ReceiveRef);
                        Lot lot = _db.Lots.Find(lotId);
                        break;
                }
            }
            else
            {
                ModelState.AddModelError("type", "Receive Type is not found.");
            }

            if (!String.IsNullOrEmpty(serviceReceive.ReceiveRef))
            {
                var exists = _db.ServiceReceives.Where(x => x.ReceiveRef == serviceReceive.ReceiveRef && x.IsVoid == false).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("Reference", serviceReceive.ReceiveRef + "  already exists in the system.");
                }
            }

            if (!String.IsNullOrEmpty(serviceReceive.Reference))
            {
                var exists = _db.ServiceReceives.Where(x => x.Reference == serviceReceive.Reference && x.IsVoid == false).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("Reference",serviceReceive.Reference + "  already exists in the system.");
                }
            }

            if (serviceReceive.GLPeriodId != 0)
            {
                var inventoryPeriod = _db.InventoryPeriods.Find(serviceReceive.GLPeriodId);
                if (inventoryPeriod != null)
                {
                    if (
                        !((serviceReceive.ReceiveDate.Date >= inventoryPeriod.DateFrom) &&
                          (serviceReceive.ReceiveDate.Date <= inventoryPeriod.DateTo)))
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

            if (serviceReceive.ReceiveDate.Date > DateTime.Today)
            {
                ModelState.AddModelError("future", "You are recording in future period,please correct the date.");
            }

            if (ModelState.IsValid)
            {


                var entity = new ServiceReceive
                    {
                        BranchId = serviceReceive.BranchId,
                        BusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name),
                        ReceiveRef = serviceReceive.ReceiveRef,
                        ReceiveTypeId = serviceReceive.ReceiveTypeId,
                        SupplierName = serviceReceive.SupplierName,
                        SupplierId= serviceReceive.SupplierId,
                        SupplierInvoiceNo = serviceReceive.SupplierInvoiceNo,
                        TenderLot = serviceReceive.TenderLot,
                        Reference = serviceReceive.Reference,
                        ReceiveDate = serviceReceive.ReceiveDate,
                        Status = Enum.GetName(typeof(OperationStatuses), 5),
                        ItemType = PurchaseItemType.Service,
                        Remark = serviceReceive.Remark,
                        GLPeriodId=serviceReceive.GLPeriodId,
                        IsVoid = serviceReceive.IsVoid,
                        VoidBy = serviceReceive.VoidBy,
                        VoidTime = serviceReceive.VoidTime,
                        IsOnlineApproved = serviceReceive.IsOnlineApproved,
                        OnlineApprovedBy = serviceReceive.OnlineApprovedBy,
                        OnlineApprovedTime = serviceReceive.OnlineApprovedTime,
                        IsOnlineTransferred = serviceReceive.IsOnlineTransferred,
                        IsOnlineTransferredBy = serviceReceive.IsOnlineTransferredBy,
                        OnlineTransferredTime = serviceReceive.OnlineTransferredTime,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        ModifiedBy = serviceReceive.ModifiedBy,
                        LastModified = serviceReceive.LastModified,
                        OrgBranchName = serviceReceive.OrgBranchName
                    };

                _db.ServiceReceives.Add(entity);
                _db.SaveChanges();
                serviceReceive.ServiceReceiveId = entity.ServiceReceiveId;

                GetServiceReceiveItems(entity);
            }

            return Json(new[] { serviceReceive }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceReceives_Update([DataSourceRequest]DataSourceRequest request, ServiceReceive serviceReceive)
        {

            if (!String.IsNullOrEmpty(serviceReceive.ReceiveRef))
            {
                var exists = _db.ServiceReceives.Where(x => x.ServiceReceiveId != serviceReceive.ServiceReceiveId).Where(x => x.ReceiveRef == serviceReceive.ReceiveRef && x.IsVoid == false).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("Reference", serviceReceive.ReceiveRef + "  already exists in the system.");
                }
            }
            if (!String.IsNullOrEmpty(serviceReceive.Reference))
            {
                var exists = _db.ServiceReceives.Where(x => x.ServiceReceiveId != serviceReceive.ServiceReceiveId).Where(x => x.Reference == serviceReceive.Reference && x.IsVoid == false).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("Reference", serviceReceive.Reference + "  already exists in the system.");
                }
            }

            if (serviceReceive.GLPeriodId != 0)
            {
                var inventoryPeriod = _db.InventoryPeriods.Find(serviceReceive.GLPeriodId);
                if (inventoryPeriod != null)
                {
                    if (
                        !((serviceReceive.ReceiveDate.Date >= inventoryPeriod.DateFrom) &&
                          (serviceReceive.ReceiveDate.Date <= inventoryPeriod.DateTo)))
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

            if (serviceReceive.ReceiveDate > DateTime.Today)
            {
                ModelState.AddModelError("future", "You are recording in future period,please correct the date.");
            }

            if (ModelState.IsValid)
            {
                var entity = new ServiceReceive
                {
                    BranchId = serviceReceive.BranchId,
                    BusinessUnitId = serviceReceive.BusinessUnitId,
                    ServiceReceiveId = serviceReceive.ServiceReceiveId,
                    ReceiveRef = serviceReceive.ReceiveRef,
                    SupplierName = serviceReceive.SupplierName,
                    SupplierInvoiceNo = serviceReceive.SupplierInvoiceNo,
                    Reference = serviceReceive.Reference,
                    ReceiveDate = serviceReceive.ReceiveDate,
                    Status = serviceReceive.Status,
                    ReceiveTypeId = serviceReceive.ReceiveTypeId,
                    Remark = serviceReceive.Remark,
                    ItemType = PurchaseItemType.Service,
                    GLPeriodId =serviceReceive.GLPeriodId,
                    IsVoid = serviceReceive.IsVoid,
                    VoidBy = serviceReceive.VoidBy,
                    VoidTime = serviceReceive.VoidTime,
                    IsOnlineApproved = serviceReceive.IsOnlineApproved,
                    OnlineApprovedBy = serviceReceive.OnlineApprovedBy,
                    OnlineApprovedTime = serviceReceive.OnlineApprovedTime,
                    IsOnlineTransferred = serviceReceive.IsOnlineTransferred,
                    IsOnlineTransferredBy = serviceReceive.IsOnlineTransferredBy,
                    OnlineTransferredTime = serviceReceive.OnlineTransferredTime,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                    OrgBranchName =serviceReceive.OrgBranchName
                };

                _db.ServiceReceives.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { serviceReceive }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceReceives_Destroy([DataSourceRequest]DataSourceRequest request, ServiceReceive serviceReceive)
        {

            ModelState.Remove("PurchaseOrderID");

            if (ModelState.IsValid)
            {
                var entity = new ServiceReceive
                {
                    BranchId = serviceReceive.BranchId,
                    BusinessUnitId = serviceReceive.BusinessUnitId,
                    ServiceReceiveId = serviceReceive.ServiceReceiveId,
                    SupplierName = serviceReceive.SupplierName,
                    SupplierInvoiceNo = serviceReceive.SupplierInvoiceNo,
                    Reference = serviceReceive.Reference,
                    ReceiveDate = serviceReceive.ReceiveDate,
                    Status = serviceReceive.Status,
                    Remark = serviceReceive.Remark,
                    IsVoid = serviceReceive.IsVoid,
                    ReceiveTypeId = serviceReceive.ReceiveTypeId,
                    ItemType = serviceReceive.ItemType,
                    VoidBy = serviceReceive.VoidBy,
                    VoidTime = serviceReceive.VoidTime,
                    IsOnlineApproved = serviceReceive.IsOnlineApproved,
                    OnlineApprovedBy = serviceReceive.OnlineApprovedBy,
                    OnlineApprovedTime = serviceReceive.OnlineApprovedTime,
                    IsOnlineTransferred = serviceReceive.IsOnlineTransferred,
                    IsOnlineTransferredBy = serviceReceive.IsOnlineTransferredBy,
                    OnlineTransferredTime = serviceReceive.OnlineTransferredTime,
                };

                _db.ServiceReceives.Attach(entity);
                _db.ServiceReceives.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted good receive number " + serviceReceive.ServiceReceiveId + " " + serviceReceive.Reference);
                #endregion
            }

            return Json(new[] { serviceReceive }.ToDataSourceResult(request, ModelState));
        }

       
      

        [AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentUser)]
        public JsonResult SendForApproval(int id)
        {
            var record = _db.ServiceReceives.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.ServiceReceiveItems.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.ServiceReceives.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
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

            else if (record.ServiceReceiveItems.Count == 0)
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

       // [AuthorizeRoles(ProcurementConstants.PropertyVoid)]
        public JsonResult Void(int id)
        {
            var record = _db.ServiceReceives.Find(id);
          


            return null;
        }

        private void CopyRecord(int id)
        {
            var record = _db.ServiceReceives.Find(id);
            if (record!=null)
            {
                
               
            }
           
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentUser)]
        public JsonResult ServiceReceive_OnlineTransfer(string id)
        {
            int myid = int.Parse(id);
            ServiceReceive record = _db.ServiceReceives.Find(myid);

            if (record != null && record.IsOnlineApproved && !record.IsOnlineTransferred)
            {
                #region check if debit and credit are equal
                decimal debit = 0;
                decimal credit = 0;
                var distribution = record.ServiceReceiveDistribution.ToList();
                if (distribution.Any())
                {
                    debit = distribution.Sum(x => x.Debit);
                    credit = distribution.Sum(x => x.Credit);
                }
                if (debit != credit)
                {
                    var formattedData = new
                    {
                        Message = "Total debit and total credit is not balanced.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                if (debit == 0)
                {
                    var formattedData = new
                    {
                        Message = "Total debit and total credit is zero.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                #endregion
                #region send to GL
                string glperiod = "";
                string glyear = "";
                #region get period
                var period = _db.GLPeriods.Where(x => x.DateFrom <= record.ReceiveDate).Where(x => x.DateTo >= record.ReceiveDate).ToList();
                if (period.Any())
                {
                    if (period.Count == 1)
                    {
                        var myPeriod = period.FirstOrDefault();
                        if (myPeriod != null)
                        {
                            glperiod = myPeriod.Name;
                            glyear = myPeriod.GlFiscalYear.Name;
                            if (myPeriod.Status == FiscalYearStatus.Close)
                            {
                                var formattedData = new
                                {
                                    Message = "The period " + glperiod + " is closed,the record cannot be transfered to closed period.",
                                    Status = "NOT_OK"
                                };
                                return Json(formattedData, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }

                    else if (period.Count > 1)
                    {
                        var formattedData = new
                        {
                            Message = "There are multiple periods defined that contain the transaction date.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var formattedData = new
                        {
                            Message = "There are no periods defined that contain the transaction date.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);

                    }
                }

                #endregion
                if (record.ServiceReceiveDistribution.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        JournalName = "ServiceReceive",
                        Description = "ServiceReceive",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "ServiceReceive",
                        SourcesName = "GR",
                        JournalNumber = "ServiceReceive." + record.ServiceReceiveId,
                        EffectiveDate = record.ReceiveDate,
                        Status = "Approved",
                        IsOnlineApproved = true,
                        OnlineApprovedBy = User.Identity.Name,
                        OnlineApprovedTime = DateTime.Now
                    };
                    _db.GLRecordJournals.Add(journal);
                    _db.SaveChanges();
                    foreach (ServiceReceiveDistribution i in record.ServiceReceiveDistribution)
                    {
                        #region debit and credit

                        var journalDetail = new GLRecordJournalDetail
                        {
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            GLRecordJournal = journal,
                            AccountCode = i.AccountCode,
                            Debit = i.Debit,
                            Credit = i.Credit
                        };
                        _db.GLRecordJournalDetails.Add(journalDetail);
                        _db.SaveChanges();
                        #endregion
                    }
                    record.IsOnlineTransferred = true;
                    record.IsOnlineTransferredBy = User.Identity.Name;
                    record.OnlineTransferredTime = DateTime.Now;
                    _db.Entry(record).State = EntityState.Modified;
                    _db.SaveChanges();
                    var formattedData = new
                    {
                        Message = "Successfully transferred.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var formattedData = new
                    {
                        Message = "There is no distribution to be transferred.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                #endregion

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

        private void GetServiceReceiveItems1(ServiceReceive record)
        {

            ReceiveType type = record.ReceiveType;

            if (type.Type == GoodReceiveType.PurchaseOrder)
            {
                #region Purchase Order Items
                int pid = int.Parse(record.ReceiveRef);
                List<PurchaseOrderItem> purchaseOrderItems = _db.PurchaseOrderItems
                    .Where(pi => pi.PurchaseOrderID == pid)
                    .Where(pi => pi.ItemType == PurchaseItemType.Service)
                    .ToList();

                foreach (var orderItem in purchaseOrderItems)
                {
                    var tax = _db.GLTaxRates.Find(orderItem.GLTaxRateID);

                    var receiveItem = new ServiceReceiveItem
                    {
                        ServiceReceiveId = record.ServiceReceiveId,
                        ServiceCategoryCode = orderItem.CatCode,
                        ServiceItemCode = orderItem.ItmCode,
                        Quantity = orderItem.Quantity,
                        UnitPrice = orderItem.UnitPrice,
                        GLTaxRateID = orderItem.GLTaxRateID,
                        TaxAmount = orderItem.TaxAmount,
                        SubTotal = orderItem.SubTotal,
                        Total = orderItem.Total
                    };

                    if (tax != null)
                    {
                        receiveItem.TaxAmount = orderItem.UnitPrice * receiveItem.Quantity * tax.CalculatedRate;
                        receiveItem.SubTotal = orderItem.UnitPrice * receiveItem.Quantity;
                        receiveItem.Total = orderItem.UnitPrice * receiveItem.Quantity + (orderItem.UnitPrice * receiveItem.Quantity * tax.CalculatedRate);
                    }

                    _db.ServiceReceiveItems.Add(receiveItem);
                    _db.SaveChanges();

                }

                #endregion
            }

            else if (type.Type == GoodReceiveType.Tender)
            {
                #region tender
                int awardid = int.Parse(record.ReceiveRef);
                var awarditems = _db.BidAwardedSupplierItems.Where(x => x.ItemType == PurchaseItemType.Service).Where(x => x.BidAwardedSupplierID == awardid).ToList();
                foreach (var awardItem in awarditems)
                {
                    decimal unitprice = awardItem.UPrice;
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
                    var tax = _db.GLTaxRates.Find(awardItem.GLTaxRateID);
                    var receiveItem = new ServiceReceiveItem
                    {
                        ServiceReceiveId = record.ServiceReceiveId,
                        ServiceCategoryCode = awardItem.CatCode,
                        ServiceItemCode = awardItem.ItmCode,
                        Quantity = awardItem.Quanity,
                        UnitPrice = unitprice,
                        GLTaxRateID = awardItem.GLTaxRateID,
                        TaxAmount = awardItem.TaxAmount,

                        ModifiedBy = User.Identity.Name,
                        LastModified = DateTime.Today,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Today
                    };
                    if (tax != null)
                    {
                        receiveItem.TaxAmount = unitprice * receiveItem.Quantity * tax.CalculatedRate;
                        receiveItem.SubTotal = unitprice * receiveItem.Quantity;
                        receiveItem.Total = unitprice * receiveItem.Quantity +
                                            (unitprice * receiveItem.Quantity * tax.CalculatedRate);
                    }

                    _db.ServiceReceiveItems.Add(receiveItem);
                    _db.SaveChanges();

                }

                #endregion

            }
            // DistributionHelper.UpdateGoodReceiveDistribution(record);

        }

        private void GetServiceReceiveItems(ServiceReceive record)
        {
            #region remove previous receive

            var receiveItems = _db.ServiceReceiveItems.Where(x => x.ServiceReceiveId == record.ServiceReceiveId).ToList();
            _db.ServiceReceiveItems.RemoveRange(receiveItems);
            _db.SaveChanges();

            #endregion

            decimal prevReceivedQty = 0;
            decimal unReceivedQty = 0;
            ReceiveType type = record.ReceiveType;

            if (type.Type == GoodReceiveType.PurchaseOrder)
            {
                #region get prev receive

                var previousReceiveIds = _db.ServiceReceives.Where(x => x.ReceiveType.Type == GoodReceiveType.PurchaseOrder)
                    .Where(x => x.ReceiveRef == record.ReceiveRef).Where(x => x.IsOnlineApproved)
                    .Where(x => x.IsVoid == false).Select(x => x.ServiceReceiveId).ToList();

                var prevReceiveItems = _db.ServiceReceiveItems.Where(x => previousReceiveIds.Contains(x.ServiceReceiveId))
                    .ToList();

                #endregion
                #region Purchase Order Items
                int pid = int.Parse(record.ReceiveRef);
                List<PurchaseOrderItem> purchaseOrderItems = _db.PurchaseOrderItems
                    .Where(pi => pi.PurchaseOrderID == pid)
                    .Where(pi => pi.ItemType == PurchaseItemType.Service)
                    .ToList();

                foreach (var orderItem in purchaseOrderItems)
                {
                    var prevReceiveItem = prevReceiveItems.Where(x => x.ServiceItemCode == orderItem.ItmCode).ToList();
                    if (prevReceiveItem.Any())
                        prevReceivedQty = prevReceiveItem.Sum(x => x.Quantity);

                    if (prevReceivedQty < orderItem.Quantity)
                        unReceivedQty = orderItem.Quantity - prevReceivedQty;


                    var tax = _db.GLTaxRates.Find(orderItem.GLTaxRateID);

                    var receiveItem = new ServiceReceiveItem
                    {
                        ServiceReceiveId = record.ServiceReceiveId,
                        ServiceCategoryCode = orderItem.CatCode,
                        ServiceCategoryName = orderItem.CategoryName,
                        ServiceItemCode = orderItem.ItmCode,
                        ServiceItemName = orderItem.ItemName,
                        Quantity = unReceivedQty,
                        //UnitPrice = orderItem.UnitPrice,
                        //Balance = unReceivedQty,
                        //UOM = orderItem.UoM,
                        GLTaxRateID = orderItem.GLTaxRateID
                    };

                    if (tax != null)
                    {
                        receiveItem.TaxAmount = orderItem.UnitPrice * unReceivedQty * tax.CalculatedRate;
                        receiveItem.SubTotal = orderItem.UnitPrice * unReceivedQty;
                        receiveItem.Total = orderItem.UnitPrice * unReceivedQty +
                                            (orderItem.UnitPrice * unReceivedQty * tax.CalculatedRate);
                    }

                    _db.ServiceReceiveItems.Add(receiveItem);
                    _db.SaveChanges();
                }

                #endregion
            }

            else if (type.Type == GoodReceiveType.Tender)
            {
                #region get prev receive

                var previousReceiveIds = _db.ServiceReceives.Where(x => x.ReceiveType.Type == GoodReceiveType.Tender)
                    .Where(x => x.ReceiveRef == record.ReceiveRef).Where(x => x.IsOnlineApproved)
                    .Where(x => x.IsVoid == false).Select(x => x.ServiceReceiveId).ToList();

                var prevReceiveItems = _db.ServiceReceiveItems.Where(x => previousReceiveIds.Contains(x.ServiceReceiveId))
                    .ToList();

                #endregion
                #region tender
                int awardid = int.Parse(record.ReceiveRef);
                var awarditems = _db.BidAwardedSupplierItems.Where(x => x.ItemType == PurchaseItemType.Service).Where(x => x.BidAwardedSupplierID == awardid).ToList();
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

                    var prevReceiveItem = prevReceiveItems.Where(x => x.ServiceItemCode == awardItem.ItmCode).ToList();
                    if (prevReceiveItem.Any())
                        prevReceivedQty = prevReceiveItem.Sum(x => x.Quantity);

                    if (prevReceivedQty < awardItem.Quanity)
                        unReceivedQty = awardItem.Quanity - prevReceivedQty;

                    var tax = _db.GLTaxRates.Find(awardItem.GLTaxRateID);
                    var receiveItem = new ServiceReceiveItem
                    {
                        ServiceReceiveId = record.ServiceReceiveId,
                        ServiceCategoryCode = awardItem.CatCode,
                        ServiceItemCode = awardItem.ItmCode,
                        Quantity = unReceivedQty,
                        UnitPrice = unitprice,
                        //Balance = unReceivedQty,
                        //UOM = awardItem.UoM,
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
                    _db.ServiceReceiveItems.Add(receiveItem);
                    _db.SaveChanges();
                }

                #endregion

            }
            //_distributionHelper.UpdateGoodReceiveDistribution(record);

        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

    }
}
