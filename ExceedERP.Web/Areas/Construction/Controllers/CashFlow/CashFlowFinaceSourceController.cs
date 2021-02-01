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
    public class CashFlowFinaceSourceController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();


        public ActionResult FinancialSources_Read([DataSourceRequest]DataSourceRequest request, int yearId)
        {
            IQueryable<CashFlowFinanceSource> financeSources = db.CashFlowFinanceSources.Where(fs => fs.CashFlowYearId == yearId);
            DataSourceResult result = financeSources.ToDataSourceResult(request, financeSource => new
            {
                CashFlowFinanceSourceId = financeSource.CashFlowFinanceSourceId,
                CashFlowYearId = financeSource.CashFlowYearId,
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

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinancialSource_Create([DataSourceRequest]DataSourceRequest request, CashFlowFinanceSource financeSource, int yearId)
        {
            if (ModelState.IsValid)
            {

                var entity = new CashFlowFinanceSource
                {
                    //CashFlowFinanceSourceId = financeSource.CashFlowFinanceSourceId,
                    CashFlowYearId = yearId,
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

                db.CashFlowFinanceSources.Add(entity);
                db.SaveChanges();
                financeSource.CashFlowFinanceSourceId = entity.CashFlowFinanceSourceId;

                this.AddFinancialSourceDetail(entity);
            }

            return Json(new[] { financeSource }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinancialSource_Update([DataSourceRequest]DataSourceRequest request, CashFlowFinanceSource financeSource)
        {
            if (ModelState.IsValid)
            {
                var entity = new CashFlowFinanceSource
                {
                    CashFlowFinanceSourceId = financeSource.CashFlowFinanceSourceId,
                    CashFlowYearId = financeSource.CashFlowYearId,
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

                db.CashFlowFinanceSources.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { financeSource }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinancialSource_Destroy([DataSourceRequest]DataSourceRequest request, CashFlowFinanceSource financeSource)
        {
            if (ModelState.IsValid)
            {
                var entity = new CashFlowFinanceSource
                {
                    CashFlowFinanceSourceId = financeSource.CashFlowFinanceSourceId,
                    CashFlowYearId = financeSource.CashFlowYearId,
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

                db.CashFlowFinanceSources.Attach(entity);
                db.CashFlowFinanceSources.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { financeSource }.ToDataSourceResult(request, ModelState));
        }
        private void AddFinancialSourceDetail(CashFlowFinanceSource record)
        {
            var contractData = db.CashFlowContractData.FirstOrDefault(cd => cd.CashFlowYearId == record.CashFlowYearId);
            if (contractData != null)
            {
                var monthlyContractDataContent = db.CashFlowContractDataDetail.Where(cd => cd.CashFlowContractDataId == contractData.CashFlowContractDataId).ToList();
                foreach (CashFlowContractDataDetail data in monthlyContractDataContent)
                {
                    var financeSourceDetail = new CashFlowFinanceSourceDetail
                    {
                        CashFlowFinanceSourceId = record.CashFlowFinanceSourceId,
                        ConstructionPeriodId = data.ConstructionPeriodId,
                        Revenue = data.TotalContractAmount,
                        TotalFinanceSource = data.TotalContractAmount
                    };
                    db.CashFlowFinanceSourceDetail.Add(financeSourceDetail);
                    db.SaveChanges();
                }

                record.Revenue = monthlyContractDataContent.Sum(d => d.TotalContractAmount);
                db.CashFlowFinanceSources.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}