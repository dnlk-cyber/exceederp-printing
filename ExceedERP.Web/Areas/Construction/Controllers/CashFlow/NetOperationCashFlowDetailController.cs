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
    public class NetOperationCashFlowDetailController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult NetCashFlows_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<NetOperationCashFlowDetail> cashFlows = db.NetOperationCashFlowDetail.Where(cf => cf.NetOperationCashFlowId == id);
            DataSourceResult result = cashFlows.ToDataSourceResult(request, cashFlow => new {
                NetOperationCashFlowDetailId = cashFlow.NetOperationCashFlowDetailId,
                NetOperationCashFlowId = cashFlow.NetOperationCashFlowId,
                ConstructionPeriodId = cashFlow.ConstructionPeriodId,
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
        public ActionResult NetCashFlow_Create([DataSourceRequest]DataSourceRequest request, NetOperationCashFlowDetail cashFlow, int id)
        {
            if (ModelState.IsValid)
            {

                var entity = new NetOperationCashFlowDetail
                {
                    //NetOperationCashFlowDetailId = cashFlow.NetOperationCashFlowDetailId,
                    NetOperationCashFlowId = id,
                    ConstructionPeriodId = cashFlow.ConstructionPeriodId,
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

                db.NetOperationCashFlowDetail.Add(entity);
                db.SaveChanges();
                cashFlow.NetOperationCashFlowId = entity.NetOperationCashFlowId;
            }

            return Json(new[] { cashFlow }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NetCashFlow_Update([DataSourceRequest]DataSourceRequest request, NetOperationCashFlowDetail cashFlow)
        {
            if (ModelState.IsValid)
            {
                var entity = new NetOperationCashFlowDetail
                {
                    NetOperationCashFlowDetailId = cashFlow.NetOperationCashFlowDetailId,
                    NetOperationCashFlowId = cashFlow.NetOperationCashFlowId,
                    ConstructionPeriodId = cashFlow.ConstructionPeriodId,
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

                db.NetOperationCashFlowDetail.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { cashFlow }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NetCashFlow_Destroy([DataSourceRequest]DataSourceRequest request, NetOperationCashFlowDetail cashFlow)
        {
            if (ModelState.IsValid)
            {
                var entity = new NetOperationCashFlowDetail
                {
                    NetOperationCashFlowDetailId = cashFlow.NetOperationCashFlowDetailId,
                    NetOperationCashFlowId = cashFlow.NetOperationCashFlowId,
                    ConstructionPeriodId = cashFlow.ConstructionPeriodId,
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

                db.NetOperationCashFlowDetail.Attach(entity);
                db.NetOperationCashFlowDetail.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { cashFlow }.ToDataSourceResult(request, ModelState));
        }
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}