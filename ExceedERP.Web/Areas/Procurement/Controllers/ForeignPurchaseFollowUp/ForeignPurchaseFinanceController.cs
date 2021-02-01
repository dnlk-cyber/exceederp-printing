using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.ForeignPurchaseFollowUp
{
    public class ForeignPurchaseFinanceController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var tax = db.GLTaxRates;
            ViewData["tax"] = tax;
            var costCenter = db.BranchCostCenter;

            var selectedcostCenter = costCenter.Select(
                i => new
                {
                    Name = i.MainCompanyStructure.Name,
                    Code = i.OrgStructureId
                }).AsQueryable();

            ViewData["costCenters"] = selectedcostCenter;

        }

        public ActionResult ForeignPurchases_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Core.Domain.Procurement.ForeignPurchase.ForeignPurchase> foreignpurchases = db.ForeignPurchases;
            DataSourceResult result = foreignpurchases.ToDataSourceResult(request, foreignPurchase => new
            {
                ForeignPurchaseId = foreignPurchase.ForeignPurchaseId,
                TenderRef = foreignPurchase.TenderRef,
                PurchaseRef = foreignPurchase.PurchaseRef,
                Supplier = foreignPurchase.Supplier,
                ItemDesc = foreignPurchase.ItemDesc,
                OrgStructureId = foreignPurchase.OrgStructureId,
                Quantity = foreignPurchase.Quantity,
                ProcurementMethod = foreignPurchase.ProcurementMethod,
                FOBMethod = foreignPurchase.FOBMethod,
                ApprovalDate = foreignPurchase.ApprovalDate,
                OpenigDate = foreignPurchase.OpenigDate,
                TechnicalDocSendDate = foreignPurchase.TechnicalDocSendDate,
                FinancialOpenigDate = foreignPurchase.FinancialOpenigDate,
                MinuteApprovedDate = foreignPurchase.MinuteApprovedDate,
                AwardDate = foreignPurchase.AwardDate,
                ContractDate = foreignPurchase.ContractDate,
                PaymentMethod = foreignPurchase.PaymentMethod,
                PaymentOpenDate = foreignPurchase.PaymentOpenDate,
                PaymentExtensionDate = foreignPurchase.PaymentExtensionDate,
                DocReceivedFromBankDate = foreignPurchase.DocReceivedFromBankDate,
                ClearanceStartDate = foreignPurchase.ClearanceStartDate,
                TransitProcessDate = foreignPurchase.TransitProcessDate,
                DeliveryDate = foreignPurchase.DeliveryDate,
                DeliveryNo = foreignPurchase.DeliveryNo,
                UnitPrice = foreignPurchase.UnitPrice,
                ForeignCurrencyProcessId = foreignPurchase.ForeignCurrencyProcessId,
                Remark = foreignPurchase.Remark,
                OrgBranchName = foreignPurchase.OrgBranchName,
                DateCreated = foreignPurchase.DateCreated,
                LastModified = foreignPurchase.LastModified,
                CreatedBy = foreignPurchase.CreatedBy,
                ModifiedBy = foreignPurchase.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchases_Create([DataSourceRequest]DataSourceRequest request, Core.Domain.Procurement.ForeignPurchase.ForeignPurchase foreignPurchase)
        {
            if (ModelState.IsValid)
            {
                var entity = new Core.Domain.Procurement.ForeignPurchase.ForeignPurchase
                {
                    TenderRef = foreignPurchase.TenderRef,
                    PurchaseRef = foreignPurchase.PurchaseRef,
                    Supplier = foreignPurchase.Supplier,
                    ItemDesc = foreignPurchase.ItemDesc,
                    OrgStructureId = foreignPurchase.OrgStructureId,
                    Quantity = foreignPurchase.Quantity,
                    ProcurementMethod = foreignPurchase.ProcurementMethod,
                    FOBMethod = foreignPurchase.FOBMethod,
                    ApprovalDate = foreignPurchase.ApprovalDate,
                    OpenigDate = foreignPurchase.OpenigDate,
                    TechnicalDocSendDate = foreignPurchase.TechnicalDocSendDate,
                    FinancialOpenigDate = foreignPurchase.FinancialOpenigDate,
                    MinuteApprovedDate = foreignPurchase.MinuteApprovedDate,
                    AwardDate = foreignPurchase.AwardDate,
                    ContractDate = foreignPurchase.ContractDate,
                    PaymentMethod = foreignPurchase.PaymentMethod,
                    PaymentOpenDate = foreignPurchase.PaymentOpenDate,
                    PaymentExtensionDate = foreignPurchase.PaymentExtensionDate,
                    DocReceivedFromBankDate = foreignPurchase.DocReceivedFromBankDate,
                    ClearanceStartDate = foreignPurchase.ClearanceStartDate,
                    TransitProcessDate = foreignPurchase.TransitProcessDate,
                    DeliveryDate = foreignPurchase.DeliveryDate,
                    DeliveryNo = foreignPurchase.DeliveryNo,
                    UnitPrice = foreignPurchase.UnitPrice,
                    ForeignCurrencyProcessId = foreignPurchase.ForeignCurrencyProcessId,
                    Remark = foreignPurchase.Remark,
                    OrgBranchName = foreignPurchase.OrgBranchName,
                    DateCreated = foreignPurchase.DateCreated,
                    LastModified = foreignPurchase.LastModified,
                    CreatedBy = foreignPurchase.CreatedBy,
                    ModifiedBy = foreignPurchase.ModifiedBy
                };

                db.ForeignPurchases.Add(entity);
                db.SaveChanges();
                foreignPurchase.ForeignPurchaseId = entity.ForeignPurchaseId;
            }

            return Json(new[] { foreignPurchase }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchases_Update([DataSourceRequest]DataSourceRequest request, Core.Domain.Procurement.ForeignPurchase.ForeignPurchase foreignPurchase)
        {
            if (ModelState.IsValid)
            {
                var entity = new Core.Domain.Procurement.ForeignPurchase.ForeignPurchase
                {
                    ForeignPurchaseId = foreignPurchase.ForeignPurchaseId,
                    TenderRef = foreignPurchase.TenderRef,
                    PurchaseRef = foreignPurchase.PurchaseRef,
                    Supplier = foreignPurchase.Supplier,
                    ItemDesc = foreignPurchase.ItemDesc,
                    OrgStructureId = foreignPurchase.OrgStructureId,
                    Quantity = foreignPurchase.Quantity,
                    ProcurementMethod = foreignPurchase.ProcurementMethod,
                    FOBMethod = foreignPurchase.FOBMethod,
                    ApprovalDate = foreignPurchase.ApprovalDate,
                    OpenigDate = foreignPurchase.OpenigDate,
                    TechnicalDocSendDate = foreignPurchase.TechnicalDocSendDate,
                    FinancialOpenigDate = foreignPurchase.FinancialOpenigDate,
                    MinuteApprovedDate = foreignPurchase.MinuteApprovedDate,
                    AwardDate = foreignPurchase.AwardDate,
                    ContractDate = foreignPurchase.ContractDate,
                    PaymentMethod = foreignPurchase.PaymentMethod,
                    PaymentOpenDate = foreignPurchase.PaymentOpenDate,
                    PaymentExtensionDate = foreignPurchase.PaymentExtensionDate,
                    DocReceivedFromBankDate = foreignPurchase.DocReceivedFromBankDate,
                    ClearanceStartDate = foreignPurchase.ClearanceStartDate,
                    TransitProcessDate = foreignPurchase.TransitProcessDate,
                    DeliveryDate = foreignPurchase.DeliveryDate,
                    DeliveryNo = foreignPurchase.DeliveryNo,
                    UnitPrice = foreignPurchase.UnitPrice,
                    ForeignCurrencyProcessId = foreignPurchase.ForeignCurrencyProcessId,
                    Remark = foreignPurchase.Remark,
                    OrgBranchName = foreignPurchase.OrgBranchName,
                    DateCreated = foreignPurchase.DateCreated,
                    LastModified = foreignPurchase.LastModified,
                    CreatedBy = foreignPurchase.CreatedBy,
                    ModifiedBy = foreignPurchase.ModifiedBy
                };

                db.ForeignPurchases.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { foreignPurchase }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchases_Destroy([DataSourceRequest]DataSourceRequest request, Core.Domain.Procurement.ForeignPurchase.ForeignPurchase foreignPurchase)
        {
            if (ModelState.IsValid)
            {
                var entity = new Core.Domain.Procurement.ForeignPurchase.ForeignPurchase
                {
                    ForeignPurchaseId = foreignPurchase.ForeignPurchaseId,
                    TenderRef = foreignPurchase.TenderRef,
                    PurchaseRef = foreignPurchase.PurchaseRef,
                    Supplier = foreignPurchase.Supplier,
                    ItemDesc = foreignPurchase.ItemDesc,
                    OrgStructureId = foreignPurchase.OrgStructureId,
                    Quantity = foreignPurchase.Quantity,
                    ProcurementMethod = foreignPurchase.ProcurementMethod,
                    FOBMethod = foreignPurchase.FOBMethod,
                    ApprovalDate = foreignPurchase.ApprovalDate,
                    OpenigDate = foreignPurchase.OpenigDate,
                    TechnicalDocSendDate = foreignPurchase.TechnicalDocSendDate,
                    FinancialOpenigDate = foreignPurchase.FinancialOpenigDate,
                    MinuteApprovedDate = foreignPurchase.MinuteApprovedDate,
                    AwardDate = foreignPurchase.AwardDate,
                    ContractDate = foreignPurchase.ContractDate,
                    PaymentMethod = foreignPurchase.PaymentMethod,
                    PaymentOpenDate = foreignPurchase.PaymentOpenDate,
                    PaymentExtensionDate = foreignPurchase.PaymentExtensionDate,
                    DocReceivedFromBankDate = foreignPurchase.DocReceivedFromBankDate,
                    ClearanceStartDate = foreignPurchase.ClearanceStartDate,
                    TransitProcessDate = foreignPurchase.TransitProcessDate,
                    DeliveryDate = foreignPurchase.DeliveryDate,
                    DeliveryNo = foreignPurchase.DeliveryNo,
                    UnitPrice = foreignPurchase.UnitPrice,
                    ForeignCurrencyProcessId = foreignPurchase.ForeignCurrencyProcessId,
                    Remark = foreignPurchase.Remark,
                    OrgBranchName = foreignPurchase.OrgBranchName,
                    DateCreated = foreignPurchase.DateCreated,
                    LastModified = foreignPurchase.LastModified,
                    CreatedBy = foreignPurchase.CreatedBy,
                    ModifiedBy = foreignPurchase.ModifiedBy
                };

                db.ForeignPurchases.Attach(entity);
                db.ForeignPurchases.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { foreignPurchase }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
