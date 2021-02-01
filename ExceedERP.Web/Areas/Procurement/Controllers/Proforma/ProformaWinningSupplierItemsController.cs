using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.GL;
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
   

    public class ProformaWinningSupplierItemItemsController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult WinningSupplierItems_Read([DataSourceRequest]DataSourceRequest request , int id )
        {
            IQueryable<WinningSupplierItem> winningSupplierItems = _db.WinningSupplierItems.Where(p => p.WinningSupplierID == id);
            DataSourceResult result = winningSupplierItems.ToDataSourceResult(request, winningSupplierItem => new {
                WinningSupplierItemId = winningSupplierItem.WinningSupplierItemId,
                ProformaItemID = winningSupplierItem.ProformaItemID,
               
                CatCode = winningSupplierItem.CatCode,
                CategoryName = winningSupplierItem.CategoryName,
                ItmCode = winningSupplierItem.ItmCode,
                ItemName = winningSupplierItem.ItemName,
                Name = winningSupplierItem.Name,
                ModelNo = winningSupplierItem.ModelNo,
                PartNo = winningSupplierItem.PartNo,
                IsApproved = winningSupplierItem.IsApproved,
                ItemSpecification = winningSupplierItem.ItemSpecification,
                UoM = winningSupplierItem.UoM,
                WinningSupplierID = winningSupplierItem.WinningSupplierID,
                GLTaxRateID = winningSupplierItem.GLTaxRateID,
                TaxAmount = winningSupplierItem.TaxAmount,
                Quanity = winningSupplierItem.Quanity,
                Discount = winningSupplierItem.Discount,
                UPrice = winningSupplierItem.UPrice,
                FinalUPrice = winningSupplierItem.FinalUPrice,
                ItemType = winningSupplierItem.ItemType,
                TPrice = winningSupplierItem.TPrice,
                OrgBranchName = winningSupplierItem.OrgBranchName,
                DateCreated = winningSupplierItem.DateCreated,
                LastModified = winningSupplierItem.LastModified,
                CreatedBy = winningSupplierItem.CreatedBy,
                ModifiedBy = winningSupplierItem.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WinningSupplierItems_Create([DataSourceRequest]DataSourceRequest request, WinningSupplierItem winningSupplierItem , int id )
        {
                var winner = _db.WinningSuppliers.Find(id);
                var participants = _db.ParticipatingSupplierItems.
                FirstOrDefault(x => x.ParticipatingSupplier.SupplierName == winner.SupplierName && x.ProformaItemID == winningSupplierItem.ProformaItemID);

                if (participants != null)
                {
                    GLTaxRate taxRate = _db.GLTaxRates.Find(participants.GLTaxRateID);
                    if (ModelState.IsValid)
                    {
                        if (taxRate != null)
                        {
                            var entity = new WinningSupplierItem
                            {
                                WinningSupplierID =id,
                                ProformaItemID = winningSupplierItem.ProformaItemID,
                                ItemType = winningSupplierItem.ItemType,
                                CatCode = winningSupplierItem.CatCode,
                                CategoryName = winningSupplierItem.CategoryName,
                                ItmCode = winningSupplierItem.ItmCode,
                                ItemName = winningSupplierItem.ItemName,
                                Name = winningSupplierItem.Name,
                                ModelNo = winningSupplierItem.ModelNo,
                                PartNo = winningSupplierItem.PartNo,
                                IsApproved = winningSupplierItem.IsApproved,
                                ItemSpecification = winningSupplierItem.ItemSpecification,
                                UoM = winningSupplierItem.UoM,
                                Quanity = participants.Quantity,
                                UPrice = participants.UPrice,
                                Discount = winningSupplierItem.Discount,
                                FinalUPrice = winningSupplierItem.FinalUPrice,
                                GLTaxRateID = participants.GLTaxRateID,
                                TaxAmount = participants.UPrice * participants.Quantity * taxRate.CalculatedRate,
                                TPrice = participants.UPrice * participants.Quantity + (participants.UPrice * participants.Quantity * taxRate.CalculatedRate),
                               
                                OrgBranchName = winningSupplierItem.OrgBranchName,
                                DateCreated = DateTime.Today,
                                LastModified = winningSupplierItem.LastModified,
                                CreatedBy = User.Identity.Name,
                                ModifiedBy = winningSupplierItem.ModifiedBy
                            };

                            _db.WinningSupplierItems.Add(entity);
                            _db.SaveChanges();
                            winningSupplierItem.WinningSupplierItemId = entity.WinningSupplierItemId;

                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "proforma purchase winner item", "Success", User.Identity.Name, $"created winner {entity.ItemName} under record {entity.WinningSupplierID}", Modules.Procurement);
                        #endregion
                    }
                }
                }

                return Json(new[] { winningSupplierItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WinningSupplierItems_Update([DataSourceRequest]DataSourceRequest request, WinningSupplierItem winningSupplierItem)
        {
            var winner = _db.WinningSuppliers.Find(winningSupplierItem.WinningSupplierID);
            var participants =_db.ParticipatingSupplierItems
                .FirstOrDefault(x => x.ParticipatingSupplier.SupplierName == winner.SupplierName && x.ProformaItemID == winningSupplierItem.ProformaItemID);

            if (participants != null)
            {
                GLTaxRate taxRate = _db.GLTaxRates.Find(participants.GLTaxRateID);
                if (ModelState.IsValid)
                {
                    if (taxRate != null)
                    {
                        var entity = new WinningSupplierItem
                        {
                            WinningSupplierID=winningSupplierItem.WinningSupplierID,
                            WinningSupplierItemId = winningSupplierItem.WinningSupplierItemId,
                            ProformaItemID = winningSupplierItem.ProformaItemID,
                            ItemType = winningSupplierItem.ItemType,
                            CatCode = winningSupplierItem.CatCode,
                            CategoryName = winningSupplierItem.CategoryName,
                            ItmCode = winningSupplierItem.ItmCode,
                            ItemName = winningSupplierItem.ItemName,
                            Name = winningSupplierItem.Name,
                            ModelNo = winningSupplierItem.ModelNo,
                            PartNo = winningSupplierItem.PartNo,
                            IsApproved = winningSupplierItem.IsApproved,
                            ItemSpecification = winningSupplierItem.ItemSpecification,
                            UoM = winningSupplierItem.UoM,
                            Quanity = participants.Quantity,
                            UPrice = participants.UPrice,
                            Discount = winningSupplierItem.Discount,
                            FinalUPrice = winningSupplierItem.FinalUPrice,
                            GLTaxRateID = participants.GLTaxRateID,
                            TaxAmount = participants.UPrice * participants.Quantity * taxRate.CalculatedRate,
                            TPrice = participants.UPrice * participants.Quantity + (participants.UPrice * participants.Quantity * taxRate.CalculatedRate),
                           
                            OrgBranchName = winningSupplierItem.OrgBranchName,
                            DateCreated = winningSupplierItem.DateCreated,
                            LastModified = DateTime.Today,
                            CreatedBy = winningSupplierItem.CreatedBy,
                            ModifiedBy = User.Identity.Name
                        };

                        _db.WinningSupplierItems.Attach(entity);
                        _db.Entry(entity).State = EntityState.Modified;
                        _db.SaveChanges();
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "proforma purchase winner item", "Success", User.Identity.Name, $"updated winner {entity.ItemName} under record {entity.WinningSupplierID}", Modules.Procurement);
                        #endregion
                    }
                   
                }
            }

            return Json(new[] { winningSupplierItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WinningSupplierItems_Destroy([DataSourceRequest]DataSourceRequest request, WinningSupplierItem winningSupplierItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new WinningSupplierItem
                {
                    WinningSupplierItemId = winningSupplierItem.WinningSupplierItemId,
                    ProformaItemID = winningSupplierItem.ProformaItemID,
                    ItemType = winningSupplierItem.ItemType,
                    CatCode = winningSupplierItem.CatCode,
                    CategoryName = winningSupplierItem.CategoryName,
                    ItmCode = winningSupplierItem.ItmCode,
                    ItemName = winningSupplierItem.ItemName,
                    PartNo = winningSupplierItem.PartNo,
                    IsApproved = winningSupplierItem.IsApproved,
                    ItemSpecification = winningSupplierItem.ItemSpecification,
                    UoM = winningSupplierItem.UoM,
                    WinningSupplierID = winningSupplierItem.WinningSupplierID,
                    Quanity = winningSupplierItem.Quanity,
                    GLTaxRateID = winningSupplierItem.GLTaxRateID,
                    UPrice = winningSupplierItem.UPrice,
                    Discount = winningSupplierItem.Discount,
                    FinalUPrice = winningSupplierItem.FinalUPrice,
                    TPrice = winningSupplierItem.TPrice,
                    OrgBranchName = winningSupplierItem.OrgBranchName,
                    DateCreated = winningSupplierItem.DateCreated,
                    LastModified = winningSupplierItem.LastModified,
                    CreatedBy = winningSupplierItem.CreatedBy,
                    ModifiedBy = winningSupplierItem.ModifiedBy
                };

                _db.WinningSupplierItems.Attach(entity);
                _db.WinningSupplierItems.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { winningSupplierItem }.ToDataSourceResult(request, ModelState));
        }

        
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
