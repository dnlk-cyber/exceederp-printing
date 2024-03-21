using System;
using System.Collections.Generic;

using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender;
using ExceedERP.Core.Domain.Procurement.Tender.Lot;
using ExceedERP.DataAccess.Context;
using ExceedERP.DataAccess.Migrations;
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
        private InventoryTransferHelper _transferHelper;

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
                        BranchId = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name),
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

         [AuthorizeRoles(ProcurementRoles.ProcurementVoid)]
        public JsonResult Void(int id)
        {
            var record = _db.ServiceReceives.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred )
            {
                    record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                    record.IsVoid = true;
                    record.VoidBy = User.Identity.Name;
                    record.VoidTime = DateTime.Now;
                    _db.ServiceReceives.Attach(record);
                    _db.Entry(record).State = EntityState.Modified;
                    _db.SaveChanges();

                   

                    #region update  po status
                    var receiveType = _db.ReceiveTypes.Where(x => x.ReceiveTypeId == record.ReceiveTypeId).FirstOrDefault();
                    if (receiveType != null)
                    {
                        if (receiveType.Type == GoodReceiveType.PurchaseOrder)
                        {
                            int did = int.Parse(record.ReceiveRef);
                            var po = _db.PurchaseOrders.Find(did);
                            if (po != null)
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
                    UserHelper.OperationLog(Request.UserHostAddress, "service receive", "Success", User.Identity.Name, $"void service receive {id} ", Modules.Procurement);
                    #endregion

                    var formattedData = new
                    {
                        Message = "Void Successful.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                
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
            var record = _db.ServiceReceives.Find(id);
            if (record != null)
            {
                #region header
                var entity = new ServiceReceive
                {
                    BranchId = record.BranchId,
                    BusinessUnitId = record.BusinessUnitId,
                    ServiceReceiveId = record.ServiceReceiveId,
                    ReceiveRef = record.ReceiveRef,
                    SupplierName = record.SupplierName,
                    SupplierInvoiceNo = record.SupplierInvoiceNo,
                    Reference = record.Reference,
                    ReceiveDate = record.ReceiveDate,
                    Status = record.Status,
                    ReceiveTypeId = record.ReceiveTypeId,
                    Remark = record.Remark,
                    ItemType = PurchaseItemType.Service,
                    GLPeriodId = record.GLPeriodId,
                    IsVoid = record.IsVoid,
                    VoidBy = record.VoidBy,
                    VoidTime = record.VoidTime,
                    IsOnlineApproved = record.IsOnlineApproved,
                    OnlineApprovedBy = record.OnlineApprovedBy,
                    OnlineApprovedTime = record.OnlineApprovedTime,
                    IsOnlineTransferred = record.IsOnlineTransferred,
                    IsOnlineTransferredBy = record.IsOnlineTransferredBy,
                    OnlineTransferredTime = record.OnlineTransferredTime,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                };

                _db.ServiceReceives.Add(entity);
                _db.SaveChanges();
                #endregion
                foreach (var i in record.ServiceReceiveItems)
                {
                    #region lines
                    var line = new ServiceReceiveItem
                    {
                        ServiceReceiveId = entity.ServiceReceiveId,
                        ServiceReceiveItemId = i.ServiceReceiveItemId,
                        ServiceCategoryCode = i.ServiceCategoryCode,
                        ServiceCategoryName = i.ServiceCategoryName,
                        ServiceItemCode = i.ServiceItemCode,
                        ServiceItemName = i.ServiceItemName,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        SubTotal = i.SubTotal,
                        Total = i.Total,
                        GLTaxRateID = i.GLTaxRateID,
                        TaxAmount = i.TaxAmount,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                    };
                    _db.ServiceReceiveItems.Add(line);
                    _db.SaveChanges();

                    #endregion

                }

            }

        }

        [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
        public JsonResult TransferDistribution(string id, string description)
        {
            int myid = int.Parse(id);
            var record = _db.ServiceReceives.Find(myid);

            if (record != null && record.IsOnlineApproved && !record.IsOnlineTransferred)
            {
                _transferHelper = new InventoryTransferHelper();
                string response = _transferHelper.ServiceReceive(myid, description, User?.Identity?.Name ?? "");
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

            var record = _db.ServiceReceives.Find(id);
            if (record != null && !record.IsOnlineTransferred)
            {
                _distributionHelper.UpdateServiceReceiveDistribution(record);
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

                var previousReceiveIds = _db.ServiceReceives
                    .Where(x => x.ReceiveType.Type == GoodReceiveType.PurchaseOrder)
                    .Where(x => x.ReceiveRef == record.ReceiveRef).Where(x => x.IsOnlineApproved)
                    .Where(x => x.IsVoid == false).Select(x => x.ServiceReceiveId).ToList();

                var prevReceiveItems = _db.ServiceReceiveItems
                    .Where(x => previousReceiveIds.Contains(x.ServiceReceiveId))
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
                        UnitPrice = orderItem.UnitPrice,
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
