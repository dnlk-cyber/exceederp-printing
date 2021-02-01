using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Construction.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Construction.DataSource
{
    [DataObject]
    public class CashFlowOds : List<CashFlowYearViewModel>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<CashFlowYearViewModel> GetCashFlowContents(string projectId, string yearId)
        {

            List<CashFlowYearViewModel> list = new List<CashFlowYearViewModel>();
            var projId = Convert.ToInt32(projectId);
            var yrId = Convert.ToInt32(yearId);
            var currentProject = db.ProjectInformation.Find(projId);
            var budgetYear = db.CashFlowYear.Find(yrId);
            if (projectId == null || yearId == null)
            {
                return null;
            }
            if (currentProject != null && budgetYear != null)
            {
                var yearRecord = new CashFlowYearViewModel
                {
                    Name = budgetYear.GlFiscalYear.Name
                };
                var contractData = db.CashFlowContractData.FirstOrDefault(c => c.CashFlowYearId == budgetYear.CashFlowYearId);
                var finaccialSource = db.CashFlowFinanceSources.FirstOrDefault(c => c.CashFlowYearId == budgetYear.CashFlowYearId);
                var deduction = db.CashFlowDeduction.FirstOrDefault(c => c.CashFlowYearId == budgetYear.CashFlowYearId);
                var cost = db.NetOperationCashFlow.FirstOrDefault(c => c.CashFlowYearId == budgetYear.CashFlowYearId);

                #region contract data
                if (contractData != null)
                {
                    var monthlyData = db.CashFlowContractDataDetail.Where(d => d.CashFlowContractDataId == contractData.CashFlowContractDataId).ToList();
                    foreach (CashFlowContractDataDetail detail in monthlyData)
                    {
                        var monthlyProgress = new CashFlowContractDataViewModel
                        {
                            AttributeName = "Monthly Progress",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            AttributeValue = detail.MonthlyProgress * 100,
                            Order = 1
                        };
                        var cummulativeProgress = new CashFlowContractDataViewModel
                        {
                            AttributeName = "Cummulative Progress",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            AttributeValue = detail.CummulativeProgress * 100,
                            Order = 2
                        };

                        var condtractorAmount = new CashFlowContractDataViewModel
                        {
                            AttributeName = "Contractor Amount (with 15% VAT)",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            AttributeValue = detail.ContractorAmountWithVat,
                            PlannedTotalAmount = contractData.ContractorAmountWithVat,
                            TCPPercentage = contractData.TotalOriginalContractAmount > 0 ? (contractData.ContractorAmountWithVat / contractData.TotalOriginalContractAmount) * 100 : 0,
                            Order = 3
                        };
                        var provisionalSum = new CashFlowContractDataViewModel
                        {
                            AttributeName = "Provisional Sums",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            AttributeValue = detail.ProvisionalSum,
                            Order = 4
                        };
                        var subContractWorks = new CashFlowContractDataViewModel
                        {
                            AttributeName = "Sub Contract Works",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            AttributeValue = detail.SubContractWorks,
                            Order = 5
                        };
                        var dayWorks = new CashFlowContractDataViewModel
                        {
                            AttributeName = "Day Works",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            AttributeValue = detail.DayWorks,
                            Order = 6
                        };
                        var totalOriginalContract = new CashFlowContractDataViewModel
                        {
                            AttributeName = "Total Original Contract (with 15% VAT)",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            PlannedTotalAmount = contractData.TotalOriginalContractAmount,
                            AttributeValue = detail.TotalOriginalContractAmount,
                            Order = 7,
                            IsMainContent = true
                        };
                        var supplementaryAgreement = new CashFlowContractDataViewModel
                        {
                            AttributeName = "Supplementary Agreement",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            AttributeValue = detail.SupplementaryAgreement,
                            Order = 8
                        };
                        var variationWorks = new CashFlowContractDataViewModel
                        {
                            AttributeName = "Variation Works",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            AttributeValue = detail.VariationWorks,
                            Order = 9
                        };
                        var totalContractPrice = new CashFlowContractDataViewModel
                        {
                            AttributeName = "Total Contract Price (with 15% VAT)",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            PlannedTotalAmount = contractData.TotalContractAmount,
                            AttributeValue = detail.TotalContractAmount,
                            Order = 10,
                            IsMainContent = true
                        };
                        yearRecord.ContractDataList.Add(monthlyProgress);
                        yearRecord.ContractDataList.Add(cummulativeProgress);
                        yearRecord.ContractDataList.Add(condtractorAmount);
                        yearRecord.ContractDataList.Add(provisionalSum);
                        yearRecord.ContractDataList.Add(subContractWorks);
                        yearRecord.ContractDataList.Add(dayWorks);
                        yearRecord.ContractDataList.Add(totalOriginalContract);
                        yearRecord.ContractDataList.Add(supplementaryAgreement);
                        yearRecord.ContractDataList.Add(variationWorks);
                        yearRecord.ContractDataList.Add(totalContractPrice);
                    }
                }
                #endregion
                #region financial sources
                if (finaccialSource != null)
                {
                    var monthlyData = db.CashFlowFinanceSourceDetail.Where(d => d.CashFlowFinanceSourceId == finaccialSource.CashFlowFinanceSourceId).ToList();
                    foreach (CashFlowFinanceSourceDetail detail in monthlyData)
                    {
                        var mainContractAdvancePayment = new CashFlowFinancialSorceViewModel
                        {
                            AttributeName = "Main Contract Advance Payment",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            AttributeValue = detail.MainContractAdvancePayment,
                            Order = 1
                        };
                        var revenu = new CashFlowFinancialSorceViewModel
                        {
                            AttributeName = "Revenue / Income (Mon. Paym)",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            PlannedTotalAmount = finaccialSource.Revenue,
                            AttributeValue = detail.Revenue,
                            Order = 2
                        };

                        var suppliersCredit = new CashFlowFinancialSorceViewModel
                        {
                            AttributeName = "Suppliers' Credit",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            AttributeValue = detail.SuppliersCredit,
                            PlannedTotalAmount = finaccialSource.SuppliersCredit,
                            //TCPPercentage = contractData.TotalOriginalContractAmount > 0 ? (contractData.ContractorAmountWithVat / contractData.TotalOriginalContractAmount) * 100 : 0,
                            Order = 3
                        };
                        var subcontractorFinancing = new CashFlowFinancialSorceViewModel
                        {
                            AttributeName = "Sub Contractor Financing",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            AttributeValue = detail.SubContractorFinancing,
                            Order = 4
                        };
                        var subContractorRetentionWithholding = new CashFlowFinancialSorceViewModel
                        {
                            AttributeName = "Sub Contractor Retention Withheld",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            AttributeValue = detail.SubContractorRetentionWithholding,
                            Order = 5
                        };
                        var subContractorAdvanceRepayment = new CashFlowFinancialSorceViewModel
                        {
                            AttributeName = "Sub Contractor Advance Repayment",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            AttributeValue = detail.SubContractorAdvanceRepayment,
                            Order = 6
                        };
                        var headOfficeFinancing = new CashFlowFinancialSorceViewModel
                        {
                            AttributeName = "Head Office Financing",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            AttributeValue = detail.HeadOfficeFinancing,
                            Order = 7
                        };
                        var mainContractRetentionReleased = new CashFlowFinancialSorceViewModel
                        {
                            AttributeName = "Main Contract Retention Released ",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            AttributeValue = detail.MainContractRetentionReleased,
                            Order = 8
                        };
                        var materialOnSite = new CashFlowFinancialSorceViewModel
                        {
                            AttributeName = "Material on Site",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            AttributeValue = detail.MaterialOnSite,
                            Order = 9
                        };
                        var loan = new CashFlowFinancialSorceViewModel
                        {
                            AttributeName = "Overdraft / Loan",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            AttributeValue = detail.Loan,
                            Order = 10
                        };

                        yearRecord.FinancialSourceList.Add(mainContractAdvancePayment);
                        yearRecord.FinancialSourceList.Add(revenu);
                        yearRecord.FinancialSourceList.Add(suppliersCredit);
                        yearRecord.FinancialSourceList.Add(subcontractorFinancing);
                        yearRecord.FinancialSourceList.Add(subContractorAdvanceRepayment);
                        yearRecord.FinancialSourceList.Add(subContractorRetentionWithholding);
                        yearRecord.FinancialSourceList.Add(headOfficeFinancing);
                        yearRecord.FinancialSourceList.Add(mainContractRetentionReleased);
                        yearRecord.FinancialSourceList.Add(materialOnSite);
                        yearRecord.FinancialSourceList.Add(loan);
                    }
                }
                #endregion
                #region deduction
                if (deduction != null)
                {
                    var monthlyData = db.CashFlowDeductionDetail.Where(d => d.CashFlowDeductionId == deduction.CashFlowDeductionId).ToList();
                    foreach (CashFlowDeductionDetail detail in monthlyData)
                    {
                        var mainContractAdvanceRepayment = new CashFlowDeductionViewModel
                        {
                            AttributeName = "Main Contract Advance Repayment",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            AttributeValue = detail.MainContractAdvanceRepayment,
                            Order = 1
                        };
                        var incomeTaxes = new CashFlowDeductionViewModel
                        {
                            AttributeName = "Income Taxes (VAT or TOT)",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            PlannedTotalAmount = deduction.IncomeTaxes,
                            AttributeValue = detail.IncomeTaxes,
                            Order = 2
                        };

                        var mainContractRetentionWithheld = new CashFlowDeductionViewModel
                        {
                            AttributeName = "Main Contract Retention Withheld",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            AttributeValue = detail.MainContractRetentionWithheld,
                            //PlannedTotalAmount = finaccialSource.SuppliersCredit,
                            //TCPPercentage = contractData.TotalOriginalContractAmount > 0 ? (contractData.ContractorAmountWithVat / contractData.TotalOriginalContractAmount) * 100 : 0,
                            Order = 3
                        };
                        var rebate = new CashFlowDeductionViewModel
                        {
                            AttributeName = "Rebate",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            AttributeValue = detail.Rebate,
                            Order = 4
                        };
                        var ommissionWorks = new CashFlowDeductionViewModel
                        {
                            AttributeName = "Ommission Works",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            AttributeValue = detail.OmmissionWorks,
                            Order = 5
                        };
                        var others = new CashFlowDeductionViewModel
                        {
                            AttributeName = "Others",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            AttributeValue = detail.Others,
                            Order = 6
                        };
                        yearRecord.DeductionList.Add(mainContractAdvanceRepayment);
                        yearRecord.DeductionList.Add(incomeTaxes);
                        yearRecord.DeductionList.Add(mainContractRetentionWithheld);
                        yearRecord.DeductionList.Add(rebate);
                        yearRecord.DeductionList.Add(ommissionWorks);
                        yearRecord.DeductionList.Add(others);
                    }
                }
                #endregion
                #region net operational cash flow
                if (finaccialSource != null && deduction != null)
                {
                    var monthlyDeduction = db.CashFlowDeductionDetail.Where(d => d.CashFlowDeductionId == deduction.CashFlowDeductionId).ToList();
                    var monthlySource = db.CashFlowFinanceSourceDetail.Where(d => d.CashFlowFinanceSourceId == finaccialSource.CashFlowFinanceSourceId).ToList();
                    foreach (CashFlowFinanceSourceDetail source in monthlySource)
                    {
                        var valueTobeDeducted = monthlyDeduction.FirstOrDefault(d => d.ConstructionPeriodId == source.ConstructionPeriodId);
                        if(valueTobeDeducted != null)
                        {
                            var netOperation = new CashFlowNetOperationViewModel
                            {
                                AttributeName = "Cum. Net Operational Cash Flow",
                                Month = source.ConstructionPeriod.Name,
                                MonthEnd = source.ConstructionPeriod.DateTo,
                                PlannedTotalAmount = finaccialSource.TotalFinanceSource - deduction.TotalDeduction,
                                AttributeValue = source.TotalFinanceSource - valueTobeDeducted.TotalDeduction,
                            };
                            yearRecord.NetOperationList.Add(netOperation);
                        }
                    }
                }
                #endregion
                #region costs/expenses
                if(cost != null)
                {
                    var costsMonthyData = db.NetOperationCashFlowDetail.Where(c => c.NetOperationCashFlowId == cost.NetOperationCashFlowId).ToList();
                    foreach(NetOperationCashFlowDetail detail in costsMonthyData)
                    {
                        var directMaterialExpense = new CashFlowExpenseViewModel
                        {
                            AttributeName = "Direct Material Expenses",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            PlannedTotalAmount = cost.DirectMaterialExpense,
                            AttributeValue = detail.DirectMaterialExpense,
                            Order = 1
                        };
                        var directAndIndirectLabor = new CashFlowExpenseViewModel
                        {
                            AttributeName = "Direct+indirect  Labor Expenses/project",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            PlannedTotalAmount = cost.DirectAndIndirectlaborExpense,
                            AttributeValue = detail.DirectAndIndirectlaborExpense,
                            Order = 2
                        };

                        var directEquipment = new CashFlowExpenseViewModel
                        {
                            AttributeName = "Direct Equipment Expenses",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            AttributeValue = detail.DirectEquipmentCost,
                            PlannedTotalAmount = cost.DirectEquipmentCost,
                            //TCPPercentage = contractData.TotalOriginalContractAmount > 0 ? (contractData.ContractorAmountWithVat / contractData.TotalOriginalContractAmount) * 100 : 0,
                            Order = 3
                        };
                        var projectOverHead = new CashFlowExpenseViewModel
                        {
                            AttributeName = "Project Site Overhead /Miscellaneous +contingency Expenses",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            AttributeValue = detail.ProjectOverheadExpense,
                            Order = 4
                        };
                        var headOfficeOverHead = new CashFlowExpenseViewModel
                        {
                            AttributeName = "Head Office Overhead Expenses",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            AttributeValue = detail.HeadOfficeOverheadExpense,
                            Order = 5
                        };
                        var subContractWorkAdvance = new CashFlowExpenseViewModel
                        {
                            AttributeName = "Sub Contract Works Advance",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            AttributeValue = detail.SubContractWorksAdvance,
                            Order = 6
                        };
                        var subContractSettlement = new CashFlowExpenseViewModel
                        {
                            AttributeName = "Sub Contract /Piece rate Works Settlement",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            AttributeValue = detail.SubContractPiecerateWorksSettlement,
                            Order = 7
                        };
                        var headOfficeFinancingSettelement = new CashFlowExpenseViewModel
                        {
                            AttributeName = "Head Office Financing Settlement",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            AttributeValue = detail.HeadOfficeFinancingSettlement,
                            Order = 8
                        };
                        var suppliersCreditSettelement = new CashFlowExpenseViewModel
                        {
                            AttributeName = "Suppliers' Credit Settlement",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            AttributeValue = detail.SuppliersCreditSettlement,
                            Order = 9
                        };
                        var dayWorksSettelement = new CashFlowExpenseViewModel
                        {
                            AttributeName = "Day Works Settlement",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            AttributeValue = detail.DayWorksSettlement,
                            Order = 10
                        };

                        var taxSettelement = new CashFlowExpenseViewModel
                        {
                            AttributeName = "Tax's Settlement",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            AttributeValue = detail.TaxSettlement,
                            Order = 11
                        };
                        var loanSettelement = new CashFlowExpenseViewModel
                        {
                            AttributeName = "Overdraft / Loan Settlement",
                            Month = detail.ConstructionPeriod.Name,
                            MonthEnd = detail.ConstructionPeriod.DateTo,
                            AttributeValue = detail.LoanSettlement,
                            Order = 12
                        };
                        yearRecord.ExpenseList.Add(directMaterialExpense);
                        yearRecord.ExpenseList.Add(directAndIndirectLabor);
                        yearRecord.ExpenseList.Add(directEquipment);
                        yearRecord.ExpenseList.Add(projectOverHead);
                        yearRecord.ExpenseList.Add(headOfficeOverHead);
                        yearRecord.ExpenseList.Add(subContractWorkAdvance);
                        yearRecord.ExpenseList.Add(subContractSettlement);
                        yearRecord.ExpenseList.Add(headOfficeFinancingSettelement);
                        yearRecord.ExpenseList.Add(suppliersCreditSettelement);
                        yearRecord.ExpenseList.Add(dayWorksSettelement);
                        yearRecord.ExpenseList.Add(taxSettelement);
                        yearRecord.ExpenseList.Add(loanSettelement);
                    }
                }
                #endregion
                #region cash flow balance
                if(finaccialSource != null && deduction != null && cost != null)
                {
                    var monthlyDeduction = db.CashFlowDeductionDetail.Where(d => d.CashFlowDeductionId == deduction.CashFlowDeductionId).ToList();
                    var monthlySource = db.CashFlowFinanceSourceDetail.Where(d => d.CashFlowFinanceSourceId == finaccialSource.CashFlowFinanceSourceId).ToList();
                    var monthlyExpense = db.NetOperationCashFlowDetail.Where(d => d.NetOperationCashFlowId == cost.NetOperationCashFlowId).ToList();

                    foreach (var source in monthlySource)
                    {
                        var deductionItem = monthlyDeduction.FirstOrDefault(d => d.ConstructionPeriodId == source.ConstructionPeriodId);
                        var costItem = monthlyExpense.FirstOrDefault(ex => ex.ConstructionPeriodId == source.ConstructionPeriodId);
                        if (deductionItem != null && costItem != null)
                        {
                            var cashFlowBalance = new CashFlowBalanceViewModel
                            {
                                AttributeName = "Cash Flow Balance",
                                Month = source.ConstructionPeriod.Name,
                                MonthEnd = source.ConstructionPeriod.DateTo,
                                PlannedTotalAmount = (finaccialSource.TotalFinanceSource - deduction.TotalDeduction) - cost.TotalExpense,
                                AttributeValue = (source.TotalFinanceSource - deductionItem.TotalDeduction) - costItem.TotalExpense
                            };
                            yearRecord.BalanceList.Add(cashFlowBalance);
                        }
                    }
                }
                #endregion
                list.Add(yearRecord);
            }
            return list;

        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<CashFlowYearReportParameterViewModel> GetCashFlowYears(string projectId)
        {
            List<CashFlowYearReportParameterViewModel> list = new List<CashFlowYearReportParameterViewModel>();
            if (projectId == null)
            {
                return null;
            }
            int id = Convert.ToInt32(projectId);
            var currentProject = db.ProjectInformation.Find(id);
            if (currentProject != null)
            {
                var years = db.CashFlowYear.Where(y => y.ProjectInformationId == currentProject.ProjectInformationId).ToList();
                foreach (CashFlowYear year in years)
                {
                    var yearRecord = new CashFlowYearReportParameterViewModel
                    {
                        Id = year.CashFlowYearId,
                        Name = year.GlFiscalYear.Name
                    };
                    list.Add(yearRecord);
                }
            }
            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ProjectHeaderVM GetProjectHeader(string projectId)
        {
            int id = Convert.ToInt32(projectId);
            var item = db.ProjectInformation.Find(id);
            ProjectHeaderVM headerDs = new ProjectHeaderVM();
            var organization = db.Companies.FirstOrDefault();
            if (item != null)
            {
                headerDs.ProjectName = item.Name;

            }

            if (organization != null)
            {
                headerDs.Name = organization.TradeName;
                headerDs.AmharicName = organization.AmharicName;
                var address =
                    db.Addresses.Where(
                        a =>
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Count > 0)
                {
                    foreach (var a in address)
                    {

                        if (headerDs.Address == null)
                        {
                            headerDs.Address = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            headerDs.Address += ", " + a.Category + "-" + a.Value;
                        }

                    }

                }
                var logo =
                    db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    headerDs.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }

            return headerDs;
        }
    }
}
