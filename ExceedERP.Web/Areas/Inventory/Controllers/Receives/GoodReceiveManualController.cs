using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Areas.Inventory.Models;
using ExceedERP.Web.Helpers;
using Hangfire;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using EntityState = System.Data.Entity.EntityState;


namespace ExceedERP.Web.Areas.Inventory.Controllers.Receives
{



    public class GoodReceiveManualController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly DistributionHelper _distributionHelper = new DistributionHelper();

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult Index()
        {

            PopulateViewData();
            return View();

        }

        private void PopulateViewData()
        {
            var categories = db.ItemCategorys;
            var items = db.InventoryItems;
            var branches = db.Branch;
            var tax = db.GLTaxRates;
            var stores = db.InventoryStores;
            var receivetype = db.ReceiveTypes;

            var inventoryPeriods = db.InventoryPeriods;
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
                    Name = i.ItemCode + "_" + i.Name,
                    ItemCode = i.ItemCode
                }).ToList();

            ViewData["Items"] = selecteditems;
            ViewData["Category"] = categories;

            var employee = db.Person_Employee;
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
            IQueryable<GoodReceive> goodreceives = db.GoodReceives.Where(x=>x.IsManual).Where(x => userStores.Contains(x.StoreCode));

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

            ReceiveType type = db.ReceiveTypes.Find(goodReceive.ReceiveTypeId);
            if (type != null)
            {
                switch (type.Type)
                {
                    case GoodReceiveType.PurchaseOrder:
                        ModelState.AddModelError("auto", "you can not register auto receive with this form, please use auto receive form");
                        break;
                    case GoodReceiveType.Tender:
                        ModelState.AddModelError("auto", "you can not register auto receive with this form, please use auto receive form");
                        break;
                }
            }
            else
            {
                ModelState.AddModelError("type", "Receive Type is not found.");
            }

            if (String.IsNullOrWhiteSpace(goodReceive.SupplierName))
                ModelState.AddModelError("supplierid", "Please enter supplier name");

            if (!String.IsNullOrEmpty(goodReceive.Reference))
            {
                var exists = db.GoodReceives.Where(x => x.Reference == goodReceive.Reference && x.IsVoid == false).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("Reference", goodReceive.Reference + "  already exists in the system.");
                }
            }

            if (!String.IsNullOrEmpty(goodReceive.Reference))
            {
                var exists = db.GoodReceives.Where(x => x.Reference == goodReceive.Reference && x.IsVoid == false).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("Reference", $"{goodReceive.Reference}  already exists in the system.");
                }
            }

            if (goodReceive.GLPeriodId != 0)
            {
                var inventoryPeriod = db.InventoryPeriods.Find(goodReceive.GLPeriodId);
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
                    IsManual = true,
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

                db.GoodReceives.Add(entity);
                db.SaveChanges();
                goodReceive.GoodReceiveId = entity.GoodReceiveId;

                GetGoodReceiveItems(entity);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "manual good receive", "Success", User.Identity.Name, $"created good receive {entity.GoodReceiveId} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { goodReceive }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GoodReceives_Update([DataSourceRequest]DataSourceRequest request, GoodReceive goodReceive)
        {
            #region validation

            ReceiveType type = db.ReceiveTypes.Find(goodReceive.ReceiveTypeId);
            if (type != null)
            {
                switch (type.Type)
                {
                    case GoodReceiveType.PurchaseOrder:
                        ModelState.AddModelError("auto", "you can not register auto receive with this form, please use auto receive form");
                        break;
                    case GoodReceiveType.Tender:
                        ModelState.AddModelError("auto", "you can not register auto receive with this form, please use auto receive form");
                        break;
                }
            }
            else
            {
                ModelState.AddModelError("type", "Receive Type is not found.");
            }

            if (String.IsNullOrWhiteSpace(goodReceive.SupplierName))
                ModelState.AddModelError("supplierid", "Please enter supplier name");

            if (!String.IsNullOrEmpty(goodReceive.Reference))
            {
                var exists = db.GoodReceives.Where(x => x.GoodReceiveId != goodReceive.GoodReceiveId).Where(x => x.Reference == goodReceive.Reference && x.IsVoid == false).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("Reference", goodReceive.Reference + "  already exists in the system.");
                }
            }
            if (!String.IsNullOrEmpty(goodReceive.Reference))
            {
                var exists = db.GoodReceives.Where(x => x.GoodReceiveId != goodReceive.GoodReceiveId).Where(x => x.Reference == goodReceive.Reference && x.IsVoid == false).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("Reference", goodReceive.Reference + "  already exists in the system.");
                }
            }

            if (goodReceive.GLPeriodId != 0)
            {
                var inventoryPeriod = db.InventoryPeriods.Find(goodReceive.GLPeriodId);
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

                db.GoodReceives.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "manual good receive", "Success", User.Identity.Name, $"updated good receive {entity.GoodReceiveId} ", Modules.Inventory);
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

                db.GoodReceives.Attach(entity);
                db.GoodReceives.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted good receive number " + goodReceive.GoodReceiveId + " " + goodReceive.Reference);
                #endregion
            }

            return Json(new[] { goodReceive }.ToDataSourceResult(request, ModelState));
        }

        private void GetGoodReceiveItems(GoodReceive record)
        {

            ReceiveType type = record.ReceiveType;

            if (type.Type == GoodReceiveType.PurchaseOrder)
            {
                #region Purchase Order Items
                int pid = int.Parse(record.ReceiveRef);
                List<PurchaseOrderItem> purchaseOrderItems = db.PurchaseOrderItems
                    .Where(pi => pi.PurchaseOrderID == pid)
                    .Where(pi => pi.ItemType == PurchaseItemType.Stock)
                    .ToList();

                foreach (var purchaseOrderItem in purchaseOrderItems)
                {
                    var tax = db.GLTaxRates.Find(purchaseOrderItem.GLTaxRateID);

                    var receiveItem = new GoodReceiveItem
                    {
                        GoodReceiveId = record.GoodReceiveId,
                        ItemCategoryCode = purchaseOrderItem.CatCode,
                        ItemCode = purchaseOrderItem.ItmCode,
                        Quantity = purchaseOrderItem.Quantity,
                        UnitPrice = purchaseOrderItem.UnitPrice,
                        Balance = purchaseOrderItem.Quantity,
                        GLTaxRateID = purchaseOrderItem.GLTaxRateID,
                        TaxAmount = purchaseOrderItem.TaxAmount,
                        SubTotal = purchaseOrderItem.SubTotal,
                        Total = purchaseOrderItem.Total
                    };

                    if (tax != null)
                    {
                        receiveItem.TaxAmount = receiveItem.UnitPrice * receiveItem.Quantity * tax.CalculatedRate;
                        receiveItem.SubTotal = receiveItem.UnitPrice * receiveItem.Quantity;
                        receiveItem.Total = receiveItem.UnitPrice * receiveItem.Quantity +(receiveItem.UnitPrice * receiveItem.Quantity * tax.CalculatedRate);
                    }

                    db.GoodReceiveItems.Add(receiveItem);
                    db.SaveChanges();

                }

                #endregion
            }

            else if (type.Type == GoodReceiveType.Tender)
            {
                #region tender
                int awardid = int.Parse(record.ReceiveRef);
                var awarditems = db.BidAwardedSupplierItems.Where(x => x.ItemType == PurchaseItemType.Stock).Where(x => x.BidAwardedSupplierID == awardid).ToList();
                foreach (var item in awarditems)
                {
                    var tax = db.GLTaxRates.Find(item.GLTaxRateID);
                    var receiveItem = new GoodReceiveItem
                    {
                        GoodReceiveId = record.GoodReceiveId,
                        ItemCategoryCode = item.CatCode,
                        ItemCode = item.ItmCode,
                        Quantity = item.Quanity,
                        UnitPrice = item.FinalUPrice,
                        Balance = item.Quanity,
                        GLTaxRateID = item.GLTaxRateID,
                        TaxAmount = item.TaxAmount,

                        ModifiedBy = User.Identity.Name,
                        LastModified = DateTime.Today,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Today
                    };
                    if (tax != null)
                    {
                        receiveItem.TaxAmount = receiveItem.UnitPrice * receiveItem.Quantity * tax.CalculatedRate;
                        receiveItem.SubTotal = receiveItem.UnitPrice * receiveItem.Quantity;
                        receiveItem.Total = receiveItem.UnitPrice * receiveItem.Quantity + (receiveItem.UnitPrice * receiveItem.Quantity * tax.CalculatedRate);
                    }
                    db.GoodReceiveItems.Add(receiveItem);
                    db.SaveChanges();
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
            Core.Domain.Inventory.Issue issuee = db.Issues.FirstOrDefault();
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
                var inventoryItem = db.InventoryItems.Find(goodReciveImport.ItemCode);
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
                        ReceiveType type = db.ReceiveTypes.Find(imp.ReceiveType);
                        if (type != null)
                        {
                            switch (type.Type)
                            {
                                case GoodReceiveType.PurchaseOrder:
                                    int pid = int.Parse(imp.ReferenceId);
                                    PurchaseOrder purchaseOrder = db.PurchaseOrders.Find(pid);
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

                    

                }

            }
            return Json(new[] { issuee }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        public JsonResult SendForApproval(int id)
        {
            var record = db.GoodReceives.Find(id);
            var items = db.GoodReceiveItems.Where(x => x.GoodReceiveId == id).ToList();
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
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.GoodReceives.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "good receive manual", "Success", User.Identity.Name, $"send for approval good receive {id} ", Modules.Inventory);
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
            var record = db.GoodReceives.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred && record.GoodReceiveItem.Count > 0)
            {
                bool valid = true;
                #region validate

                foreach (var i in record.GoodReceiveItem)
                {
                    #region stock

                    var storeItem = db.StoreItemAssignments.Where(x => x.ItemCode == i.ItemCode).FirstOrDefault(x => x.StoreCode == record.StoreCode);
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
                    db.GoodReceives.Attach(record);
                    db.Entry(record).State = EntityState.Modified;
                   // db.SaveChanges();

                    #region update stock

                    string storecode = record.StoreCode;

                    foreach (GoodReceiveItem i in record.GoodReceiveItem)
                    {
                        string itemCode = i.ItemCode;

                        InventoryItem stockItem = db.InventoryItems.Find(itemCode);
                        int storeBranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(storecode);

                        #region stock

                        BranchItemAssignment branchItem =
                            db.BranchItemAssignments.Where(x => x.BranchId == storeBranchId)
                                .FirstOrDefault(x => x.ItemCode == i.ItemCode);
                        StoreItemAssignment storeItem =
                            db.StoreItemAssignments.FirstOrDefault(
                                x => x.StoreCode == storecode && x.ItemCode == itemCode);

                        if (storeItem != null)
                        {
                            storeItem.Balance = storeItem.Balance - i.Quantity;
                            db.Entry(storeItem).State = EntityState.Modified;
                            // db.SaveChanges();

                            #region update bin card

                            #region bin card line

                            BinCardLine binCardLine = new BinCardLine
                            {
                                StoreItemAssignment = storeItem,
                                ReferenceDoc = "Void Receive " + record.GoodReceiveId,
                                Reference = !String.IsNullOrEmpty(record.ReceiveRef) ? record.ReceiveRef + " " + record.Reference : record.Reference,
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
                            db.BinCardLines.Add(binCardLine);
                            //db.SaveChanges();

                            #endregion

                            #endregion
                            var jobId = BackgroundJob.Enqueue(() => InventoryBalanceHelper.UpdateSigleItemBinCardTransactions(storeItem.StoreItemAssignmentID, record.ReceiveDate));
                        }
                        if (branchItem != null)
                        {
                            branchItem.Balance = branchItem.Balance - i.Quantity;
                            db.Entry(branchItem).State = EntityState.Modified;
                            //db.SaveChanges();

                        }
                        if (stockItem != null)
                        {
                            stockItem.Balance = stockItem.Balance - i.Quantity;
                            db.Entry(stockItem).State = EntityState.Modified;
                            //db.SaveChanges();
                        }



                        #endregion

                    }

                    db.SaveChanges();
                    #endregion

                    //copy record
                    CopyRecord(id);

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "good receive manual", "Success", User.Identity.Name, $"void good receive {id} ", Modules.Inventory);
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
            var record = db.GoodReceives.Find(id);
            if (record!=null)
            {
                #region header
                 var entity = new GoodReceive
                    {
                        BranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(record.StoreCode),
                        ReceiveRef = record.ReceiveRef,
                        IsManual=record.IsManual,
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

                db.GoodReceives.Add(entity);
                db.SaveChanges();
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
                    db.GoodReceiveItems.Add(line);
                    db.SaveChanges();
                 
                    #endregion

                }
               
            }
           
        }

        [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
        public JsonResult TransferDistribution(string id, string description)
        {
            int myid = int.Parse(id);
            GoodReceive record = db.GoodReceives.Find(myid);

            if (record != null && record.IsOnlineApproved && !record.IsOnlineTransferred)
            {
                #region check segment length validation
                AccountHelper accountHelper = new AccountHelper();
                foreach (var dist in record.GoodReceiveDistribution)
                {
                    string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                    if (!String.IsNullOrEmpty(message))
                    {
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                }


                #endregion
                #region check if debit and credit are equal
                decimal debit = 0;
                decimal credit = 0;
                var distribution = record.GoodReceiveDistribution.ToList();
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

                var period = db.InventoryPeriods.Find(record.GLPeriodId);


                if (period != null)
                {
                    var glperiodObj = db.GLPeriods.FirstOrDefault(x => x.Name == period.Name);

                    if (glperiodObj != null)
                    {
                        glperiod = glperiodObj.Name;
                        glyear = glperiodObj.GlFiscalYear.Name;
                        if (glperiodObj.Status == FiscalYearStatus.Close)
                        {
                            var formattedData = new
                            {
                                Message = $"The period { glperiodObj.Name } is closed,the record cannot be transfered to closed period.",
                                Status = "NOT_OK"
                            };
                            return Json(formattedData, JsonRequestBehavior.AllowGet);
                        }
                    }

                    else
                    {
                        var formattedData = new
                        {
                            Message = $"The corresponding GL period {period.Name} is not found.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);

                    }
                }
                else
                {
                    var formattedData = new
                    {
                        Message = $"The period is not found in inventory period.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                #endregion
                if (record.GoodReceiveDistribution.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        JournalName = "GoodReceive",
                        Description = !String.IsNullOrEmpty(description) ? description: "GoodReceive",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "GoodReceive",
                        SourcesName = "GR",
                        JournalNumber = "GoodRecive." + record.GoodReceiveId,
                        EffectiveDate = record.ReceiveDate.Date,
                        Status = "Approved",
                        IsOnlineApproved = true,
                        OnlineApprovedBy = User.Identity.Name,
                        OnlineApprovedTime = DateTime.Now,
                        OrgBranchName=record.OrgBranchName
                    };
                    db.GLRecordJournals.Add(journal);
                    db.SaveChanges();
                    foreach (GoodReceiveDistribution i in record.GoodReceiveDistribution)
                    {
                        #region debit and credit

                        var journalDetail = new GLRecordJournalDetail
                        {
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            GLRecordJournal = journal,
                            AccountCode = i.AccountCode,
                            AccountDesc = i.AccountDesc,
                            AdditionalDescription = i.AdditionalDescription,
                            Debit = i.Debit,
                            Credit = i.Credit
                        };
                        db.GLRecordJournalDetails.Add(journalDetail);
                        db.SaveChanges();
                        #endregion
                    }
                    record.IsOnlineTransferred = true;
                    record.IsOnlineTransferredBy = User.Identity.Name;
                    record.OnlineTransferredTime = DateTime.Now;
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
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

        [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
        public JsonResult GenerateDistribution(int id)
        {

            var record = db.GoodReceives.Find(id);
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
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
