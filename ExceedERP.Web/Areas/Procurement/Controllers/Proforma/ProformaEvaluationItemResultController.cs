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
       
    public class ProformaEvaluationItemResultController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult ProformaEvaluationItemResults_Read([DataSourceRequest]DataSourceRequest request , int id)
        {
            IQueryable<ProformaEvaluationItemResult> proformaEvaluationItemResults = _db.ProformaEvaluationItemResults.Where(x=>x.ProformaEvaluationResultID==id).AsQueryable();
            DataSourceResult result = proformaEvaluationItemResults.ToDataSourceResult(request, proformaEvaluationItemResult => new {
                ProformaEvaluationItemResultId = proformaEvaluationItemResult.ProformaEvaluationItemResultId,
                ProformaEvaluationResultID = proformaEvaluationItemResult.ProformaEvaluationResultID,
                ProformaItemID = proformaEvaluationItemResult.ProformaItemID,
                CatCode = proformaEvaluationItemResult.CatCode,
                CategoryName = proformaEvaluationItemResult.CategoryName,
                ItmCode = proformaEvaluationItemResult.ItmCode,
                ItemName = proformaEvaluationItemResult.ItemSpecification,
                Name = proformaEvaluationItemResult.Name,
                ModelNo = proformaEvaluationItemResult.ModelNo,
                Financial = proformaEvaluationItemResult.Financial,

                Technical = proformaEvaluationItemResult.Technical,
                Pass = proformaEvaluationItemResult.Pass,
                Total = proformaEvaluationItemResult.Total,
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaEvaluationItemResults_Create([DataSourceRequest]DataSourceRequest request, ProformaEvaluationItemResult proformaEvaluationItemResult , int id)
            {
            if (ModelState.IsValid)
               
            {
                
                var entity = new ProformaEvaluationItemResult
                {
                    ProformaEvaluationResultID = id,
                    ProformaItemID = proformaEvaluationItemResult.ProformaItemID,
                    CatCode = proformaEvaluationItemResult.CatCode,
                    CategoryName = proformaEvaluationItemResult.CategoryName,
                    ItmCode = proformaEvaluationItemResult.ItmCode,
                    ItemSpecification = proformaEvaluationItemResult.ItemSpecification,
                    Name = proformaEvaluationItemResult.Name,
                    ModelNo = proformaEvaluationItemResult.ModelNo,
                    Financial = proformaEvaluationItemResult.Financial,
                    Technical = proformaEvaluationItemResult.Technical,
                    Pass = proformaEvaluationItemResult.Pass,
                    Total = proformaEvaluationItemResult.Total,
                    DateCreated = DateTime.Now.Date,
                    LastModified = proformaEvaluationItemResult.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = proformaEvaluationItemResult.ModifiedBy
                };

                _db.ProformaEvaluationItemResults.Add(entity);
                _db.SaveChanges();
                proformaEvaluationItemResult.ProformaEvaluationItemResultId = entity.ProformaEvaluationItemResultId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "proforma evaluation result item", "Success", User.Identity.Name, $"created evaluation result item {entity.ItemSpecification} under record {entity.ProformaEvaluationResultID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { proformaEvaluationItemResult }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaEvaluationItemResults_Update([DataSourceRequest]DataSourceRequest request, ProformaEvaluationItemResult proformaEvaluationItemResult)
    {
            if (ModelState.IsValid)
            {
                var entity = new ProformaEvaluationItemResult
                {
                    ProformaEvaluationResultID = proformaEvaluationItemResult.ProformaEvaluationResultID,
                    ProformaItemID = proformaEvaluationItemResult.ProformaItemID,
                    CatCode = proformaEvaluationItemResult.CatCode,
                    CategoryName = proformaEvaluationItemResult.CategoryName,
                    ItmCode = proformaEvaluationItemResult.ItmCode,
                    ItemSpecification = proformaEvaluationItemResult.ItemSpecification,
                    Name = proformaEvaluationItemResult.Name,
                    ModelNo = proformaEvaluationItemResult.ModelNo,
                    Financial = proformaEvaluationItemResult.Financial,
                    Technical = proformaEvaluationItemResult.Technical,
                    Pass = proformaEvaluationItemResult.Pass,
                    Total = proformaEvaluationItemResult.Total,
                    DateCreated = DateTime.Now.Date,
                    LastModified = proformaEvaluationItemResult.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = proformaEvaluationItemResult.ModifiedBy
                };
                _db.ProformaEvaluationItemResults.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "proforma evaluation result item", "Success", User.Identity.Name, $"updated evaluation result item {entity.ItemSpecification} under record {entity.ProformaEvaluationResultID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { proformaEvaluationItemResult }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaEvaluationItemResults_Destroy([DataSourceRequest]DataSourceRequest request, ProformaEvaluationItemResult proformaEvaluationItemResult)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProformaEvaluationItemResult
                {
                    ProformaEvaluationItemResultId = proformaEvaluationItemResult.ProformaEvaluationItemResultId,
                    ProformaItemID = proformaEvaluationItemResult.ProformaItemID,
                    ProformaEvaluationResultID = proformaEvaluationItemResult.ProformaEvaluationResultID,
                    CatCode = proformaEvaluationItemResult.CatCode,
                    CategoryName = proformaEvaluationItemResult.CategoryName,
                    ItmCode = proformaEvaluationItemResult.ItmCode,
                    ItemSpecification = proformaEvaluationItemResult.ItemSpecification,
                    Name = proformaEvaluationItemResult.Name,
                    Financial = proformaEvaluationItemResult.Financial,
                    Technical = proformaEvaluationItemResult.Technical,
                    Pass = proformaEvaluationItemResult.Pass,
                    Total = proformaEvaluationItemResult.Total,
                    DateCreated = proformaEvaluationItemResult.DateCreated,
                    LastModified = proformaEvaluationItemResult.LastModified,
                    CreatedBy = proformaEvaluationItemResult.CreatedBy,
                    ModifiedBy = proformaEvaluationItemResult.ModifiedBy
                };

                _db.ProformaEvaluationItemResults.Attach(entity);
                _db.ProformaEvaluationItemResults.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { proformaEvaluationItemResult }.ToDataSourceResult(request, ModelState));
        }
       
        [HttpPost]
       

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
