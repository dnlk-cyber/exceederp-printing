using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.FixedAsset.Receives;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.FixedAsset;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.GL.SubModule
{
    [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
    public class FixedAssetTransactionsController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private FixedAssetTransferHelper _transferHelper;
        private string _response = "SUCCESS";
  
        // GET: Finance/FixedAssetTransactions
        public ActionResult FixedAssetReceive()
        {
            var periods = _db.FixedAssetPeriods.ToList();
            ViewData["PeriodList"] = periods;
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
            return View();
        }

        public ActionResult FixedAssetReceives_Read([DataSourceRequest]DataSourceRequest request)
        {

            IQueryable<FixedAssetReceive> goodreceives = _db.FixedAssetReceives.Where(x => x.IsOnlineApproved && !x.IsVoid && !x.IsOnlineTransferred);
            DataSourceResult result = goodreceives.ToDataSourceResult(request, fixedAssetReceive => new
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

        [AuthorizeRoles(FinanceConstants.GeneralLedgerUser)]
        public JsonResult BatchtransferGrn(string id)
        {

            var untransffered = _db.FixedAssetReceives.Where(x => x.GLPeriodId.ToString() == id).Where(x => x.IsOnlineApproved && !x.IsVoid && !x.IsOnlineTransferred).ToList();

            if (!untransffered.Any())
            {
                var formattedData1 = new
                {
                    Message = "Data not found for transffer.",
                    Status = "NOT_OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);
            }

            foreach (var record in untransffered)
            {
                _transferHelper = new FixedAssetTransferHelper();
                _response = _transferHelper.GoodReceive(record.FixedAssetReceiveId, "Bach Transfer", User.Identity.Name);
                if (_response != "SUCCESS")
                    ;// Logger.Debug("Fixed Grn transfer ID:" + record.FixedAssetReceiveId + _response);
            }

            #region auto job



            #endregion

            #region operation log
            UserHelper.OperationLog(Request.UserHostAddress, "GL sub module fixed asset receive", "Success", User.Identity.Name, $"batch transfer for period {id} ", Modules.Finance);
            #endregion

            var formattedData = new
            {
                Message = $"Operation Completed. {untransffered.Count} records transffred.",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
    }
}