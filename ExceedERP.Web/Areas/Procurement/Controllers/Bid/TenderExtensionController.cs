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

    public class TenderExtensionController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult TenderExtension_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<TenderExtension> tenderExtensions = _db.TenderExtensions.Where(t => t.TenderID == id);
            DataSourceResult result = tenderExtensions.ToDataSourceResult(request, tenderExtension => new
            {
                TenderExtensionId = tenderExtension.TenderExtensionId,
                TenderID = tenderExtension.TenderID,
                ClosingDate = tenderExtension.ClosingDate,
                OpenigDate = tenderExtension.OpenigDate,
                TechnicalOpenigDate = tenderExtension.TechnicalOpenigDate,
                FinancialOpenigDate = tenderExtension.FinancialOpenigDate,
                Remark = tenderExtension.Remark
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderExtension_Create([DataSourceRequest]DataSourceRequest request, TenderExtension tenderExtension, int id)
        {
            #region validation
           

            //if (tenderExtension.ClosingDate >= tenderExtension.OpenigDate)
            //    ModelState.AddModelError("float", "Tender closing date should be before opening date");

            //if (tenderExtension.TechnicalOpenigDate != null && tenderExtension.TechnicalOpenigDate <= tenderExtension.OpenigDate)
            //    ModelState.AddModelError("tech", "Technical Opening Date should be after opening date");

            //if (tenderExtension.FinancialOpenigDate != null && tenderExtension.FinancialOpenigDate < tenderExtension.OpenigDate)
            //    ModelState.AddModelError("tech", "Financial Opening Date should be after opening date");

            #endregion
            if (ModelState.IsValid)
            {
                    var entity = new TenderExtension
                    {
                        TenderID = id,
                        ClosingDate = tenderExtension.ClosingDate,
                        OpenigDate = tenderExtension.OpenigDate,
                        TechnicalOpenigDate = tenderExtension.TechnicalOpenigDate,
                        FinancialOpenigDate = tenderExtension.FinancialOpenigDate,
                        Remark = tenderExtension.Remark
                    };

                    _db.TenderExtensions.Add(entity);
                    _db.SaveChanges();
                    tenderExtension.TenderExtensionId = entity.TenderExtensionId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "tender extension", "Success", User.Identity.Name, $"created extention {entity.TenderExtensionId} under record {entity.TenderID} ", Modules.Procurement);
                #endregion

            }

            return Json(new[] { tenderExtension }.ToDataSourceResult(request, ModelState));
        }

       

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderExtension_Update([DataSourceRequest]DataSourceRequest request, TenderExtension tenderExtension)
        {
            #region validation
           

            if (tenderExtension.ClosingDate >= tenderExtension.OpenigDate)
                ModelState.AddModelError("float", "Tender closing date should be before opening date");

            if (tenderExtension.TechnicalOpenigDate != null && tenderExtension.TechnicalOpenigDate <= tenderExtension.OpenigDate)
                ModelState.AddModelError("tech", "Technical Opening Date should be after opening date");

            if (tenderExtension.FinancialOpenigDate != null && tenderExtension.FinancialOpenigDate < tenderExtension.OpenigDate)
                ModelState.AddModelError("tech", "Financial Opening Date should be after opening date");

            #endregion
            if (ModelState.IsValid)
            {
                    var entity = new TenderExtension
                    {
                        TenderExtensionId = tenderExtension.TenderExtensionId,
                        TenderID = tenderExtension.TenderID,
                        ClosingDate = tenderExtension.ClosingDate,
                        OpenigDate = tenderExtension.OpenigDate,
                        TechnicalOpenigDate = tenderExtension.TechnicalOpenigDate,
                        FinancialOpenigDate = tenderExtension.FinancialOpenigDate,
                        Remark = tenderExtension.Remark
                    };

                    _db.TenderExtensions.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "tender extension", "Success", User.Identity.Name, $"updated extention {entity.TenderExtensionId} under record {entity.TenderID} ", Modules.Procurement);
                #endregion

            }

            return Json(new[] { tenderExtension }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderExtension_Destroy([DataSourceRequest]DataSourceRequest request, TenderExtension tenderExtension)
        {
         
            if (ModelState.IsValid)
            {
                var entity = new TenderExtension
                {
                    TenderExtensionId = tenderExtension.TenderExtensionId,
                    TenderID = tenderExtension.TenderID,
                    ClosingDate = tenderExtension.ClosingDate,
                    OpenigDate = tenderExtension.OpenigDate,
                    TechnicalOpenigDate = tenderExtension.TechnicalOpenigDate,
                    FinancialOpenigDate = tenderExtension.FinancialOpenigDate,
                    Remark = tenderExtension.Remark
                };

                _db.TenderExtensions.Attach(entity);
                _db.TenderExtensions.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { tenderExtension }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

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
