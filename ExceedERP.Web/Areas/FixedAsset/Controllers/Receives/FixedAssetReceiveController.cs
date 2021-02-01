using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Receives;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender.Lot;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.FixedAsset;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Areas.Inventory.Models;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using EntityState = System.Data.Entity.EntityState;


namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Receives
{


    public class FixedAssetReceiveController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        private readonly FixedAssetDistributionHelper _distributionHelper = new FixedAssetDistributionHelper();
        private FixedAssetTransferHelper _transferHelper;

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            
            var branches = _db.Branch;
            var tax = _db.GLTaxRates;
            var stores = _db.InventoryStores;
            var receivetype = _db.ReceiveTypes;
            var conditions = _db.FixedAssetConditions;

            var fixedAssetCategories = _db.FixedAssetCategories;
            ViewData["fixedAssetCategories"] = fixedAssetCategories.ToList();

            var fixedAssetSubCategories = _db.FixedAssetSubCategories;
            ViewData["fixedAssetSubCategories"] = fixedAssetSubCategories.ToList();

            ViewData["Conditions"] = conditions;
            ViewData["stores"] = stores;
            ViewData["tax"] = tax;
            ViewData["ReeceiveType"] = receivetype;
            ViewData["Branches"] = branches;

            var employee = _db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;

        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult FixedAssetReceives_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> userStores = UserInformationHelper.GetUserStores(User.Identity.Name);
            IQueryable<FixedAssetReceive> fixedAssetReceives = _db.FixedAssetReceives.Where(x => userStores.Contains(x.StoreCode));
           ;
            DataSourceResult result = fixedAssetReceives.ToDataSourceResult(request, fixedAssetReceive => new
            {
                IsManual = fixedAssetReceive.IsManual,
                ReceiveRef = fixedAssetReceive.ReceiveRef,
                BranchId = fixedAssetReceive.BranchId,
                ReceiveTypeId = fixedAssetReceive.ReceiveTypeId,
                FixedAssetReceiveId = fixedAssetReceive.FixedAssetReceiveId,
                TenderLot = fixedAssetReceive.TenderLot,
                StoreCode = fixedAssetReceive.StoreCode,
                SupplierName = fixedAssetReceive.SupplierName,
                SupplierId = fixedAssetReceive.SupplierId,
                SupplierInvoiceNo = fixedAssetReceive.SupplierInvoiceNo,
                GLPeriodId = fixedAssetReceive.GLPeriodId,
                Reference = fixedAssetReceive.Reference,
                ReceiveDate = fixedAssetReceive.ReceiveDate,
                DateCreated = fixedAssetReceive.DateCreated,
                CreatedBy = fixedAssetReceive.CreatedBy,
                Status = fixedAssetReceive.Status,
                Remark = fixedAssetReceive.Remark,
                IsVoid = fixedAssetReceive.IsVoid,
                VoidBy = fixedAssetReceive.VoidBy,
                VoidTime = fixedAssetReceive.VoidTime,

                IsSendForApproval = fixedAssetReceive.IsSendForApproval,
                SendForApprovalBy = fixedAssetReceive.SendForApprovalBy,
                SendForApprovalTime = fixedAssetReceive.SendForApprovalTime,

                IsOnlineApproved = fixedAssetReceive.IsOnlineApproved,
                OnlineApprovedBy = fixedAssetReceive.OnlineApprovedBy,
                OnlineApprovedTime = fixedAssetReceive.OnlineApprovedTime,
                IsOnlineTransferred = fixedAssetReceive.IsOnlineTransferred,
                IsOnlineTransferredBy = fixedAssetReceive.IsOnlineTransferredBy,
                OnlineTransferredTime = fixedAssetReceive.OnlineTransferredTime,
            });

            return Json(result);

        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult FixedAssetReceives_Filtered([DataSourceRequest]DataSourceRequest request, int id, int index)
        {
            IQueryable<FixedAssetReceive> fixedAssetReceives = _db.FixedAssetReceives.Where(go => go.FixedAssetReceiveId == id);
            DataSourceResult result = null;
            switch (index)
            {
                case 0:
                    result = fixedAssetReceives.ToDataSourceResult(request, fixedAssetReceive => new
                    {
                        FixedAssetReceiveId = fixedAssetReceive.FixedAssetReceiveId,
                        BranchId = fixedAssetReceive.BranchId,
                        ReceiveTypeId = fixedAssetReceive.ReceiveTypeId,
                        StoreCode = fixedAssetReceive.StoreCode,
                        SupplierName = fixedAssetReceive.SupplierName,
                        SupplierId = fixedAssetReceive.SupplierId,
                        SupplierInvoiceNo = fixedAssetReceive.SupplierInvoiceNo,
                        GLPeriodId = fixedAssetReceive.GLPeriodId,
                        Reference = fixedAssetReceive.Reference,
                        ReceiveDate = fixedAssetReceive.ReceiveDate,
                        Status = fixedAssetReceive.Status,
                        Remark = fixedAssetReceive.Remark,
                        IsVoid = fixedAssetReceive.IsVoid,
                        VoidBy = fixedAssetReceive.VoidBy,
                        VoidTime = fixedAssetReceive.VoidTime,
                        IsOnlineApproved = fixedAssetReceive.IsOnlineApproved,
                        OnlineApprovedBy = fixedAssetReceive.OnlineApprovedBy,
                        OnlineApprovedTime = fixedAssetReceive.OnlineApprovedTime,
                        IsOnlineTransferred = fixedAssetReceive.IsOnlineTransferred,
                        IsOnlineTransferredBy = fixedAssetReceive.IsOnlineTransferredBy,
                        OnlineTransferredTime = fixedAssetReceive.OnlineTransferredTime,
                    });
                    break;

                case 1:

                    break;

            }



            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetReceives_Create([DataSourceRequest]DataSourceRequest request, FixedAssetReceive fixedAssetReceive)
        {
            #region validation

            ReceiveType type = _db.ReceiveTypes.Find(fixedAssetReceive.ReceiveTypeId);
            if (type != null)
            {
                switch (type.Type)
                {
                    case GoodReceiveType.PurchaseOrder:
                        int pid = int.Parse(fixedAssetReceive.ReceiveRef);
                        PurchaseOrder purchaseOrder = _db.PurchaseOrders.Find(pid);
                        if (purchaseOrder != null)
                        {
                            fixedAssetReceive.SupplierId = purchaseOrder.SupplierId;
                            fixedAssetReceive.SupplierName = purchaseOrder.SupplierName;

                            if (String.IsNullOrWhiteSpace(purchaseOrder.SupplierName))
                                ModelState.AddModelError("supplierid", "Supplier is not found in selected purchase order");
                        }
                        break;
                    case GoodReceiveType.Tender:
                        int awardid = int.Parse(fixedAssetReceive.ReceiveRef);
                        BidAwardedSupplier award = _db.BidAwardedSuppliers.Find(awardid);
                        if (award != null)
                        {
                            fixedAssetReceive.SupplierId = award.SupplierId;
                            fixedAssetReceive.SupplierName = award.SupplierName;

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

            //if (!String.IsNullOrEmpty(fixedAssetReceive.ReceiveRef))
            //{
            //    var exists = db.FixedAssetReceives.Where(x => x.ReceiveRef == fixedAssetReceive.ReceiveRef && x.IsVoid == false).ToList();

            //    if (exists.Any())
            //    {
            //        ModelState.AddModelError("Reference", fixedAssetReceive.ReceiveRef + "  already exists in the system.");
            //    }
            //}
            //to allow multiple receive

            //if (!String.IsNullOrEmpty(fixedAssetReceive.Reference))
            //{
            //    var exists = db.GoodReceives.Where(x => x.Reference == fixedAssetReceive.Reference && x.IsVoid == false).ToList();

            //    if (exists.Any())
            //    {
            //        ModelState.AddModelError("Reference", $"{fixedAssetReceive.Reference}  already exists in the system.");
            //    }
            //}

            if (fixedAssetReceive.GLPeriodId != 0)
            {
                var fixedAssetPeriod = _db.FixedAssetPeriods.Find(fixedAssetReceive.GLPeriodId);
                if (fixedAssetPeriod != null)
                {
                    if (
                        !((fixedAssetReceive.ReceiveDate.Date >= fixedAssetPeriod.DateFrom) &&
                          (fixedAssetReceive.ReceiveDate.Date <= fixedAssetPeriod.DateTo)))
                        ModelState.AddModelError("",
                            $"Receive Date should be within the period: " + fixedAssetPeriod.Name + "(From " +
                            fixedAssetPeriod.DateFrom.ToShortDateString() + " To " +
                            fixedAssetPeriod.DateTo.ToShortDateString() + ")");

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

            if (fixedAssetReceive.ReceiveDate.Date > DateTime.Now)
            {
                ModelState.AddModelError("future", "You are recording in future period,please correct the date.");
            }

            #endregion



            if (ModelState.IsValid)
            {

                var entity = new FixedAssetReceive
                    {
                        IsManual = fixedAssetReceive.IsManual,
                        BranchId = InventoryHelper.GetStoreBranch(fixedAssetReceive.StoreCode),
                        ReceiveRef = fixedAssetReceive.ReceiveRef,
                        ReceiveTypeId = fixedAssetReceive.ReceiveTypeId,
                        StoreCode = fixedAssetReceive.StoreCode,
                        SupplierName = fixedAssetReceive.SupplierName,
                        SupplierId = fixedAssetReceive.SupplierId,
                        SupplierInvoiceNo = fixedAssetReceive.SupplierInvoiceNo,
                        GLPeriodId = fixedAssetReceive.GLPeriodId,
                        TenderLot = fixedAssetReceive.TenderLot,
                        Reference = fixedAssetReceive.Reference,
                        ReceiveDate = fixedAssetReceive.ReceiveDate,
                        Status = Enum.GetName(typeof(OperationStatuses), 5),
                        Remark = fixedAssetReceive.Remark,
                        IsVoid = fixedAssetReceive.IsVoid,
                        VoidBy = fixedAssetReceive.VoidBy,
                        VoidTime = fixedAssetReceive.VoidTime,
                        IsOnlineApproved = fixedAssetReceive.IsOnlineApproved,
                        OnlineApprovedBy = fixedAssetReceive.OnlineApprovedBy,
                        OnlineApprovedTime = fixedAssetReceive.OnlineApprovedTime,
                        IsOnlineTransferred = fixedAssetReceive.IsOnlineTransferred,
                        IsOnlineTransferredBy = fixedAssetReceive.IsOnlineTransferredBy,
                        OnlineTransferredTime = fixedAssetReceive.OnlineTransferredTime,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Today.Date,
                        ModifiedBy = User.Identity.Name,
                        LastModified = DateTime.Today,
                    };

                _db.FixedAssetReceives.Add(entity);
                _db.SaveChanges();
                fixedAssetReceive.FixedAssetReceiveId = entity.FixedAssetReceiveId;

                GetGoodReceiveItems(entity);
            }

            return Json(new[] { fixedAssetReceive }.ToDataSourceResult(request, ModelState));
        }

       [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetReceives_Update([DataSourceRequest]DataSourceRequest request, FixedAssetReceive fixedAssetReceive)
        {
            #region validation

            ReceiveType type = _db.ReceiveTypes.Find(fixedAssetReceive.ReceiveTypeId);
            if (type != null)
            {
                switch (type.Type)
                {
                    case GoodReceiveType.PurchaseOrder:
                        int pid = int.Parse(fixedAssetReceive.ReceiveRef);
                        PurchaseOrder purchaseOrder = _db.PurchaseOrders.Find(pid);
                        if (purchaseOrder != null)
                        {
                            fixedAssetReceive.SupplierId = purchaseOrder.SupplierId;
                            fixedAssetReceive.SupplierName = purchaseOrder.SupplierName;

                            if (String.IsNullOrWhiteSpace(purchaseOrder.SupplierName))
                                ModelState.AddModelError("supplierid", "Supplier is not found in selected purchase order");
                        }
                        break;
                    case GoodReceiveType.Tender:
                        int awardid = int.Parse(fixedAssetReceive.ReceiveRef);
                        BidAwardedSupplier award = _db.BidAwardedSuppliers.Find(awardid);
                        if (award != null)
                        {
                            fixedAssetReceive.SupplierId = award.SupplierId;
                            fixedAssetReceive.SupplierName = award.SupplierName;

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

            //if (!String.IsNullOrEmpty(fixedAssetReceive.ReceiveRef))
            //{
            //    var exists = db.FixedAssetReceives.Where(x => x.FixedAssetReceiveId != fixedAssetReceive.FixedAssetReceiveId).Where(x => x.ReceiveRef == fixedAssetReceive.ReceiveRef && x.IsVoid == false).ToList();

            //    if (exists.Any())
            //    {
            //        ModelState.AddModelError("Reference", fixedAssetReceive.ReceiveRef + "  already exists in the system.");
            //    }
            //}
            //to allow multiple receive
            //if (!String.IsNullOrEmpty(fixedAssetReceive.Reference))
            //{
            //    var exists = db.FixedAssetReceives.Where(x => x.FixedAssetReceiveId != fixedAssetReceive.FixedAssetReceiveId).Where(x => x.Reference == fixedAssetReceive.Reference && x.IsVoid == false).ToList();

            //    if (exists.Any())
            //    {
            //        ModelState.AddModelError("Reference", fixedAssetReceive.Reference + "  already exists in the system.");
            //    }
            //}

            if (fixedAssetReceive.GLPeriodId != 0)
            {
                var fixedAssetPeriod = _db.FixedAssetPeriods.Find(fixedAssetReceive.GLPeriodId);
                if (fixedAssetPeriod != null)
                {
                    if (
                        !((fixedAssetReceive.ReceiveDate.Date >= fixedAssetPeriod.DateFrom) &&
                          (fixedAssetReceive.ReceiveDate.Date <= fixedAssetPeriod.DateTo)))
                        ModelState.AddModelError("",
                            "Receive Date should be within the period: " + fixedAssetPeriod.Name + "(From " +
                            fixedAssetPeriod.DateFrom.ToShortDateString() + " To " +
                            fixedAssetPeriod.DateTo.ToShortDateString() + ")");

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

            if (fixedAssetReceive.ReceiveDate > DateTime.Today)
            {
                ModelState.AddModelError("future", "You are recording in future period,please correct the date.");
            }

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetReceive
                {
                    IsManual = fixedAssetReceive.IsManual,
                    BranchId = InventoryHelper.GetStoreBranch(fixedAssetReceive.StoreCode),
                    FixedAssetReceiveId = fixedAssetReceive.FixedAssetReceiveId,
                    ReceiveRef = fixedAssetReceive.ReceiveRef,
                    StoreCode = fixedAssetReceive.StoreCode,
                    SupplierName = fixedAssetReceive.SupplierName,
                    SupplierId = fixedAssetReceive.SupplierId,
                    SupplierInvoiceNo = fixedAssetReceive.SupplierInvoiceNo,
                    GLPeriodId = fixedAssetReceive.GLPeriodId,
                    Reference = fixedAssetReceive.Reference,
                    ReceiveDate = fixedAssetReceive.ReceiveDate,
                    Status = fixedAssetReceive.Status,
                    ReceiveTypeId = fixedAssetReceive.ReceiveTypeId,
                    Remark = fixedAssetReceive.Remark,
                    IsVoid = fixedAssetReceive.IsVoid,
                    VoidBy = fixedAssetReceive.VoidBy,
                    VoidTime = fixedAssetReceive.VoidTime,
                    IsOnlineApproved = fixedAssetReceive.IsOnlineApproved,
                    OnlineApprovedBy = fixedAssetReceive.OnlineApprovedBy,
                    OnlineApprovedTime = fixedAssetReceive.OnlineApprovedTime,
                    IsOnlineTransferred = fixedAssetReceive.IsOnlineTransferred,
                    IsOnlineTransferredBy = fixedAssetReceive.IsOnlineTransferredBy,
                    OnlineTransferredTime = fixedAssetReceive.OnlineTransferredTime,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                };

                _db.FixedAssetReceives.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                GetGoodReceiveItems(entity);
            }

            return Json(new[] { fixedAssetReceive }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetReceives_Destroy([DataSourceRequest]DataSourceRequest request, FixedAssetReceive fixedAssetReceive)
        {

            ModelState.Remove("PurchaseOrderID");

            if (ModelState.IsValid)
            {
                var entity = new FixedAssetReceive
                {
                    IsManual = fixedAssetReceive.IsManual,
                    BranchId = fixedAssetReceive.BranchId,
                    FixedAssetReceiveId = fixedAssetReceive.FixedAssetReceiveId,
                    StoreCode = fixedAssetReceive.StoreCode,
                    SupplierName = fixedAssetReceive.SupplierName,
                    SupplierId = fixedAssetReceive.SupplierId,
                    SupplierInvoiceNo = fixedAssetReceive.SupplierInvoiceNo,
                    GLPeriodId = fixedAssetReceive.GLPeriodId,
                    Reference = fixedAssetReceive.Reference,
                    ReceiveDate = fixedAssetReceive.ReceiveDate,
                    Status = fixedAssetReceive.Status,
                    Remark = fixedAssetReceive.Remark,
                    IsVoid = fixedAssetReceive.IsVoid,
                    ReceiveTypeId = fixedAssetReceive.ReceiveTypeId,
                    VoidBy = fixedAssetReceive.VoidBy,
                    VoidTime = fixedAssetReceive.VoidTime,
                    IsOnlineApproved = fixedAssetReceive.IsOnlineApproved,
                    OnlineApprovedBy = fixedAssetReceive.OnlineApprovedBy,
                    OnlineApprovedTime = fixedAssetReceive.OnlineApprovedTime,
                    IsOnlineTransferred = fixedAssetReceive.IsOnlineTransferred,
                    IsOnlineTransferredBy = fixedAssetReceive.IsOnlineTransferredBy,
                    OnlineTransferredTime = fixedAssetReceive.OnlineTransferredTime,
                };

                _db.FixedAssetReceives.Attach(entity);
                _db.FixedAssetReceives.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { fixedAssetReceive }.ToDataSourceResult(request, ModelState));
        }
      

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        public ActionResult ReceiveImport([DataSourceRequest]DataSourceRequest request, IEnumerable<HttpPostedFileBase> file)
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

                            FixedAssetReceive fixedAssetReceive = new FixedAssetReceive
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
                            //fixedAssetReceive.IssueItems = issueItems;
                            //fixedAssetReceive.CreatedBy = User.Identity.Name;
                            //fixedAssetReceive.DateCreated = DateTime.Now;
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

                    //foreach (var line in fixedAssetReceive.IssueItems)
                    //{
                    //    var Item = db.InventoryItems.FirstOrDefault(it => it.ItemCode == line.ItemCode);
                    //    ItemCategory itemCategory = db.ItemCategorys.Find(Item.ItemCategoryCode);

                    //    string acc = itemCategory.Description;
                    //    string[] account = acc.Split('-');
                    //    string newacc = account[0] + "-" + account[1] + "-" + fixedAssetReceive.StoreRequisition.Department + "-" + account[3];


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

                    //if (fixedAssetReceive.IssueItems.Sum(x => x.Total) != 0)
                    //{
                    //    foreach (IssueItem items in fixedAssetReceive.IssueItems)
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

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        public JsonResult SendForApproval(int id)
        {
            var record = _db.FixedAssetReceives.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.FixedAssetReceiveItem.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.FixedAssetReceives.Attach(record);
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

            else if (record.FixedAssetReceiveItem.Count == 0)
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

        [AuthorizeRoles(FixedAssetConstants.FixedAssetVoid)]
        public JsonResult Void(int id)
        {
            var record = _db.FixedAssetReceives.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred && record.FixedAssetReceiveItem.Count > 0)
            {
                #region remove assets

                var assetRegistrations = _db.AssetRegistrations.Where(x => x.GRN == record.FixedAssetReceiveId.ToString())
                    .ToList();

                _db.AssetRegistrations.RemoveRange(assetRegistrations);
                _db.SaveChanges();


                #endregion
                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                //record.IsVoid = true;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.FixedAssetReceives.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
               

                //copy
                //CopyRecord(id);

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

            if (record.FixedAssetReceiveItem.Count == 0)
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

       
        private void GetGoodReceiveItems(FixedAssetReceive record)
        {
            #region remove previous receive

            var receiveItems = _db.FixedAssetReceiveItems.Where(x => x.FixedAssetReceiveId == record.FixedAssetReceiveId).ToList();
            _db.FixedAssetReceiveItems.RemoveRange(receiveItems);
            _db.SaveChanges();

            #endregion

            int prevReceivedQty = 0;
            int unReceivedQty = 0;
            var defaultCondition = _db.FixedAssetConditions.FirstOrDefault();
            ReceiveType type = record.ReceiveType;

            if (type.Type == GoodReceiveType.PurchaseOrder)
            {
                #region get prev receive

                var previousReceiveIds = _db.FixedAssetReceives.Where(x => x.ReceiveType.Type == GoodReceiveType.PurchaseOrder)
                    .Where(x => x.ReceiveRef == record.ReceiveRef).Where(x => x.IsOnlineApproved)
                    .Where(x => x.IsVoid == false).Select(x => x.FixedAssetReceiveId).ToList();

                var prevReceiveItems = _db.FixedAssetReceiveItems.Where(x => previousReceiveIds.Contains(x.FixedAssetReceiveId))
                    .ToList();

                #endregion
                #region Purchase Order Items
                int pid = int.Parse(record.ReceiveRef);
                List<PurchaseOrderItem> purchaseOrderItems = _db.PurchaseOrderItems
                    .Where(pi => pi.PurchaseOrderID == pid)
                    .Where(pi => pi.ItemType == PurchaseItemType.FixedAsset)
                    .ToList();

                foreach (var orderItem in purchaseOrderItems)
                {
                    var prevReceiveItem = prevReceiveItems
                        .Where(x => x.FixedAssetSubCategoryID.ToString() == orderItem.ItmCode)
                        .Where(x=>x.Name==orderItem.Name).ToList();

                    if (prevReceiveItem.Any())
                        prevReceivedQty = prevReceiveItem.Sum(x => x.Quantity);

                    if (prevReceivedQty < orderItem.Quantity)
                        unReceivedQty = (int)(orderItem.Quantity - prevReceivedQty);

                    var tax = _db.GLTaxRates.Find(orderItem.GLTaxRateID);

                    int subCatId = int.Parse(orderItem.ItmCode);
                    var subCat = _db.FixedAssetSubCategories.Find(subCatId);

                    if (subCat != null)
                    {
                        if (subCat.IsGroup)
                        {
                            #region by group

                            var receiveItem = new FixedAssetReceiveItem
                            {
                                FixedAssetReceiveId = record.FixedAssetReceiveId,
                                FixedAssetCategoryID = int.Parse(orderItem.CatCode),
                                FixedAssetSubCategoryID = int.Parse(orderItem.ItmCode),
                                IsGroup = true,
                                Quantity = unReceivedQty,
                                UnitPrice = orderItem.UnitPrice,
                                Balance = unReceivedQty,
                                GLTaxRateID = orderItem.GLTaxRateID,
                                Name = !string.IsNullOrEmpty(orderItem.Name) ? orderItem.Name : "NA",
                                FixedAssetConditionId = (int)defaultCondition?.FixedAssetConditionId,
                                CreatedBy = User.Identity.Name,
                                DateCreated = DateTime.Now

                            };

                            if (tax != null)
                            {
                                receiveItem.TaxAmount = orderItem.UnitPrice * unReceivedQty * tax.CalculatedRate;
                                receiveItem.SubTotal = orderItem.UnitPrice * unReceivedQty;
                                receiveItem.Total = orderItem.UnitPrice * unReceivedQty +
                                                    (orderItem.UnitPrice * unReceivedQty * tax.CalculatedRate);
                            }
                            _db.FixedAssetReceiveItems.Add(receiveItem);
                            _db.SaveChanges();

                            #endregion 
                        }
                        else
                        {
                            #region individually

                            for (int i = 0; i < unReceivedQty; i++)
                            {
                                var receiveItem = new FixedAssetReceiveItem
                                {
                                    FixedAssetReceiveId = record.FixedAssetReceiveId,
                                    FixedAssetCategoryID = int.Parse(orderItem.CatCode),
                                    FixedAssetSubCategoryID = int.Parse(orderItem.ItmCode),
                                    IsGroup=false,
                                    Quantity = 1,
                                    UnitPrice = orderItem.UnitPrice,
                                    Balance = 1,
                                    GLTaxRateID = orderItem.GLTaxRateID,
                                    Name = !string.IsNullOrEmpty(orderItem.Name) ? orderItem.Name : "NA",
                                    FixedAssetConditionId = (int)defaultCondition?.FixedAssetConditionId,
                                    CreatedBy = User.Identity.Name,
                                    DateCreated = DateTime.Now

                                };

                                if (tax != null)
                                {
                                    receiveItem.TaxAmount = orderItem.UnitPrice * 1 * tax.CalculatedRate;
                                    receiveItem.SubTotal = orderItem.UnitPrice * 1;
                                    receiveItem.Total = orderItem.UnitPrice * 1 +
                                                        (orderItem.UnitPrice * 1 * tax.CalculatedRate);
                                }
                                _db.FixedAssetReceiveItems.Add(receiveItem);
                                _db.SaveChanges();
                            }

                            #endregion
                        }

                    }

                    

                }

                #endregion
            }

            else if (type.Type == GoodReceiveType.Tender)
            {
                #region get prev receive

                var previousReceiveIds = _db.FixedAssetReceives.Where(x => x.ReceiveType.Type == GoodReceiveType.Tender)
                    .Where(x => x.ReceiveRef == record.ReceiveRef).Where(x => x.IsOnlineApproved)
                    .Where(x => x.IsVoid == false).Select(x => x.FixedAssetReceiveId).ToList();

                var prevReceiveItems = _db.FixedAssetReceiveItems.Where(x => previousReceiveIds.Contains(x.FixedAssetReceiveId))
                    .ToList();

                #endregion
                #region tender
                int awardid = int.Parse(record.ReceiveRef);
                var awarditems = _db.BidAwardedSupplierItems.Where(x => x.ItemType == PurchaseItemType.FixedAsset).Where(x => x.BidAwardedSupplierID == awardid).ToList();
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
                        .Where(x => x.FixedAssetSubCategoryID.ToString() == awardItem.ItmCode)
                        .Where(x => x.Name == awardItem.Name).ToList();
                    if (prevReceiveItem.Any())
                        prevReceivedQty = prevReceiveItem.Sum(x => x.Quantity);

                    if (prevReceivedQty < awardItem.Quanity)
                        unReceivedQty = (int)(awardItem.Quanity - prevReceivedQty);

                    var tax = _db.GLTaxRates.Find(awardItem.GLTaxRateID);

                    int subCatId = int.Parse(awardItem.ItmCode);
                    var subCat = _db.FixedAssetSubCategories.Find(subCatId);

                    if (subCat != null)
                    {
                        if (subCat.IsGroup)
                        {
                            #region by group

                            var receiveItem = new FixedAssetReceiveItem
                            {
                                FixedAssetReceiveId = record.FixedAssetReceiveId,
                                FixedAssetCategoryID = int.Parse(awardItem.CatCode),
                                FixedAssetSubCategoryID = int.Parse(awardItem.ItmCode),
                                IsGroup = true,
                                Quantity = unReceivedQty,
                                UnitPrice = unitprice,
                                Balance = unReceivedQty,
                                GLTaxRateID = awardItem.GLTaxRateID,

                                Name = !string.IsNullOrEmpty(awardItem.Name) ? awardItem.Name : "NA",
                                FixedAssetConditionId = (int)defaultCondition?.FixedAssetConditionId,
                                CreatedBy = User.Identity.Name,
                                DateCreated = DateTime.Now
                            };
                            if (tax != null)
                            {
                                receiveItem.TaxAmount = unitprice * unReceivedQty * tax.CalculatedRate;
                                receiveItem.SubTotal = unitprice * unReceivedQty;
                                receiveItem.Total = unitprice * unReceivedQty +
                                                    (unitprice * unReceivedQty * tax.CalculatedRate);
                            }

                            _db.FixedAssetReceiveItems.Add(receiveItem);
                            _db.SaveChanges();

                            #endregion
                        }
                        else
                        {
                            #region individually

                            for (int i = 0; i < unReceivedQty; i++)
                            {
                                var receiveItem = new FixedAssetReceiveItem
                                {
                                    FixedAssetReceiveId = record.FixedAssetReceiveId,
                                    FixedAssetCategoryID = int.Parse(awardItem.CatCode),
                                    FixedAssetSubCategoryID = int.Parse(awardItem.ItmCode),
                                    Quantity = 1,
                                    UnitPrice = unitprice,
                                    Balance = 1,
                                    GLTaxRateID = awardItem.GLTaxRateID,

                                    Name = !string.IsNullOrEmpty(awardItem.Name) ? awardItem.Name : "NA",
                                    FixedAssetConditionId = (int)defaultCondition?.FixedAssetConditionId,
                                    CreatedBy = User.Identity.Name,
                                    DateCreated = DateTime.Now
                                };
                                if (tax != null)
                                {
                                    receiveItem.TaxAmount = unitprice * 1 * tax.CalculatedRate;
                                    receiveItem.SubTotal = unitprice * 1;
                                    receiveItem.Total = unitprice * 1 +
                                                        (unitprice * 1 * tax.CalculatedRate);
                                }

                                _db.FixedAssetReceiveItems.Add(receiveItem);
                                _db.SaveChanges();
                            }

                            #endregion
                        }

                    }

                    
                }

                #endregion

            }

        }

        private void GetGoodReceiveItems_addesingroup(FixedAssetReceive record)
        {
            #region remove previous receive

            var receiveItems = _db.FixedAssetReceiveItems.Where(x => x.FixedAssetReceiveId == record.FixedAssetReceiveId).ToList();
            _db.FixedAssetReceiveItems.RemoveRange(receiveItems);
            _db.SaveChanges();

            #endregion

            decimal prevReceivedQty = 0;
            decimal unReceivedQty = 0;
            var defaultCondition = _db.FixedAssetConditions.FirstOrDefault();
            ReceiveType type = record.ReceiveType;

            if (type.Type == GoodReceiveType.PurchaseOrder)
            {
                #region get prev receive

                var previousReceiveIds = _db.FixedAssetReceives.Where(x => x.ReceiveType.Type == GoodReceiveType.PurchaseOrder)
                    .Where(x => x.ReceiveRef == record.ReceiveRef).Where(x => x.IsOnlineApproved)
                    .Where(x => x.IsVoid == false).Select(x => x.FixedAssetReceiveId).ToList();

                var prevReceiveItems = _db.FixedAssetReceiveItems.Where(x => previousReceiveIds.Contains(x.FixedAssetReceiveId))
                    .ToList();

                #endregion
                #region Purchase Order Items
                int pid = int.Parse(record.ReceiveRef);
                List<PurchaseOrderItem> purchaseOrderItems = _db.PurchaseOrderItems
                    .Where(pi => pi.PurchaseOrderID == pid)
                    .Where(pi => pi.ItemType == PurchaseItemType.FixedAsset)
                    .ToList();

                foreach (var orderItem in purchaseOrderItems)
                {
                    var prevReceiveItem = prevReceiveItems.Where(x => x.FixedAssetSubCategoryID.ToString() == orderItem.ItmCode).ToList();
                    if (prevReceiveItem.Any())
                        prevReceivedQty = prevReceiveItem.Sum(x => x.Quantity);

                    if (prevReceivedQty < orderItem.Quantity)
                        unReceivedQty = orderItem.Quantity - prevReceivedQty;

                    var tax = _db.GLTaxRates.Find(orderItem.GLTaxRateID);

                    var receiveItem = new FixedAssetReceiveItem
                    {
                        FixedAssetReceiveId = record.FixedAssetReceiveId,
                        FixedAssetCategoryID = int.Parse(orderItem.CatCode),
                        FixedAssetSubCategoryID = int.Parse(orderItem.ItmCode),
                        Quantity = (int)unReceivedQty,
                        UnitPrice = orderItem.UnitPrice,
                        Balance = unReceivedQty,
                        GLTaxRateID = orderItem.GLTaxRateID,
                        Name = !string.IsNullOrEmpty(orderItem.Name) ? orderItem.Name : "NA",
                        FixedAssetConditionId = (int)defaultCondition?.FixedAssetConditionId,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now

                    };

                    if (tax != null)
                    {
                        receiveItem.TaxAmount = orderItem.UnitPrice * unReceivedQty * tax.CalculatedRate;
                        receiveItem.SubTotal = orderItem.UnitPrice * unReceivedQty;
                        receiveItem.Total = orderItem.UnitPrice * unReceivedQty + (orderItem.UnitPrice * unReceivedQty * tax.CalculatedRate);
                    }
                    _db.FixedAssetReceiveItems.Add(receiveItem);
                    _db.SaveChanges();

                }

                #endregion
            }

            else if (type.Type == GoodReceiveType.Tender)
            {
                #region get prev receive

                var previousReceiveIds = _db.FixedAssetReceives.Where(x => x.ReceiveType.Type == GoodReceiveType.Tender)
                    .Where(x => x.ReceiveRef == record.ReceiveRef).Where(x => x.IsOnlineApproved)
                    .Where(x => x.IsVoid == false).Select(x => x.FixedAssetReceiveId).ToList();

                var prevReceiveItems = _db.FixedAssetReceiveItems.Where(x => previousReceiveIds.Contains(x.FixedAssetReceiveId))
                    .ToList();

                #endregion
                #region tender
                int awardid = int.Parse(record.ReceiveRef);
                var awarditems = _db.BidAwardedSupplierItems.Where(x => x.ItemType == PurchaseItemType.FixedAsset).Where(x => x.BidAwardedSupplierID == awardid).ToList();
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
                    var prevReceiveItem = prevReceiveItems.Where(x => x.FixedAssetSubCategoryID.ToString() == awardItem.ItmCode).ToList();
                    if (prevReceiveItem.Any())
                        prevReceivedQty = prevReceiveItem.Sum(x => x.Quantity);

                    if (prevReceivedQty < awardItem.Quanity)
                        unReceivedQty = awardItem.Quanity - prevReceivedQty;

                    var tax = _db.GLTaxRates.Find(awardItem.GLTaxRateID);

                    var receiveItem = new FixedAssetReceiveItem
                    {
                        FixedAssetReceiveId = record.FixedAssetReceiveId,
                        FixedAssetCategoryID = int.Parse(awardItem.CatCode),
                        FixedAssetSubCategoryID = int.Parse(awardItem.ItmCode),
                        Quantity = (int)unReceivedQty,
                        UnitPrice = unitprice,
                        Balance = unReceivedQty,
                        GLTaxRateID = awardItem.GLTaxRateID,

                        Name = !string.IsNullOrEmpty(awardItem.Name) ? awardItem.Name : "NA",
                        FixedAssetConditionId = (int)defaultCondition?.FixedAssetConditionId,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now
                    };
                    if (tax != null)
                    {
                        receiveItem.TaxAmount = unitprice * unReceivedQty * tax.CalculatedRate;
                        receiveItem.SubTotal = unitprice * unReceivedQty;
                        receiveItem.Total = unitprice * unReceivedQty + (unitprice * unReceivedQty * tax.CalculatedRate);
                    }

                    _db.FixedAssetReceiveItems.Add(receiveItem);
                    _db.SaveChanges();
                }

                #endregion

            }

        }

        [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
        public JsonResult TransferDistribution(string id, string description)
        {
            int myid = int.Parse(id);
            var record = _db.FixedAssetReceives.Find(myid);

            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred)
            {
                #region check if already transffered 
                var exist = _db.GLRecordJournals
                   .Where(x => x.Source == JournalSources.PPE_Receive.ToString())
                   .Where(x => x.SourceId == record.FixedAssetReceiveId.ToString())
                   .FirstOrDefault();
                if (exist != null)
                {
                    var formattedData = new
                    {
                        Message = "The record is already transffered, it can't be retransferred.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                #endregion
                _transferHelper = new FixedAssetTransferHelper();
                string response = _transferHelper.GoodReceive(myid, description, User?.Identity?.Name ?? "");
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

            var record = _db.FixedAssetReceives.Find(id);
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
