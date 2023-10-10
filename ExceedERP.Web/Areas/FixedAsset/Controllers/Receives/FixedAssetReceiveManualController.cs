using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Receives;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
//using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Areas.Inventory.Models;
//using ExceedERP.Web.Areas.Inventory.Models;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using EntityState = System.Data.Entity.EntityState;


namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Receives
{


    public class FixedAssetReceiveManualController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover,FixedAssetConstants.FixedAssetInspector)]
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
            var conditions = db.FixedAssetConditions;

            var fixedAssetCategories = db.FixedAssetCategories;
            ViewData["fixedAssetCategories"] = fixedAssetCategories.ToList();

            var fixedAssetSubCategories = db.FixedAssetSubCategories;
            ViewData["fixedAssetSubCategories"] = fixedAssetSubCategories.ToList();

            ViewData["Conditions"] = conditions;
            ViewData["stores"] = stores;
            ViewData["tax"] = tax;
            ViewData["ReeceiveType"] = receivetype;
            ViewData["Branches"] = branches;
            ViewData["Items"] = items;
            ViewData["Category"] = categories;
            ViewData["defaultCategory"] = categories.FirstOrDefault();

            var employee = db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;

        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover, FixedAssetConstants.FixedAssetInspector)]
        public ActionResult FixedAssetReceives_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> userStores = UserInformationHelper.GetUserStores(User.Identity.Name);
            IQueryable<FixedAssetReceive> fixedAssetReceives = db.FixedAssetReceives.Where(x=>x.IsManual).Where(x => userStores.Contains(x.StoreCode));
           ;
            DataSourceResult result = fixedAssetReceives.ToDataSourceResult(request, fixedAssetReceive => new
            {
                IsManual=fixedAssetReceive.IsManual,
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
            IQueryable<FixedAssetReceive> fixedAssetReceives = db.FixedAssetReceives.Where(go => go.FixedAssetReceiveId == id);
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

            ReceiveType type = db.ReceiveTypes.Find(fixedAssetReceive.ReceiveTypeId);
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

            if (String.IsNullOrWhiteSpace(fixedAssetReceive.SupplierName))
                ModelState.AddModelError("supplierid", "Please enter supplier name");

            if (!String.IsNullOrEmpty(fixedAssetReceive.Reference))
            {
                var exists = db.FixedAssetReceives
                    .Where(x => x.Reference == fixedAssetReceive.Reference && x.IsVoid == false).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("Reference", fixedAssetReceive.Reference + "  already exists in the system.");
                }
            }

            if (!String.IsNullOrEmpty(fixedAssetReceive.ReceiveRef))
            {
                var exists = db.FixedAssetReceives
                    .Where(x=>x.IsVoid==false).Where(x => x.FixedAssetReceiveId != fixedAssetReceive.FixedAssetReceiveId).Where(x => x.ReceiveRef == fixedAssetReceive.ReceiveRef && x.IsVoid == false).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("Reference", fixedAssetReceive.ReceiveRef + "  already exists in the system.");
                }
            }

            if (!String.IsNullOrEmpty(fixedAssetReceive.Reference))
            {
                var exists = db.FixedAssetReceives.Where(x => x.Reference == fixedAssetReceive.Reference && x.IsVoid == false).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("Reference", $"{fixedAssetReceive.Reference}  already exists in the system.");
                }
            }

            if (fixedAssetReceive.GLPeriodId != 0)
            {
                var fixedAssetPeriod = db.FixedAssetPeriods.Find(fixedAssetReceive.GLPeriodId);
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
                        IsManual =true,
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

                db.FixedAssetReceives.Add(entity);
                db.SaveChanges();
                fixedAssetReceive.FixedAssetReceiveId = entity.FixedAssetReceiveId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetReceive", "Success", User.Identity.Name, $"created FixedAssetReceive  {entity.FixedAssetReceiveId} Tag {entity.StoreCode}", Modules.FixedAsset);
                #endregion

            }

            return Json(new[] { fixedAssetReceive }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetReceives_Update([DataSourceRequest]DataSourceRequest request, FixedAssetReceive fixedAssetReceive)
        {
            #region validation

            ReceiveType type = db.ReceiveTypes.Find(fixedAssetReceive.ReceiveTypeId);
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

            if (String.IsNullOrWhiteSpace(fixedAssetReceive.SupplierName))
                ModelState.AddModelError("supplierid", "Please enter supplier name");

            if (!String.IsNullOrEmpty(fixedAssetReceive.ReceiveRef))
            {
                var exists = db.FixedAssetReceives
                    .Where(x => x.IsVoid == false)
                    .Where(x => x.FixedAssetReceiveId != fixedAssetReceive.FixedAssetReceiveId).Where(x => x.ReceiveRef == fixedAssetReceive.ReceiveRef && x.IsVoid == false).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("Reference", fixedAssetReceive.ReceiveRef + "  already exists in the system.");
                }
            }

            if (!String.IsNullOrEmpty(fixedAssetReceive.Reference))
            {
                var exists = db.FixedAssetReceives.Where(x => x.FixedAssetReceiveId != fixedAssetReceive.FixedAssetReceiveId).Where(x => x.Reference == fixedAssetReceive.Reference && x.IsVoid == false).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("Reference", fixedAssetReceive.Reference + "  already exists in the system.");
                }
            }
           

            if (fixedAssetReceive.GLPeriodId != 0)
            {
                var fixedAssetPeriod = db.FixedAssetPeriods.Find(fixedAssetReceive.GLPeriodId);
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

                db.FixedAssetReceives.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetReceive", "Success", User.Identity.Name, $"updated FixedAssetReceive  {entity.FixedAssetReceiveId} Tag {entity.StoreCode}", Modules.FixedAsset);
                #endregion
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

                db.FixedAssetReceives.Attach(entity);
                db.FixedAssetReceives.Remove(entity);
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted FixedAssetReceive  {fixedAssetReceive.FixedAssetReceiveId} Tag {fixedAssetReceive.StoreCode}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { fixedAssetReceive }.ToDataSourceResult(request, ModelState));
        }

      
 

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        public ActionResult ReceiveImport([DataSourceRequest]DataSourceRequest request, IEnumerable<HttpPostedFileBase> file)
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
            var record = db.FixedAssetReceives.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.FixedAssetReceiveItem.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.FixedAssetReceives.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
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
            var record = db.FixedAssetReceives.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred && record.FixedAssetReceiveItem.Count > 0)
            {
                #region remove assets

                var assetRegistrations = db.AssetRegistrations.Where(x => x.GRN == record.FixedAssetReceiveId.ToString())
                    .ToList();

                db.AssetRegistrations.RemoveRange(assetRegistrations);
                db.SaveChanges();


                #endregion
                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                //record.IsVoid = true;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                db.FixedAssetReceives.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();


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

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        public ActionResult FixedAssetReceive_OnlineTransfer(string id)
        {
            int myid = int.Parse(id);
            FixedAssetReceive record = db.FixedAssetReceives.Find(myid);
            if (!record.IsOnlineApproved)
            {
                //ViewBag.Note = "The record cannot be transffred before validation.";
                //return View("Error");
            }
            else
            {
                if (!record.IsOnlineTransferred)
                {
                    #region check if debit and credit are equal
                    decimal TDebit = 0;
                    decimal TCredit = 0;
                    var distribution = record.FixedAssetReceiveDistribution.ToList();
                    if (distribution != null)
                    {
                        TDebit = distribution.Sum(x => x.Debit);
                        TCredit = distribution.Sum(x => x.Credit);
                    }
                    if (TDebit != TCredit)
                    {
                        //ViewBag.Note = "Total debit and total credit is not balanced.";
                        // return View("Error");
                    }
                    if (TDebit == 0)
                    {
                        //ViewBag.Note = "Total debit and total credit is zero.";
                        //return View("Error");
                    }
                    #endregion
                    #region send to GL
                    string glperiod = "";
                    string glyear = "";
                    #region get period
                    var period = db.GLPeriods.Where(x => x.DateFrom <= record.ReceiveDate)
                                           .Where(x => x.DateTo >= record.ReceiveDate).ToList();
                    if (period != null)
                    {

                        if (period.Count() == 1)
                        {
                            var myPeriod = period.SingleOrDefault();
                            glperiod = myPeriod.Name;
                            glyear = myPeriod.GlFiscalYear.Name.ToString();
                            if (myPeriod.Status == FiscalYearStatus.Close)
                            {
                                //ViewBag.Note = "The period " + glperiod + " is closed,the record cannot be transffred to closed period.";
                                //return View("Error");
                            }
                        }
                        else if (!period.Any())
                        {
                            //ViewBag.Note = "There are no periods defined that contain the Invoice date.";
                            //return View("Error");
                        }
                        else if (period.Any())
                        {
                            //ViewBag.Note = "There are multiple periods defined that contain the Invoice date.";
                            //return View("Error");
                        }
                        else
                        {

                        }
                    }

                    #endregion
                    if (record.FixedAssetReceiveDistribution.Any())
                    {
                        GlRecordJournal journal = new GlRecordJournal();
                        journal.DateCreated = DateTime.Now;
                        journal.CreatedBy = User.Identity.Name;
                        journal.JournalName = "FixedAssetReceive";
                        journal.Description = "FixedAssetReceive";
                        journal.FiscalYear = glyear;
                        journal.PeriodName = glperiod;
                        journal.CategoryName = "FixedAssetReceive";
                        journal.SourcesName = "GR";
                        journal.JournalNumber = "FixedAssetReceive." + record.FixedAssetReceiveId.ToString();
                        journal.EffectiveDate = record.ReceiveDate;
                        journal.Status = "Payment";
                        journal.IsOnlineApproved = true;
                        journal.OnlineApprovedBy = User.Identity.Name;
                        journal.OnlineApprovedTime = DateTime.Now;
                        db.GLRecordJournals.Add(journal);
                        db.SaveChanges();
                        GLRecordJournalDetail journalDetail;
                        foreach (FixedAssetReceiveDistribution i in record.FixedAssetReceiveDistribution)
                        {
                            #region debit and credit
                            journalDetail = new GLRecordJournalDetail();
                            journalDetail.DateCreated = DateTime.Now;
                            journalDetail.CreatedBy = User.Identity.Name;
                            journalDetail.GLRecordJournal = journal;
                            journalDetail.AccountCode = i.AccountCode;
                            journalDetail.Debit = i.Debit;
                            journalDetail.Credit = i.Credit;
                            db.GLRecordJournalDetails.Add(journalDetail);
                            db.SaveChanges();
                            #endregion
                        }
                    }
                    #endregion
                    record.IsOnlineTransferred = true;
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    //ViewBag.Note = "You cannot transfer this transaction, it is already transffered.";
                    //return View("Error");
                }
            }
            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
