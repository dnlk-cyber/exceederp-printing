using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Proforma;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Proforma
{


    public class ProformaItemController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult ProformaItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ProformaItem> proformaitems = _db.ProformaItems.Where(p => p.ProformaPurchaseID == id);
            DataSourceResult result = proformaitems.ToDataSourceResult(request, proformaItem => new
            {
                ProformaItemID = proformaItem.ProformaItemID,
                ProformaPurchaseID = proformaItem.ProformaPurchaseID,
                CatCode = proformaItem.CatCode,// + "-" + proformaItem.CategoryName,
                CategoryName = proformaItem.CategoryName,
                ItmCode = proformaItem.ItmCode,//+ "-" + proformaItem.ItemName,
                ItemName = proformaItem.ItemName,
                PartNo = proformaItem.PartNo,
                Quantity = proformaItem.Quantity,
                IsApproved = proformaItem.IsApproved,
                Name = proformaItem.Name,
                ModelNo = proformaItem.ModelNo,
                ItemSpecification = proformaItem.ItemSpecification,
                UoM = proformaItem.UoM,
                ItemType = proformaItem.ItemType,
                OrgBranchName = proformaItem.OrgBranchName,
                DateCreated = proformaItem.DateCreated,
                LastModified = proformaItem.LastModified,
                CreatedBy = proformaItem.CreatedBy,
                ModifiedBy = proformaItem.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaItems_Create([DataSourceRequest]DataSourceRequest request, ProformaItem proformaItem, int id)
        {
            #region validation

            var proformapr = _db.ProformaPurchaseRequisitions.Where(x => x.ProformaPurchaseID == id).Select(x=>x.PurchaseRequisitionID).ToList();
            var pritems = _db.PurchaseRequisitionItems.Where(x => proformapr.Contains(x.PurchaseRequisitionID)).ToList();
            decimal requetedQty = pritems.Where(x => x.ItmCode == proformaItem.ItmCode).Sum(x => x.Quantity);

            if (proformaItem.Quantity > requetedQty)
                ModelState.AddModelError("above",$"Quantity is above requested quantity: {requetedQty} ");

            #endregion


            if (ModelState.IsValid)
            {
                var entity = new ProformaItem
                {
                    ProformaPurchaseID = id,
                    CatCode = proformaItem.CatCode,
                    CategoryName = proformaItem.CategoryName,
                    ItmCode = proformaItem.ItmCode,
                    ItemName = proformaItem.ItemName,
                    PartNo = proformaItem.PartNo,
                    Quantity = proformaItem.Quantity,
                    IsApproved = proformaItem.IsApproved,
                    Name = proformaItem.Name,
                    ModelNo = proformaItem.ModelNo,
                    ItemSpecification = proformaItem.ItemSpecification,
                    UoM = proformaItem.UoM,
                    ItemType = proformaItem.ItemType,
                    OrgBranchName = proformaItem.OrgBranchName,
                    DateCreated = DateTime.Now.Date,
                    LastModified = proformaItem.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = proformaItem.ModifiedBy
                };

                _db.ProformaItems.Add(entity);
                _db.SaveChanges();
                proformaItem.ProformaItemID = entity.ProformaItemID;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "proforma item", "Success", User.Identity.Name, $"created item {entity.ItemName} under record {entity.ProformaPurchaseID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { proformaItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaItems_Update([DataSourceRequest]DataSourceRequest request, ProformaItem proformaItem)
        {
            #region validation

            var proformapr = _db.ProformaPurchaseRequisitions.Where(x => x.ProformaPurchaseID == proformaItem.ProformaPurchaseID).Select(x => x.PurchaseRequisitionID).ToList();
            var pritems = _db.PurchaseRequisitionItems.Where(x => proformapr.Contains(x.PurchaseRequisitionID)).ToList();
            decimal requetedQty = pritems.Where(x => x.ItmCode == proformaItem.ItmCode).Sum(x => x.Quantity);

            if (proformaItem.Quantity > requetedQty)
                ModelState.AddModelError("above", $"Quantity is above requested quantity: {requetedQty} ");

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ProformaItem
                {
                    ProformaItemID = proformaItem.ProformaItemID,
                    ProformaPurchaseID = proformaItem.ProformaPurchaseID,
                    CatCode = proformaItem.CatCode,
                    CategoryName = proformaItem.CategoryName,
                    ItmCode = proformaItem.ItmCode,
                    ItemName = proformaItem.ItemName,
                    PartNo = proformaItem.PartNo,
                    Quantity = proformaItem.Quantity,
                    IsApproved = proformaItem.IsApproved,
                    Name = proformaItem.Name,
                    ModelNo = proformaItem.ModelNo,
                    ItemSpecification = proformaItem.ItemSpecification,
                    UoM = proformaItem.UoM,
                    ItemType = proformaItem.ItemType,
                    OrgBranchName = proformaItem.OrgBranchName,
                    DateCreated = proformaItem.DateCreated,
                    LastModified = proformaItem.LastModified,
                    CreatedBy = proformaItem.CreatedBy,
                    ModifiedBy = proformaItem.ModifiedBy
                };

                _db.ProformaItems.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "proforma item", "Success", User.Identity.Name, $"updated item {entity.ItemName} under record {entity.ProformaPurchaseID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { proformaItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaItems_Destroy([DataSourceRequest]DataSourceRequest request, ProformaItem proformaItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProformaItem
                {
                    ProformaItemID = proformaItem.ProformaItemID,
                    ProformaPurchaseID = proformaItem.ProformaPurchaseID,
                    CatCode = proformaItem.CatCode,
                    CategoryName = proformaItem.CategoryName,
                    ItmCode = proformaItem.ItmCode,
                    ItemName = proformaItem.ItemName,
                    PartNo = proformaItem.PartNo,
                    Quantity = proformaItem.Quantity,
                    IsApproved = proformaItem.IsApproved,
                    ItemSpecification = proformaItem.ItemSpecification,
                    UoM = proformaItem.UoM,
                    ItemType = proformaItem.ItemType,
                    OrgBranchName = proformaItem.OrgBranchName,
                    DateCreated = proformaItem.DateCreated,
                    LastModified = proformaItem.LastModified,
                    CreatedBy = proformaItem.CreatedBy,
                    ModifiedBy = proformaItem.ModifiedBy
                };

                _db.ProformaItems.Attach(entity);
                _db.ProformaItems.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { proformaItem }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult GetProformaItem(string id)
        {
            int pid = int.Parse(id);
            var items = _db.ProformaItems.Where(x => x.ProformaPurchaseID == pid);


            return Json(items.Select(i => new { Name = i.ItmCode + "_" + i.ItemName, ItmCode = i.ItmCode }), JsonRequestBehavior.AllowGet);

        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
