using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Proforma;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Proforma
{
    public class ProformTechnicalController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult ProformTechnicals_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            IQueryable<ProformTechnical> proformtechnicals = _db.ProformTechnicals.Where(x => x.ProformaPurchaseID == id).OrderBy(x=>x.ItmCode).ThenByDescending(x=>x.Result);
            DataSourceResult result = proformtechnicals.ToDataSourceResult(request, proformTechnical => new {
                ProformaPurchaseID = proformTechnical.ProformaPurchaseID,
                ProformTechnicalId = proformTechnical.ProformTechnicalId,
                SupplierName = proformTechnical.SupplierName,
                SupplierId = proformTechnical.SupplierId,
                CatCode = proformTechnical.CatCode,
                CategoryName = proformTechnical.CategoryName,
                ItmCode = proformTechnical.ItmCode,
                ItemName = proformTechnical.ItemName,
                Name = proformTechnical.Name,
                ItemSpecification = proformTechnical.ItemSpecification,
                ModelNo = proformTechnical.ModelNo,
                ProformaItemID = proformTechnical.ProformaItemID,
                Result = proformTechnical.Result,
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformTechnicals_Create(int id,[DataSourceRequest]DataSourceRequest request, ProformTechnical proformTechnical)
        {
            #region validation
            var type = _db.ProformaEvaluationCriterias.Where(x => x.ProformaPurchaseID == id)
               .Where(x => x.Type == CriteriaType.Technical).ToList();
            if (!type.Any())
            {
                ModelState.AddModelError("invalidaCriteria", "You cannot fill technical result since there is no technical criteria.");
            }

            var criteria = _db.ProformaEvaluationCriterias.FirstOrDefault(x => x.ProformaPurchaseID == id && x.Type == CriteriaType.Technical);
            if (criteria != null)
            {
                if (proformTechnical.Result > criteria.Weight)
                {
                    ModelState.AddModelError("aboveWeight", "The result cannot be above the criteria weight " + criteria.Weight);
                }

            }

            if (_db.ProformTechnicals.Any(x => x.ItmCode == proformTechnical.ItmCode && x.SupplierName == proformTechnical.SupplierName && x.ProformaPurchaseID ==id))
            {
                ModelState.AddModelError("alreadyExist", "You have already assigned this Item for the selected Supplier");
            }

            #endregion

            var existing = _db.ProformTechnicals
                .Where(x => x.ProformaPurchaseID == proformTechnical.ProformaPurchaseID)
                .ToList();

            var participatingSuppliers = _db.ParticipatingSupplierItems
                .Where(x => x.ParticipatingSupplier.ProformaPurchaseID == id).Where(x=>x.IsOutOfStock==false).ToList();
            if (ModelState.IsValid)
            {
                _db.ProformTechnicals.RemoveRange(existing);
                _db.SaveChanges();

                foreach (var participating in participatingSuppliers)
                {
                    var entity = new ProformTechnical
                    {
                        ProformaPurchaseID = id,
                        SupplierName = participating.ParticipatingSupplier.SupplierName,
                        SupplierId = participating.ParticipatingSupplier.SupplierId,
                        IsNewSupplier = participating.ParticipatingSupplier.IsNewSupplier,
                        ProformaItemID = participating.ProformaItemID,
                        CatCode = participating.CatCode,
                        CategoryName = participating.CategoryName,
                        ItmCode = participating.ItmCode,
                        ItemName = participating.ItemName,
                        Name = proformTechnical.Name,
                        ItemSpecification = participating.ItemSpecification,
                        ModelNo = proformTechnical.ModelNo,
                        Result = proformTechnical.Result,
                        DateCreated = DateTime.Now,
                        LastModified = proformTechnical.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = proformTechnical.ModifiedBy
                    };

                    _db.ProformTechnicals.Add(entity);
                    _db.SaveChanges();
                    proformTechnical.ProformTechnicalId = entity.ProformTechnicalId;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "proforma technical", "Success", User.Identity.Name, $"created technical for {entity.SupplierName} under record {entity.ProformaPurchaseID}", Modules.Procurement);
                    #endregion
                }
            }

            return Json(new[] { proformTechnical }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformTechnicals_Update([DataSourceRequest]DataSourceRequest request, ProformTechnical proformTechnical)
        {
            #region validation
            var type = _db.ProformaEvaluationCriterias.Where(x => x.ProformaPurchaseID == proformTechnical.ProformaPurchaseID)
               .Where(x => x.Type == CriteriaType.Technical).ToList();
            if (!type.Any())
            {
                ModelState.AddModelError("invalidaCriteria", "You cannot fill technical result since there is no technical criteria.");
            }

            var criteria = _db.ProformaEvaluationCriterias.FirstOrDefault(x => x.ProformaPurchaseID == proformTechnical.ProformaPurchaseID && x.Type == CriteriaType.Technical);
            if (proformTechnical.Result > criteria?.Weight)
                ModelState.AddModelError("aboveWeight", "The result cannot be above the criteria weight " + criteria.Weight);

            var assigned = _db.ProformTechnicals.Where(x => x.ProformTechnicalId != proformTechnical.ProformTechnicalId)
                .Where(x => x.ItmCode == proformTechnical.ItmCode)
                .Where(x => x.SupplierName == proformTechnical.SupplierName)
                .Where(x => x.ProformaPurchaseID == proformTechnical.ProformaPurchaseID).ToList();
            if(assigned.Any())
                ModelState.AddModelError("alreadyExist", "You have already assigned this Item for the selected Supplier");

            var participatingSuppliers = _db.ParticipatingSupplierItems
                .Where(x => x.ParticipatingSupplier.SupplierName == proformTechnical.SupplierName)
                .FirstOrDefault(x => x.ItmCode == proformTechnical.ItmCode);

            if(participatingSuppliers != null && participatingSuppliers.IsOutOfStock)
                ModelState.AddModelError("outofstock", $"the item {proformTechnical.ItmCode} is out of stock for supplier {proformTechnical.SupplierName}");

            #endregion

            //var proformaItem = new ProformaItem();

            //if (!string.IsNullOrEmpty(proformTechnical.ItmCode))
            //{
            //    string itemcode = proformTechnical.ItmCode.Split('_')[0];
            //    string name = proformTechnical.ItmCode.Split('_')[1];

            //    proformTechnical.ItmCode = itemcode;
            //    proformTechnical.Name = name;

            //    if (!String.IsNullOrEmpty(name))
            //    {
            //        var item = _db.ProformaItems
            //            .Where(x => x.ProformaPurchaseID == proformTechnical.ProformaPurchaseID)
            //            .Where(x => x.ItmCode == itemcode)
            //            .FirstOrDefault(x => x.Name == name);
            //        if (item == null)
            //            ModelState.AddModelError("notfound", "Proforma Item not found.");
            //        proformaItem = item;
            //    }
            //    if (String.IsNullOrEmpty(name))
            //    {
            //        var item = _db.ProformaItems
            //            .Where(x => x.ProformaPurchaseID == proformTechnical.ProformaPurchaseID)
            //            .FirstOrDefault(x => x.ItmCode == itemcode);
            //        if (item == null)
            //            ModelState.AddModelError("notfound", "Proforma Item not found.");

            //        proformaItem = item;
            //    }
            //}
            //else
            //{
            //    ModelState.AddModelError("selectite", "Please select item");
            //}

            if (proformTechnical.ProformaItemID == 0)
                ModelState.AddModelError("selectitem", "Please Select an Item.");

            #region get proforma item info

            var participatingSupplier = _db.ParticipatingSupplierItems
                .Where(x=>x.IsOutOfStock==false)
                .Where(x => x.ParticipatingSupplier.SupplierName == proformTechnical.SupplierName)
                .FirstOrDefault(x=>x.ProformaItemID==proformTechnical.ProformaItemID);

            if(participatingSupplier==null)
                ModelState.AddModelError("didnotparticipate",$"The supplier {proformTechnical.SupplierName} did not participate or is out of stock for this item.");

            if (participatingSupplier != null)
            {
                proformTechnical.CatCode = participatingSupplier.CatCode;
                proformTechnical.CategoryName = participatingSupplier.CategoryName;
                proformTechnical.ItmCode = participatingSupplier.ItmCode;
                proformTechnical.ItemName = participatingSupplier.ItemName;
                proformTechnical.Name = participatingSupplier.Name;
                proformTechnical.ModelNo = participatingSupplier.ModelNo;
                proformTechnical.ItemSpecification = participatingSupplier.ItemSpecification;
            }

            #endregion

            if (ModelState.IsValid)
            {
                if (participatingSupplier != null)
                {
                    var entity = new ProformTechnical
                    {
                        ProformaPurchaseID = proformTechnical.ProformaPurchaseID,
                        ProformTechnicalId = proformTechnical.ProformTechnicalId,
                        SupplierName = proformTechnical.SupplierName,
                        SupplierId = proformTechnical.SupplierId,
                        ProformaItemID = proformTechnical.ProformaItemID,
                        CatCode = proformTechnical.CatCode,
                        CategoryName = proformTechnical.CategoryName,
                        ItemName = participatingSupplier.ItemName,
                        ItmCode = participatingSupplier.ItmCode,
                        Name = participatingSupplier.Name,
                        ItemSpecification = participatingSupplier.ItemSpecification,
                        ModelNo = proformTechnical.ModelNo,
                        Result = proformTechnical.Result,
                    };

                    _db.ProformTechnicals.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "proforma technical", "Success", User.Identity.Name, $"updated technical for {entity.SupplierName} under record {entity.ProformaPurchaseID}", Modules.Procurement);
                    #endregion
                }
               
            }

            return Json(new[] { proformTechnical }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformTechnicals_Destroy([DataSourceRequest]DataSourceRequest request, ProformTechnical proformTechnical)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProformTechnical
                {
                    ProformaPurchaseID = proformTechnical.ProformaPurchaseID,
                    ProformTechnicalId = proformTechnical.ProformTechnicalId,
                    SupplierName = proformTechnical.SupplierName,
                    SupplierId = proformTechnical.SupplierId,
                    ProformaItemID = proformTechnical.ProformaItemID,
                    ItmCode = proformTechnical.ItmCode,
                    Name = proformTechnical.Name,
                    Result = proformTechnical.Result,
                };

                _db.ProformTechnicals.Attach(entity);
                _db.ProformTechnicals.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { proformTechnical }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
