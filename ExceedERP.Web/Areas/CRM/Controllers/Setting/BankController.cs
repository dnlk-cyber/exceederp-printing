using ExceedERP.Web.Helpers;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles( CRMRoles.CRMAdmin)] 
    public class BankController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult Organizations_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Bank> organizations = db.Banks;
            DataSourceResult result = organizations.ToDataSourceResult(request, bank => new
            {
                BankId = bank.BankId,
                Name = bank.Name,
                Category = bank.Category,
                IsActive = bank.IsActive,
                ParentId = bank.ParentId,
                //CreatedBy = bank.CreatedBy,
                //DateCreated = bank.DateCreated,
                //LastModified = bank.LastModified,
                //ModifiedBy = bank.ModifiedBy,
                //OrgBranchName = bank.OrgBranchName,
                Remark = bank.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Organizations_Create([DataSourceRequest]DataSourceRequest request, Bank bank)
        {
            if (ModelState.IsValid)
            {
                var entity = new Bank
                {
                    Name = bank.Name,
                    ParentId = bank.ParentId,
                    Remark = bank.Remark,
                    Category = bank.Category,
                    //CreatedBy = User.Identity.Name,
                    //DateCreated = DateTime.Now,
                    IsActive = bank.IsActive
                };

                db.Banks.Add(entity);
                db.SaveChanges();
                bank.BankId = entity.BankId;
            }

            return Json(new[] { bank }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Organizations_Update([DataSourceRequest]DataSourceRequest request, Bank bank)
        {
            if (ModelState.IsValid)
            {
                var entity = new Bank
                {
                    BankId = bank.BankId,
                    Name = bank.Name,
                    Remark = bank.Remark,
                    ParentId = bank.ParentId,
                    Category = bank.Category,
                    IsActive = bank.IsActive
                };

                db.Banks.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { bank }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Organizations_Destroy([DataSourceRequest]DataSourceRequest request, Bank bank)
        {
            if (ModelState.IsValid)
            {
                var entity = new Bank
                {
                    BankId = bank.BankId,
                    Name = bank.Name,
                    Category = bank.Category,
                    IsActive = bank.IsActive,
                    ParentId = bank.ParentId,
                    Remark = bank.Remark
                };

                db.Banks.Attach(entity);
                db.Banks.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { bank }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
