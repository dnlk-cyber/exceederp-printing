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
    public class CashFlowBalanceController : Controller
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
       
        public ActionResult ProjectCashFlowBalance_Read([DataSourceRequest]DataSourceRequest request, int yearId)
        {
            List<CashFlowBalanceVieModel> listToReturn = new List<CashFlowBalanceVieModel>();
            var financialSource = db.CashFlowFinanceSources.FirstOrDefault(fs => fs.CashFlowYearId == yearId);
            var deduction = db.CashFlowDeduction.FirstOrDefault(d => d.CashFlowYearId == yearId);
            var cost = db.NetOperationCashFlow.FirstOrDefault(c => c.CashFlowYearId == yearId);
            if (financialSource != null && deduction != null && cost != null)
            {
                var net = new CashFlowBalanceVieModel
                {
                    Id = yearId,
                    NetBalance = (financialSource.TotalFinanceSource - deduction.TotalDeduction) - cost.TotalExpense
                };
                listToReturn.Add(net);
            }
            return Json(listToReturn.AsQueryable().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ProjectCashFlowBalanceDetail_Read([DataSourceRequest]DataSourceRequest request, int yearId)
        {
            List<CashFlowBalanceDetailVieModel> listToReturn = new List<CashFlowBalanceDetailVieModel>();
            var financialSource = db.CashFlowFinanceSources.FirstOrDefault(fs => fs.CashFlowYearId == yearId);
            var deduction = db.CashFlowDeduction.FirstOrDefault(d => d.CashFlowYearId == yearId);
            var cost = db.NetOperationCashFlow.FirstOrDefault(c => c.CashFlowYearId == yearId);

            if (financialSource != null && deduction != null && cost != null)
            {
                var finacesourceDetails = db.CashFlowFinanceSourceDetail.Where(fd => fd.CashFlowFinanceSourceId == financialSource.CashFlowFinanceSourceId).ToList();
                var deductions = db.CashFlowDeductionDetail.Where(fd => fd.CashFlowDeductionId == deduction.CashFlowDeductionId).ToList();
                var expenses = db.NetOperationCashFlowDetail.Where(d => d.NetOperationCashFlowId == cost.NetOperationCashFlowId).ToList();

                foreach (var source in finacesourceDetails)
                {
                    var deductionItem = deductions.FirstOrDefault(d => d.ConstructionPeriodId == source.ConstructionPeriodId);
                    var costItem = expenses.FirstOrDefault(ex => ex.ConstructionPeriodId == source.ConstructionPeriodId);
                    if (deductionItem != null && costItem != null)
                    {
                        var netOperationalCashFlowRecord = new CashFlowBalanceDetailVieModel
                        {
                            Month = source.ConstructionPeriod.Name,
                            NetBalance = (source.TotalFinanceSource - deductionItem.TotalDeduction) - costItem.TotalExpense
                        };
                        listToReturn.Add(netOperationalCashFlowRecord);
                    }
                }
            }
            return Json(listToReturn.AsQueryable().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}