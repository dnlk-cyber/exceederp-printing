using ExceedERP.Web.Helpers;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles( CRMRoles.CRMAdmin)] 
    public class BankReferenceController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BankReferences_Read([DataSourceRequest]DataSourceRequest request, int id, string bankReferenceType)
        {
            IQueryable<BankReference> bankreferences = db.BankReferences.Where(br => br.ReferenceType == bankReferenceType);
            DataSourceResult result = bankreferences.ToDataSourceResult(request, bankReference => new {
                BankReferenceId = bankReference.BankReferenceId,
                BankId = bankReference.BankId,
                ReferenceType = bankReference.ReferenceType,
                Reference = bankReference.Reference,
                Type = bankReference.Type,
                AccountNumber = bankReference.AccountNumber,
                //CreatedBy = bankReference.CreatedBy,
                //DateCreated = bankReference.DateCreated,
                //LastModified = bankReference.LastModified,
                //ModifiedBy = bankReference.ModifiedBy,
                //OrgBranchName = bankReference.OrgBranchName,
                Remark = bankReference.Remark
            });
            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BankReferences_Create([DataSourceRequest]DataSourceRequest request, BankReference bankReference, int id, string bankReferenceType)
        {
            if (ModelState.IsValid)
            {
                var entity = new BankReference
                {
                    BankId = bankReference.BankId,
                    ReferenceType = bankReferenceType,
                    Reference = id.ToString(),
                    Type = bankReference.Type,
                    AccountNumber = bankReference.AccountNumber,
                   
                    Remark = bankReference.Remark
                };
                db.BankReferences.Add(entity);
                db.SaveChanges();
                //bankReference.BankReferenceId = entity.BankReferenceId;
            }

            return Json(new[] { bankReference }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BankReferences_Update([DataSourceRequest]DataSourceRequest request, BankReference bankReference)
        {
            if (ModelState.IsValid)
            {
                var entity = new BankReference
                {
                    BankReferenceId = bankReference.BankReferenceId,
                    BankId = bankReference.BankId,
                    ReferenceType = bankReference.ReferenceType,
                    Reference = bankReference.Reference,
                    Type = bankReference.Type,
                    AccountNumber = bankReference.AccountNumber,
                    Remark = bankReference.Remark
                };

                db.BankReferences.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { bankReference }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BankReferences_Destroy([DataSourceRequest]DataSourceRequest request, BankReference bankReference)
        {
            if (ModelState.IsValid)
            {
                var entity = new BankReference
                {
                    BankReferenceId = bankReference.BankReferenceId,
                    BankId = bankReference.BankId,
                    ReferenceType = bankReference.ReferenceType,
                    Reference = bankReference.Reference,
                    Type = bankReference.Type,
                    AccountNumber = bankReference.AccountNumber,
                    Remark = bankReference.Remark
                };

                db.BankReferences.Attach(entity);
                db.BankReferences.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { bankReference }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
