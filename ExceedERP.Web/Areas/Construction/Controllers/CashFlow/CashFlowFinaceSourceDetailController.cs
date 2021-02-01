using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.CashFlow
{
    public class CashFlowFinaceSourceDetailController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();


        public ActionResult FinancialSources_Read([DataSourceRequest]DataSourceRequest request, int srcId)
        {
            IQueryable<CashFlowFinanceSourceDetail> financeSources = db.CashFlowFinanceSourceDetail.Where(fs => fs.CashFlowFinanceSourceId  == srcId);
            DataSourceResult result = financeSources.ToDataSourceResult(request, financeSource => new {
                CashFlowFinanceSourceDetailId = financeSource.CashFlowFinanceSourceDetailId,
                CashFlowFinanceSourceId = financeSource.CashFlowFinanceSourceId,
                ConstructionPeriodId = financeSource.ConstructionPeriodId,
                MainContractAdvancePayment = financeSource.MainContractAdvancePayment,
                Revenue = financeSource.Revenue,
                SuppliersCredit = financeSource.SuppliersCredit,
                SubContractorFinancing = financeSource.SubContractorFinancing,
                SubContractorRetentionWithholding = financeSource.SubContractorRetentionWithholding,
                SubContractorAdvanceRepayment = financeSource.SubContractorAdvanceRepayment,
                HeadOfficeFinancing = financeSource.HeadOfficeFinancing,
                MainContractRetentionReleased = financeSource.MainContractRetentionReleased,
                MaterialOnSite = financeSource.MaterialOnSite,
                Loan = financeSource.Loan,
                TotalFinanceSource = financeSource.TotalFinanceSource,

                OrgBranchName = financeSource.OrgBranchName,
                DateCreated = financeSource.DateCreated,
                LastModified = financeSource.LastModified,
                CreatedBy = financeSource.CreatedBy,
                ModifiedBy = financeSource.ModifiedBy
            });

            return Json(result);
        }
        TimeSpan sp = DateTime.Now - DateTime.Now;

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinancialSource_Create([DataSourceRequest]DataSourceRequest request, CashFlowFinanceSourceDetail financeSource, int srcId)
        {
            if (ModelState.IsValid)
            {

                var entity = new CashFlowFinanceSourceDetail
                {
                    //CashFlowFinanceSourceId = financeSource.CashFlowFinanceSourceId,
                    CashFlowFinanceSourceDetailId = financeSource.CashFlowFinanceSourceDetailId,
                    CashFlowFinanceSourceId = srcId,
                    ConstructionPeriodId = financeSource.ConstructionPeriodId,
                    MainContractAdvancePayment = financeSource.MainContractAdvancePayment,
                    Revenue = financeSource.Revenue,
                    SuppliersCredit = financeSource.SuppliersCredit,
                    SubContractorFinancing = financeSource.SubContractorFinancing,
                    SubContractorRetentionWithholding = financeSource.SubContractorRetentionWithholding,
                    SubContractorAdvanceRepayment = financeSource.SubContractorAdvanceRepayment,
                    HeadOfficeFinancing = financeSource.HeadOfficeFinancing,
                    MainContractRetentionReleased = financeSource.MainContractRetentionReleased,
                    MaterialOnSite = financeSource.MaterialOnSite,
                    Loan = financeSource.Loan,
                    TotalFinanceSource = financeSource.TotalFinanceSource,

                    OrgBranchName = financeSource.OrgBranchName,
                    DateCreated = financeSource.DateCreated,
                    LastModified = financeSource.LastModified,
                    CreatedBy = financeSource.CreatedBy,
                    ModifiedBy = financeSource.ModifiedBy
                };
                entity.TotalFinanceSource = entity.MainContractAdvancePayment +
                                              entity.Revenue + entity.SuppliersCredit +
                                              entity.SubContractorFinancing + entity.SubContractorRetentionWithholding +
                                              entity.SubContractorAdvanceRepayment +
                                              entity.HeadOfficeFinancing + entity.MainContractAdvancePayment +
                                              entity.MaterialOnSite;

                db.CashFlowFinanceSourceDetail.Add(entity);
                db.SaveChanges();
                financeSource.CashFlowFinanceSourceDetailId = entity.CashFlowFinanceSourceDetailId;
            }

            return Json(new[] { financeSource }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinancialSource_Update([DataSourceRequest]DataSourceRequest request, CashFlowFinanceSourceDetail financeSource)
        {
            if (ModelState.IsValid)
            {
                var entity = new CashFlowFinanceSourceDetail
                {
                    CashFlowFinanceSourceDetailId = financeSource.CashFlowFinanceSourceDetailId,
                    CashFlowFinanceSourceId = financeSource.CashFlowFinanceSourceId,
                    ConstructionPeriodId = financeSource.ConstructionPeriodId,
                    MainContractAdvancePayment = financeSource.MainContractAdvancePayment,
                    Revenue = financeSource.Revenue,
                    SuppliersCredit = financeSource.SuppliersCredit,
                    SubContractorFinancing = financeSource.SubContractorFinancing,
                    SubContractorRetentionWithholding = financeSource.SubContractorRetentionWithholding,
                    SubContractorAdvanceRepayment = financeSource.SubContractorAdvanceRepayment,
                    HeadOfficeFinancing = financeSource.HeadOfficeFinancing,
                    MainContractRetentionReleased = financeSource.MainContractRetentionReleased,
                    MaterialOnSite = financeSource.MaterialOnSite,
                    Loan = financeSource.Loan,
                    TotalFinanceSource = financeSource.TotalFinanceSource,

                    OrgBranchName = financeSource.OrgBranchName,
                    DateCreated = financeSource.DateCreated,
                    LastModified = financeSource.LastModified,
                    CreatedBy = financeSource.CreatedBy,
                    ModifiedBy = financeSource.ModifiedBy
                };
                entity.TotalFinanceSource = entity.MainContractAdvancePayment +
                                            entity.Revenue + entity.SuppliersCredit +
                                            entity.SubContractorFinancing + entity.SubContractorRetentionWithholding +
                                            entity.SubContractorAdvanceRepayment +
                                            entity.HeadOfficeFinancing + entity.MainContractAdvancePayment +
                                            entity.MaterialOnSite;

                db.CashFlowFinanceSourceDetail.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { financeSource }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinancialSource_Destroy([DataSourceRequest]DataSourceRequest request, CashFlowFinanceSourceDetail financeSource)
        {
            if (ModelState.IsValid)
            {
                var entity = new CashFlowFinanceSourceDetail
                {
                    CashFlowFinanceSourceDetailId = financeSource.CashFlowFinanceSourceDetailId,
                    CashFlowFinanceSourceId = financeSource.CashFlowFinanceSourceId,
                    ConstructionPeriodId = financeSource.ConstructionPeriodId,
                    MainContractAdvancePayment = financeSource.MainContractAdvancePayment,
                    Revenue = financeSource.Revenue,
                    SuppliersCredit = financeSource.SuppliersCredit,
                    SubContractorFinancing = financeSource.SubContractorFinancing,
                    SubContractorRetentionWithholding = financeSource.SubContractorRetentionWithholding,
                    SubContractorAdvanceRepayment = financeSource.SubContractorAdvanceRepayment,
                    HeadOfficeFinancing = financeSource.HeadOfficeFinancing,
                    MainContractRetentionReleased = financeSource.MainContractRetentionReleased,
                    MaterialOnSite = financeSource.MaterialOnSite,
                    Loan = financeSource.Loan,
                    TotalFinanceSource = financeSource.TotalFinanceSource,

                    OrgBranchName = financeSource.OrgBranchName,
                    DateCreated = financeSource.DateCreated,
                    LastModified = financeSource.LastModified,
                    CreatedBy = financeSource.CreatedBy,
                    ModifiedBy = financeSource.ModifiedBy
                };

                db.CashFlowFinanceSourceDetail.Attach(entity);
                db.CashFlowFinanceSourceDetail.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { financeSource }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}