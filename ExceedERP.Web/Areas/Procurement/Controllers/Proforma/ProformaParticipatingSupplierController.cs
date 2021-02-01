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


    public class ProformaParticipatingSupplierController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult ParticipatingSuppliers_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ParticipatingSupplier> participatingsuppliers = _db.ParticipatingSuppliers.Where(p => p.ProformaPurchaseID == id);
            DataSourceResult result = participatingsuppliers.ToDataSourceResult(request, participatingSupplier => new
            {
                ParticipatingSupplierID = participatingSupplier.ParticipatingSupplierID,
                ProformaPurchaseID = participatingSupplier.ProformaPurchaseID,
                CatCode = participatingSupplier.CatCode,
                CategoryName = participatingSupplier.CategoryName,
                ItmCode = participatingSupplier.ItmCode,
                ItemName = participatingSupplier.ItemName,
                PartNo = participatingSupplier.PartNo,
                IsApproved = participatingSupplier.IsApproved,
                Name = participatingSupplier.Name,
                ItemSpecification = participatingSupplier.ItemSpecification,
                UoM = participatingSupplier.UoM,
                ItemType = participatingSupplier.ItemType,
                ProformaItemID = participatingSupplier.ProformaItemID,
                GLTaxRateID = participatingSupplier.GLTaxRateID,
                TaxAmount = participatingSupplier.TaxAmount,
                Quantity = participatingSupplier.Quantity,
                UPrice = participatingSupplier.UPrice,
                TPrice = participatingSupplier.TPrice,
                SupplierId = participatingSupplier.SupplierId,
                IsNewSupplier = participatingSupplier.IsNewSupplier,
                SupplierName = participatingSupplier.SupplierName,
                Representative = participatingSupplier.Representative,
                Address = participatingSupplier.Address,
                Telephone = participatingSupplier.Telephone,
                ProformaSubmitionDate = participatingSupplier.ProformaSubmitionDate,
                ProformaSubmitionTime = participatingSupplier.ProformaSubmitionTime,
                IsOutOfStock = participatingSupplier.IsOutOfStock,
                Remark = participatingSupplier.Remark,
                OrgBranchName = participatingSupplier.OrgBranchName,
                DateCreated = participatingSupplier.DateCreated,
                LastModified = participatingSupplier.LastModified,
                CreatedBy = participatingSupplier.CreatedBy,
                ModifiedBy = participatingSupplier.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ParticipatingSuppliers_Create([DataSourceRequest]DataSourceRequest request, ParticipatingSupplier participatingSupplier, int id)
        {

            #region supplier validation
            if (!participatingSupplier.IsNewSupplier)
            {
                if (String.IsNullOrEmpty(participatingSupplier.SupplierId))
                {
                    ModelState.AddModelError("supplierid", "Please select supplier.");
                }
                else
                {
                    var supplier = _db.ProcurementSuppliers.FirstOrDefault(x => x.SupplierId == participatingSupplier.SupplierId);
                    if (supplier != null)
                        participatingSupplier.SupplierName = supplier.Name;
                }
            }

            else
            {
                if (String.IsNullOrEmpty(participatingSupplier.SupplierName))
                {
                    ModelState.AddModelError("suppliername", "Please enter supplier name.");
                    participatingSupplier.SupplierId = "";
                }
            }

            #endregion
            var tax = _db.GLTaxRates.Find(participatingSupplier.GLTaxRateID);
           // var proformaItem = _db.ProformaItems.FirstOrDefault(x => x.ItmCode == participatingSupplier.ItmCode && x.ProformaPurchaseID == id);

            

            if (ModelState.IsValid)
            {
                var entity = new ParticipatingSupplier
                {
                    ProformaPurchaseID = id,
                    ProformaItemID = 0,
                    SupplierId = participatingSupplier.SupplierId,
                    IsNewSupplier = participatingSupplier.IsNewSupplier,
                    SupplierName = participatingSupplier.SupplierName,
                    Representative = participatingSupplier.Representative,
                    Address = participatingSupplier.Address,
                    Telephone = participatingSupplier.Telephone,
                    ProformaSubmitionDate = participatingSupplier.ProformaSubmitionDate,
                    ProformaSubmitionTime = participatingSupplier.ProformaSubmitionTime,
                    IsOutOfStock = participatingSupplier.IsOutOfStock,
                    Remark = participatingSupplier.Remark,
                    OrgBranchName = participatingSupplier.OrgBranchName,
                    DateCreated = participatingSupplier.DateCreated,
                    LastModified = participatingSupplier.LastModified,
                    CreatedBy = participatingSupplier.CreatedBy,
                    ModifiedBy = participatingSupplier.ModifiedBy
                };

                _db.ParticipatingSuppliers.Add(entity);
                _db.SaveChanges();
                participatingSupplier.ParticipatingSupplierID = entity.ParticipatingSupplierID;

                GetParticipantItems(entity.ParticipatingSupplierID);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "proforma suppliers", "Success", User.Identity.Name, $"created supplier {entity.SupplierName} under record {entity.ProformaPurchaseID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { participatingSupplier }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ParticipatingSuppliers_Update([DataSourceRequest]DataSourceRequest request, ParticipatingSupplier participatingSupplier)
        {
            #region supplier validation
            if (!participatingSupplier.IsNewSupplier)
            {
                if (String.IsNullOrEmpty(participatingSupplier.SupplierId))
                {
                    ModelState.AddModelError("supplierid", "Please select supplier.");
                }
                else
                {
                    var supplier = _db.ProcurementSuppliers.FirstOrDefault(x => x.SupplierId == participatingSupplier.SupplierId);
                    if (supplier != null) participatingSupplier.SupplierName = supplier.Name;
                }
            }

            else
            {
                if (String.IsNullOrEmpty(participatingSupplier.SupplierName))
                {
                    ModelState.AddModelError("suppliername", "Please enter supplier name.");
                    participatingSupplier.SupplierId = "";
                }
            }

            #endregion

            if (ModelState.IsValid)
            {
                var entity = new ParticipatingSupplier
                {
                    ParticipatingSupplierID = participatingSupplier.ParticipatingSupplierID,
                    ProformaItemID = participatingSupplier.ProformaItemID,
                    ProformaPurchaseID = participatingSupplier.ProformaPurchaseID,
                    SupplierId = participatingSupplier.SupplierId,
                    IsNewSupplier = participatingSupplier.IsNewSupplier,
                    SupplierName = participatingSupplier.SupplierName,
                    Representative = participatingSupplier.Representative,
                    Address = participatingSupplier.Address,
                    Telephone = participatingSupplier.Telephone,
                    ProformaSubmitionDate = participatingSupplier.ProformaSubmitionDate,
                    ProformaSubmitionTime = participatingSupplier.ProformaSubmitionTime,
                    IsOutOfStock = participatingSupplier.IsOutOfStock,
                    Remark = participatingSupplier.Remark,
                    OrgBranchName = participatingSupplier.OrgBranchName,
                    DateCreated = participatingSupplier.DateCreated,
                    LastModified = participatingSupplier.LastModified,
                    CreatedBy = participatingSupplier.CreatedBy,
                    ModifiedBy = participatingSupplier.ModifiedBy
                };

                _db.ParticipatingSuppliers.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "proforma suppliers", "Success", User.Identity.Name, $"updated supplier {entity.SupplierName} under record {entity.ProformaPurchaseID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { participatingSupplier }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ParticipatingSuppliers_Destroy([DataSourceRequest]DataSourceRequest request, ParticipatingSupplier participatingSupplier)
        {
            if (ModelState.IsValid)
            {
                var entity = new ParticipatingSupplier
                {
                    ParticipatingSupplierID = participatingSupplier.ParticipatingSupplierID,
                    ProformaPurchaseID = participatingSupplier.ProformaPurchaseID,
                    CatCode = participatingSupplier.CatCode,
                    CategoryName = participatingSupplier.CategoryName,
                    ItmCode = participatingSupplier.ItmCode,
                    ItemName = participatingSupplier.ItemName,
                    PartNo = participatingSupplier.PartNo,
                    IsApproved = participatingSupplier.IsApproved,
                    ItemSpecification = participatingSupplier.ItemSpecification,
                    UoM = participatingSupplier.UoM,
                    ItemType = participatingSupplier.ItemType,
                    GLTaxRateID = participatingSupplier.GLTaxRateID,
                    TaxAmount = participatingSupplier.TaxAmount,
                    Quantity = participatingSupplier.Quantity,
                    UPrice = participatingSupplier.UPrice,
                    TPrice = participatingSupplier.TPrice,
                    ProformaItemID = participatingSupplier.ProformaItemID,
                    SupplierId = participatingSupplier.SupplierId,
                    IsNewSupplier = participatingSupplier.IsNewSupplier,
                    SupplierName = participatingSupplier.SupplierName,
                    Representative = participatingSupplier.Representative,
                    Address = participatingSupplier.Address,
                    Telephone = participatingSupplier.Telephone,
                    ProformaSubmitionDate = participatingSupplier.ProformaSubmitionDate,
                    ProformaSubmitionTime = participatingSupplier.ProformaSubmitionTime,
                    Remark = participatingSupplier.Remark,
                    OrgBranchName = participatingSupplier.OrgBranchName,
                    DateCreated = participatingSupplier.DateCreated,
                    LastModified = participatingSupplier.LastModified,
                    CreatedBy = participatingSupplier.CreatedBy,
                    ModifiedBy = participatingSupplier.ModifiedBy
                };

                _db.ParticipatingSuppliers.Attach(entity);
                _db.ParticipatingSuppliers.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { participatingSupplier }.ToDataSourceResult(request, ModelState));
        }

        private void GetParticipantItems(int participantId)
        {
            var record = _db.ParticipatingSuppliers.Find(participantId);
            var proformaItems = _db.ProformaItems.Where(x=>x.ProformaPurchaseID==record.ProformaPurchaseID).ToList();
            foreach (var proformaItem in proformaItems)
            {
                var entity = new ParticipatingSupplierItem
                {
                    ParticipatingSupplierID = participantId,
                    ProformaItemID = proformaItem.ProformaItemID,
                    CatCode = proformaItem.CatCode,
                    CategoryName = proformaItem.CategoryName,
                    ItmCode = proformaItem.ItmCode,
                    ItemName = proformaItem.ItemName,
                    PartNo = proformaItem.PartNo,
                    Name = proformaItem.Name,
                    ItemSpecification = proformaItem.ItemSpecification,
                    UoM = proformaItem.UoM,
                    ItemType = proformaItem.ItemType,
                    GLTaxRateID = 0,
                    TaxAmount = 0,
                    Quantity = proformaItem.Quantity,
                    UPrice = 0,
                    TPrice = 0,

                    IsOutOfStock = false
                };

                _db.ParticipatingSupplierItems.Add(entity);
                _db.SaveChanges();
            }

        }

        public JsonResult GetRegisteredSuppliers(string itemCode, string id)
        {
            int pid = int.Parse(id);
            var requestedSuppliers = _db.RequestedSuppliers.Where(x => x.ItmCode == itemCode && x.ProformaPurchaseID == pid).AsQueryable();
            return Json(requestedSuppliers.Select(i => new { Supplier = i.SupplierName }), JsonRequestBehavior.AllowGet);

        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
