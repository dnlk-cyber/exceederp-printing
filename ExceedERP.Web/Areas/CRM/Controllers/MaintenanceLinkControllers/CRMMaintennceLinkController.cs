using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.Inventory;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    public class CRMMaintennceLinkController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        //[AuthorizeRoles(InventoryConstants.PropertyRequistion, InventoryConstants.UserDepartment)]
        public ActionResult StoreRequisitionItems_Read([DataSourceRequest]DataSourceRequest request, int reqNumber)
        {
            IQueryable<StoreRequisitionItem> storerequisitionItems = db.StoreRequisitionItems.Where(p => p.StoreRequisitionID == reqNumber);
            DataSourceResult result = storerequisitionItems.ToDataSourceResult(request, storeRequisitionItem => new {

                StoreRequisitionItemID = storeRequisitionItem.StoreRequisitionItemID,
                StoreRequisitionID = storeRequisitionItem.StoreRequisitionID,
                ItemCategoryCode = storeRequisitionItem.ItemCategoryCode,
                ItemCode = storeRequisitionItem.ItemCode,
                PartNo = storeRequisitionItem.PartNo,
                ModelNo = storeRequisitionItem.ModelNo,
                RequestedQuantity = storeRequisitionItem.RequestedQuantity,
                ApprovedQuantity = storeRequisitionItem.ApprovedQuantity,
                IssuedQuantity = storeRequisitionItem.IssuedQuantity,
                PurchaseQuantity = storeRequisitionItem.PurchaseQuantity,
                IsApproved = storeRequisitionItem.IsApproved,
                ItemSpecification = storeRequisitionItem.ItemSpecification,
                Unit = ExceedERP.Helpers.Inventory.InventoryHelper.GetItemUom(storeRequisitionItem.ItemCode),
                CurrentBalance = 0
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