using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Office2010.Excel;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Core.Domain.Maintenance.SpareStoreRequisition;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using EntityState = System.Data.Entity.EntityState;

namespace ExceedERP.Web.Areas.Maintenance.Controllers.WorkFlows
{

    //[AuthorizeRoles(MaintenanceRoles.MaintenanceOrderUser, MaintenanceRoles.MaintenanceWorkShopUser, MaintenanceRoles.ServiceUser)]

    public class SparePartStoreRequisitionController : Controller
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

        public ActionResult SparePartStoreRequisitions_Read([DataSourceRequest] DataSourceRequest request, Guid? id)
        {            
            var storeRequisitions = _db.StoreRequisitions
                                    .Where(q=>q.IsForMaintenance)
                                    .AsQueryable();

            if (id != null)
            {
                storeRequisitions = storeRequisitions.Where(x => x.ServiceJobOrderNo == id.ToString());
            }

            
            var result = storeRequisitions.ToDataSourceResult(request, storeRequisition => new
                {
                    StoreRequisitionID = storeRequisition.StoreRequisitionID,
                    //CurrentAvgPrice = storeRequisition.,
                    IsForMaintenance = storeRequisition.IsForMaintenance,
                    Reference = storeRequisition.Reference,
                    OrgStructureId = storeRequisition.OrgStructureId,
                    BranchId = storeRequisition.BranchId,
                    BusinessUnitId=storeRequisition.BusinessUnitId,
                    IsTransfer = storeRequisition.IsTransfer,
                    ToStoreCode = storeRequisition.ToStoreCode,
                    EmployeeId = storeRequisition.EmployeeId,
                    RequestDate = storeRequisition.RequestDate,
                    StoreCode = storeRequisition.StoreCode,
                    IsForOther = storeRequisition.IsForOther,
                    ForBranchId = storeRequisition.ForBranchId,
                    ModelNo = storeRequisition.ModelNo,
                    JobOrderNo = storeRequisition.JobOrderNo,
                    ServiceJobOrderNo = storeRequisition.ServiceJobOrderNo,
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

        public ActionResult SparePartStoreRequisitions_ReadJobOrder([DataSourceRequest] DataSourceRequest request, int num)
        {
            var serviceCenter = _db.ServiceCenterJob.FirstOrDefault(x => x.Num == num);
            if (serviceCenter == null)
            {
                return Json(new { Error = "Job Order not found" });
            }

            var storeRequisitions = _db.StoreRequisitions
                                        .Where(q => q.IsForMaintenance)
                                        .Where(x => x.ServiceJobOrderNo == serviceCenter.ServiceCenterJobId.ToString())
                                        .AsQueryable();
            
            var result = storeRequisitions.ToDataSourceResult(request, storeRequisition => new
            {
                StoreRequisitionID = storeRequisition.StoreRequisitionID,
                IsForMaintenance = storeRequisition.IsForMaintenance,
                Reference = storeRequisition.Reference,
                OrgStructureId = storeRequisition.OrgStructureId,
                BranchId = storeRequisition.BranchId,
                BusinessUnitId = storeRequisition.BusinessUnitId,
                IsTransfer = storeRequisition.IsTransfer,
                ToStoreCode = storeRequisition.ToStoreCode,
                EmployeeId = storeRequisition.EmployeeId,
                RequestDate = storeRequisition.RequestDate,
                StoreCode = storeRequisition.StoreCode,
                IsForOther = storeRequisition.IsForOther,
                ForBranchId = storeRequisition.ForBranchId,
                ModelNo = storeRequisition.ModelNo,
                JobOrderNo = storeRequisition.JobOrderNo,
                ServiceJobOrderNo = storeRequisition.ServiceJobOrderNo,
                PlateNo = storeRequisition.PlateNo,
                //CurrentAvgPrice = storeRequisition.,

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

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //[AuthorizeRoles(InventoryConstants.PropertyRequistion)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SparePartStoreRequisitions_Create([DataSourceRequest] DataSourceRequest request, StoreRequisition storeRequisition, Guid id)
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



            //if (!String.IsNullOrEmpty(storeRequisition.Reference))
            //{
            //    var exists = _db.StoreRequisitions.Where(x => x.Reference == storeRequisition.Reference && x.IsVoid == false).ToList();

            //    if (exists.Any())
            //    {
            //        ModelState.AddModelError("Reference",
            //            storeRequisition.Reference + " is already exists in the system.");
            //    }
            //}

            if (storeRequisition.IsForOther && storeRequisition.ForBranchId==null)
            {
                ModelState.AddModelError("otherbranch"," Please select other branch.");
            }

            #endregion
            var FetchWorkShopRemrak = _db.CloseWorkShopWithRemarks.Where(x => x.JobId == id.ToString());
            if (FetchWorkShopRemrak.Any())
            {
                ModelState.AddModelError("Closed With Remark", "This machine is already Closed with Remark");

            }
            if (ModelState.IsValid)
            {
                var serviceJob = _db.ServiceCenterJob.Find(id);
                Guid jobOrderId = (Guid)serviceJob?.JobOrderID;
                var jobOrderNum = _db.JobOrder.Where(q=>q.JobOrderID==jobOrderId).FirstOrDefault()?.Num;

                    var entity = new StoreRequisition
                    {
                        Reference = storeRequisition.Reference,
                        IsForMaintenance = true,
                        OrgStructureId = UserInformationHelper.GetEmployeeWorkUnitById((int)storeRequisition.EmployeeId),
                        BranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(storeRequisition.StoreCode), //UserInformationHelper.GetEmployeeBranchId(User.Identity.Name),
                        BusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name),
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
                        ServiceJobOrderNo = id.ToString(),
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
        public ActionResult SparePartStoreRequisitions_Update([DataSourceRequest] DataSourceRequest request, StoreRequisition storeRequisition)
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
                        OrgStructureId = UserInformationHelper.GetEmployeeWorkUnitById((int)storeRequisition.EmployeeId),
                        BranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(storeRequisition.StoreCode), //UserInformationHelper.GetEmployeeBranchId(User.Identity.Name),
                        BusinessUnitId=storeRequisition.BusinessUnitId,
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
                        ServiceJobOrderNo = storeRequisition.ServiceJobOrderNo,
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
        public ActionResult SparePartStoreRequisitions_Destroy([DataSourceRequest] DataSourceRequest request, StoreRequisition storeRequisition)
        {
            if (ModelState.IsValid)
            {
                var entity = new StoreRequisition
                    {
                        StoreRequisitionID = storeRequisition.StoreRequisitionID,
                    IsForMaintenance = storeRequisition.IsForMaintenance,
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
                        PlateNo = storeRequisition.PlateNo,
                    JobOrderNo = storeRequisition.JobOrderNo,
                    ServiceJobOrderNo = storeRequisition.ServiceJobOrderNo,


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

        [AuthorizeRoles(InventoryConstants.PropertyVoid)]
        public JsonResult Void(int id)
        {
            var record = _db.StoreRequisitions.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred && record.StoreRequisitionItem.Count > 0)
            {
                // current store requisition cannot be used in issue
                #region validate issue

                //var issue = _db.Issues
                //    .Where(x => x.IsVoid == false).FirstOrDefault(x => x.StoreRequisitionID == record.StoreRequisitionID);

                //if (issue != null)
                //{
                //    var formattedData2 = new
                //    {
                //        Message = $"This store requisition can not be void since it is referred in issue number {issue.IssueID}.",
                //        Status = "NOT_OK"
                //    };
                //    return Json(formattedData2, JsonRequestBehavior.AllowGet);
                //}
                #endregion
                // this store requistion cannot be used in ItemTransfer.
                #region validate transfer send

                //var itemTransfer = _db.ItemTransfers
                //    .Where(x => x.IsVoid == false).FirstOrDefault(x => x.StoreRequisitionID == record.StoreRequisitionID);

                //if (itemTransfer != null)
                //{
                //    var formattedData2 = new
                //    {
                //        Message = $"This store requisition can not be void since it is referred in item transfer send {itemTransfer.ItemTransferID}.",
                //        Status = "NOT_OK"
                //    };
                //    return Json(formattedData2, JsonRequestBehavior.AllowGet);
                //}
                #endregion
                #region remove linked shipment
                var shipment = _db.SalesShipments
                    .FirstOrDefault(s => s.Source == ShipmentSource.MaintenanceStoreRequisition &&
                        s.StoreRequisitionNumber == record.StoreRequisitionID.ToString()
                    );
                if (shipment != null)
                {
                    if (shipment.IsOnlineApproved || shipment.IsSendForApproval)
                    {
                        return Json(new
                        {
                            Message = $"Shipment No. {shipment.SalesShipmentId} with Store Req no {record.StoreRequisitionID} is in {shipment.Status} status. You cannot void this store requisition"
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        _db.SalesShipments.Attach(shipment);
                        _db.SalesShipments.Remove(shipment);
                        _db.SaveChanges();
                    }
                }
                #endregion
                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsVoid = true;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.StoreRequisitions.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();

                //copy
                CopyRecord(id);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "store requisition", "Success", User.Identity.Name, $"void store requisition {id} ", Modules.Inventory);
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

            if (record.StoreRequisitionItem.Count == 0)
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
            var record = _db.StoreRequisitions.Find(id);
            if (record != null)
            {
                #region header
                var entity = new StoreRequisition
                {
                    Reference = record.Reference,
                    IsForMaintenance = record.IsForMaintenance,
                    OrgStructureId = UserInformationHelper.GetEmployeeWorkUnitById((int)record.EmployeeId),
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