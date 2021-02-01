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
    public class ProformPassFailController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult ProformPassFails_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            IQueryable<ProformPassFail> proformpassfails = _db.ProformPassFails.Where(x=>x.ProformaPurchaseID==id);
            DataSourceResult result = proformpassfails.ToDataSourceResult(request, proformPassFail => new {
                ProformaPurchaseID = id,
                ProformPassFailId = proformPassFail.ProformPassFailId,
                SupplierId = proformPassFail.SupplierId,
                IsNewSupplier = proformPassFail.IsNewSupplier,
                SupplierName = proformPassFail.SupplierName,
                ProformaItemID = proformPassFail.ProformaItemID,
                CatCode = proformPassFail.CatCode,
                CategoryName = proformPassFail.CategoryName,
                ItmCode = proformPassFail.ItmCode,
                ItemName = proformPassFail.ItemName,
                Name = proformPassFail.Name,
                ItemSpecification = proformPassFail.ItemSpecification,
                ModelNo = proformPassFail.ModelNo,
                Result = proformPassFail.Result,
                Remark = proformPassFail.Remark,
                DateCreated = proformPassFail.DateCreated,
                LastModified = proformPassFail.LastModified,
                CreatedBy = proformPassFail.CreatedBy,
                ModifiedBy = proformPassFail.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformPassFails_Create(int id,[DataSourceRequest]DataSourceRequest request, ProformPassFail proformPassFail)
        {
            #region validation
            var criteria = _db.ProformaEvaluationCriterias.Where(x => x.ProformaPurchaseID == id)
               .Where(x => x.Type == CriteriaType.Pass_Fail).ToList();
            if (!criteria.Any())
            {
                ModelState.AddModelError("invalidaCriteria", "You cannot fill pass fail result since there is no pass fail criteria.");
            }
            var assigned = _db.ProformPassFails
                .Where(x => x.ItmCode == proformPassFail.ItmCode)
                .Where(x => x.SupplierName == proformPassFail.SupplierName)
                .Where(x => x.ProformaPurchaseID ==id).ToList();
            if (assigned.Any())
            {
                ModelState.AddModelError("alreadyExist", "You have already assigned this Item for the selected Supplier");
            }

            var supplier = _db.ParticipatingSupplierItems
                .Where(x => x.ParticipatingSupplier.SupplierName == proformPassFail.SupplierName)
                .FirstOrDefault(x => x.ItmCode == proformPassFail.ItmCode);

            if (supplier != null && supplier.IsOutOfStock)
                ModelState.AddModelError("outofstock", $"the item {proformPassFail.ItmCode} is out of stock for supplier {proformPassFail.SupplierName}");


            #endregion

            var existing = _db.ProformPassFails.Where(x => x.ProformaPurchaseID == id).ToList();

            var participatingSuppliers = _db.ParticipatingSupplierItems
                .Where(x => x.ParticipatingSupplier.ProformaPurchaseID == id)
                .Where(x => x.IsOutOfStock == false).ToList();

            if (ModelState.IsValid)
            {
                _db.ProformPassFails.RemoveRange(existing);
                _db.SaveChanges();
                foreach (var participating in participatingSuppliers)
                {
                    var entity = new ProformPassFail
                    {
                        ProformaPurchaseID = id,
                        ProformaItemID = participating.ProformaItemID,
                        SupplierId = participating.ParticipatingSupplier.SupplierId,
                        IsNewSupplier = participating.ParticipatingSupplier.IsNewSupplier,
                        SupplierName = participating.ParticipatingSupplier.SupplierName,
                        CatCode = participating.CatCode,
                        CategoryName = participating.CategoryName,
                        ItmCode = participating.ItmCode,
                        ItemName = participating.ItemName,
                        Name = participating.Name,
                        ItemSpecification = participating.ItemSpecification,
                        ModelNo = proformPassFail.ModelNo,
                        Result = proformPassFail.Result,
                        Remark = proformPassFail.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = proformPassFail.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = proformPassFail.ModifiedBy
                    };

                    _db.ProformPassFails.Add(entity);
                    _db.SaveChanges();
                    proformPassFail.ProformPassFailId = entity.ProformPassFailId;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "proforma pass fail", "Success", User.Identity.Name, $"created pass fail for {entity.SupplierName} under record {entity.ProformaPurchaseID}", Modules.Procurement);
                    #endregion
                }
            }

            return Json(new[] { proformPassFail }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformPassFails_Update([DataSourceRequest]DataSourceRequest request, ProformPassFail proformPassFail)
        {
            #region validation
            var criteria = _db.ProformaEvaluationCriterias.Where(x => x.ProformaPurchaseID == proformPassFail.ProformaPurchaseID)
               .Where(x => x.Type ==CriteriaType.Pass_Fail).ToList();
            if (!criteria.Any())
            {
                ModelState.AddModelError("invalidaCriteria", "You cannot fill pass fail result since there is no pass fail criteria.");
            }

           
            var assigned = _db.ProformPassFails.Where(x => x.ProformPassFailId != proformPassFail.ProformPassFailId)
                .Where(x => x.ItmCode == proformPassFail.ItmCode)
                .Where(x => x.SupplierName == proformPassFail.SupplierName)
                .Where(x => x.ProformaPurchaseID == proformPassFail.ProformaPurchaseID).ToList();

            if (assigned.Any())
                ModelState.AddModelError("alreadyExist", "You have already assigned this Item for the selected Supplier");

            var participatingSuppliers = _db.ParticipatingSupplierItems
                .Where(x => x.ParticipatingSupplier.SupplierName == proformPassFail.SupplierName)
                .FirstOrDefault(x => x.ItmCode == proformPassFail.ItmCode);

            if (participatingSuppliers != null && participatingSuppliers.IsOutOfStock)
                ModelState.AddModelError("outofstock", $"the item {proformPassFail.ItmCode} is out of stock for supplier {proformPassFail.SupplierName}");


            #endregion



            //var proformaItem = new ProformaItem();

            //if (!string.IsNullOrEmpty(proformPassFail.ItmCode))
            //{
            //    string itemcode = proformPassFail.ItmCode.Split('_')[0];
            //    string name = proformPassFail.ItmCode.Split('_')[1];

            //    proformPassFail.ItmCode = itemcode;
            //    proformPassFail.Name = name;

            //    if (!String.IsNullOrEmpty(name))
            //    {
            //        var item = _db.ProformaItems
            //            .Where(x => x.ProformaPurchaseID == proformPassFail.ProformaPurchaseID)
            //            .Where(x => x.ItmCode == itemcode)
            //            .FirstOrDefault(x => x.Name == name);
            //        if (item == null)
            //            ModelState.AddModelError("notfound", "Proforma Item not found.");
            //        proformaItem = item;
            //    }
            //    if (String.IsNullOrEmpty(name))
            //    {
            //        var item = _db.ProformaItems
            //            .Where(x => x.ProformaPurchaseID == proformPassFail.ProformaPurchaseID)
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

            if (proformPassFail.ProformaItemID == 0)
                ModelState.AddModelError("selectitem", "Please Select an Item.");

            #region get proforma item info

            var participatingSupplier = _db.ParticipatingSupplierItems
                .Where(x => x.IsOutOfStock == false)
                .Where(x => x.ParticipatingSupplier.SupplierName == proformPassFail.SupplierName)
                .FirstOrDefault(x => x.ProformaItemID == proformPassFail.ProformaItemID);

            if (participatingSupplier == null)
                ModelState.AddModelError("didnotparticipate", $"The supplier {proformPassFail.SupplierName} did not participate or is out of stock for this item.");

            if (participatingSupplier != null)
            {
                proformPassFail.CatCode = participatingSupplier.CatCode;
                proformPassFail.CategoryName = participatingSupplier.CategoryName;
                proformPassFail.ItmCode = participatingSupplier.ItmCode;
                proformPassFail.ItemName = participatingSupplier.ItemName;
                proformPassFail.Name = participatingSupplier.Name;
                proformPassFail.ModelNo = participatingSupplier.ModelNo;
                proformPassFail.ItemSpecification = participatingSupplier.ItemSpecification;
            }

            #endregion

            if (ModelState.IsValid)
            {
                if (participatingSupplier != null)
                {
                    var entity = new ProformPassFail
                    {
                        ProformaPurchaseID = proformPassFail.ProformaPurchaseID,
                        ProformPassFailId = proformPassFail.ProformPassFailId,
                        SupplierId = proformPassFail.SupplierId,
                        IsNewSupplier = proformPassFail.IsNewSupplier,
                        SupplierName = proformPassFail.SupplierName,
                        ProformaItemID = proformPassFail.ProformaItemID,
                        CatCode = participatingSupplier.CatCode,
                        CategoryName = participatingSupplier.CategoryName,
                        ItmCode = participatingSupplier.ItmCode,
                        ItemName = participatingSupplier.ItemName,
                        ItemSpecification = participatingSupplier.ItemSpecification,
                        ModelNo = participatingSupplier.ModelNo,
                        Name = participatingSupplier.Name,
                        Result = proformPassFail.Result,
                        Remark = proformPassFail.Remark,
                        DateCreated = proformPassFail.DateCreated,
                        LastModified = proformPassFail.LastModified,
                        CreatedBy = proformPassFail.CreatedBy,
                        ModifiedBy = proformPassFail.ModifiedBy
                    };

                    _db.ProformPassFails.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "proforma pass fail", "Success", User.Identity.Name, $"updated pass fail for {entity.SupplierName} under record {entity.ProformaPurchaseID}", Modules.Procurement);
                    #endregion
                }
               
            }

            return Json(new[] { proformPassFail }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformPassFails_Destroy([DataSourceRequest]DataSourceRequest request, ProformPassFail proformPassFail)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProformPassFail
                {
                    ProformaPurchaseID = proformPassFail.ProformaPurchaseID,
                    ProformPassFailId = proformPassFail.ProformPassFailId,
                    SupplierId = proformPassFail.SupplierId,
                    IsNewSupplier = proformPassFail.IsNewSupplier,
                    SupplierName = proformPassFail.SupplierName,
                    ProformaItemID = proformPassFail.ProformaItemID,
                    ItmCode = proformPassFail.ItmCode,
                    Result = proformPassFail.Result,
                    Remark = proformPassFail.Remark,
                    DateCreated = proformPassFail.DateCreated,
                    LastModified = proformPassFail.LastModified,
                    CreatedBy = proformPassFail.CreatedBy,
                    ModifiedBy = proformPassFail.ModifiedBy
                };

                _db.ProformPassFails.Attach(entity);
                _db.ProformPassFails.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { proformPassFail }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
