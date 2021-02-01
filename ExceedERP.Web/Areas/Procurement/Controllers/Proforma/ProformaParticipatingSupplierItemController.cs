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


    public class ProformaParticipatingSupplierItemController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult ParticipatingSupplierItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ParticipatingSupplierItem> participatingSupplierItems = _db.ParticipatingSupplierItems.Where(p => p.ParticipatingSupplierID == id);
            DataSourceResult result = participatingSupplierItems.ToDataSourceResult(request, participatingSupplier => new
            {
                ParticipatingSupplierItemId = participatingSupplier.ParticipatingSupplierItemId,
                ParticipatingSupplierID = participatingSupplier.ParticipatingSupplierID,
                CatCode = participatingSupplier.CatCode,
                CategoryName = participatingSupplier.CategoryName,
                ItmCode = participatingSupplier.ItmCode,
                ItemName = participatingSupplier.ItemName,
                PartNo = participatingSupplier.PartNo,
                Name = participatingSupplier.Name,
                ModelNo = participatingSupplier.ModelNo,
                ItemSpecification = participatingSupplier.ItemSpecification,
                UoM = participatingSupplier.UoM,
                ItemType = participatingSupplier.ItemType,
                ProformaItemID = participatingSupplier.ProformaItemID,
                GLTaxRateID = participatingSupplier.GLTaxRateID,
                TaxAmount = participatingSupplier.TaxAmount,
                Quantity = participatingSupplier.Quantity,
                UPrice = participatingSupplier.UPrice,
                TPrice = participatingSupplier.TPrice,
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
        public ActionResult ParticipatingSupplierItems_Create([DataSourceRequest]DataSourceRequest request, ParticipatingSupplierItem participatingSupplier, int id)
        {
            var tax = _db.GLTaxRates.Find(participatingSupplier.GLTaxRateID);
           
            var proformaItem = _db.ProformaItems.Find(participatingSupplier.ProformaItemID);

            if (ModelState.IsValid)
            {
                if (tax != null&&proformaItem!=null)
                {
                    
                    var exist = _db.ParticipatingSupplierItems
                        .Where(x => x.ParticipatingSupplierID == id)
                        .FirstOrDefault(x => x.ProformaItemID == proformaItem.ProformaItemID);

                    if (exist == null)
                    {
                        var entity = new ParticipatingSupplierItem
                        {
                            ParticipatingSupplierID = id,
                            ProformaItemID = proformaItem.ProformaItemID,
                            CatCode = proformaItem.CatCode,
                            CategoryName = proformaItem.CategoryName,
                            ItmCode = proformaItem.ItmCode,
                            ItemName = proformaItem.ItemName,
                            PartNo = proformaItem.PartNo,
                            Name = proformaItem.Name,
                            ModelNo = proformaItem.ModelNo,
                            ItemSpecification = proformaItem.ItemSpecification,
                            UoM = proformaItem.UoM,
                            ItemType = proformaItem.ItemType,
                            GLTaxRateID = participatingSupplier.GLTaxRateID,
                            TaxAmount = (tax.CalculatedRate * proformaItem.Quantity * participatingSupplier.UPrice),
                            Quantity = proformaItem.Quantity,
                            UPrice = participatingSupplier.UPrice,
                            TPrice = proformaItem.Quantity * participatingSupplier.UPrice +
                                        (tax.CalculatedRate * proformaItem.Quantity * participatingSupplier.UPrice),
                                
                            IsOutOfStock = participatingSupplier.IsOutOfStock,
                            Remark = participatingSupplier.Remark,
                            OrgBranchName = participatingSupplier.OrgBranchName,
                            DateCreated = participatingSupplier.DateCreated,
                            LastModified = participatingSupplier.LastModified,
                            CreatedBy = participatingSupplier.CreatedBy,
                            ModifiedBy = participatingSupplier.ModifiedBy
                        };

                        _db.ParticipatingSupplierItems.Add(entity);
                        _db.SaveChanges();
                        participatingSupplier.ParticipatingSupplierItemId = entity.ParticipatingSupplierItemId;

                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "proforma supplier item", "Success", User.Identity.Name, $"created supplier item {entity.ItemName} under record {entity.ParticipatingSupplierID}", Modules.Procurement);
                        #endregion
                    }

                }
            }

            return Json(new[] { participatingSupplier }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ParticipatingSupplierItems_Update([DataSourceRequest]DataSourceRequest request, ParticipatingSupplierItem participatingSupplier)
        {
            var tax = _db.GLTaxRates.Find(participatingSupplier.GLTaxRateID);

            var proformaItem = _db.ProformaItems.Find(participatingSupplier.ProformaItemID);

            #region get proforma item info

            
            if (proformaItem != null)
            {
                participatingSupplier.CatCode = proformaItem.CatCode;
                participatingSupplier.CategoryName = proformaItem.CategoryName;
                participatingSupplier.ItmCode = proformaItem.ItmCode;
                participatingSupplier.ItemName = proformaItem.ItemName;
                participatingSupplier.Name = proformaItem.Name;
                participatingSupplier.ModelNo = proformaItem.ModelNo;
                participatingSupplier.Quantity = proformaItem.Quantity;
            }

            #endregion

           

            if (ModelState.IsValid)
            {
                if (tax != null)
                {
                    if (proformaItem != null)
                    {
                        var entity = new ParticipatingSupplierItem
                        {
                            ParticipatingSupplierItemId = participatingSupplier.ParticipatingSupplierItemId,
                            ParticipatingSupplierID = participatingSupplier.ParticipatingSupplierID,
                            CatCode = participatingSupplier.CatCode,
                            CategoryName = participatingSupplier.CategoryName,
                            ItmCode = participatingSupplier.ItmCode,
                            ItemName = proformaItem.ItemName,
                            PartNo = participatingSupplier.PartNo,
                            Name = proformaItem.Name,
                            ModelNo = proformaItem.ModelNo,
                            ItemSpecification = participatingSupplier.ItemSpecification,
                            UoM = participatingSupplier.UoM,
                            ItemType = participatingSupplier.ItemType,
                            ProformaItemID = participatingSupplier.ProformaItemID,
                            GLTaxRateID = participatingSupplier.GLTaxRateID,
                            TaxAmount = (tax.CalculatedRate * proformaItem.Quantity * participatingSupplier.UPrice),
                            Quantity = proformaItem.Quantity,
                            UPrice = participatingSupplier.UPrice,
                            TPrice = proformaItem.Quantity * participatingSupplier.UPrice + (tax.CalculatedRate * proformaItem.Quantity * participatingSupplier.UPrice),
                            
                            IsOutOfStock = participatingSupplier.IsOutOfStock,
                            Remark = participatingSupplier.Remark,
                            OrgBranchName = participatingSupplier.OrgBranchName,
                            DateCreated = participatingSupplier.DateCreated,
                            LastModified = participatingSupplier.LastModified,
                            CreatedBy = participatingSupplier.CreatedBy,
                            ModifiedBy = participatingSupplier.ModifiedBy
                        };

                        _db.ParticipatingSupplierItems.Attach(entity);
                        _db.Entry(entity).State = EntityState.Modified;
                        _db.SaveChanges();
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "proforma supplier item", "Success", User.Identity.Name, $"updated supplier item {entity.ItemName} under record {entity.ParticipatingSupplierID}", Modules.Procurement);
                        #endregion
                    }
                }
               
            }

            return Json(new[] { participatingSupplier }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ParticipatingSupplierItems_Destroy([DataSourceRequest]DataSourceRequest request, ParticipatingSupplierItem participatingSupplier)
        {
            if (ModelState.IsValid)
            {
                var entity = new ParticipatingSupplierItem
                {
                    ParticipatingSupplierItemId = participatingSupplier.ParticipatingSupplierItemId,
                    ParticipatingSupplierID = participatingSupplier.ParticipatingSupplierID,
                    CatCode = participatingSupplier.CatCode,
                    CategoryName = participatingSupplier.CategoryName,
                    ItmCode = participatingSupplier.ItmCode,
                    ItemName = participatingSupplier.ItemName,
                    PartNo = participatingSupplier.PartNo,
                    ItemSpecification = participatingSupplier.ItemSpecification,
                    UoM = participatingSupplier.UoM,
                    ItemType = participatingSupplier.ItemType,
                    GLTaxRateID = participatingSupplier.GLTaxRateID,
                    TaxAmount = participatingSupplier.TaxAmount,
                    Quantity = participatingSupplier.Quantity,
                    UPrice = participatingSupplier.UPrice,
                    TPrice = participatingSupplier.TPrice,
                    ProformaItemID = participatingSupplier.ProformaItemID,
                    Remark = participatingSupplier.Remark,
                    OrgBranchName = participatingSupplier.OrgBranchName,
                    DateCreated = participatingSupplier.DateCreated,
                    LastModified = participatingSupplier.LastModified,
                    CreatedBy = participatingSupplier.CreatedBy,
                    ModifiedBy = participatingSupplier.ModifiedBy
                };

                _db.ParticipatingSupplierItems.Attach(entity);
                _db.ParticipatingSupplierItems.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { participatingSupplier }.ToDataSourceResult(request, ModelState));
        }
        
        
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
