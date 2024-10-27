using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
using ExceedERP.Core.Domain.Procurement.Settings;


namespace ExceedERP.Web.Areas.Procurement.Controllers
{


    public class PurchaseForeignFollowUpController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementUser, ProcurementRoles.ProcurementApprover)]
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {

            var categories = db.ItemCategorys;
            var items = db.InventoryItems;
            var serviceCategories = db.ServiceCategories;
            var serviceItems = db.ServiceItems;


            ViewData["Items"] = items;
            ViewData["Category"] = categories;

            ViewData["ServiceItems"] = serviceItems;
            ViewData["ServiceCategory"] = serviceCategories;

            ViewData["defaultCategory"] = categories.FirstOrDefault();

            var store = db.InventoryStores.AsQueryable();
            ViewData["storesInCompany"] = store;

            var employee = db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstName + " " + i.MiddleName + " " + i.LastName,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;

            var costCenter = db.BranchCostCenter;

            var selectedcostCenter = costCenter.Select(
                i => new
                {
                    Name = i.Branch.Name,
                    Code = i.OrgStructureId
                }).AsQueryable();

            ViewData["costCenters"] = selectedcostCenter;

            var branches = db.Branch;
            ViewData["Branches"] = branches;

        }

        [AuthorizeRoles(ProcurementRoles.ProcurementUser, ProcurementRoles.ProcurementApprover)]
        public ActionResult PurchaseFollowUps_Read([DataSourceRequest] DataSourceRequest request)
        {
            IQueryable<PurchaseFollowUp> purchaseFollowUps = db.PurchaseFollowUps.Where(x=>x.PurchaseCategory==PurchaseCategory.ForeignPurchase);
            var result = purchaseFollowUps.ToDataSourceResult(request, purchaseRequisition => new
            {
                PurchaseRequisitionID = purchaseRequisition.PurchaseRequisitionID,
                PurchaseFollowUpID = purchaseRequisition.PurchaseFollowUpID,
                BranchId = purchaseRequisition.BranchId,
                OrgStructureId = purchaseRequisition.OrgStructureId,
                EmployeeId = purchaseRequisition.EmployeeId,
                ReferenceNo = purchaseRequisition.ReferenceNo,
                ProcurementOfficer = purchaseRequisition.ProcurementOfficer,
                RequestDate = purchaseRequisition.RequestDate,
                ProcurementMethod = purchaseRequisition.ProcurementMethod,
                PurchaseCategory = purchaseRequisition.PurchaseCategory,

                PurchaseStatus = purchaseRequisition.PurchaseStatus
            });

            return Json(result);
        }

    

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
