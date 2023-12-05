using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Returns;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using EntityState = System.Data.Entity.EntityState;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Return
{


    public class FixedAssetReturnController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var assetCategories = _db.FixedAssetCategories;
            var assetSubCategories = _db.FixedAssetSubCategories;
            var fixedAsset = _db.AssetRegistrations;
            var branches = _db.Branch;
            var costCenter = _db.BranchCostCenter;
            var stores = _db.InventoryStores;
            var selectedcostCenter = costCenter.Select(
              i => new
              {
                  Name = i.MainCompanyStructure.Name,
                  Code = i.OrgStructureId
              }).AsQueryable();

            var selectedAsset = fixedAsset.Select(
              i => new
              {
                  Name = i.Tag + "-" + i.Name,
                  AssetRegistrationId = i.AssetRegistrationId
              }).AsQueryable();

            var employee = _db.Person_Employee;
            var selectedEmployee = employee.Select(
              i => new
              {
                  Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                  Code = i.EmployeeId
              }).AsQueryable();

            ViewData["stores"] = stores;
            ViewData["personEmployees"] = selectedEmployee;

            ViewData["costCenters"] = selectedcostCenter;
            ViewData["Branches"] = branches;
            ViewData["SubCategory"] = assetSubCategories;
            ViewData["Category"] = assetCategories;
            ViewData["FixedAsset"] = selectedAsset;
            ViewData["defaultCategory"] = assetCategories.FirstOrDefault();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult FixedAssetReturns_Read([DataSourceRequest]DataSourceRequest request)
        {
            //List<string> userStores = UserInformationHelper.GetUserStores(User.Identity.Name);
           // IQueryable<FixedAssetReturn> fixedAssetReturns = _db.FixedAssetReturns.Where(x => userStores.Contains(x.StoreCode));

            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            IQueryable<FixedAssetReturn> fixedAssetReturns = _db.FixedAssetReturns
                .Where(x => mybraches.Contains(x.BranchId))
                .Where(x => x.IsDepartmentOnlineApproved);

           
            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                List<int> myBusinessUnits = UserInformationHelper.GetUserBusinessUnits(User.Identity.Name);
                fixedAssetReturns = fixedAssetReturns.Where(x => myBusinessUnits.Contains(x.BusinessUnitId));
            }


            DataSourceResult result = fixedAssetReturns.ToDataSourceResult(request, fixedAssetReturn => new
            {
                FixedAssetReturnId = fixedAssetReturn.FixedAssetReturnId,
                OrgStructureId = fixedAssetReturn.OrgStructureId,
                BranchId = fixedAssetReturn.BranchId,
                BusinessUnitId = fixedAssetReturn.BusinessUnitId,
                CreatedAtBranchId = fixedAssetReturn.CreatedAtBranchId,
                CreatedAtBusinessUnitId = fixedAssetReturn.CreatedAtBusinessUnitId,
                CreatedAtOrgStructureId = fixedAssetReturn.CreatedAtOrgStructureId,
                EmployeeId = fixedAssetReturn.EmployeeId,
                StoreCode = fixedAssetReturn.StoreCode,
                Reference = fixedAssetReturn.Reference,
                ReturnDate = fixedAssetReturn.ReturnDate,


                Status = fixedAssetReturn.Status,
                Remark = fixedAssetReturn.Remark,
                IsVoid = fixedAssetReturn.IsVoid,
                VoidBy = fixedAssetReturn.VoidBy,
                VoidTime = fixedAssetReturn.VoidTime,
                IsOnlineApproved = fixedAssetReturn.IsOnlineApproved,
                OnlineApprovedBy = fixedAssetReturn.OnlineApprovedBy,
                OnlineApprovedTime = fixedAssetReturn.OnlineApprovedTime,
                IsSendForApproval = fixedAssetReturn.IsSendForApproval,
                SendForApprovalBy = fixedAssetReturn.SendForApprovalBy,
                SendForApprovalTime = fixedAssetReturn.SendForApprovalTime,
                IsOnlineTransferred = fixedAssetReturn.IsOnlineTransferred,
                IsOnlineTransferredBy = fixedAssetReturn.IsOnlineTransferredBy,
                OnlineTransferredTime = fixedAssetReturn.OnlineTransferredTime,

                IsDepartmentOnlineApproved = fixedAssetReturn.IsDepartmentOnlineApproved,
                DepartmentOnlineApprovedBy = fixedAssetReturn.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = fixedAssetReturn.DepartmentOnlineApprovedTime,
                IsDepartmentSendForApproval = fixedAssetReturn.IsDepartmentSendForApproval,
                DepartmentSendForApprovalBy = fixedAssetReturn.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = fixedAssetReturn.DepartmentSendForApprovalTime,

                DateCreated = fixedAssetReturn.DateCreated,
                CreatedBy = fixedAssetReturn.CreatedBy,
                LastModified = fixedAssetReturn.LastModified,
                ModifiedBy = fixedAssetReturn.ModifiedBy,
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetReturns_Create([DataSourceRequest]DataSourceRequest request, FixedAssetReturn fixedAssetReturn)
        {
            #region data 
            if (fixedAssetReturn.EmployeeId == 0)
            {
                ModelState.AddModelError("requester", "Please select requestor");
            }
            if (fixedAssetReturn.EmployeeId != 0)
            {
                var placement = _db.Placements.FirstOrDefault(x => x.EmployeeId == fixedAssetReturn.EmployeeId && x.isCurrent);
                if (placement != null)
                {
                    fixedAssetReturn.OrgStructureId = placement.OrgStructureId;
                    fixedAssetReturn.BranchId = placement.BranchId;
                    fixedAssetReturn.BusinessUnitId = placement.BusinessUnitId;
                }
                else
                {
                    ModelState.AddModelError("placement", "Employee has not current placemnt");
                }
            }

            fixedAssetReturn.CreatedAtBranchId = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name);
            fixedAssetReturn.CreatedAtBusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
            fixedAssetReturn.CreatedAtOrgStructureId = UserInformationHelper.GetUserPlacementWorkUnit(User.Identity.Name);
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetReturn
                {
                    OrgStructureId = fixedAssetReturn.OrgStructureId,
                    BranchId = fixedAssetReturn.BranchId,
                    BusinessUnitId = fixedAssetReturn.BusinessUnitId,
                    CreatedAtBranchId = fixedAssetReturn.CreatedAtBranchId,
                    CreatedAtBusinessUnitId = fixedAssetReturn.CreatedAtBusinessUnitId,
                    CreatedAtOrgStructureId = fixedAssetReturn.CreatedAtOrgStructureId,
                    EmployeeId = fixedAssetReturn.EmployeeId,
                    StoreCode = fixedAssetReturn.StoreCode,
                    Reference = fixedAssetReturn.Reference,
                    ReturnDate = fixedAssetReturn.ReturnDate,
                    Status = Enum.GetName(typeof(OperationStatuses), 5),
                    Remark = fixedAssetReturn.Remark,


                    IsVoid = fixedAssetReturn.IsVoid,
                    VoidBy = fixedAssetReturn.VoidBy,
                    VoidTime = fixedAssetReturn.VoidTime,
                    IsOnlineApproved = fixedAssetReturn.IsOnlineApproved,
                    OnlineApprovedBy = fixedAssetReturn.OnlineApprovedBy,
                    OnlineApprovedTime = fixedAssetReturn.OnlineApprovedTime,
                    IsOnlineTransferred = fixedAssetReturn.IsOnlineTransferred,
                    IsOnlineTransferredBy = fixedAssetReturn.IsOnlineTransferredBy,
                    OnlineTransferredTime = fixedAssetReturn.OnlineTransferredTime,

                    IsDepartmentOnlineApproved = true,//to make it display
                    DepartmentOnlineApprovedBy = fixedAssetReturn.DepartmentOnlineApprovedBy,
                    DepartmentOnlineApprovedTime = fixedAssetReturn.DepartmentOnlineApprovedTime,
                    IsDepartmentSendForApproval = fixedAssetReturn.IsDepartmentSendForApproval,
                    DepartmentSendForApprovalBy = fixedAssetReturn.DepartmentSendForApprovalBy,
                    DepartmentSendForApprovalTime = fixedAssetReturn.DepartmentSendForApprovalTime,

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    LastModified = fixedAssetReturn.LastModified,
                    ModifiedBy = fixedAssetReturn.ModifiedBy,
                };

                _db.FixedAssetReturns.Add(entity);
                _db.SaveChanges();
                fixedAssetReturn.FixedAssetReturnId = entity.FixedAssetReturnId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetReturn", "Success", User.Identity.Name, $"created FixedAssetReturn  {entity.FixedAssetReturnId} Tag {entity.EmployeeId}", Modules.FixedAsset);
                #endregion


            }

            return Json(new[] { fixedAssetReturn }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetReturns_Update([DataSourceRequest]DataSourceRequest request, FixedAssetReturn fixedAssetReturn)
        {
            #region data 
            if (fixedAssetReturn.EmployeeId == 0)
            {
                ModelState.AddModelError("requester", "Please select requestor");
            }
            if (fixedAssetReturn.EmployeeId != 0)
            {
                var placement = _db.Placements.FirstOrDefault(x => x.EmployeeId == fixedAssetReturn.EmployeeId && x.isCurrent);
                if (placement != null)
                {
                    fixedAssetReturn.OrgStructureId = placement.OrgStructureId;
                    fixedAssetReturn.BranchId = placement.BranchId;
                    fixedAssetReturn.BusinessUnitId = placement.BusinessUnitId;
                }
                else
                {
                    ModelState.AddModelError("placement", "Employee has not current placemnt");
                }
            }

            fixedAssetReturn.CreatedAtBranchId = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name);
            fixedAssetReturn.CreatedAtBusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
            fixedAssetReturn.CreatedAtOrgStructureId = UserInformationHelper.GetUserPlacementWorkUnit(User.Identity.Name);
            #endregion

            if (ModelState.IsValid)
            {
                var entity = new FixedAssetReturn
                {
                    OrgStructureId = fixedAssetReturn.OrgStructureId,
                    BranchId = fixedAssetReturn.BranchId,
                    BusinessUnitId = fixedAssetReturn.BusinessUnitId,
                    CreatedAtBranchId = fixedAssetReturn.CreatedAtBranchId,
                    CreatedAtBusinessUnitId = fixedAssetReturn.CreatedAtBusinessUnitId,
                    CreatedAtOrgStructureId = fixedAssetReturn.CreatedAtOrgStructureId,
                    EmployeeId = fixedAssetReturn.EmployeeId,
                    StoreCode = fixedAssetReturn.StoreCode,
                    Reference = fixedAssetReturn.Reference,
                    ReturnDate = fixedAssetReturn.ReturnDate,
                    Status = fixedAssetReturn.Status,
                    Remark = fixedAssetReturn.Remark,


                    IsVoid = fixedAssetReturn.IsVoid,
                    VoidBy = fixedAssetReturn.VoidBy,
                    VoidTime = fixedAssetReturn.VoidTime,
                    IsOnlineApproved = fixedAssetReturn.IsOnlineApproved,
                    OnlineApprovedBy = fixedAssetReturn.OnlineApprovedBy,
                    OnlineApprovedTime = fixedAssetReturn.OnlineApprovedTime,
                    IsOnlineTransferred = fixedAssetReturn.IsOnlineTransferred,
                    IsOnlineTransferredBy = fixedAssetReturn.IsOnlineTransferredBy,
                    OnlineTransferredTime = fixedAssetReturn.OnlineTransferredTime,

                    IsDepartmentOnlineApproved = fixedAssetReturn.IsDepartmentOnlineApproved,
                    DepartmentOnlineApprovedBy = fixedAssetReturn.DepartmentOnlineApprovedBy,
                    DepartmentOnlineApprovedTime = fixedAssetReturn.DepartmentOnlineApprovedTime,
                    IsDepartmentSendForApproval = fixedAssetReturn.IsDepartmentSendForApproval,
                    DepartmentSendForApprovalBy = fixedAssetReturn.DepartmentSendForApprovalBy,
                    DepartmentSendForApprovalTime = fixedAssetReturn.DepartmentSendForApprovalTime,

                    DateCreated = fixedAssetReturn.DateCreated,
                    CreatedBy = fixedAssetReturn.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,

                };

                _db.FixedAssetReturns.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetReturn", "Success", User.Identity.Name, $"updated FixedAssetReturn  {entity.FixedAssetReturnId} Tag {entity.EmployeeId}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { fixedAssetReturn }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetReturns_Destroy([DataSourceRequest]DataSourceRequest request, FixedAssetReturn fixedAssetReturn)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetReturn
                {
                    OrgStructureId = fixedAssetReturn.OrgStructureId,
                    BranchId = fixedAssetReturn.BranchId,
                    BusinessUnitId = fixedAssetReturn.BusinessUnitId,
                    CreatedAtBranchId = fixedAssetReturn.CreatedAtBranchId,
                    CreatedAtBusinessUnitId = fixedAssetReturn.CreatedAtBusinessUnitId,
                    CreatedAtOrgStructureId = fixedAssetReturn.CreatedAtOrgStructureId,
                    EmployeeId = fixedAssetReturn.EmployeeId,
                    FixedAssetReturnId = fixedAssetReturn.FixedAssetReturnId,
                    StoreCode = fixedAssetReturn.StoreCode,
                    Reference = fixedAssetReturn.Reference,
                    ReturnDate = fixedAssetReturn.ReturnDate,
                    Status = fixedAssetReturn.Status,
                    Remark = fixedAssetReturn.Remark,
                    IsVoid = fixedAssetReturn.IsVoid,
                    VoidBy = fixedAssetReturn.VoidBy,
                    VoidTime = fixedAssetReturn.VoidTime,
                    IsOnlineApproved = fixedAssetReturn.IsOnlineApproved,
                    OnlineApprovedBy = fixedAssetReturn.OnlineApprovedBy,
                    OnlineApprovedTime = fixedAssetReturn.OnlineApprovedTime,
                    IsOnlineTransferred = fixedAssetReturn.IsOnlineTransferred,
                    IsOnlineTransferredBy = fixedAssetReturn.IsOnlineTransferredBy,
                    OnlineTransferredTime = fixedAssetReturn.OnlineTransferredTime,

                    IsDepartmentOnlineApproved = fixedAssetReturn.IsDepartmentOnlineApproved,
                    DepartmentOnlineApprovedBy = fixedAssetReturn.DepartmentOnlineApprovedBy,
                    DepartmentOnlineApprovedTime = fixedAssetReturn.DepartmentOnlineApprovedTime,
                    IsDepartmentSendForApproval = fixedAssetReturn.IsDepartmentSendForApproval,
                    DepartmentSendForApprovalBy = fixedAssetReturn.DepartmentSendForApprovalBy,
                    DepartmentSendForApprovalTime = fixedAssetReturn.DepartmentSendForApprovalTime,

                    DateCreated = fixedAssetReturn.DateCreated,
                    CreatedBy = fixedAssetReturn.CreatedBy,
                    LastModified = fixedAssetReturn.LastModified,
                    ModifiedBy = fixedAssetReturn.ModifiedBy,
                };

                _db.FixedAssetReturns.Attach(entity);
                _db.FixedAssetReturns.Remove(entity);
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"deleted FixedAssetReturn  {fixedAssetReturn.FixedAssetReturnId} Tag {fixedAssetReturn.EmployeeId}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { fixedAssetReturn }.ToDataSourceResult(request, ModelState));
        }
        public FileResult DownloadTemplate()
        {

            string physicalPath = "~/Content/InventoryTemplate/fixedAssetReturnTemplate.xlsx";
            return File(physicalPath, "application/force-download", Path.GetFileName(physicalPath));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        public JsonResult SendForApproval(int id)
        {
            var record = _db.FixedAssetReturns.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.FixedAssetReturnItems.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.FixedAssetReturns.Attach(record);
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

            else if (record.FixedAssetReturnItems.Count == 0)
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
            var record = _db.FixedAssetReturns.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred && record.FixedAssetReturnItems.Count > 0)
            {
                //reverse transaction here


                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsVoid = true;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.FixedAssetReturns.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();

                //copy
                CopyRecord(id);

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

            if (record.FixedAssetReturnItems.Count == 0)
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
            var fixedAssetReturn = _db.FixedAssetReturns.Find(id);
            if (fixedAssetReturn != null)
            {
                #region header
                var entity = new FixedAssetReturn
                {
                    OrgStructureId = UserInformationHelper.GetEmployeeWorkUnitById(fixedAssetReturn.EmployeeId),
                    BranchId = fixedAssetReturn.BranchId,
                    EmployeeId = fixedAssetReturn.EmployeeId,
                    StoreCode = fixedAssetReturn.StoreCode,
                    Reference = fixedAssetReturn.Reference,
                    ReturnDate = fixedAssetReturn.ReturnDate,
                    Status = Enum.GetName(typeof(OperationStatuses), 5),
                    Remark = fixedAssetReturn.Remark,

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    OrgBranchName = fixedAssetReturn.OrgBranchName,

                };

                _db.FixedAssetReturns.Add(entity);
                _db.SaveChanges();
                #endregion
                foreach (var issueItem in fixedAssetReturn.FixedAssetReturnItems)
                {
                    #region lines
                    var line = new FixedAssetReturnItem
                    {
                        FixedAssetReturnId = entity.FixedAssetReturnId,
                        FixedAssetCategoryId = issueItem.FixedAssetCategoryId,
                        FixedAssetSubCategoryId = issueItem.FixedAssetSubCategoryId,
                        AssetRegistrationId = issueItem.AssetRegistrationId,
                        TagNo = issueItem.TagNo,
                    };

                    _db.FixedAssetReturnItems.Add(line);
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
