using System;
using ExceedERP.Web.Helpers;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles( CRMRoles.CRMAdmin)] 
    public class BankAccountDetailController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BankAccountDetails_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<BankAccountDetail> bankaccountdetails = db.BankAccountDetails.Where(ba=>ba.Reference==id.ToString()) ;
            DataSourceResult result = bankaccountdetails.ToDataSourceResult(request, bankAccountDetail => new {
                BankAccountDetailID = bankAccountDetail.BankAccountDetailID,
                Reference = bankAccountDetail.Reference,
                Description = bankAccountDetail.Description,
                Type = bankAccountDetail.BankAccountDetailType,
                Bank = bankAccountDetail.Bank,
                AccountNumber = bankAccountDetail.AccountNumber,
                CreatedBy = bankAccountDetail.CreatedBy,
                DateCreated = bankAccountDetail.DateCreated,
                LastModified = bankAccountDetail.LastModified,
                ModifiedBy = bankAccountDetail.ModifiedBy,
                OrgBranchName = bankAccountDetail.OrgBranchName,
                Remark = bankAccountDetail.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BankAccountDetails_Create([DataSourceRequest]DataSourceRequest request, BankAccountDetail bankAccountDetail, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new BankAccountDetail
                {
                    Reference = id.ToString(),
                    Description = bankAccountDetail.Description,
                    BankAccountDetailType = bankAccountDetail.BankAccountDetailType,
                    Bank = bankAccountDetail.Bank,
                    AccountNumber = bankAccountDetail.AccountNumber,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Remark = bankAccountDetail.Remark
                };

                db.BankAccountDetails.Add(entity);
                db.SaveChanges();
                bankAccountDetail.BankAccountDetailID = entity.BankAccountDetailID;
            }

            return Json(new[] { bankAccountDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BankAccountDetails_Update([DataSourceRequest]DataSourceRequest request, BankAccountDetail bankAccountDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new BankAccountDetail
                {
                    BankAccountDetailID = bankAccountDetail.BankAccountDetailID,
                    Reference = bankAccountDetail.Reference,
                    Description = bankAccountDetail.Description,
                    BankAccountDetailType = bankAccountDetail.BankAccountDetailType,
                    Bank = bankAccountDetail.Bank,
                    AccountNumber = bankAccountDetail.AccountNumber,
                    Remark = bankAccountDetail.Remark
                };

                db.BankAccountDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { bankAccountDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BankAccountDetails_Destroy([DataSourceRequest]DataSourceRequest request, BankAccountDetail bankAccountDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new BankAccountDetail
                {
                    BankAccountDetailID = bankAccountDetail.BankAccountDetailID,
                    Reference = bankAccountDetail.Reference,
                    Description = bankAccountDetail.Description,
                    BankAccountDetailType = bankAccountDetail.BankAccountDetailType,
                    Bank = bankAccountDetail.Bank,
                    AccountNumber = bankAccountDetail.AccountNumber,
                    Remark = bankAccountDetail.Remark
                };

                db.BankAccountDetails.Attach(entity);
                db.BankAccountDetails.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { bankAccountDetail }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
