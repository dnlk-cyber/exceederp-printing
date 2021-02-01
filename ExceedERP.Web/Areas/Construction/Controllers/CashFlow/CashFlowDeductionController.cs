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
    public class CashFlowDeductionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();


        public ActionResult Deductions_Read([DataSourceRequest]DataSourceRequest request, int yearId)
        {
            IQueryable<CashFlowDeduction> deductions = db.CashFlowDeduction.Where(fs => fs.CashFlowYearId == yearId);
            DataSourceResult result = deductions.ToDataSourceResult(request, deduction => new {
                CashFlowDeductionId = deduction.CashFlowDeductionId,
                CashFlowYearId = deduction.CashFlowYearId,
                MainContractAdvanceRepayment = deduction.MainContractAdvanceRepayment,
                IncomeTaxes = deduction.IncomeTaxes,
                MainContractRetentionWithheld = deduction.MainContractRetentionWithheld,
                Rebate = deduction.Rebate,
                OmmissionWorks = deduction.OmmissionWorks,
                Others = deduction.Others,
                TotalDeduction = deduction.TotalDeduction,
                

                OrgBranchName = deduction.OrgBranchName,
                DateCreated = deduction.DateCreated,
                LastModified = deduction.LastModified,
                CreatedBy = deduction.CreatedBy,
                ModifiedBy = deduction.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Deduction_Create([DataSourceRequest]DataSourceRequest request, CashFlowDeduction deduction, int yearId)
        {
            if (ModelState.IsValid)
            {

                var entity = new CashFlowDeduction
                {
                    //CashFlowDeductionId = deduction.CashFlowDeductionId,
                    CashFlowYearId = yearId,
                    MainContractAdvanceRepayment = deduction.MainContractAdvanceRepayment,
                    IncomeTaxes = deduction.IncomeTaxes,
                    MainContractRetentionWithheld = deduction.MainContractRetentionWithheld,
                    Rebate = deduction.Rebate,
                    OmmissionWorks = deduction.OmmissionWorks,
                    Others = deduction.Others,
                    TotalDeduction = deduction.TotalDeduction,

                    OrgBranchName = deduction.OrgBranchName,
                    DateCreated = deduction.DateCreated,
                    LastModified = deduction.LastModified,
                    CreatedBy = deduction.CreatedBy,
                    ModifiedBy = deduction.ModifiedBy
                };
                entity.TotalDeduction = entity.MainContractAdvanceRepayment +
                                        entity.IncomeTaxes + entity.MainContractRetentionWithheld +
                                        entity.Rebate + entity.OmmissionWorks + entity.Others;


                db.CashFlowDeduction.Add(entity);
                db.SaveChanges();
                deduction.CashFlowDeductionId = entity.CashFlowDeductionId;

                this.AddFinancialDeductionDetail(entity);
            }

            return Json(new[] { deduction }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Deduction_Update([DataSourceRequest]DataSourceRequest request, CashFlowDeduction deduction)
        {
            if (ModelState.IsValid)
            {
                var entity = new CashFlowDeduction
                {
                    CashFlowDeductionId = deduction.CashFlowDeductionId,
                    CashFlowYearId = deduction.CashFlowYearId,
                    MainContractAdvanceRepayment = deduction.MainContractAdvanceRepayment,
                    IncomeTaxes = deduction.IncomeTaxes,
                    MainContractRetentionWithheld = deduction.MainContractRetentionWithheld,
                    Rebate = deduction.Rebate,
                    OmmissionWorks = deduction.OmmissionWorks,
                    Others = deduction.Others,
                    TotalDeduction = deduction.TotalDeduction,

                    OrgBranchName = deduction.OrgBranchName,
                    DateCreated = deduction.DateCreated,
                    LastModified = deduction.LastModified,
                    CreatedBy = deduction.CreatedBy,
                    ModifiedBy = deduction.ModifiedBy
                };
                entity.TotalDeduction = entity.MainContractAdvanceRepayment +
                                        entity.IncomeTaxes + entity.MainContractRetentionWithheld +
                                        entity.Rebate + entity.OmmissionWorks + entity.Others;

                db.CashFlowDeduction.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { deduction }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Deduction_Destroy([DataSourceRequest]DataSourceRequest request, CashFlowDeduction deduction)
        {
            if (ModelState.IsValid)
            {
                var entity = new CashFlowDeduction
                {
                    CashFlowDeductionId = deduction.CashFlowDeductionId,
                    CashFlowYearId = deduction.CashFlowYearId,
                    MainContractAdvanceRepayment = deduction.MainContractAdvanceRepayment,
                    IncomeTaxes = deduction.IncomeTaxes,
                    MainContractRetentionWithheld = deduction.MainContractRetentionWithheld,
                    Rebate = deduction.Rebate,
                    OmmissionWorks = deduction.OmmissionWorks,
                    Others = deduction.Others,
                    TotalDeduction = deduction.TotalDeduction,

                    OrgBranchName = deduction.OrgBranchName,
                    DateCreated = deduction.DateCreated,
                    LastModified = deduction.LastModified,
                    CreatedBy = deduction.CreatedBy,
                    ModifiedBy = deduction.ModifiedBy
                };

                db.CashFlowDeduction.Attach(entity);
                db.CashFlowDeduction.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { deduction }.ToDataSourceResult(request, ModelState));
        }
        private void AddFinancialDeductionDetail(CashFlowDeduction record) {
            var financeSource = db.CashFlowFinanceSources.FirstOrDefault(fs => fs.CashFlowYearId == record.CashFlowYearId);
            if(financeSource != null)
            {
                var monthlySources = db.CashFlowFinanceSourceDetail.Where(fsd => fsd.CashFlowFinanceSourceId == financeSource.CashFlowFinanceSourceId).ToList();
                var totalTax = new decimal();
                foreach (CashFlowFinanceSourceDetail data in monthlySources)
                {
                    var deductionRecord = new CashFlowDeductionDetail
                    {
                        CashFlowDeductionId = record.CashFlowDeductionId,
                        ConstructionPeriodId = data.ConstructionPeriodId,
                        IncomeTaxes = data.Revenue - (data.Revenue / new decimal(1.15)),
                        TotalDeduction = data.Revenue - (data.Revenue / new decimal(1.15)),
                    };
                    totalTax += deductionRecord.IncomeTaxes;

                    db.CashFlowDeductionDetail.Add(deductionRecord);
                    db.SaveChanges();
                }

                record.IncomeTaxes = totalTax;
                db.CashFlowDeduction.Attach(record);
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