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
    public class NetOperationCashFlowController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult ProjectNetOperationalCashFlow_Read([DataSourceRequest]DataSourceRequest request, int yearId)
        {
            List<NetOperationalCahFlowViewModel> listToReturn = new List<NetOperationalCahFlowViewModel>();
            var financialSource = db.CashFlowFinanceSources.FirstOrDefault(fs => fs.CashFlowYearId == yearId);
            var deduction = db.CashFlowDeduction.FirstOrDefault(d => d.CashFlowYearId == yearId);
            if(financialSource != null && deduction != null)
            {
                var net = new NetOperationalCahFlowViewModel
                {
                    Id = yearId,
                    CummulativeNetOperationalCashFlow = financialSource.TotalFinanceSource - deduction.TotalDeduction
                };
                listToReturn.Add(net);
            }
            return Json(listToReturn.AsQueryable().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }
        public ActionResult ProjectNetOperationalCashFlowMonthlyValue_Read([DataSourceRequest]DataSourceRequest request, int yearId)
        {
            List<NetOperationalCahFlowDetailViewModel> listToReturn = new List<NetOperationalCahFlowDetailViewModel>();
            var financialSource = db.CashFlowFinanceSources.FirstOrDefault(fs => fs.CashFlowYearId == yearId);
            var deduction = db.CashFlowDeduction.FirstOrDefault(d => d.CashFlowYearId == yearId);
            if (financialSource != null && deduction != null)
            {
                var finacesourceDetails = db.CashFlowFinanceSourceDetail.Where(fd => fd.CashFlowFinanceSourceId == financialSource.CashFlowFinanceSourceId).ToList();
                var deductions = db.CashFlowDeductionDetail.Where(fd => fd.CashFlowDeductionId == deduction.CashFlowDeductionId).ToList();
                //var net = new NetOperationalCahFlowViewModel
                //{
                //    Id = yearId,
                //    CummulativeNetOperationalCashFlow = financialSource.TotalFinanceSource - deduction.TotalDeduction
                //};
               foreach(var source in finacesourceDetails)
                {
                    var valueTobeDeducted = deductions.FirstOrDefault(d => d.ConstructionPeriodId == source.ConstructionPeriodId);
                    if(valueTobeDeducted != null)
                    {
                        var netOperationalCashFlowRecord = new NetOperationalCahFlowDetailViewModel
                        {
                            Month = source.ConstructionPeriod.Name,
                            CummulativeNetOperationalCashFlow = source.TotalFinanceSource - valueTobeDeducted.TotalDeduction
                        };
                        listToReturn.Add(netOperationalCashFlowRecord);
                    }
                }
            }
            return Json(listToReturn.AsQueryable().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }
        public ActionResult NetCashFlows_Read([DataSourceRequest]DataSourceRequest request, int yearId)
        {
            IQueryable<NetOperationCashFlow> cashFlows = db.NetOperationCashFlow.Where(cf => cf.CashFlowYearId == yearId);
            DataSourceResult result = cashFlows.ToDataSourceResult(request, cashFlow => new
            {
                NetOperationCashFlowId = cashFlow.NetOperationCashFlowId,
                CashFlowYearId = cashFlow.CashFlowYearId,
                CummulativeNetOperationalCashFlow = cashFlow.CummulativeNetOperationalCashFlow,
                DirectMaterialExpense = cashFlow.DirectMaterialExpense,
                DirectAndIndirectlaborExpense = cashFlow.DirectAndIndirectlaborExpense,
                DirectEquipmentCost = cashFlow.DirectEquipmentCost,
                ProjectOverheadExpense = cashFlow.ProjectOverheadExpense,
                HeadOfficeOverheadExpense = cashFlow.HeadOfficeOverheadExpense,
                SubContractWorksAdvance = cashFlow.SubContractWorksAdvance,
                SubContractPiecerateWorksSettlement = cashFlow.SubContractPiecerateWorksSettlement,
                HeadOfficeFinancingSettlement = cashFlow.HeadOfficeFinancingSettlement,
                SuppliersCreditSettlement = cashFlow.SuppliersCreditSettlement,
                DayWorksSettlement = cashFlow.DayWorksSettlement,
                TaxSettlement = cashFlow.TaxSettlement,
                LoanSettlement = cashFlow.LoanSettlement,
                TotalExpense = cashFlow.TotalExpense,

                OrgBranchName = cashFlow.OrgBranchName,
                DateCreated = cashFlow.DateCreated,
                LastModified = cashFlow.LastModified,
                CreatedBy = cashFlow.CreatedBy,
                ModifiedBy = cashFlow.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NetCashFlow_Create([DataSourceRequest]DataSourceRequest request, NetOperationCashFlow cashFlow, int yearId)
        {
            if (ModelState.IsValid)
            {

                var entity = new NetOperationCashFlow
                {
                    //NetOperationCashFlowId = cashFlow.NetOperationCashFlowId,
                    CashFlowYearId = yearId,
                    CummulativeNetOperationalCashFlow = cashFlow.CummulativeNetOperationalCashFlow,
                    DirectMaterialExpense = cashFlow.DirectMaterialExpense,
                    DirectAndIndirectlaborExpense = cashFlow.DirectAndIndirectlaborExpense,
                    DirectEquipmentCost = cashFlow.DirectEquipmentCost,
                    ProjectOverheadExpense = cashFlow.ProjectOverheadExpense,
                    HeadOfficeOverheadExpense = cashFlow.HeadOfficeOverheadExpense,
                    SubContractWorksAdvance = cashFlow.SubContractWorksAdvance,
                    SubContractPiecerateWorksSettlement = cashFlow.SubContractPiecerateWorksSettlement,
                    HeadOfficeFinancingSettlement = cashFlow.HeadOfficeFinancingSettlement,
                    SuppliersCreditSettlement = cashFlow.SuppliersCreditSettlement,
                    DayWorksSettlement = cashFlow.DayWorksSettlement,
                    TaxSettlement = cashFlow.TaxSettlement,
                    LoanSettlement = cashFlow.LoanSettlement,
                    TotalExpense = cashFlow.TotalExpense,

                    OrgBranchName = cashFlow.OrgBranchName,
                    DateCreated = cashFlow.DateCreated,
                    LastModified = cashFlow.LastModified,
                    CreatedBy = cashFlow.CreatedBy,
                    ModifiedBy = cashFlow.ModifiedBy
                };
                entity.TotalExpense = entity.DirectMaterialExpense + entity.DirectAndIndirectlaborExpense
                      + entity.DirectEquipmentCost + entity.ProjectOverheadExpense + entity.HeadOfficeOverheadExpense +
                      entity.SubContractWorksAdvance + entity.SubContractPiecerateWorksSettlement +
                      entity.HeadOfficeFinancingSettlement + entity.SuppliersCreditSettlement +
                      entity.DayWorksSettlement + entity.TaxSettlement + entity.LoanSettlement;

                db.NetOperationCashFlow.Add(entity);
                db.SaveChanges();
                cashFlow.NetOperationCashFlowId = entity.NetOperationCashFlowId;

                this.AddMonthlyMaterialConsumption(entity);
            }

            return Json(new[] { cashFlow }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NetCashFlow_Update([DataSourceRequest]DataSourceRequest request, NetOperationCashFlow cashFlow)
        {
            if (ModelState.IsValid)
            {
                var entity = new NetOperationCashFlow
                {
                    NetOperationCashFlowId = cashFlow.NetOperationCashFlowId,
                    CashFlowYearId = cashFlow.CashFlowYearId,
                    CummulativeNetOperationalCashFlow = cashFlow.CummulativeNetOperationalCashFlow,
                    DirectMaterialExpense = cashFlow.DirectMaterialExpense,
                    DirectAndIndirectlaborExpense = cashFlow.DirectAndIndirectlaborExpense,
                    DirectEquipmentCost = cashFlow.DirectEquipmentCost,
                    ProjectOverheadExpense = cashFlow.ProjectOverheadExpense,
                    HeadOfficeOverheadExpense = cashFlow.HeadOfficeOverheadExpense,
                    SubContractWorksAdvance = cashFlow.SubContractWorksAdvance,
                    SubContractPiecerateWorksSettlement = cashFlow.SubContractPiecerateWorksSettlement,
                    HeadOfficeFinancingSettlement = cashFlow.HeadOfficeFinancingSettlement,
                    SuppliersCreditSettlement = cashFlow.SuppliersCreditSettlement,
                    DayWorksSettlement = cashFlow.DayWorksSettlement,
                    TaxSettlement = cashFlow.TaxSettlement,
                    LoanSettlement = cashFlow.LoanSettlement,
                    TotalExpense = cashFlow.TotalExpense,

                    OrgBranchName = cashFlow.OrgBranchName,
                    DateCreated = cashFlow.DateCreated,
                    LastModified = cashFlow.LastModified,
                    CreatedBy = cashFlow.CreatedBy,
                    ModifiedBy = cashFlow.ModifiedBy
                };
                entity.TotalExpense = entity.DirectMaterialExpense + entity.DirectAndIndirectlaborExpense
                      + entity.DirectEquipmentCost + entity.ProjectOverheadExpense + entity.HeadOfficeOverheadExpense +
                      entity.SubContractWorksAdvance + entity.SubContractPiecerateWorksSettlement +
                      entity.HeadOfficeFinancingSettlement + entity.SuppliersCreditSettlement +
                      entity.DayWorksSettlement + entity.TaxSettlement + entity.LoanSettlement;

                db.NetOperationCashFlow.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { cashFlow }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NetCashFlow_Destroy([DataSourceRequest]DataSourceRequest request, NetOperationCashFlow cashFlow)
        {
            if (ModelState.IsValid)
            {
                var entity = new NetOperationCashFlow
                {
                    NetOperationCashFlowId = cashFlow.NetOperationCashFlowId,
                    CashFlowYearId = cashFlow.CashFlowYearId,
                    CummulativeNetOperationalCashFlow = cashFlow.CummulativeNetOperationalCashFlow,
                    DirectMaterialExpense = cashFlow.DirectMaterialExpense,
                    DirectAndIndirectlaborExpense = cashFlow.DirectAndIndirectlaborExpense,
                    DirectEquipmentCost = cashFlow.DirectEquipmentCost,
                    ProjectOverheadExpense = cashFlow.ProjectOverheadExpense,
                    HeadOfficeOverheadExpense = cashFlow.HeadOfficeOverheadExpense,
                    SubContractWorksAdvance = cashFlow.SubContractWorksAdvance,
                    SubContractPiecerateWorksSettlement = cashFlow.SubContractPiecerateWorksSettlement,
                    HeadOfficeFinancingSettlement = cashFlow.HeadOfficeFinancingSettlement,
                    SuppliersCreditSettlement = cashFlow.SuppliersCreditSettlement,
                    DayWorksSettlement = cashFlow.DayWorksSettlement,
                    TaxSettlement = cashFlow.TaxSettlement,
                    LoanSettlement = cashFlow.LoanSettlement,
                    TotalExpense = cashFlow.TotalExpense,

                    OrgBranchName = cashFlow.OrgBranchName,
                    DateCreated = cashFlow.DateCreated,
                    LastModified = cashFlow.LastModified,
                    CreatedBy = cashFlow.CreatedBy,
                    ModifiedBy = cashFlow.ModifiedBy
                };

                db.NetOperationCashFlow.Attach(entity);
                db.NetOperationCashFlow.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { cashFlow }.ToDataSourceResult(request, ModelState));
        }
        private void AddMonthlyMaterialConsumption(NetOperationCashFlow record)
        {
            var contractData = db.CashFlowContractData.FirstOrDefault(fs => fs.CashFlowYearId == record.CashFlowYearId);
            if (contractData != null)
            {
                var monthlyContractValues = db.CashFlowContractDataDetail.Where(cd => cd.CashFlowContractDataId == contractData.CashFlowContractDataId).ToList();
                var periodIds = new HashSet<int>(monthlyContractValues.Select(mv => mv.ConstructionPeriodId));
                var periods = db.ConstructionPeriod.Where(p => periodIds.Contains(p.ConstructionPeriodId)).ToList();

                var totalConsumptionInYear = new decimal();
                foreach (ConstructionPeriod period in periods)
                {
                    var cashFlowYear = record.CashFlowYear ?? new CashFlowYear();
                    var monthlyRecord = new NetOperationCashFlowDetail
                    {
                        DirectMaterialExpense = ResourceUtilizationHelper.GetProjectMaterialConsumptionPerPeriod(cashFlowYear.ProjectInformationId, period),
                        ConstructionPeriodId = period.ConstructionPeriodId,
                        NetOperationCashFlowId = record.NetOperationCashFlowId,
                    };
                    totalConsumptionInYear += monthlyRecord.DirectMaterialExpense;
                    monthlyRecord.TotalExpense = monthlyRecord.DirectMaterialExpense;

                    db.NetOperationCashFlowDetail.Add(monthlyRecord);
                    db.SaveChanges();
                }

                record.DirectMaterialExpense = totalConsumptionInYear;
                record.TotalExpense = record.DirectMaterialExpense;

                db.NetOperationCashFlow.Attach(record);
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