using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Bid
{
       
    public class TenderEvaluationItemResultController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult TenderEvaluationItemResults_Read([DataSourceRequest]DataSourceRequest request , int id)
        {
            IQueryable<TenderEvaluationItemResult> tenderEvaluationItemResults = _db.TenderEvaluationItemResults.Where(x=>x.TenderEvaluationResultID==id).AsQueryable();
            DataSourceResult result = tenderEvaluationItemResults.ToDataSourceResult(request, tenderEvaluationItemResult => new {
                TenderEvaluationItemResultId = tenderEvaluationItemResult.TenderEvaluationItemResultId,
                TenderEvaluationResultID = tenderEvaluationItemResult.TenderEvaluationResultID,
                LotItemID = tenderEvaluationItemResult.LotItemID,
                CatCode = tenderEvaluationItemResult.CatCode,
                CategoryName = tenderEvaluationItemResult.CategoryName,
                ItmCode = tenderEvaluationItemResult.ItmCode,
                ItemName = tenderEvaluationItemResult.ItemName,
                ItemSpecification = tenderEvaluationItemResult.ItemSpecification,
                Name = tenderEvaluationItemResult.Name,
                ModelNo = tenderEvaluationItemResult.ModelNo,
                Financial = tenderEvaluationItemResult.Financial,

                Technical = tenderEvaluationItemResult.Technical,
                Pass = tenderEvaluationItemResult.Pass,
                Total = tenderEvaluationItemResult.Total,
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderEvaluationItemResults_Create([DataSourceRequest]DataSourceRequest request, TenderEvaluationItemResult tenderEvaluationItemResult , int id)
            {
            if (ModelState.IsValid)
               
            {
                
                var entity = new TenderEvaluationItemResult
                {
                    TenderEvaluationResultID = id,
                    LotItemID = tenderEvaluationItemResult.LotItemID,
                    CatCode = tenderEvaluationItemResult.CatCode,
                    CategoryName = tenderEvaluationItemResult.CategoryName,
                    ItmCode = tenderEvaluationItemResult.ItmCode,
                    ItemName = tenderEvaluationItemResult.ItemName,
                    ItemSpecification = tenderEvaluationItemResult.ItemSpecification,
                    Name = tenderEvaluationItemResult.Name,
                    ModelNo = tenderEvaluationItemResult.ModelNo,
                    Financial = tenderEvaluationItemResult.Financial,
                    Technical = tenderEvaluationItemResult.Technical,
                    Pass = tenderEvaluationItemResult.Pass,
                    Total = tenderEvaluationItemResult.Total,
                    DateCreated = DateTime.Now.Date,
                    LastModified = tenderEvaluationItemResult.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = tenderEvaluationItemResult.ModifiedBy
                };

                _db.TenderEvaluationItemResults.Add(entity);
                _db.SaveChanges();
                tenderEvaluationItemResult.TenderEvaluationItemResultId = entity.TenderEvaluationItemResultId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "lot evaluation result item", "Success", User.Identity.Name, $"created result for {entity.ItemName} under record {entity.TenderEvaluationResultID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { tenderEvaluationItemResult }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderEvaluationItemResults_Update([DataSourceRequest]DataSourceRequest request, TenderEvaluationItemResult tenderEvaluationItemResult)
    {
            if (ModelState.IsValid)
            {
                var entity = new TenderEvaluationItemResult
                {
                    TenderEvaluationResultID = tenderEvaluationItemResult.TenderEvaluationResultID,
                    LotItemID = tenderEvaluationItemResult.LotItemID,
                    CatCode = tenderEvaluationItemResult.CatCode,
                    CategoryName = tenderEvaluationItemResult.CategoryName,
                    ItmCode = tenderEvaluationItemResult.ItmCode,
                    ItemName = tenderEvaluationItemResult.ItemName,
                    ItemSpecification = tenderEvaluationItemResult.ItemSpecification,
                    Name = tenderEvaluationItemResult.Name,
                    ModelNo = tenderEvaluationItemResult.ModelNo,
                    Financial = tenderEvaluationItemResult.Financial,
                    Technical = tenderEvaluationItemResult.Technical,
                    Pass = tenderEvaluationItemResult.Pass,
                    Total = tenderEvaluationItemResult.Total,
                    DateCreated = DateTime.Now.Date,
                    LastModified = tenderEvaluationItemResult.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = tenderEvaluationItemResult.ModifiedBy
                };
                _db.TenderEvaluationItemResults.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "lot evaluation result item", "Success", User.Identity.Name, $"updated result for {entity.ItemName} under record {entity.TenderEvaluationResultID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { tenderEvaluationItemResult }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderEvaluationItemResults_Destroy([DataSourceRequest]DataSourceRequest request, TenderEvaluationItemResult tenderEvaluationItemResult)
        {
            if (ModelState.IsValid)
            {
                var entity = new TenderEvaluationItemResult
                {
                    TenderEvaluationItemResultId = tenderEvaluationItemResult.TenderEvaluationItemResultId,
                    LotItemID = tenderEvaluationItemResult.LotItemID,
                    TenderEvaluationResultID = tenderEvaluationItemResult.TenderEvaluationResultID,
                    CatCode = tenderEvaluationItemResult.CatCode,
                    CategoryName = tenderEvaluationItemResult.CategoryName,
                    ItmCode = tenderEvaluationItemResult.ItmCode,
                    ItemName = tenderEvaluationItemResult.ItemName,
                    Name = tenderEvaluationItemResult.Name,
                    Financial = tenderEvaluationItemResult.Financial,
                    Technical = tenderEvaluationItemResult.Technical,
                    Pass = tenderEvaluationItemResult.Pass,
                    Total = tenderEvaluationItemResult.Total,
                    DateCreated = tenderEvaluationItemResult.DateCreated,
                    LastModified = tenderEvaluationItemResult.LastModified,
                    CreatedBy = tenderEvaluationItemResult.CreatedBy,
                    ModifiedBy = tenderEvaluationItemResult.ModifiedBy
                };

                _db.TenderEvaluationItemResults.Attach(entity);
                _db.TenderEvaluationItemResults.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { tenderEvaluationItemResult }.ToDataSourceResult(request, ModelState));
        }
       
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
        [AuthorizeRoles(ProcurementRoles.ProcurementAdmin, ProcurementRoles.ProcurementTenderApprover, ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementBondUser)]
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
