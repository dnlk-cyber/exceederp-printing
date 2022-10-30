using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    public class QuotationTermsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult QuotationTerm_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<QuotationTerm> quotationterm = db.QuotationTerms;
            DataSourceResult result = quotationterm.ToDataSourceResult(request, quotationTerm => new {
                QuotationTermId = quotationTerm.QuotationTermId,
                Desription = quotationTerm.Desription,
                Remark = quotationTerm.Remark,
                QuotationInDays = quotationTerm.QuotationInDays,
                OrgBranchName = quotationTerm.OrgBranchName,
                DateCreated = quotationTerm.DateCreated,
                LastModified = quotationTerm.LastModified,
                CreatedBy = quotationTerm.CreatedBy,
                ModifiedBy = quotationTerm.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QuotationTerm_Create([DataSourceRequest]DataSourceRequest request, QuotationTerm quotationTerm)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(quotationTerm.Desription))
                {
                    quotationTerm.Desription = quotationTerm.QuotationInDays + " days";
                }
                var entity = new QuotationTerm
                {
                    Desription = quotationTerm.Desription,
                    Remark = quotationTerm.Remark,
                    QuotationInDays = quotationTerm.QuotationInDays,

                    OrgBranchName = quotationTerm.OrgBranchName,
                    DateCreated = quotationTerm.DateCreated,
                    LastModified = quotationTerm.LastModified,
                    CreatedBy = quotationTerm.CreatedBy,
                    ModifiedBy = quotationTerm.ModifiedBy
                };

                db.QuotationTerms.Add(entity);
                db.SaveChanges();
                quotationTerm.QuotationTermId = entity.QuotationTermId;
            }

            return Json(new[] { quotationTerm }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QuotationTerm_Update([DataSourceRequest]DataSourceRequest request, QuotationTerm quotationTerm)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(quotationTerm.Desription))
                {
                    quotationTerm.Desription = quotationTerm.QuotationInDays + " days";
                }
                var entity = new QuotationTerm
                {
                    QuotationTermId = quotationTerm.QuotationTermId,
                    Desription = quotationTerm.Desription,
                    Remark = quotationTerm.Remark,
                    QuotationInDays = quotationTerm.QuotationInDays,
                    OrgBranchName = quotationTerm.OrgBranchName,
                    DateCreated = quotationTerm.DateCreated,
                    LastModified = quotationTerm.LastModified,
                    CreatedBy = quotationTerm.CreatedBy,
                    ModifiedBy = quotationTerm.ModifiedBy
                };

                db.QuotationTerms.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { quotationTerm }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QuotationTerm_Destroy([DataSourceRequest]DataSourceRequest request, QuotationTerm quotationTerm)
        {
            if (ModelState.IsValid)
            {
                var entity = new QuotationTerm
                {
                    QuotationTermId = quotationTerm.QuotationTermId,
                    Desription = quotationTerm.Desription,
                    Remark = quotationTerm.Remark,
                    QuotationInDays = quotationTerm.QuotationInDays,
                    OrgBranchName = quotationTerm.OrgBranchName,
                    DateCreated = quotationTerm.DateCreated,
                    LastModified = quotationTerm.LastModified,
                    CreatedBy = quotationTerm.CreatedBy,
                    ModifiedBy = quotationTerm.ModifiedBy
                };

                db.QuotationTerms.Attach(entity);
                db.QuotationTerms.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { quotationTerm }.ToDataSourceResult(request, ModelState));
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
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
