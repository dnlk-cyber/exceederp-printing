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
    public class CashFlowDeductionDetailController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();


        public ActionResult Deductions_Read([DataSourceRequest]DataSourceRequest request, int deductionId)
        {
            IQueryable<CashFlowDeductionDetail> deductions = db.CashFlowDeductionDetail.Where(fs => fs.CashFlowDeductionId == deductionId);
            DataSourceResult result = deductions.ToDataSourceResult(request, deduction => new {
                CashFlowDeductionDetailId = deduction.CashFlowDeductionDetailId,
                CashFlowDeductionId = deduction.CashFlowDeductionId,
                ConstructionPeriodId = deduction.ConstructionPeriodId,
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
        public ActionResult Deduction_Create([DataSourceRequest]DataSourceRequest request, CashFlowDeductionDetail deduction, int deductionId)
        {
            if (ModelState.IsValid)
            {

                var entity = new CashFlowDeductionDetail
                {
                    //CashFlowDeductionDetailId = deduction.CashFlowDeductionDetailId,
                    CashFlowDeductionId = deductionId,
                    ConstructionPeriodId = deduction.ConstructionPeriodId,
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

                db.CashFlowDeductionDetail.Add(entity);
                db.SaveChanges();
                deduction.CashFlowDeductionId = entity.CashFlowDeductionId;
            }

            return Json(new[] { deduction }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Deduction_Update([DataSourceRequest]DataSourceRequest request, CashFlowDeductionDetail deduction)
        {
            if (ModelState.IsValid)
            {
                var entity = new CashFlowDeductionDetail
                {
                    CashFlowDeductionDetailId = deduction.CashFlowDeductionDetailId,
                    CashFlowDeductionId = deduction.CashFlowDeductionId,
                    ConstructionPeriodId = deduction.ConstructionPeriodId,
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

                db.CashFlowDeductionDetail.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { deduction }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Deduction_Destroy([DataSourceRequest]DataSourceRequest request, CashFlowDeductionDetail deduction)
        {
            if (ModelState.IsValid)
            {
                var entity = new CashFlowDeductionDetail
                {
                    CashFlowDeductionDetailId = deduction.CashFlowDeductionDetailId,
                    CashFlowDeductionId = deduction.CashFlowDeductionId,
                    ConstructionPeriodId = deduction.ConstructionPeriodId,
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

                db.CashFlowDeductionDetail.Attach(entity);
                db.CashFlowDeductionDetail.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { deduction }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}