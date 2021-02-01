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
       
    public class TenderEvaluationResultController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult TenderEvaluationResults_Read([DataSourceRequest]DataSourceRequest request , int id)
        {
            IQueryable<TenderEvaluationResult> tenderevaluationresults = _db.TenderEvaluationResults.Where(x=>x.LotID==id).AsQueryable();
            DataSourceResult result = tenderevaluationresults.ToDataSourceResult(request, tenderEvaluationResult => new {
                TenderEvaluationResultID = tenderEvaluationResult.TenderEvaluationResultID,
                LotID = tenderEvaluationResult.LotID,
                SupplierId = tenderEvaluationResult.SupplierId,
                IsNewSupplier = tenderEvaluationResult.IsNewSupplier,
                SupplierName = tenderEvaluationResult.SupplierName,
                Financial = tenderEvaluationResult.Financial,
                Technical = tenderEvaluationResult.Technical,
                Pass = tenderEvaluationResult.Pass,
                Total = tenderEvaluationResult.Total,
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderEvaluationResults_Create([DataSourceRequest]DataSourceRequest request, TenderEvaluationResult tenderEvaluationResult , int id)
            {
            if (ModelState.IsValid)
               
            {
                
                var entity = new TenderEvaluationResult
                {
                    LotID = id,
                    SupplierId = tenderEvaluationResult.SupplierId,
                    IsNewSupplier = tenderEvaluationResult.IsNewSupplier,
                    SupplierName = tenderEvaluationResult.SupplierName,
                    Financial = tenderEvaluationResult.Financial,
                    Technical = tenderEvaluationResult.Technical,
                    Pass = tenderEvaluationResult.Pass,
                    Total = tenderEvaluationResult.Total,
                    DateCreated = DateTime.Now.Date,
                    LastModified = tenderEvaluationResult.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = tenderEvaluationResult.ModifiedBy
                };

                _db.TenderEvaluationResults.Add(entity);
                _db.SaveChanges();
                tenderEvaluationResult.TenderEvaluationResultID = entity.TenderEvaluationResultID;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "lot evaluation result", "Success", User.Identity.Name, $"created result for supplier {entity.SupplierName} under record {entity.LotID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { tenderEvaluationResult }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderEvaluationResults_Update([DataSourceRequest]DataSourceRequest request, TenderEvaluationResult tenderEvaluationResult)
    {
            if (ModelState.IsValid)
            {
                var entity = new TenderEvaluationResult
                {
                    LotID = tenderEvaluationResult.LotID,
                    SupplierId = tenderEvaluationResult.SupplierId,
                    IsNewSupplier = tenderEvaluationResult.IsNewSupplier,
                    SupplierName = tenderEvaluationResult.SupplierName,
                    Financial = tenderEvaluationResult.Financial,
                    Technical = tenderEvaluationResult.Technical,
                    Pass = tenderEvaluationResult.Pass,
                    Total = tenderEvaluationResult.Total,
                    DateCreated = DateTime.Now.Date,
                    LastModified = tenderEvaluationResult.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = tenderEvaluationResult.ModifiedBy
                };
                _db.TenderEvaluationResults.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "lot evaluation result", "Success", User.Identity.Name, $"updated result for supplier {entity.SupplierName} under record {entity.LotID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { tenderEvaluationResult }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderEvaluationResults_Destroy([DataSourceRequest]DataSourceRequest request, TenderEvaluationResult tenderEvaluationResult)
        {
            if (ModelState.IsValid)
            {
                var entity = new TenderEvaluationResult
                {
                    TenderEvaluationResultID = tenderEvaluationResult.TenderEvaluationResultID,
                    LotID = tenderEvaluationResult.LotID,
                    SupplierId = tenderEvaluationResult.SupplierId,
                    IsNewSupplier = tenderEvaluationResult.IsNewSupplier,
                    SupplierName = tenderEvaluationResult.SupplierName,
                    Financial = tenderEvaluationResult.Financial,
                    Technical = tenderEvaluationResult.Technical,
                    Pass = tenderEvaluationResult.Pass,
                    Total = tenderEvaluationResult.Total,
                    DateCreated = tenderEvaluationResult.DateCreated,
                    LastModified = tenderEvaluationResult.LastModified,
                    CreatedBy = tenderEvaluationResult.CreatedBy,
                    ModifiedBy = tenderEvaluationResult.ModifiedBy
                };

                _db.TenderEvaluationResults.Attach(entity);
                _db.TenderEvaluationResults.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { tenderEvaluationResult }.ToDataSourceResult(request, ModelState));
        }
       
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
        [AuthorizeRoles(ProcurementConstants.ProcurementAdmin, ProcurementConstants.ProcurementTenderApprover, ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementBondUser)]
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
