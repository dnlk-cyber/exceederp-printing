using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.DocumentApproval;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.Contract;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Supplier;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Contract
{

   
    public class ContractManagementFinanceController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(FinanceConstants.AccountPayableView)]
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var branches = _db.Branch;
            ViewData["Branches"] = branches;
            var stores = _db.InventoryStores;
            ViewData["stores"] = stores;
            var receivetype = _db.ReceiveTypes;
            ViewData["ReeceiveType"] = receivetype;
            var tax = _db.GLTaxRates;

            ViewData["tax"] = tax;

            var suppliers = _db.ProcurementSuppliers.ToList();

            ViewData["suppliers"] = suppliers.Select(c => new ProcurementSupplier
            {
                Name = c.Name

            });

            var items = _db.ContractManagementItems;
            var selecteditems = items.Select(
                i => new
                {
                    Name = i.ItmCode + "-" + i.ItemName,
                    ItmCode = i.ItmCode
                }).AsQueryable();

            ViewData["Items"] = selecteditems;

            var selectedcatedories = items.Select(
                i => new
                {
                    Name = i.CatCode + "-" + i.CategoryName,
                    CatCode = i.CatCode
                }).AsQueryable();
            ViewData["Category"] = selectedcatedories;

            var inventoryPeriods = _db.InventoryPeriods;
            var selectedinventoryPeriods = inventoryPeriods.Select(
                i => new
                {
                    Name = i.Name,
                    GLPeriodId = i.InventoryPeriodId
                }).AsQueryable();
            ViewData["inventoryPeriods"] = selectedinventoryPeriods;
        }


        [AuthorizeRoles(FinanceConstants.AccountPayableView)]
        public ActionResult ContractManagements_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ContractManagement> contractmanagements = _db.ContractManagements;
            DataSourceResult result = contractmanagements.ToDataSourceResult(request, contractManagement => new {
                ContractManagementID = contractManagement.ContractManagementID,
                BranchId = contractManagement.BranchId,
                SupplierName = contractManagement.SupplierName,
                IsNewSupplier = contractManagement.IsNewSupplier,
                SupplierId = contractManagement.SupplierId,
                ContractRefNo = contractManagement.ContractRefNo,
                TenderRefNo = contractManagement.TenderRefNo,
                ProcurementRefNo = contractManagement.ProcurementRefNo,
                SigningDate = contractManagement.SigningDate,
                ContractType = contractManagement.ContractType,
                PurchaseCategory = contractManagement.PurchaseCategory,
                DeliveryPlace = contractManagement.DeliveryPlace,
                DeliveryDate = contractManagement.DeliveryDate,
                ContractAmount = contractManagement.ContractAmount,
                RetentionPercentage = contractManagement.RetentionPercentage,
                ProcurementOfficer = contractManagement.ProcurementOfficer,
                Advance = contractManagement.Advance,

                ForwardedFrom = contractManagement.ForwardedFrom,
                ForwardedTo = contractManagement.ForwardedTo,
                IsFinalApproval = contractManagement.IsFinalApproval,

                Status = contractManagement.Status,
                IsTransferredToPayment = contractManagement.IsTransferredToPayment,
                IsTransferredToFollowUp = contractManagement.IsTransferredToFollowUp,

                IsOnlineApproved = contractManagement.IsOnlineApproved,
                OnlineApprovedBy = contractManagement.OnlineApprovedBy,
                OnlineApprovedTime = contractManagement.OnlineApprovedTime,
                IsSendForApproval = contractManagement.IsSendForApproval,
                SendForApprovalBy = contractManagement.SendForApprovalBy,
                SendForApprovalTime = contractManagement.SendForApprovalTime,
                IsVoid = contractManagement.IsVoid,
                VoidBy = contractManagement.VoidBy,
                VoidTime = contractManagement.VoidTime,
                OrgBranchName = contractManagement.OrgBranchName,
                DateCreated = contractManagement.DateCreated,
                LastModified = contractManagement.LastModified,
                CreatedBy = contractManagement.CreatedBy,
                ModifiedBy = contractManagement.ModifiedBy
            });

            return Json(result);
        }
   

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
