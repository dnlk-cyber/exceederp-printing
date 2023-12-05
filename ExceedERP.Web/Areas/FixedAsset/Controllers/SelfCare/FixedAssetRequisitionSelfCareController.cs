using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Requisitions;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Areas.Inventory.Models;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using EntityState = System.Data.Entity.EntityState;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.SelfCare
{

    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
    public class FixedAssetRequisitionSelfCareController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly ApplicationDbContext _userdb = new ApplicationDbContext();

        
        public ActionResult Index()
        {
            PopulateViewData();
            ViewBag.User = User.Identity.Name;
            return View();
        }
        private void PopulateViewData()
        {
            var assetCategories = _db.FixedAssetCategories;
            var assetSubCategories = _db.FixedAssetSubCategories;
            var fixedAsset = _db.AssetRegistrations;
            var assetSubCategoryNames = _db.FixedAssetSubCategoryNames;

            var costCenter = _db.BranchCostCenter;
            var branches = _db.Branch;

            ViewData["Branches"] = branches;
            ViewData["SubCategory"] = assetSubCategories;
            ViewData["Category"] = assetCategories;
            ViewData["SubCategoryName"] = assetSubCategoryNames;
            ViewData["FixedAsset"] = fixedAsset;

            var store = _db.InventoryStores.AsQueryable();
            ViewData["storesInCompany"] = store;

            var employee = _db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstName + " " + i.MiddleName + " " + i.LastName,
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

            var fixedAssetPeriods = _db.FixedAssetPeriods;
            var selectedfixedAssetPeriods = fixedAssetPeriods.Select(
                i => new
                {
                    Name = i.Name,
                    GLPeriodId = i.FixedAssetPeriodId
                }).AsQueryable();
            ViewData["fixedassetPeriods"] = selectedfixedAssetPeriods;

        }

       
        public ActionResult FixedAssetRequisitions_Read([DataSourceRequest] DataSourceRequest request)
        {
            var user = _userdb.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
            var storekeepers = _db.StoreKeeperAssignments.Where(x => x.StoreKeeper == user.Id).ToList();
            List<string> userStores = UserInformationHelper.GetUserStores(User.Identity.Name);
            //int userworkunit = UserInformationHelper.GetUserReportsTo(User.Identity.Name);
            List<int> userworkunit = UserInformationHelper.GetUserHeadWorkUnitAndChildWorkUnits(User.Identity.Name);//.GetUserReportsToAndChildWorkUnits(User.Identity.Name);

            var storeRequisitions = _db.FixedAssetRequisitions.ToList();

            if (System.Web.HttpContext.Current.User.IsInRole(HrmConstants.HRDepartmentHead)
           && System.Web.HttpContext.Current.User.IsInRole(HrmConstants.HREmployeeSelfService))
            {
                storeRequisitions = storeRequisitions.Where(x => userworkunit.Contains(x.OrgStructureId)
                                                                    || x.CreatedBy == User.Identity.Name).ToList();
            }

            else if (System.Web.HttpContext.Current.User.IsInRole(HrmConstants.HRDepartmentHead))
            {
                storeRequisitions = storeRequisitions.Where(x => userworkunit.Contains(x.OrgStructureId) && userworkunit != null
                                                                     || x.CreatedBy == User.Identity.Name).ToList();
            }
            else if (System.Web.HttpContext.Current.User.IsInRole(HrmConstants.HREmployeeSelfService))
            {
                storeRequisitions = storeRequisitions.Where(x => x.CreatedBy == User.Identity.Name).ToList();
            }

            //if (System.Web.HttpContext.Current.User.IsInRole(FixedAssetConstants.FixedAssetRequistionSecondApprover)
            //    && System.Web.HttpContext.Current.User.IsInRole(FixedAssetConstants.FixedAssetRequistionApprover)
            //    && System.Web.HttpContext.Current.User.IsInRole(FixedAssetConstants.FixedAssetRequistion))
            //{

            //    if (!storekeepers.Any())
            //    {
            //        storeRequisitions = storeRequisitions.Where(x => x.NextApprover == User.Identity.Name || userworkunit.Contains(x.OrgStructureId) || x.CreatedBy == User.Identity.Name).ToList();
            //    }
            //    else
            //    {
            //        storeRequisitions = storeRequisitions.Where(x => userStores.Contains(x.StoreCode) || x.CreatedBy == User.Identity.Name).ToList();
            //    }
            //}

            //else if (System.Web.HttpContext.Current.User.IsInRole(FixedAssetConstants.FixedAssetRequistionApprover)
            //    && System.Web.HttpContext.Current.User.IsInRole(FixedAssetConstants.FixedAssetRequistion))
            //{
            //    storeRequisitions = storeRequisitions.Where(x => userworkunit.Contains(x.OrgStructureId) || x.CreatedBy == User.Identity.Name).ToList();
            //}
            //else if (System.Web.HttpContext.Current.User.IsInRole(FixedAssetConstants.FixedAssetRequistionSecondApprover)
            //    && System.Web.HttpContext.Current.User.IsInRole(FixedAssetConstants.FixedAssetRequistion))
            //{
            //    storeRequisitions = storeRequisitions.Where(x => x.NextApprover == User.Identity.Name || x.CreatedBy == User.Identity.Name).ToList();
            //}
            //else if (System.Web.HttpContext.Current.User.IsInRole(FixedAssetConstants.FixedAssetRequistionSecondApprover)
            //    && System.Web.HttpContext.Current.User.IsInRole(FixedAssetConstants.FixedAssetRequistionApprover))
            //{
            //    if (!storekeepers.Any())
            //    {
            //        storeRequisitions = storeRequisitions.Where(x => x.NextApprover == User.Identity.Name || userworkunit.Contains(x.OrgStructureId) || x.CreatedBy == User.Identity.Name).ToList();
            //    }
            //    else
            //    {
            //        storeRequisitions = storeRequisitions.Where(x => userStores.Contains(x.StoreCode) || x.CreatedBy == User.Identity.Name).ToList();
            //    }
            //}

            //else if (System.Web.HttpContext.Current.User.IsInRole(FixedAssetConstants.FixedAssetRequistionSecondApprover))
            //{
            //    storeRequisitions = storeRequisitions.Where(x => x.NextApprover == User.Identity.Name || x.CreatedBy == User.Identity.Name).ToList();
            //}
            //else if (System.Web.HttpContext.Current.User.IsInRole(FixedAssetConstants.FixedAssetRequistionApprover))
            //{
            //    if (!storekeepers.Any())
            //    {
            //        storeRequisitions = storeRequisitions.Where(x => userworkunit.Contains(x.OrgStructureId) && userworkunit != null
            //                                                         || x.CreatedBy == User.Identity.Name).ToList();
            //    }
            //    else
            //    {
            //        storeRequisitions = storeRequisitions.Where(x => userStores.Contains(x.StoreCode) || x.CreatedBy == User.Identity.Name).ToList();
            //    }
            //}
            //else if (System.Web.HttpContext.Current.User.IsInRole(FixedAssetConstants.FixedAssetRequistion))
            //{
            //    storeRequisitions = storeRequisitions.Where(x => x.CreatedBy == User.Identity.Name).ToList();
            //}
            var result = storeRequisitions.ToDataSourceResult(request, storeRequisition => new
                {
                    FixedAssetRequisitionId = storeRequisition.FixedAssetRequisitionId,
                    Reference = storeRequisition.Reference,
                    OrgStructureId = storeRequisition.OrgStructureId,
                    BranchId = storeRequisition.BranchId,
                    IsForOther = storeRequisition.IsForOther,
                    ForBranchId = storeRequisition.ForBranchId,
                    IsTransfer = storeRequisition.IsTransfer,
                    ToStoreCode = storeRequisition.ToStoreCode,
                    EmployeeId = storeRequisition.EmployeeId,
                    GLPeriodId = storeRequisition.GLPeriodId,
                    CreatedBy = storeRequisition.CreatedBy,
                    DateCreated = storeRequisition.DateCreated,
                    RequestDate = storeRequisition.RequestDate,
                    StoreCode = storeRequisition.StoreCode,
                    PlateNo = storeRequisition.PlateNo,
                    NextApprover = storeRequisition.NextApprover,
                    Status = storeRequisition.Status,
                    Remark = storeRequisition.Remark,
                    IsVoid = storeRequisition.IsVoid,
                    VoidBy = storeRequisition.VoidBy,
                    VoidTime = storeRequisition.VoidTime,
                    IsOnlineApproved = storeRequisition.IsOnlineApproved,
                    OnlineApprovedBy = storeRequisition.OnlineApprovedBy,
                    OnlineApprovedTime = storeRequisition.OnlineApprovedTime,

                    IsMiddleOnlineApproved = storeRequisition.IsMiddleOnlineApproved,
                    MiddleOnlineApprovedBy = storeRequisition.MiddleOnlineApprovedBy,
                    MiddleOnlineApprovedTime = storeRequisition.MiddleOnlineApprovedTime,

                    IsSecondOnlineApproved = storeRequisition.IsSecondOnlineApproved,
                    SecondOnlineApprovedBy = storeRequisition.SecondOnlineApprovedBy,
                    SecondOnlineApprovedTime = storeRequisition.SecondOnlineApprovedTime,

                    IsSendForApproval = storeRequisition.IsSendForApproval,
                    SendForApprovalBy = storeRequisition.SendForApprovalBy,
                    SendForApprovalTime = storeRequisition.SendForApprovalTime,

                    IsMiddleSendForApproval = storeRequisition.IsMiddleSendForApproval,
                    MiddleSendForApprovalBy = storeRequisition.MiddleSendForApprovalBy,
                    MiddleSendForApprovalTime = storeRequisition.MiddleSendForApprovalTime,

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



        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetRequisitions_Create([DataSourceRequest] DataSourceRequest request, FixedAssetRequisition storeRequisition)
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

            
            if (!String.IsNullOrEmpty(storeRequisition.Reference))
            {
                var exists = _db.FixedAssetRequisitions.Where(x => x.Reference == storeRequisition.Reference).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("Reference",
                        storeRequisition.Reference + " is already exists in the system.");
                   
                }
            }

            if (storeRequisition.EmployeeId == 0)
                ModelState.AddModelError("requester", "Please choose requetster");

            //if (storeRequisition.BranchId == 0)
            //    ModelState.AddModelError("branch", "Please choose branch");

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetRequisition
                    {
                        Reference = storeRequisition.Reference,
                        OrgStructureId = UserInformationHelper.GetEmployeeWorkUnitById(storeRequisition.EmployeeId),
                        BranchId = InventoryHelper.GetStoreBranch(storeRequisition.StoreCode), //UserInformationHelper.GetEmployeeBranchId(User.Identity.Name),
                        IsForOther = storeRequisition.IsForOther,
                        ForBranchId = storeRequisition.ForBranchId,
                        EmployeeId = storeRequisition.EmployeeId, // UserInformationHelper.GetEmployeeId(User.Identity.Name),
                        RequestDate = storeRequisition.RequestDate,// DateTime.Today,
                        GLPeriodId = storeRequisition.GLPeriodId,
                        StoreCode = storeRequisition.StoreCode,
                        PlateNo = storeRequisition.PlateNo,
                        NextApprover = storeRequisition.NextApprover,
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
                        IsMiddleOnlineApproved = storeRequisition.IsMiddleOnlineApproved,
                        MiddleOnlineApprovedBy = storeRequisition.MiddleOnlineApprovedBy,
                        MiddleOnlineApprovedTime = storeRequisition.MiddleOnlineApprovedTime,
                        IsSecondOnlineApproved = storeRequisition.IsSecondOnlineApproved,
                        SecondOnlineApprovedBy = storeRequisition.SecondOnlineApprovedBy,
                        SecondOnlineApprovedTime = storeRequisition.SecondOnlineApprovedTime,
                        IsOnlineTransferred = storeRequisition.IsOnlineTransferred,
                        IsOnlineTransferredBy = storeRequisition.IsOnlineTransferredBy,
                        OnlineTransferredTime = storeRequisition.OnlineTransferredTime,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now
                    };

                    _db.FixedAssetRequisitions.Add(entity);
                    _db.SaveChanges();
                    storeRequisition.FixedAssetRequisitionId = entity.FixedAssetRequisitionId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetRequisition", "Success", User.Identity.Name, $"created FixedAssetRequisition {entity.FixedAssetRequisitionId} Emp {entity.EmployeeId}", Modules.FixedAsset);
                    #endregion

            }

            return Json(ModelState.IsValid ? new object() : ModelState.ToDataSourceResult());
        }


    

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetRequisitions_Update([DataSourceRequest] DataSourceRequest request, FixedAssetRequisition storeRequisition)
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


            bool valid = true;
            if (!String.IsNullOrEmpty(storeRequisition.Reference))
            {
                var exists = _db.FixedAssetRequisitions.Where(x => x.Reference == storeRequisition.Reference).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("Reference",
                        storeRequisition.Reference + " is already exists in the system.");
                    valid = false;
                }
            }

            if (storeRequisition.EmployeeId == 0)
                ModelState.AddModelError("requester", "Please choose requetster");

            //if (storeRequisition.BranchId == 0)
            //    ModelState.AddModelError("branch", "Please choose branch");

            #endregion

            if (ModelState.IsValid)
            {

                if (valid)
                {
                    var entity = new FixedAssetRequisition
                    {
                        FixedAssetRequisitionId = storeRequisition.FixedAssetRequisitionId,
                        Reference = storeRequisition.Reference,
                        OrgStructureId = UserInformationHelper.GetEmployeeWorkUnitById(storeRequisition.EmployeeId),
                        BranchId = InventoryHelper.GetStoreBranch(storeRequisition.StoreCode), //UserInformationHelper.GetEmployeeBranchId(User.Identity.Name),
                        IsForOther = storeRequisition.IsForOther,
                        ForBranchId = storeRequisition.ForBranchId,
                        EmployeeId = storeRequisition.EmployeeId, // UserInformationHelper.GetEmployeeId(User.Identity.Name),
                        RequestDate = storeRequisition.RequestDate,// DateTime.Today,
                        GLPeriodId = storeRequisition.GLPeriodId,
                        StoreCode = storeRequisition.StoreCode,
                        PlateNo = storeRequisition.PlateNo,
                        NextApprover = storeRequisition.NextApprover,
                        Status = storeRequisition.Status,
                        Remark = storeRequisition.Remark,
                        IsFuel = storeRequisition.IsFuel,
                        IsTransfer = storeRequisition.IsTransfer,
                        ToStoreCode = storeRequisition.ToStoreCode,
                        IsVoid = storeRequisition.IsVoid,
                        VoidBy = storeRequisition.VoidBy,
                        VoidTime = storeRequisition.VoidTime,
                        IsOnlineApproved = storeRequisition.IsOnlineApproved,
                        OnlineApprovedBy = storeRequisition.OnlineApprovedBy,
                        OnlineApprovedTime = storeRequisition.OnlineApprovedTime,
                        IsMiddleOnlineApproved = storeRequisition.IsMiddleOnlineApproved,
                        MiddleOnlineApprovedBy = storeRequisition.MiddleOnlineApprovedBy,
                        MiddleOnlineApprovedTime = storeRequisition.MiddleOnlineApprovedTime,
                        IsSecondOnlineApproved = storeRequisition.IsSecondOnlineApproved,
                        SecondOnlineApprovedBy = storeRequisition.SecondOnlineApprovedBy,
                        SecondOnlineApprovedTime = storeRequisition.SecondOnlineApprovedTime,
                        IsOnlineTransferred = storeRequisition.IsOnlineTransferred,
                        IsOnlineTransferredBy = storeRequisition.IsOnlineTransferredBy,
                        OnlineTransferredTime = storeRequisition.OnlineTransferredTime,
                        CreatedBy = storeRequisition.CreatedBy,
                        DateCreated =storeRequisition.DateCreated,
                        ModifiedBy = User.Identity.Name,
                        LastModified = DateTime.Now
                    };

                    _db.FixedAssetRequisitions.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetRequisition", "Success", User.Identity.Name, $"update FixedAssetRequisition {entity.FixedAssetRequisitionId} Emp {entity.EmployeeId}", Modules.FixedAsset);
                    #endregion

                }

            }

            return Json(ModelState.IsValid ? new object() : ModelState.ToDataSourceResult());
        }

       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetRequisitions_Destroy([DataSourceRequest] DataSourceRequest request, FixedAssetRequisition storeRequisition)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetRequisition
                    {
                        FixedAssetRequisitionId = storeRequisition.FixedAssetRequisitionId,
                        Reference = storeRequisition.Reference,
                        BranchId = storeRequisition.BranchId,
                        OrgStructureId = storeRequisition.OrgStructureId,
                        EmployeeId = storeRequisition.EmployeeId,
                        RequestDate = storeRequisition.RequestDate,
                        GLPeriodId = storeRequisition.GLPeriodId,
                        StoreCode = storeRequisition.StoreCode,
                        NextApprover = storeRequisition.NextApprover,
                        Status = storeRequisition.Status,
                        Remark = storeRequisition.Remark,
                        IsVoid = storeRequisition.IsVoid,
                        IsTransfer = storeRequisition.IsTransfer,
                        ToStoreCode = storeRequisition.ToStoreCode,
                        VoidBy = storeRequisition.VoidBy,
                        VoidTime = storeRequisition.VoidTime,
                        IsOnlineApproved = storeRequisition.IsOnlineApproved,
                        OnlineApprovedBy = storeRequisition.OnlineApprovedBy,
                        OnlineApprovedTime = storeRequisition.OnlineApprovedTime,
                        IsMiddleOnlineApproved = storeRequisition.IsMiddleOnlineApproved,
                        MiddleOnlineApprovedBy = storeRequisition.MiddleOnlineApprovedBy,
                        MiddleOnlineApprovedTime = storeRequisition.MiddleOnlineApprovedTime,
                        IsSecondOnlineApproved = storeRequisition.IsSecondOnlineApproved,
                        SecondOnlineApprovedBy = storeRequisition.SecondOnlineApprovedBy,
                        SecondOnlineApprovedTime = storeRequisition.SecondOnlineApprovedTime,
                        IsOnlineTransferred = storeRequisition.IsOnlineTransferred,
                        IsOnlineTransferredBy = storeRequisition.IsOnlineTransferredBy,
                        OnlineTransferredTime = storeRequisition.OnlineTransferredTime
                    };

                _db.FixedAssetRequisitions.Attach(entity);
                _db.FixedAssetRequisitions.Remove(entity);
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Delete FixedAssetRequisition {storeRequisition.FixedAssetRequisitionId} Emp {storeRequisition.EmployeeId}", Modules.FixedAsset);
                #endregion

            }

            return Json(new[] { storeRequisition }.ToDataSourceResult(request, ModelState));
        }

        
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
            List<FixedAssetRequistionImport> storerequistions = new List<FixedAssetRequistionImport>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int j = 0; j <= ds.Tables[0].Rows.Count - 1; j++)
                {
                    FixedAssetRequistionImport import = new FixedAssetRequistionImport
                    {
                        No = ds.Tables[0].Rows[j].ItemArray[0].ToString(),
                        StoreCode = ds.Tables[0].Rows[j].ItemArray[1].ToString(),
                        WorkUnit = ds.Tables[0].Rows[j].ItemArray[2].ToString(),
                        Requestor = ds.Tables[0].Rows[j].ItemArray[3].ToString(),
                        RequestDate = DateTime.Parse(ds.Tables[0].Rows[j].ItemArray[4].ToString()),
                        ItemCategoryCode = ds.Tables[0].Rows[j].ItemArray[5].ToString(),
                        ItemCode = ds.Tables[0].Rows[j].ItemArray[6].ToString(),
                        PartNo = ds.Tables[0].Rows[j].ItemArray[7].ToString(),
                        RequestedQuantity = int.Parse(ds.Tables[0].Rows[j].ItemArray[8].ToString()),
                        ApprovedQuantity = int.Parse(ds.Tables[0].Rows[j].ItemArray[9].ToString()),
                        IssuedQuantity = int.Parse(ds.Tables[0].Rows[j].ItemArray[10].ToString()),
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
                    FixedAssetRequisition storeRequisition = new FixedAssetRequisition
                    {
                        StoreCode = import.StoreCode,
                        OrgStructureId = 1,
                        IsFuel = false,
                        EmployeeId = 1,
                        RequestDate = import.RequestDate,
                        Reference = import.DocumentReference,
                        Status = Enum.GetName(typeof(OperationStatuses), 5)
                    };

                    List<FixedAssetRequisitionItem> items = new List<FixedAssetRequisitionItem>();
                    foreach (FixedAssetRequistionImport source in storerequistions.Where(x => x.No == import.No))
                    {
                        FixedAssetRequisitionItem item = new FixedAssetRequisitionItem
                        {
                            //CategoryCode = source.ItemCode,
                            //SubCategoryCode = source.ItemCode,
                            RequestedQuantity = source.RequestedQuantity,
                            ApprovedQuantity = source.ApprovedQuantity,
                            IssuedQuantity = source.IssuedQuantity,

                        };
                        items.Add(item);
                    }
                    storeRequisition.FixedAssetRequisitionItem = items;
                    storeRequisition.CreatedBy = User.Identity.Name;
                    storeRequisition.DateCreated = DateTime.Now;
                    _db.FixedAssetRequisitions.Add(storeRequisition);
                }
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public JsonResult GetStatus(int id)
        {
            var formattedData = new
            {
                Message = "There are items with status not updated.",
                Status = "False"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        //First Send For Approval
       
        public JsonResult SendForApproval(int id)
        {
            var record = _db.FixedAssetRequisitions.Find(id);

            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.FixedAssetRequisitionItem.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.IsVoid = false;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.FixedAssetRequisitions.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
                var formattedData = new
                {
                    Message = "Successfully Sent.",
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

            if (record.FixedAssetRequisitionItem.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add line items first.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsOnlineApproved)
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

        //Middle Send For Approval 
       
        public JsonResult SendForSecondApproval(int id,string user)
        {
            var record = _db.FixedAssetRequisitions.Find(id);
            if (record != null && !record.IsMiddleSendForApproval && !record.IsMiddleOnlineApproved && record.FixedAssetRequisitionItem.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 13);
                record.NextApprover = user;
                record.IsMiddleSendForApproval = true;
                record.IsVoid = false;
                record.MiddleSendForApprovalBy = User.Identity.Name;
                record.MiddleSendForApprovalTime = DateTime.Now;
                _db.FixedAssetRequisitions.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
                var formattedData = new
                {
                    Message = "Successfully Sent.",
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

            if (record.FixedAssetRequisitionItem.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add line items first.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsMiddleOnlineApproved)
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

        [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
        public JsonResult Void(int id)
        {
            var record = _db.FixedAssetRequisitions.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred && record.FixedAssetRequisitionItem.Count > 0)
            {
                #region validate issue

                var issue = _db.FixedAssetIssues
                    .Where(x => x.IsVoid == false).FirstOrDefault(x => x.FixedAssetRequisitionId == record.FixedAssetRequisitionId);

                if (issue != null)
                {
                    var formattedData2 = new
                    {
                        Message = $"This store requisition can not be void since it is referred in issue number {issue.FixedAssetIssueID}.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData2, JsonRequestBehavior.AllowGet);
                }
                #endregion
            

                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsVoid = true;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.IsOnlineApproved = false;
                record.IsMiddleOnlineApproved = false;
                record.IsMiddleSendForApproval = false;
                record.IsSecondSendForApproval = false;
                record.IsSecondOnlineApproved = false;
                record.IsOnlineTransferred = false;
                record.IsSendForApproval = false;
                record.Status = Enum.GetName(typeof(OperationStatuses), 5);
                _db.FixedAssetRequisitions.Attach(record);
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

            if (record.FixedAssetRequisitionItem.Count == 0)
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
            var record = _db.FixedAssetRequisitions.Find(id);
            if (record != null)
            {
                #region header
                var entity = new FixedAssetRequisition
                {
                    Reference = record.Reference,
                    OrgStructureId = UserInformationHelper.GetEmployeeWorkUnitById(record.EmployeeId),
                    BranchId = InventoryHelper.GetStoreBranch(record.StoreCode), //UserInformationHelper.GetEmployeeBranchId(User.Identity.Name),
                    IsForOther = record.IsForOther,
                    ForBranchId = record.ForBranchId,
                    EmployeeId = record.EmployeeId, // UserInformationHelper.GetEmployeeId(User.Identity.Name),
                    RequestDate = record.RequestDate,// DateTime.Today,
                    GLPeriodId = record.GLPeriodId,
                    StoreCode = record.StoreCode,
                    PlateNo = record.PlateNo,
                    NextApprover = "",
                    Status = Enum.GetName(typeof(OperationStatuses), 5),
                    Remark = record.Remark,
                    IsVoid = record.IsVoid,
                    IsFuel = false,
                    IsTransfer = record.IsTransfer,
                    ToStoreCode = record.ToStoreCode,
                    

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    OrgBranchName = InventoryHelper.GetStoreBranch(record.StoreCode).ToString(),

                };

                _db.FixedAssetRequisitions.Add(entity);
                _db.SaveChanges();
                #endregion
                foreach (var storeRequisitionItem in record.FixedAssetRequisitionItem)
                {
                    #region lines
                    var line = new FixedAssetRequisitionItem
                    {
                        FixedAssetRequisitionId = entity.FixedAssetRequisitionId,
                        FixedAssetCategoryID = storeRequisitionItem.FixedAssetCategoryID,
                        FixedAssetSubCategoryID = storeRequisitionItem.FixedAssetSubCategoryID,
                        Name = storeRequisitionItem.Name,
                        PartNo = storeRequisitionItem.PartNo,
                        ModelNo = storeRequisitionItem.ModelNo,
                        SerialNo = storeRequisitionItem.SerialNo,
                        ChasisNo = storeRequisitionItem.ChasisNo,
                        UoM = storeRequisitionItem.UoM,
                        RequestedQuantity = storeRequisitionItem.RequestedQuantity,
                        ApprovedQuantity = storeRequisitionItem.RequestedQuantity,
                        IssuedQuantity = storeRequisitionItem.RequestedQuantity,
                        PurchaseQuantity = storeRequisitionItem.PurchaseQuantity,
                        IsApproved = storeRequisitionItem.IsApproved,
                        ItemSpecification = storeRequisitionItem.ItemSpecification,
                    };

                    _db.FixedAssetRequisitionItems.Add(line);
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