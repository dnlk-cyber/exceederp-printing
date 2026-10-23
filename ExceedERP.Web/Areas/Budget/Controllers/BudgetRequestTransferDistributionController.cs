using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Budget;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;

namespace ExceedERP.Web.Areas.Budget.Controllers
{
    public class BudgetRequestTransferDistributionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly AccountHelper _accountHelper = new AccountHelper();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult BudgetRequestTransferDistributions_Read([DataSourceRequest]DataSourceRequest request, string Id)
        {
            List<BudgetRequestTransferDistribution> BudgetRequestTransferDistributions = db.BudgetRequestTransferDistributions.Where(x => x.BudgetRequestId.ToString() == Id).ToList();
            DataSourceResult result = BudgetRequestTransferDistributions.Select(s => new BudgetRequestTransferDistribution()
            {
                BudgetRequestTransferDistributionId = s.BudgetRequestTransferDistributionId,
                BudgetRequestId = s.BudgetRequestId,
                AccountCode = s.AccountCode,
                AccountDesc = s.AccountDesc,
                Debit = s.Debit,
                Credit = s.Credit,
                DateCreated = s.DateCreated,
                LastModified = s.LastModified,
                CreatedBy = s.CreatedBy,
                ModifiedBy = s.ModifiedBy


            }).ToDataSourceResult(request);


            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetRequestTransferDistributions_Create([DataSourceRequest]DataSourceRequest request, BudgetRequestTransferDistribution BudgetRequestTransferDistribution, int Id)
        {
            if(string.IsNullOrWhiteSpace(BudgetRequestTransferDistribution.AccountCode))
            {
                ModelState.Clear();
                ModelState.AddModelError("AccountCodeRequired", " AccountCode Required");
                return Json(new[] { BudgetRequestTransferDistribution }.ToDataSourceResult(request, ModelState));
            }
            if (ModelState.IsValid)
            {
                var RequestLines = db.BudgetRequestLines.Where(x => x.BudgetRequestId == BudgetRequestTransferDistribution.BudgetRequestId).ToList();
               var RequestLine = RequestLines.Sum(x=>x.Amount);
                AccountHelper accountHelper = new AccountHelper();
                var CreditDistribution = db.BudgetRequestTransferDistributions.ToList();
                var DebitDistribution = db.BudgetRequestTransferDistributions.ToList();
                var CreditDistributions = CreditDistribution.Sum(x => x.Credit);
                var DebitDistributions = DebitDistribution.Sum(x => x.Debit);
                RequestLine = RequestLine - ( CreditDistributions + DebitDistributions);
 
                    string AccountDesc = accountHelper.GetAccountDesc(BudgetRequestTransferDistribution.AccountCode);

                    var entity = new BudgetRequestTransferDistribution
                    {
                        BudgetRequestId = Id,
                        AccountCode = BudgetRequestTransferDistribution.AccountCode,
                        AccountDesc = AccountDesc,
                        Debit = BudgetRequestTransferDistribution.Debit,
                        Credit = BudgetRequestTransferDistribution.Credit,
                        DateCreated = DateTime.Now,
                        LastModified = BudgetRequestTransferDistribution.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = BudgetRequestTransferDistribution.ModifiedBy
                    };

                    db.BudgetRequestTransferDistributions.Add(entity);
                    db.SaveChanges();
                    BudgetRequestTransferDistribution.BudgetRequestTransferDistributionId = entity.BudgetRequestTransferDistributionId;
                    BudgetRequestTransferDistribution.AccountDesc = entity.AccountDesc;
                }

           

            return Json(new[] { BudgetRequestTransferDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetRequestTransferDistributions_Update([DataSourceRequest]DataSourceRequest request, BudgetRequestTransferDistribution BudgetRequestTransferDistribution)
        {
            if (ModelState.IsValid)
            {
                var RequestLines = db.BudgetRequestLines.Where(x => x.BudgetRequestId == BudgetRequestTransferDistribution.BudgetRequestId).ToList();
                var RequestLine = RequestLines.Sum(x => x.Amount);

                AccountHelper accountHelper = new AccountHelper();

                var CreditDistributions = db.BudgetRequestTransferDistributions.Sum(x => x.Credit);
                var DebitDistributions = db.BudgetRequestTransferDistributions.Sum(x => x.Debit);
                RequestLine = RequestLine - (CreditDistributions + DebitDistributions);

               
                  
                        var entity = new BudgetRequestTransferDistribution
                        {
                            BudgetRequestTransferDistributionId = BudgetRequestTransferDistribution.BudgetRequestTransferDistributionId,
                            BudgetRequestId = BudgetRequestTransferDistribution.BudgetRequestId,
                            AccountCode = BudgetRequestTransferDistribution.AccountCode,
                            AccountDesc = _accountHelper.GetAccountDesc(BudgetRequestTransferDistribution.AccountCode),
                            Debit = BudgetRequestTransferDistribution.Debit,
                            Credit = BudgetRequestTransferDistribution.Credit,

                            DateCreated = BudgetRequestTransferDistribution.DateCreated,
                            LastModified = DateTime.Now,
                            CreatedBy = BudgetRequestTransferDistribution.CreatedBy,
                            ModifiedBy = User.Identity.Name
                        };

                        db.BudgetRequestTransferDistributions.Attach(entity);
                        db.Entry(entity).State = EntityState.Modified;
                        db.SaveChanges();
                    }
             
          
            return Json(new[] { BudgetRequestTransferDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetRequestTransferDistributions_Destroy([DataSourceRequest]DataSourceRequest request, BudgetRequestTransferDistribution BudgetRequestTransferDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetRequestTransferDistribution
                {
                    BudgetRequestTransferDistributionId = BudgetRequestTransferDistribution.BudgetRequestTransferDistributionId,
                    BudgetRequestId = BudgetRequestTransferDistribution.BudgetRequestId,
                    AccountCode = BudgetRequestTransferDistribution.AccountCode,
                    AccountDesc = BudgetRequestTransferDistribution.AccountDesc,
                    Debit = BudgetRequestTransferDistribution.Debit,
                    Credit = BudgetRequestTransferDistribution.Credit,

                    DateCreated = BudgetRequestTransferDistribution.DateCreated,
                    LastModified = BudgetRequestTransferDistribution.LastModified,
                    CreatedBy = BudgetRequestTransferDistribution.CreatedBy,
                    ModifiedBy = BudgetRequestTransferDistribution.ModifiedBy
                };

                db.BudgetRequestTransferDistributions.Attach(entity);
                db.BudgetRequestTransferDistributions.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { BudgetRequestTransferDistribution }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
