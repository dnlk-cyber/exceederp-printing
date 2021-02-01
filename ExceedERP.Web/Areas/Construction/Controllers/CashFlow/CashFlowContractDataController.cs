using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
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
    public class CashFlowContractDataController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();


        public ActionResult ContractData_Read([DataSourceRequest]DataSourceRequest request, int yearId)
        {
            IQueryable<CashFlowContractData> contractDatas = db.CashFlowContractData.Where(y => y.CashFlowYearId == yearId);
            DataSourceResult result = contractDatas.ToDataSourceResult(request, contractData => new
            {
                CashFlowContractDataId = contractData.CashFlowContractDataId,
                CashFlowYearId = contractData.CashFlowYearId,
                ContractorAmountWithVat = contractData.ContractorAmountWithVat,
                ProvisionalSum = contractData.ProvisionalSum,
                SubContractWorks = contractData.SubContractWorks,
                DayWorks = contractData.DayWorks,
                TotalOriginalContractAmount = contractData.TotalOriginalContractAmount,
                SupplementaryAgreement = contractData.SupplementaryAgreement,
                VariationWorks = contractData.VariationWorks,
                TotalContractAmount = contractData.TotalContractAmount,
                TCPPercentage = contractData.TCPPercentage,

                OrgBranchName = contractData.OrgBranchName,
                DateCreated = contractData.DateCreated,
                LastModified = contractData.LastModified,
                CreatedBy = contractData.CreatedBy,
                ModifiedBy = contractData.ModifiedBy
            });

            return Json(result);
        }

        public bool ContractDataExists(int yearId)
        {
            var ctrtDataExists = false;
            var temp = db.CashFlowContractData.FirstOrDefault(cd => cd.CashFlowYearId == yearId);
            if (temp != null)
            {
                ctrtDataExists = true;
            }
            return ctrtDataExists;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractData_Create([DataSourceRequest]DataSourceRequest request, CashFlowContractData contractData, int yearId)
        {
            if (ModelState.IsValid)
            {
                if (ContractDataExists(yearId))
                {
                    ModelState.AddModelError("", "Contract Data exists already");
                }
                else
                {
                    var entity = new CashFlowContractData
                    {
                        CashFlowContractDataId = contractData.CashFlowContractDataId,
                        CashFlowYearId = yearId,
                        ContractorAmountWithVat = contractData.ContractorAmountWithVat,
                        ProvisionalSum = contractData.ProvisionalSum,
                        SubContractWorks = contractData.SubContractWorks,
                        DayWorks = contractData.DayWorks,
                        TotalOriginalContractAmount = contractData.TotalOriginalContractAmount,
                        SupplementaryAgreement = contractData.SupplementaryAgreement,
                        VariationWorks = contractData.VariationWorks,
                        TotalContractAmount = contractData.TotalContractAmount,
                        TCPPercentage = contractData.TCPPercentage,

                        OrgBranchName = contractData.OrgBranchName,
                        DateCreated = contractData.DateCreated,
                        LastModified = contractData.LastModified,
                        CreatedBy = contractData.CreatedBy,
                        ModifiedBy = contractData.ModifiedBy
                    };
                    var by = db.CashFlowYear.Find(yearId);
                    if (by != null)
                    {
                        var contract = ContractMgmtHelper.GetProjectMainContract(by.ProjectInformationId);
                        entity.TotalOriginalContractAmount = contract.ContractAmount;
                        entity.SupplementaryAgreement = contract.Supplemetary ?? 0;
                        entity.VariationWorks = contract.Variation ?? 0;
                        entity.TotalContractAmount = contract.TotalProjectAmount ?? 0;
                    }
                    db.CashFlowContractData.Add(entity);
                    db.SaveChanges();
                    contractData.CashFlowContractDataId = entity.CashFlowContractDataId;

                    this.AddPlannedValuesFromScheduleToContractData(entity);
                }

            }

            return Json(new[] { contractData }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractData_Update([DataSourceRequest]DataSourceRequest request, CashFlowContractData contractData)
        {
            if (ModelState.IsValid)
            {
                var entity = new CashFlowContractData
                {
                    CashFlowContractDataId = contractData.CashFlowContractDataId,
                    CashFlowYearId = contractData.CashFlowYearId,
                    ContractorAmountWithVat = contractData.ContractorAmountWithVat,
                    ProvisionalSum = contractData.ProvisionalSum,
                    SubContractWorks = contractData.SubContractWorks,
                    DayWorks = contractData.DayWorks,
                    TotalOriginalContractAmount = contractData.TotalOriginalContractAmount,
                    SupplementaryAgreement = contractData.SupplementaryAgreement,
                    VariationWorks = contractData.VariationWorks,
                    TotalContractAmount = contractData.TotalContractAmount,
                    TCPPercentage = contractData.TCPPercentage,

                    OrgBranchName = contractData.OrgBranchName,
                    DateCreated = contractData.DateCreated,
                    LastModified = contractData.LastModified,
                    CreatedBy = contractData.CreatedBy,
                    ModifiedBy = contractData.ModifiedBy
                };

                db.CashFlowContractData.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { contractData }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractData_Destroy([DataSourceRequest]DataSourceRequest request, CashFlowContractData contractData)
        {
            if (ModelState.IsValid)
            {
                var entity = new CashFlowContractData
                {
                    CashFlowContractDataId = contractData.CashFlowContractDataId,
                    CashFlowYearId = contractData.CashFlowYearId,
                    ContractorAmountWithVat = contractData.ContractorAmountWithVat,
                    ProvisionalSum = contractData.ProvisionalSum,
                    SubContractWorks = contractData.SubContractWorks,
                    DayWorks = contractData.DayWorks,
                    TotalOriginalContractAmount = contractData.TotalOriginalContractAmount,
                    SupplementaryAgreement = contractData.SupplementaryAgreement,
                    VariationWorks = contractData.VariationWorks,
                    TotalContractAmount = contractData.TotalContractAmount,
                    TCPPercentage = contractData.TCPPercentage,

                    OrgBranchName = contractData.OrgBranchName,
                    DateCreated = contractData.DateCreated,
                    LastModified = contractData.LastModified,
                    CreatedBy = contractData.CreatedBy,
                    ModifiedBy = contractData.ModifiedBy
                };

                db.CashFlowContractData.Attach(entity);
                db.CashFlowContractData.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { contractData }.ToDataSourceResult(request, ModelState));
        }
        private void AddPlannedValuesFromScheduleToContractData(CashFlowContractData record)
        {
            var joinedPeriods = (from gl in db.GLPeriods
                                 join cp in db.ConstructionPeriod
                                 on gl.Name equals cp.Name
                                 where gl.GlFiscalYearId == record.CashFlowYear.GlFiscalYearId
                                 select new
                                 {
                                     GlFiscalYearId = gl.GlFiscalYearId,
                                     GLPeriodId = gl.GLPeriodId,
                                     Name = cp.Name,
                                     ConstructionPeriodId = cp.ConstructionPeriodId
                                 }).ToList();
            var ids = new HashSet<int>(joinedPeriods.Select(p => p.ConstructionPeriodId));
            //var planneAmountForThisMonth = db.
            var costBreakDown = db.TaskDetail
                .Where(d => d.Task.WorkItemDuration.ProjectInformationId == record.CashFlowYear.ProjectInformationId &&
                !d.Task.IsSubActivityCreatedInSchedule &&
                ids.Contains(d.ConstructionPeriodId)
                ).ToList();
            var sortedPeriods = db.ConstructionPeriod.Where(p => ids.Contains(p.ConstructionPeriodId)).OrderBy(p => p.DateTo).ToList();
            //foreach (var periodId in ids)
            foreach (ConstructionPeriod period in sortedPeriods)
            {
                var plannedCost = new decimal();
                var costsOfThisMonth = costBreakDown.Where(cb => cb.ConstructionPeriodId == period.ConstructionPeriodId).ToList();
                if (costsOfThisMonth.Any())
                {
                    plannedCost = costsOfThisMonth.Sum(c => c.TotalCost);
                }

                var contractDataDetail = new CashFlowContractDataDetail
                {
                    CashFlowContractDataId = record.CashFlowContractDataId,
                    ConstructionPeriodId = period.ConstructionPeriodId,

                    ContractorAmountWithVat = plannedCost * (decimal)1.15,
                    ProvisionalSum = 0,
                    SubContractWorks = 0,
                    DayWorks = 0,

                    SupplementaryAgreement = 0,
                    VariationWorks = 0,
                };
                contractDataDetail.TotalOriginalContractAmount = contractDataDetail.ContractorAmountWithVat +
                                                                 contractDataDetail.ProvisionalSum +
                                                                 contractDataDetail.SubContractWorks +
                                                                 contractDataDetail.DayWorks;

                contractDataDetail.TotalContractAmount = contractDataDetail.TotalOriginalContractAmount +
                                                        contractDataDetail.SupplementaryAgreement +
                                                       contractDataDetail.VariationWorks;
                db.CashFlowContractDataDetail.Add(contractDataDetail);
                db.SaveChanges();

            }
            this.UpdateProgressInformation(record.CashFlowContractDataId);
        }
        private void UpdateProgressInformation(int id) {
            var currentContractData = db.CashFlowContractData.Find(id);
            if(currentContractData != null)
            {
                var detailData = db.CashFlowContractDataDetail.Where(d => d.CashFlowContractDataId == id).OrderBy(d => d.ConstructionPeriod.DateTo).ToList();
                if (detailData.Any())
                {
                    var totalAmount = detailData.Sum(d => d.ContractorAmountWithVat);
                    // update monthly progress
                    foreach(CashFlowContractDataDetail detail in detailData)
                    {
                        detail.MonthlyProgress = totalAmount > 0 ? detail.ContractorAmountWithVat / totalAmount : 0;

                        db.CashFlowContractDataDetail.Attach(detail);
                        db.Entry(detail).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                }
                if (detailData.Any())
                {
                    // update cummulative progress

                    //var totalAmount = detailData.Sum(d => d.ContractorAmountWithVat);
                    foreach (CashFlowContractDataDetail detail in detailData)
                    {
                        var dataOfPreviousMonth = db.CashFlowContractDataDetail
                                       .Where(d => d.CashFlowContractDataId == id &&
                                       d.ConstructionPeriod.DateTo < detail.ConstructionPeriod.DateFrom).ToList();
                        var cummulativeProgress = detail.MonthlyProgress;
                        if (dataOfPreviousMonth.Any())
                        {
                            cummulativeProgress = dataOfPreviousMonth.Sum(d => d.MonthlyProgress);
                        }
                        //detail.MonthlyProgress = totalAmount > 0 ? detail.ContractorAmountWithVat / totalAmount : 0;
                        detail.CummulativeProgress = cummulativeProgress;
                        db.CashFlowContractDataDetail.Attach(detail);
                        db.Entry(detail).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                //var detailData = db.CashFlowContractDataDetail.Where(d => d.CashFlowContractDataId == id).OrderBy(d => d.ConstructionPeriod.DateTo).ToList();

            }
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}