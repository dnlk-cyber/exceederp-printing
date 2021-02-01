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
    public class CashFlowContractDataDetailController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();


        public ActionResult ContractData_Read([DataSourceRequest]DataSourceRequest request, int ctrdataId)
        {
            IQueryable<CashFlowContractDataDetail> details = db.CashFlowContractDataDetail.Where(d => d.CashFlowContractDataId == ctrdataId);
            DataSourceResult result = details.ToDataSourceResult(request, detail => new
            {
                CashFlowContractDataDetailId = detail.CashFlowContractDataDetailId,
                CashFlowContractDataId = detail.CashFlowContractDataId,
                ConstructionPeriodId = detail.ConstructionPeriodId,
                MonthlyProgress = detail.MonthlyProgress,
                CummulativeProgress = detail.CummulativeProgress,
                ContractorAmountWithVat = detail.ContractorAmountWithVat,
                ProvisionalSum = detail.ProvisionalSum,
                SubContractWorks = detail.SubContractWorks,
                DayWorks = detail.DayWorks,
                TotalOriginalContractAmount = detail.TotalOriginalContractAmount,
                SupplementaryAgreement = detail.SupplementaryAgreement,
                VariationWorks = detail.VariationWorks,
                TotalContractAmount = detail.TotalContractAmount,

                OrgBranchName = detail.OrgBranchName,
                DateCreated = detail.DateCreated,
                LastModified = detail.LastModified,
                CreatedBy = detail.CreatedBy,
                ModifiedBy = detail.ModifiedBy
            });

            return Json(result);
        }
        private bool ContractDataOfThisMonthExists(int periodId, int headerId)
        {
            var valueExists = false;
            var temp = db.CashFlowContractDataDetail.FirstOrDefault(d => d.CashFlowContractDataId == headerId && d.ConstructionPeriodId == periodId);
            if (temp != null)
            {
                valueExists = true;
            }
            return valueExists;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractData_Create([DataSourceRequest]DataSourceRequest request, CashFlowContractDataDetail detail, int ctrdataId)
        {
            if (ModelState.IsValid)
            {
                if (ContractDataOfThisMonthExists(detail.ConstructionPeriodId, ctrdataId))
                {
                    ModelState.AddModelError("", "Data for this Period Exists");
                }
                else
                {
                    var entity = new CashFlowContractDataDetail
                    {
                        CashFlowContractDataDetailId = detail.CashFlowContractDataDetailId,
                        CashFlowContractDataId = ctrdataId,
                        ConstructionPeriodId = detail.ConstructionPeriodId,
                        MonthlyProgress = detail.MonthlyProgress,
                        CummulativeProgress = this.CalculateCummulativeProgress(detail, ctrdataId),
                        ContractorAmountWithVat = detail.ContractorAmountWithVat,
                        ProvisionalSum = detail.ProvisionalSum,
                        SubContractWorks = detail.SubContractWorks,
                        DayWorks = detail.DayWorks,
                        TotalOriginalContractAmount = detail.TotalOriginalContractAmount,
                        SupplementaryAgreement = detail.SupplementaryAgreement,
                        VariationWorks = detail.VariationWorks,
                        TotalContractAmount = detail.TotalContractAmount,

                        OrgBranchName = detail.OrgBranchName,
                        DateCreated = detail.DateCreated,
                        LastModified = detail.LastModified,
                        CreatedBy = detail.CreatedBy,
                        ModifiedBy = detail.ModifiedBy
                    };
                    entity.TotalOriginalContractAmount = entity.ContractorAmountWithVat +
                        entity.ProvisionalSum + entity.SubContractWorks + entity.DayWorks;
                    //var by = db.CashFlowYear.Find(yearId);
                    //if (by != null)
                    //{
                    //    var contract = ContractMgmtHelper.GetProjectMainContract(by.ProjectInformationId);
                    //    entity.TotalOriginalContractAmount = contract.ContractAmount;
                    //    entity.SupplementaryAgreement = contract.Supplemetary ?? 0;
                    //    entity.VariationWorks = contract.Variation ?? 0;
                    //    entity.TotalContractAmount = contract.TotalProjectAmount ?? 0;
                    //}
                    db.CashFlowContractDataDetail.Add(entity);
                    db.SaveChanges();
                    detail.CashFlowContractDataId = entity.CashFlowContractDataId;
                }
            }

            return Json(new[] { detail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractData_Update([DataSourceRequest]DataSourceRequest request, CashFlowContractDataDetail detail)
        {
            if (ModelState.IsValid)
            {
                var entity = new CashFlowContractDataDetail
                {
                    CashFlowContractDataDetailId = detail.CashFlowContractDataDetailId,
                    CashFlowContractDataId = detail.CashFlowContractDataId,
                    ConstructionPeriodId = detail.ConstructionPeriodId,
                    MonthlyProgress = detail.MonthlyProgress,
                    CummulativeProgress = this.CalculateCummulativeProgress(detail, detail.CashFlowContractDataId),
                    ContractorAmountWithVat = detail.ContractorAmountWithVat,
                    ProvisionalSum = detail.ProvisionalSum,
                    SubContractWorks = detail.SubContractWorks,
                    DayWorks = detail.DayWorks,
                    TotalOriginalContractAmount = detail.TotalOriginalContractAmount,
                    SupplementaryAgreement = detail.SupplementaryAgreement,
                    VariationWorks = detail.VariationWorks,
                    TotalContractAmount = detail.TotalContractAmount,

                    OrgBranchName = detail.OrgBranchName,
                    DateCreated = detail.DateCreated,
                    LastModified = detail.LastModified,
                    CreatedBy = detail.CreatedBy,
                    ModifiedBy = detail.ModifiedBy
                };
                entity.TotalOriginalContractAmount = entity.ContractorAmountWithVat +
                       entity.ProvisionalSum + entity.SubContractWorks + entity.DayWorks;

                db.CashFlowContractDataDetail.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { detail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractData_Destroy([DataSourceRequest]DataSourceRequest request, CashFlowContractDataDetail detail)
        {
            if (ModelState.IsValid)
            {
                var entity = new CashFlowContractDataDetail
                {
                    CashFlowContractDataDetailId = detail.CashFlowContractDataDetailId,
                    CashFlowContractDataId = detail.CashFlowContractDataId,
                    ConstructionPeriodId = detail.ConstructionPeriodId,
                    MonthlyProgress = detail.MonthlyProgress,
                    CummulativeProgress = detail.CummulativeProgress,
                    ContractorAmountWithVat = detail.ContractorAmountWithVat,
                    ProvisionalSum = detail.ProvisionalSum,
                    SubContractWorks = detail.SubContractWorks,
                    DayWorks = detail.DayWorks,
                    TotalOriginalContractAmount = detail.TotalOriginalContractAmount,
                    SupplementaryAgreement = detail.SupplementaryAgreement,
                    VariationWorks = detail.VariationWorks,
                    TotalContractAmount = detail.TotalContractAmount,

                    OrgBranchName = detail.OrgBranchName,
                    DateCreated = detail.DateCreated,
                    LastModified = detail.LastModified,
                    CreatedBy = detail.CreatedBy,
                    ModifiedBy = detail.ModifiedBy
                };

                db.CashFlowContractDataDetail.Attach(entity);
                db.CashFlowContractDataDetail.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { detail }.ToDataSourceResult(request, ModelState));
        }
        private decimal CalculateCummulativeProgress(CashFlowContractDataDetail record, int id)
        {
            var cummulativeProgress = record.MonthlyProgress;
            if (record.ConstructionPeriod != null)
            {
                var dataOfPreviousMonth = db.CashFlowContractDataDetail
                                        .Where(d => d.CashFlowContractDataId == id &&
                                        d.ConstructionPeriod.DateTo < record.ConstructionPeriod.DateFrom).ToList();
                if (dataOfPreviousMonth.Any())
                {
                    cummulativeProgress = dataOfPreviousMonth.Sum(d => d.MonthlyProgress);
                }
            }

            return cummulativeProgress;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}