using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Areas.Inventory.Models;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using EntityState = System.Data.Entity.EntityState;

namespace ExceedERP.Web.Areas.Inventory.Controllers.Requisitions
{



    public class StoreRequisitionPropertyReviewController : Controller
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(InventoryConstants.PropertyUser, InventoryConstants.PropertyAuthrizer)]
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {

            var categories = db.ItemCategorys;
            var items = db.InventoryItems;
            var costCenter = db.BranchCostCenter;
            var branches = db.Branch;

            ViewData["Branches"] = branches;
            ViewData["Items"] = items;
            ViewData["Category"] = categories;
            ViewData["defaultCategory"] = categories.FirstOrDefault();

            var store = db.InventoryStores.AsQueryable();
            ViewData["storesInCompany"] = store;

            var employee = db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;

            var selectedcostCenter = costCenter.Select(
                i => new
                {
                    Name = i.MainCompanyStructure.Name,
                    Code = i.OrgStructureId
                }).AsQueryable();

            ViewData["costCenters"] = selectedcostCenter;

            var inventoryPeriods = db.InventoryPeriods;
            var selectedinventoryPeriods = inventoryPeriods.Select(
                i => new
                {
                    Name = i.Name,
                    GLPeriodId = i.InventoryPeriodId
                }).AsQueryable();
            ViewData["inventoryPeriods"] = selectedinventoryPeriods;

        }
        

        [AuthorizeRoles(InventoryConstants.PropertyUser, InventoryConstants.PropertyAuthrizer)]

        public ActionResult StoreRequisitionPropertyReviews_Read([DataSourceRequest] DataSourceRequest request)
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            //IQueryable<StoreRequisition> storerequisitions = db.StoreRequisitions.Where(x => x.IsOnlineApproved && !x.IsVoid).Where(x => mybraches.Contains(x.OrgBranchName));
            List<string> userStores = UserInformationHelper.GetUserStores(User.Identity.Name);
            IQueryable<StoreRequisition> storerequisitions = db.StoreRequisitions.Where(x => x.IsForMaintenance == false).Where(x => x.IsOnlineApproved && !x.IsVoid).Where(x => userStores.Contains(x.StoreCode));
            var result = storerequisitions.ToDataSourceResult(request, storeRequisition => new
                {
                    StoreRequisitionID = storeRequisition.StoreRequisitionID,
                    Reference = storeRequisition.Reference,
                    OrgStructureId = storeRequisition.OrgStructureId,
                    BranchId = storeRequisition.BranchId,
                    IsTransfer = storeRequisition.IsTransfer,
                    ToStoreCode = storeRequisition.ToStoreCode,
                    EmployeeId = storeRequisition.EmployeeId,
                    CreatedBy = storeRequisition.CreatedBy,
                    DateCreated = storeRequisition.DateCreated,
                    RequestDate = storeRequisition.RequestDate,
                    StoreCode = storeRequisition.StoreCode,
                    ModelNo = storeRequisition.ModelNo,
                    PlateNo = storeRequisition.PlateNo,

                    Status = storeRequisition.Status,
                    Remark = storeRequisition.Remark,
                    GLPeriodId = storeRequisition.GLPeriodId,
                    IsVoid = storeRequisition.IsVoid,
                    VoidBy = storeRequisition.VoidBy,
                    VoidTime = storeRequisition.VoidTime,
                    IsOnlineApproved = storeRequisition.IsOnlineApproved,
                    OnlineApprovedBy = storeRequisition.OnlineApprovedBy,
                    OnlineApprovedTime = storeRequisition.OnlineApprovedTime,

                    IsSecondOnlineApproved = storeRequisition.IsSecondOnlineApproved,
                    SecondOnlineApprovedBy = storeRequisition.SecondOnlineApprovedBy,
                    SecondOnlineApprovedTime = storeRequisition.SecondOnlineApprovedTime,

                    IsSendForApproval = storeRequisition.IsSendForApproval,
                    SendForApprovalBy = storeRequisition.SendForApprovalBy,
                    SendForApprovalTime = storeRequisition.SendForApprovalTime,

                    IsSecondSendForApproval = storeRequisition.IsSecondSendForApproval,
                    SecondSendForApprovalBy = storeRequisition.SecondSendForApprovalBy,
                    SecondSendForApprovalTime = storeRequisition.SecondSendForApprovalTime,

                    IsOnlineTransferred = storeRequisition.IsOnlineTransferred,
                    IsOnlineTransferredBy = storeRequisition.IsOnlineTransferredBy,
                    OnlineTransferredTime = storeRequisition.OnlineTransferredTime,
                    IsFuel = storeRequisition.IsFuel
                });

            return Json(result);
        }

        [AuthorizeRoles(InventoryConstants.PropertyUser)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreRequisitionPropertyReviews_Create([DataSourceRequest] DataSourceRequest request, StoreRequisition storeRequisition)
        {
            if (storeRequisition.IsTransfer && storeRequisition.ToStoreCode == null)
            {
                ModelState.AddModelError("invalidEntry", "Please select the Requester Store for Transfer");
            }
            if (ModelState.IsValid)
            {
                #region validation

                bool valid = true;
                var exists = db.StoreRequisitions.Where(x => x.Reference == storeRequisition.Reference).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("Reference", storeRequisition.Reference + " is already exists in the system.");
                    valid = false;
                }

                #endregion

                if (valid)
                {
                    var entity = new StoreRequisition
                    {
                        Reference = storeRequisition.Reference,
                        OrgStructureId = storeRequisition.OrgStructureId,// UserInformationHelper.GetEmployeeCostCenterId(User.Identity.Name),
                        BranchId = storeRequisition.BranchId,//UserInformationHelper.GetEmployeeBranchId(User.Identity.Name),
                        EmployeeId = storeRequisition.EmployeeId, //UserInformationHelper.GetEmployeeId(User.Identity.Name),
                        RequestDate = storeRequisition.RequestDate,
                        StoreCode = storeRequisition.StoreCode,
                        ModelNo = storeRequisition.ModelNo,
                        PlateNo = storeRequisition.PlateNo,

                        Status = Enum.GetName(typeof(OperationStatuses), 5),
                        Remark = storeRequisition.Remark,
                        IsVoid = storeRequisition.IsVoid,
                        IsFuel = false,
                        IsTransfer = storeRequisition.IsTransfer,
                        ToStoreCode = storeRequisition.ToStoreCode,
                        VoidBy = storeRequisition.VoidBy,
                        VoidTime = storeRequisition.VoidTime,

                        IsOnlineApproved = storeRequisition.IsOnlineApproved,
                        OnlineApprovedBy = storeRequisition.OnlineApprovedBy,
                        OnlineApprovedTime = storeRequisition.OnlineApprovedTime,
                        IsSecondOnlineApproved = storeRequisition.IsSecondOnlineApproved,
                        SecondOnlineApprovedBy = storeRequisition.SecondOnlineApprovedBy,
                        SecondOnlineApprovedTime = storeRequisition.SecondOnlineApprovedTime,
                        IsOnlineTransferred = storeRequisition.IsOnlineTransferred,
                        IsOnlineTransferredBy = storeRequisition.IsOnlineTransferredBy,
                        OnlineTransferredTime = storeRequisition.OnlineTransferredTime,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        OrgBranchName = storeRequisition.BranchId.ToString()
                    };

                    db.StoreRequisitions.Add(entity);
                    db.SaveChanges();
                    storeRequisition.StoreRequisitionID = entity.StoreRequisitionID;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "store requisition", "Success", User.Identity.Name, $"created store requisition {entity.StoreRequisitionID} ", Modules.Inventory);
                    #endregion
                }
            }

            return Json(ModelState.IsValid ? new object() : ModelState.ToDataSourceResult());
        }


        [AuthorizeRoles(InventoryConstants.PropertyUser)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreRequisitionPropertyReviews_Update([DataSourceRequest] DataSourceRequest request, StoreRequisition storeRequisition)
        {
            #region validation
            if (storeRequisition.IsTransfer && storeRequisition.ToStoreCode == null)
                ModelState.AddModelError("invalidEntry", "Please select the Requester Store for Transfer");

            if (!String.IsNullOrEmpty(storeRequisition.Reference))
            {
                var exists = db.StoreRequisitions
                    .Where(x => x.StoreRequisitionID != storeRequisition.StoreRequisitionID)
                    .Where(x => x.Reference == storeRequisition.Reference).ToList();

                if (exists.Any())
                    ModelState.AddModelError("Reference",
                        storeRequisition.Reference + " is already exists in the system.");
            }
            #endregion

            if (ModelState.IsValid)
            {
                var entity = new StoreRequisition
                {
                    StoreRequisitionID = storeRequisition.StoreRequisitionID,
                    Reference = storeRequisition.Reference,
                    BranchId = storeRequisition.BranchId,//UserInformationHelper.GetEmployeeBranchId(User.Identity.Name),
                    EmployeeId = storeRequisition.EmployeeId, //UserInformationHelper.GetEmployeeId(User.Identity.Name),
                    RequestDate = storeRequisition.RequestDate,
                    StoreCode = storeRequisition.StoreCode,
                    Status = storeRequisition.Status,
                    Remark = storeRequisition.Remark,
                    IsFuel = storeRequisition.IsFuel,
                    IsTransfer = storeRequisition.IsTransfer,
                    ToStoreCode = storeRequisition.ToStoreCode,
                    ModelNo = storeRequisition.ModelNo,
                    PlateNo = storeRequisition.PlateNo,
                    OrgStructureId = storeRequisition.OrgStructureId,
                    GLPeriodId = storeRequisition.GLPeriodId,

                    IsVoid = storeRequisition.IsVoid,
                    VoidBy = storeRequisition.VoidBy,
                    VoidTime = storeRequisition.VoidTime,
                    IsOnlineApproved = storeRequisition.IsOnlineApproved,
                    OnlineApprovedBy = storeRequisition.OnlineApprovedBy,
                    OnlineApprovedTime = storeRequisition.OnlineApprovedTime,
                    IsSecondOnlineApproved = storeRequisition.IsSecondOnlineApproved,
                    SecondOnlineApprovedBy = storeRequisition.SecondOnlineApprovedBy,
                    SecondOnlineApprovedTime = storeRequisition.SecondOnlineApprovedTime,
                    IsOnlineTransferred = storeRequisition.IsOnlineTransferred,
                    IsOnlineTransferredBy = storeRequisition.IsOnlineTransferredBy,
                    OnlineTransferredTime = storeRequisition.OnlineTransferredTime,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                    OrgBranchName = storeRequisition.BranchId.ToString()
                };

                db.StoreRequisitions.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "store requisition review", "Success", User.Identity.Name, $"created store requisition {entity.StoreRequisitionID} ", Modules.Inventory);
                #endregion

            }

            return Json(ModelState.IsValid ? new object() : ModelState.ToDataSourceResult());
        }

        [AuthorizeRoles(InventoryConstants.PropertyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreRequisitionPropertyReviews_Destroy([DataSourceRequest] DataSourceRequest request, StoreRequisition storeRequisition)
        {
            if (ModelState.IsValid)
            {
                var entity = new StoreRequisition
                    {
                        StoreRequisitionID = storeRequisition.StoreRequisitionID,
                        Reference = storeRequisition.Reference,
                        BranchId = storeRequisition.BranchId,
                        OrgStructureId = storeRequisition.OrgStructureId,
                        EmployeeId = storeRequisition.EmployeeId,
                        RequestDate = storeRequisition.RequestDate,
                        StoreCode = storeRequisition.StoreCode,
                        Status = storeRequisition.Status,
                        Remark = storeRequisition.Remark,
                        IsVoid = storeRequisition.IsVoid,
                        IsTransfer = storeRequisition.IsTransfer,
                        ToStoreCode = storeRequisition.ToStoreCode,
                        ModelNo = storeRequisition.ModelNo,
                        IsFuel = storeRequisition.IsFuel,

                       VoidBy = storeRequisition.VoidBy,
                        VoidTime = storeRequisition.VoidTime,
                        IsOnlineApproved = storeRequisition.IsOnlineApproved,
                        OnlineApprovedBy = storeRequisition.OnlineApprovedBy,
                        OnlineApprovedTime = storeRequisition.OnlineApprovedTime,
                        IsSecondOnlineApproved = storeRequisition.IsSecondOnlineApproved,
                        SecondOnlineApprovedBy = storeRequisition.SecondOnlineApprovedBy,
                        SecondOnlineApprovedTime = storeRequisition.SecondOnlineApprovedTime,
                        IsOnlineTransferred = storeRequisition.IsOnlineTransferred,
                        IsOnlineTransferredBy = storeRequisition.IsOnlineTransferredBy,
                        OnlineTransferredTime = storeRequisition.OnlineTransferredTime
                    };

                db.StoreRequisitions.Attach(entity);
                db.StoreRequisitions.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted store requisition " + storeRequisition.StoreRequisitionID);
                #endregion
            }

            return Json(new[] { storeRequisition }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyUser)]

        public ActionResult StoreRequistion_Import([DataSourceRequest] DataSourceRequest request, IEnumerable<HttpPostedFileBase> file)
        {
            DataSet ds = new DataSet();
            foreach (HttpPostedFileBase fileBase in file)
            {
                var fileforImport = Request.Files["file"];
                if (fileforImport != null)
                {
                    string fileExtension = System.IO.Path.GetExtension(fileforImport.FileName);
                    if (fileExtension == ".xls" || fileExtension == ".xlsx" || fileExtension == ".xlsm")
                    {
                        string fileLocation = Server.MapPath("~/Content/Import/") + "SR" + DateTime.Now.Year.ToString() +
                                              DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() +
                                              DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() +
                                              DateTime.Now.Second.ToString() + fileforImport.FileName;
                        if (System.IO.File.Exists(fileLocation))
                        {
                            System.IO.File.Delete(fileLocation);
                        }
                        fileforImport.SaveAs(fileLocation);
                        var excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                    ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        //connection String for xls file format.
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
            }
            List<StoreRequistionImport> storerequistions = new List<StoreRequistionImport>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int j = 0; j <= ds.Tables[0].Rows.Count - 1; j++)
                {
                    StoreRequistionImport import = new StoreRequistionImport
                    {
                        No = ds.Tables[0].Rows[j].ItemArray[0].ToString(),
                        StoreCode = ds.Tables[0].Rows[j].ItemArray[1].ToString(),
                        WorkUnit = ds.Tables[0].Rows[j].ItemArray[2].ToString(),
                        Requestor = ds.Tables[0].Rows[j].ItemArray[3].ToString(),
                        RequestDate = DateTime.Parse(ds.Tables[0].Rows[j].ItemArray[4].ToString()),
                        ItemCategoryCode = ds.Tables[0].Rows[j].ItemArray[5].ToString(),
                        ItemCode = ds.Tables[0].Rows[j].ItemArray[6].ToString(),
                        PartNo = ds.Tables[0].Rows[j].ItemArray[7].ToString(),
                        RequestedQuantity = decimal.Parse(ds.Tables[0].Rows[j].ItemArray[8].ToString()),
                        ApprovedQuantity = decimal.Parse(ds.Tables[0].Rows[j].ItemArray[9].ToString()),
                        IssuedQuantity = decimal.Parse(ds.Tables[0].Rows[j].ItemArray[10].ToString()),
                        DocumentReference = ds.Tables[0].Rows[j].ItemArray[11].ToString()
                    };
                    storerequistions.Add(import);



                }
            }
            var imports = storerequistions.GroupBy(x => x.No).Select(x => x.FirstOrDefault());
            foreach (var import in imports)
            {
                if (import != null)
                {
                    StoreRequisition storeRequisition = new StoreRequisition
                    {
                        StoreCode = import.StoreCode,
                        OrgStructureId = 1,
                        IsFuel = false,
                        EmployeeId = 1,
                        RequestDate = import.RequestDate,
                        Reference = import.DocumentReference,
                        Status = Enum.GetName(typeof(OperationStatuses), 5)
                    };

                    List<StoreRequisitionItem> items = new List<StoreRequisitionItem>();
                    foreach (StoreRequistionImport source in storerequistions.Where(x => x.No == import.No))
                    {
                        StoreRequisitionItem item = new StoreRequisitionItem
                        {
                            ItemCode = source.ItemCode,
                            RequestedQuantity = source.RequestedQuantity,
                            ApprovedQuantity = source.ApprovedQuantity,
                            IssuedQuantity = source.IssuedQuantity,
                            PartNo = source.PartNo
                        };
                        items.Add(item);
                    }
                    storeRequisition.StoreRequisitionItem = items;
                    storeRequisition.CreatedBy = User.Identity.Name;
                    storeRequisition.DateCreated = DateTime.Now;
                    db.StoreRequisitions.Add(storeRequisition);
                }
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [AuthorizeRoles(InventoryConstants.PropertyUser)]

        public ActionResult SendForApproval(int id)
        {
            var record = db.StoreRequisitions.Find(id);
            if (record != null && !record.IsSecondSendForApproval && !record.IsSecondOnlineApproved && record.StoreRequisitionItem.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 9);
                record.IsSecondSendForApproval = true;
                record.SecondSendForApprovalBy = User.Identity.Name;
                record.SecondSendForApprovalTime = DateTime.Now;
                db.StoreRequisitions.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "store requisition", "Success", User.Identity.Name, $"send for second approval store requisition {id} ", Modules.Inventory);
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

            else if (record.StoreRequisitionItem.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add line items first.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            else if (record.IsSecondSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            else if (record.IsSecondOnlineApproved)
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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}