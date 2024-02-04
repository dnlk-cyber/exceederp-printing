using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using EntityState = System.Data.Entity.EntityState;

namespace ExceedERP.Web.Areas.Printing.Controllers.ProductionFollowUp.Operation
{

    //[AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpRJobOrderUser)]
    public class ProductionStoreRequisitionController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly ApplicationDbContext _userdb = new ApplicationDbContext();

        public ActionResult Index()
        {
            
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {

            var categories = _db.ItemCategorys;
            var items = _db.InventoryItems;
            var costCenter = _db.BranchCostCenter;
            var branches = _db.Branch;

            var inventoryPeriods = _db.InventoryPeriods;
            var selectedinventoryPeriods = inventoryPeriods.Select(
             i => new
             {
                 Name = i.Name,
                 GLPeriodId = i.InventoryPeriodId
             }).AsQueryable();
            ViewData["inventoryPeriods"] = selectedinventoryPeriods;

            ViewData["Branches"] = branches;

            var selecteditems = items.Select(
                i => new
                {
                    Name = i.ItemCode + "_" + i.Name,
                    ItemCode = i.ItemCode
                }).ToList();
            ViewData["Items"] = selecteditems;
            ViewData["Category"] = categories;
            ViewData["defaultCategory"] = categories.FirstOrDefault();

            var store = _db.InventoryStores.AsQueryable();
            ViewData["storesInCompany"] = store;

            var employee = _db.Person_Employee;
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

        }



      //  [AuthorizeRoles(InventoryConstants.PropertyRequistion, InventoryConstants.UserDepartment)]

        public ActionResult ProductionStoreRequisitions_Read([DataSourceRequest] DataSourceRequest request, int id)
        {
            var user = _userdb.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
            //if(user!=null)
            //    {
            //    var storekeepers = _db.StoreKeeperAssignments.Where(x => x.StoreKeeper == user.Id).ToList();

            //}
            //int userworkunit = UserInformationHelper.GetUserReportsTo(User.Identity.Name);
            List<int> userworkunit = UserInformationHelper.GetUserReportsToAndChildWorkUnits(User.Identity.Name);
            List<string> userStores = UserInformationHelper.GetUserStores(User.Identity.Name);

            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            //IQueryable<StoreRequisition> storerequisitions = db.StoreRequisitions.Where(x => x.IsFuel == false).Where(x => mybraches.Contains(x.OrgBranchName));
            var job = _db.Jobs.Where(q=>q.JobId == id).FirstOrDefault()?.JobId;
            var storeRequisitions = _db.StoreRequisitions.Where(q=>q.JobOrderNo == job.ToString()).Where(x => x.IsFuel == false)
                .AsQueryable();

            //if (System.Web.HttpContext.Current.User.IsInRole(InventoryConstants.UserDepartment)
            //&& System.Web.HttpContext.Current.User.IsInRole(InventoryConstants.PropertyRequistion))
            //{
            //    if (!storekeepers.Any() && userworkunit.Any())
            //    {
            //        storeRequisitions = storeRequisitions.Where(x => userworkunit.Contains(x.OrgStructureId)
            //                                                         || x.CreatedBy == User.Identity.Name).AsQueryable();
            //    }
            //    else
            //    {
            //        storeRequisitions = storeRequisitions.Where(x => userStores.Contains(x.StoreCode)
            //                                                         || x.CreatedBy == User.Identity.Name);
            //    }
            //}

            //else if (System.Web.HttpContext.Current.User.IsInRole(InventoryConstants.UserDepartment))
            //{
            //    if (!storekeepers.Any() && userworkunit.Any())
            //    {
            //        storeRequisitions = storeRequisitions.Where(x => userworkunit.Contains(x.OrgStructureId) && userworkunit != null
            //                                                         || x.CreatedBy == User.Identity.Name).AsQueryable();
            //    }
            //    else
            //    {
            //        storeRequisitions = storeRequisitions.Where(x => userStores.Contains(x.StoreCode) || x.CreatedBy == User.Identity.Name);
            //    }
            //}
            //else if (System.Web.HttpContext.Current.User.IsInRole(InventoryConstants.PropertyRequistion))
            //{
            //    storeRequisitions = storeRequisitions.Where(x => x.CreatedBy == User.Identity.Name).AsQueryable();
            //}


            //old
            //List<string> userStores = UserInformationHelper.GetUserStores(User.Identity.Name);
            //IQueryable<StoreRequisition> storerequisitions = db.StoreRequisitions.Where(x => x.IsFuel == false).Where(x => userStores.Contains(x.StoreCode));
            var result = storeRequisitions.ToDataSourceResult(request, storeRequisition => new
                {
                    StoreRequisitionID = storeRequisition.StoreRequisitionID,
                    Reference = storeRequisition.Reference,
                    OrgStructureId = storeRequisition.OrgStructureId,
                    BranchId = storeRequisition.BranchId,
                    IsTransfer = storeRequisition.IsTransfer,
                    ToStoreCode = storeRequisition.ToStoreCode,
                    EmployeeId = storeRequisition.EmployeeId,
                    RequestDate = storeRequisition.RequestDate,
                    StoreCode = storeRequisition.StoreCode,
                    IsForOther = storeRequisition.IsForOther,
                    ForBranchId = storeRequisition.ForBranchId,
                    ModelNo = storeRequisition.ModelNo,
                    JobOrderNo = storeRequisition.JobOrderNo,
                    PlateNo = storeRequisition.PlateNo,

                    Status = storeRequisition.Status,
                    Remark = storeRequisition.Remark,
                    GLPeriodId = storeRequisition.GLPeriodId,

                    CreatedBy = storeRequisition.CreatedBy,
                    DateCreated = storeRequisition.DateCreated,
                    ModifiedBy = storeRequisition.ModifiedBy,
                    LastModified = storeRequisition.LastModified,

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

            return Json(result,JsonRequestBehavior.AllowGet);
        }

        //[AuthorizeRoles(InventoryConstants.PropertyRequistion)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionStoreRequisitions_Create([DataSourceRequest] DataSourceRequest request, StoreRequisition storeRequisition, int id)
        {
            #region validation
            if (storeRequisition.IsTransfer && storeRequisition.ToStoreCode == null)
            {
                ModelState.AddModelError("invalidEntry", "Please select the Requester Store for Transfer");
            }

            if (storeRequisition.IsTransfer && storeRequisition.ToStoreCode == storeRequisition.StoreCode)
            {
                ModelState.AddModelError("invalidEntry", "Transfer is not allowed within one store");
            }

            if (storeRequisition.GLPeriodId != 0)
            {
                var inventoryPeriod = _db.InventoryPeriods.Find(storeRequisition.GLPeriodId);
                if (inventoryPeriod != null)
                {
                    if (
                        !((storeRequisition.RequestDate.Date >= inventoryPeriod.DateFrom) &&
                          (storeRequisition.RequestDate.Date <= inventoryPeriod.DateTo)))
                        ModelState.AddModelError("",
                            "Issue Date should be within the period: " + inventoryPeriod.Name + "(From " +
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

            if (storeRequisition.EmployeeId == 0)
                ModelState.AddModelError("requester", "Please choose requetster");

            //if (storeRequisition.BranchId == 0)
            //    ModelState.AddModelError("branch", "Please choose branch");

            //if (storeRequisition.RequestDate > DateTime.Now.Date)
            //{
            //    ModelState.AddModelError("future", "You are recording in future period,please correct the date.");
            //}



            if (!String.IsNullOrEmpty(storeRequisition.Reference))
            {
                var exists = _db.StoreRequisitions.Where(x => x.Reference == storeRequisition.Reference && x.IsVoid == false).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("Reference",
                        storeRequisition.Reference + " is already exists in the system.");
                }
            }

            if (storeRequisition.IsForOther && storeRequisition.ForBranchId==null)
            {
                ModelState.AddModelError("otherbranch"," Please select other branch.");
            }

            #endregion
            if (ModelState.IsValid)
            {
                var jobOrderNum = _db.Jobs.Where(q=>q.JobId==id).FirstOrDefault()?.JobId;

                    var entity = new StoreRequisition
                    {
                        Reference = storeRequisition.Reference,
                        OrgStructureId = UserInformationHelper.GetEmployeeReportsTo(storeRequisition.EmployeeId),
                        BranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(storeRequisition.StoreCode), //UserInformationHelper.GetEmployeeBranchId(User.Identity.Name),
                        EmployeeId = storeRequisition.EmployeeId, // UserInformationHelper.GetEmployeeId(User.Identity.Name),
                        RequestDate = storeRequisition.RequestDate.Date,// DateTime.Today,
                        StoreCode = storeRequisition.StoreCode,
                        Status = Enum.GetName(typeof(OperationStatuses), 5),
                        Remark = storeRequisition.Remark,
                        GLPeriodId = storeRequisition.GLPeriodId,
                        IsVoid = storeRequisition.IsVoid,
                        IsFuel = false,
                        IsTransfer = storeRequisition.IsTransfer,
                        ToStoreCode = storeRequisition.ToStoreCode,
                        IsForOther = storeRequisition.IsForOther,
                        ForBranchId = storeRequisition.ForBranchId,
                        ModelNo = storeRequisition.ModelNo,
                        JobOrderNo = jobOrderNum.ToString(),
                        PlateNo = storeRequisition.PlateNo,

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
                        OrgBranchName = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(storeRequisition.StoreCode).ToString(),
                    };

                    _db.StoreRequisitions.Add(entity);
                    _db.SaveChanges();
                    storeRequisition.StoreRequisitionID = entity.StoreRequisitionID;
                //#region operation log
                //UserHelper.OperationLog(Request.UserHostAddress, "store requisition", "Success", User.Identity.Name, $"created store requisition {entity.StoreRequisitionID} ", Modules.Inventory);
                //#endregion

            }

            return Json(ModelState.IsValid ? new object() : ModelState.ToDataSourceResult());
        }


       // [AuthorizeRoles(InventoryConstants.PropertyRequistion)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionStoreRequisitions_Update([DataSourceRequest] DataSourceRequest request, StoreRequisition storeRequisition)
        {
            #region validation
            if (storeRequisition.IsTransfer && storeRequisition.ToStoreCode == null)
            {
                ModelState.AddModelError("invalidEntry", "Please select the Requester Store for Transfer");
            }

            if (storeRequisition.IsTransfer && storeRequisition.ToStoreCode == storeRequisition.StoreCode)
            {
                ModelState.AddModelError("invalidEntry", "Transfer is not allowed within one store");
            }

            if (storeRequisition.GLPeriodId != 0)
            {
                var inventoryPeriod = _db.InventoryPeriods.Find(storeRequisition.GLPeriodId);
                if (inventoryPeriod != null)
                {
                    if (
                        !((storeRequisition.RequestDate.Date >= inventoryPeriod.DateFrom) &&
                          (storeRequisition.RequestDate.Date <= inventoryPeriod.DateTo)))
                        ModelState.AddModelError("",
                            "Issue Date should be within the period: " + inventoryPeriod.Name + "(From " +
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

            if (storeRequisition.EmployeeId == 0)
                ModelState.AddModelError("requester", "Please choose requetster");

            //if (storeRequisition.BranchId == 0)
            //    ModelState.AddModelError("branch", "Please choose branch");
            //if (storeRequisition.RequestDate > DateTime.Now.Date)
            //{
            //    ModelState.AddModelError("future", "You are recording in future period,please correct the date.");
            //}


            if (!String.IsNullOrEmpty(storeRequisition.Reference))
            {
                var exists = _db.StoreRequisitions.Where(x => x.StoreRequisitionID != storeRequisition.StoreRequisitionID).Where(x => x.Reference == storeRequisition.Reference && x.IsVoid == false).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("Reference",
                        storeRequisition.Reference + " is already exists in the system.");
                }
            }

            if (storeRequisition.IsForOther && storeRequisition.ForBranchId == null)
            {
                ModelState.AddModelError("otherbranch", " Please select other branch.");
            }

            #endregion

            if (ModelState.IsValid)
            {
                    var entity = new StoreRequisition
                    {
                        StoreRequisitionID = storeRequisition.StoreRequisitionID,
                        Reference = storeRequisition.Reference,
                        OrgStructureId = UserInformationHelper.GetEmployeeReportsTo(storeRequisition.EmployeeId),
                        BranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(storeRequisition.StoreCode), //UserInformationHelper.GetEmployeeBranchId(User.Identity.Name),
                        EmployeeId = storeRequisition.EmployeeId, // UserInformationHelper.GetEmployeeId(User.Identity.Name),
                        RequestDate = storeRequisition.RequestDate,
                        StoreCode = storeRequisition.StoreCode,
                        Status = storeRequisition.Status,
                        Remark = storeRequisition.Remark,
                        GLPeriodId = storeRequisition.GLPeriodId,
                        IsFuel = storeRequisition.IsFuel,
                        IsTransfer = storeRequisition.IsTransfer,
                        ToStoreCode = storeRequisition.ToStoreCode,
                        IsForOther = storeRequisition.IsForOther,
                        ForBranchId = storeRequisition.ForBranchId,
                        ModelNo = storeRequisition.ModelNo,
                        JobOrderNo = storeRequisition.JobOrderNo,
                        PlateNo = storeRequisition.PlateNo,

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
                        CreatedBy = storeRequisition.CreatedBy,
                        DateCreated =storeRequisition.DateCreated,
                        ModifiedBy = User.Identity.Name,
                        LastModified = DateTime.Now,
                        OrgBranchName = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(storeRequisition.StoreCode).ToString(),
                    };

                    _db.StoreRequisitions.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "store requisition", "Success", User.Identity.Name, $"updated store requisition {entity.StoreRequisitionID} ", Modules.Inventory);
                #endregion
            }

            return Json(ModelState.IsValid ? new object() : ModelState.ToDataSourceResult());
        }

      //  [AuthorizeRoles(InventoryConstants.PropertyRequistion)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionStoreRequisitions_Destroy([DataSourceRequest] DataSourceRequest request, StoreRequisition storeRequisition)
        {
            if (ModelState.IsValid)
            {
                var entity = new StoreRequisition
                    {
                        StoreRequisitionID = storeRequisition.StoreRequisitionID,
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
                        PlateNo = storeRequisition.PlateNo,

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

                _db.StoreRequisitions.Attach(entity);
                _db.StoreRequisitions.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted store requisition  " + storeRequisition.StoreRequisitionID);
                #endregion
            }

            return Json(new[] { storeRequisition }.ToDataSourceResult(request, ModelState));
        }

        //[AuthorizeRoles(InventoryConstants.PropertyRequistion, InventoryConstants.PropertyImport)]

        //public ActionResult StoreRequistion_Import([DataSourceRequest] DataSourceRequest request, IEnumerable<HttpPostedFileBase> file)
        //{
        //    DataSet ds = new DataSet();
        //    foreach (HttpPostedFileBase fileBase in file)
        //    {
        //        var fileforImport = Request.Files["file"];
        //        if (fileforImport != null)
        //        {
        //            string fileExtension = System.IO.Path.GetExtension(fileforImport.FileName);
        //            if (fileExtension == ".xls" || fileExtension == ".xlsx" || fileExtension == ".xlsm")
        //            {
        //                string fileLocation = Server.MapPath("~/Content/Import/") + "SR" + DateTime.Now.Year.ToString() +
        //                                      DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() +
        //                                      DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() +
        //                                      DateTime.Now.Second.ToString() + fileforImport.FileName;
        //                if (System.IO.File.Exists(fileLocation))
        //                {
        //                    System.IO.File.Delete(fileLocation);
        //                }
        //                fileforImport.SaveAs(fileLocation);
        //                var excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
        //                                            ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
        //                //connection String for xls file format.
        //                if (fileExtension == ".xls")
        //                {
        //                    excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation +
        //                                            ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
        //                }
        //                //connection String for xlsx file format.
        //                else if (fileExtension == ".xlsx")
        //                {

        //                    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
        //                                            ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
        //                }
        //                else if (fileExtension == ".xlsm")
        //                {

        //                    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
        //                                            ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
        //                }
        //                //Create Connection to Excel work book and add oledb namespace
        //                OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
        //                excelConnection.Open();
        //                DataTable dt = new DataTable();

        //                dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
        //                if (dt == null)
        //                {
        //                    return null;
        //                }

        //                String[] excelSheets = new String[dt.Rows.Count];
        //                int t = 0;
        //                //excel data saves in temp file here.
        //                foreach (DataRow row in dt.Rows)
        //                {
        //                    excelSheets[t] = row["TABLE_NAME"].ToString();
        //                    t++;
        //                }
        //                OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


        //                string query = string.Format("Select * from [{0}]", excelSheets[0]);
        //                using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
        //                {
        //                    dataAdapter.Fill(ds);
        //                }

        //            }
        //        }
        //    }
        //    List<StoreRequistionImport> storerequistions = new List<StoreRequistionImport>();
        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        for (int j = 0; j <= ds.Tables[0].Rows.Count - 1; j++)
        //        {
        //            StoreRequistionImport import = new StoreRequistionImport
        //            {
        //                No = ds.Tables[0].Rows[j].ItemArray[0].ToString(),
        //                StoreCode = ds.Tables[0].Rows[j].ItemArray[1].ToString(),
        //                WorkUnit = ds.Tables[0].Rows[j].ItemArray[2].ToString(),
        //                Requestor = ds.Tables[0].Rows[j].ItemArray[3].ToString(),
        //                RequestDate = DateTime.Parse(ds.Tables[0].Rows[j].ItemArray[4].ToString()),
        //                ItemCategoryCode = ds.Tables[0].Rows[j].ItemArray[5].ToString(),
        //                ItemCode = ds.Tables[0].Rows[j].ItemArray[6].ToString(),
        //                PartNo = ds.Tables[0].Rows[j].ItemArray[7].ToString(),
        //                RequestedQuantity = decimal.Parse(ds.Tables[0].Rows[j].ItemArray[8].ToString()),
        //                ApprovedQuantity = decimal.Parse(ds.Tables[0].Rows[j].ItemArray[9].ToString()),
        //                IssuedQuantity = decimal.Parse(ds.Tables[0].Rows[j].ItemArray[10].ToString()),
        //                DocumentReference = ds.Tables[0].Rows[j].ItemArray[11].ToString()
        //            };
        //            storerequistions.Add(import);



        //        }
        //    }
        //    var imports = storerequistions.GroupBy(x => x.No).Select(x => x.FirstOrDefault());
        //    foreach (var import in imports)
        //    {
        //        if (import != null)
        //        {
        //            StoreRequisition storeRequisition = new StoreRequisition
        //            {
        //                StoreCode = import.StoreCode,
        //                OrgStructureId = 1,
        //                IsFuel = false,
        //                EmployeeId = 1,
        //                RequestDate = import.RequestDate,
        //                Reference = import.DocumentReference,
        //                Status = Enum.GetName(typeof(OperationStatuses), 5)
        //            };

        //            List<StoreRequisitionItem> items = new List<StoreRequisitionItem>();
        //            foreach (StoreRequistionImport source in storerequistions.Where(x => x.No == import.No))
        //            {
        //                StoreRequisitionItem item = new StoreRequisitionItem
        //                {
        //                    ItemCode = source.ItemCode,
        //                    RequestedQuantity = source.RequestedQuantity,
        //                    ApprovedQuantity = source.ApprovedQuantity,
        //                    IssuedQuantity = source.IssuedQuantity,
        //                    PartNo = source.PartNo
        //                };
        //                items.Add(item);
        //            }
        //            storeRequisition.StoreRequisitionItem = items;
        //            storeRequisition.CreatedBy = User.Identity.Name;
        //            storeRequisition.DateCreated = DateTime.Now;
        //            _db.StoreRequisitions.Add(storeRequisition);
        //        }
        //        _db.SaveChanges();
        //    }
        //    return RedirectToAction("Index");
        //}


        public JsonResult GetStatus(int id)
        {
            var formattedData = new
            {
                Message = "There are items with status not updated.",
                Status = "False"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }


     //   [AuthorizeRoles(InventoryConstants.PropertyRequistion, InventoryConstants.PropertyImport)]
        public JsonResult SendForApproval(int id)
        {
            var record = _db.StoreRequisitions.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.StoreRequisitionItem.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.StoreRequisitions.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "store requisition", "Success", User.Identity.Name, $"send for approval store requisition {id} ", Modules.Inventory);
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

      //  [AuthorizeRoles(InventoryConstants.PropertyVoid)]
        //public JsonResult Void(int id)
        //{
        //    var record = _db.StoreRequisitions.Find(id);
        //    if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred && record.StoreRequisitionItem.Count > 0)
        //    {
        //        #region validate issue

        //        var issue = _db.Issues
        //            .Where(x => x.IsVoid == false).FirstOrDefault(x => x.StoreRequisitionID == record.StoreRequisitionID);

        //        if (issue != null)
        //        {
        //            var formattedData2 = new
        //            {
        //                Message = $"This store requisition can not be void since it is referred in issue number {issue.IssueID}.",
        //                Status = "NOT_OK"
        //            };
        //            return Json(formattedData2, JsonRequestBehavior.AllowGet);
        //        }
        //        #endregion
        //        #region validate transfer send

        //        var itemTransfer = _db.ItemTransfers
        //            .Where(x => x.IsVoid == false).FirstOrDefault(x => x.StoreRequisitionID == record.StoreRequisitionID);

        //        if (itemTransfer != null)
        //        {
        //            var formattedData2 = new
        //            {
        //                Message = $"This store requisition can not be void since it is referred in item transfer send {itemTransfer.ItemTransferID}.",
        //                Status = "NOT_OK"
        //            };
        //            return Json(formattedData2, JsonRequestBehavior.AllowGet);
        //        }
        //        #endregion

        //        record.Status = Enum.GetName(typeof(OperationStatuses), 10);
        //        record.IsVoid = true;
        //        record.VoidBy = User.Identity.Name;
        //        record.VoidTime = DateTime.Now;
        //        _db.StoreRequisitions.Attach(record);
        //        _db.Entry(record).State = EntityState.Modified;
        //        _db.SaveChanges();

        //        //copy
        //        CopyRecord(id);

        //        #region operation log
        //        UserHelper.OperationLog(Request.UserHostAddress, "store requisition", "Success", User.Identity.Name, $"void store requisition {id} ", Modules.Inventory);
        //        #endregion

        //        var formattedData = new
        //        {
        //            Message = "Void Successful.",
        //            Status = "OK"
        //        };
        //        return Json(formattedData, JsonRequestBehavior.AllowGet);
        //    }
        //    if (record == null)
        //    {
        //        var formattedData = new
        //        {
        //            Message = "The record is not found.",
        //            Status = "NOT_OK"
        //        };
        //        return Json(formattedData, JsonRequestBehavior.AllowGet);
        //    }

        //    if (record.StoreRequisitionItem.Count == 0)
        //    {
        //        var formattedData = new
        //        {
        //            Message = "Please add line items first.",
        //            Status = "NOT_OK"
        //        };
        //        return Json(formattedData, JsonRequestBehavior.AllowGet);
        //    }
        //    if (record.IsVoid)
        //    {
        //        var formattedData = new
        //        {
        //            Message = "The record is already void.You can not void again.",
        //            Status = "NOT_OK"
        //        };
        //        return Json(formattedData, JsonRequestBehavior.AllowGet);
        //    }
        //    if (record.IsOnlineTransferred)
        //    {
        //        var formattedData = new
        //        {
        //            Message = "The record is transferred to GL.You can not void .",
        //            Status = "NOT_OK"
        //        };
        //        return Json(formattedData, JsonRequestBehavior.AllowGet);
        //    }
        //    if (!record.IsOnlineApproved)
        //    {
        //        var formattedData = new
        //        {
        //            Message = "The record is not approved hence you can modify by editing.",
        //            Status = "NOT_OK"
        //        };
        //        return Json(formattedData, JsonRequestBehavior.AllowGet);
        //    }


        //    return null;
        //}

        private void CopyRecord(int id)
        {
            var record = _db.StoreRequisitions.Find(id);
            if (record != null)
            {
                #region header
                var entity = new StoreRequisition
                {
                    Reference = record.Reference,
                    OrgStructureId = UserInformationHelper.GetEmployeeReportsTo(record.EmployeeId),
                    BranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(record.StoreCode), //UserInformationHelper.GetEmployeeBranchId(User.Identity.Name),
                    EmployeeId = record.EmployeeId, // UserInformationHelper.GetEmployeeId(User.Identity.Name),
                    RequestDate = record.RequestDate.Date,// DateTime.Today,
                    StoreCode = record.StoreCode,
                    Status = Enum.GetName(typeof(OperationStatuses), 5),
                    Remark = record.Remark,
                    GLPeriodId = record.GLPeriodId,
                    
                    IsFuel = false,
                    IsTransfer = record.IsTransfer,
                    ToStoreCode = record.ToStoreCode,
                    IsForOther = record.IsForOther,
                    ForBranchId = record.ForBranchId,
                    ModelNo = record.ModelNo,
                    JobOrderNo = record.JobOrderNo,
                    PlateNo = record.PlateNo,
                   
                    CreatedBy = record.CreatedBy,
                    DateCreated = DateTime.Now,
                    OrgBranchName = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(record.StoreCode).ToString(),

                };

                _db.StoreRequisitions.Add(entity);
                _db.SaveChanges();
                #endregion
                foreach (var storeRequisitionItem in record.StoreRequisitionItem)
                {
                    #region lines
                    var line = new StoreRequisitionItem
                    {
                        StoreRequisitionID = entity.StoreRequisitionID,
                        ItemCategoryCode = storeRequisitionItem.ItemCategoryCode,
                        ItemCode = storeRequisitionItem.ItemCode,
                        PartNo = storeRequisitionItem.PartNo,
                        ModelNo = storeRequisitionItem.ModelNo,
                        SerialNo = storeRequisitionItem.SerialNo,
                        ChasisNo = storeRequisitionItem.ChasisNo,
                        RequestedQuantity = storeRequisitionItem.RequestedQuantity,
                        ApprovedQuantity = storeRequisitionItem.RequestedQuantity,
                        IssuedQuantity = storeRequisitionItem.RequestedQuantity,
                        PurchaseQuantity = storeRequisitionItem.PurchaseQuantity,
                        IsApproved = storeRequisitionItem.IsApproved,
                        ItemSpecification = storeRequisitionItem.ItemSpecification,
                    };

                    _db.StoreRequisitionItems.Add(line);
                    _db.SaveChanges();

                    #endregion

                }

            }

        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}