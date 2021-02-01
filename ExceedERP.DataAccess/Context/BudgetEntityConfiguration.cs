using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Budget.Task;
using ExceedERP.Core.Domain.Budget.GAAP;

namespace ExceedERP.DataAccess.Context
{
        public  class BudgetEntityConfiguration
        {
            #region GAAP Budget Config

      

            public class BalanceSheetEntityTypeConfiguration : EntityTypeConfiguration<BalanceSheet>
            {

                public BalanceSheetEntityTypeConfiguration()
                {
                    this.ToTable("BalanceSheet", "BudgetGAAP");
                    this.HasKey<int>(t => t.BalanceSheetId);

                }
            }

            public class BudgetSheetCategoryEntityTypeConfiguration : EntityTypeConfiguration<BudgetSheetCategory>
            {

                public BudgetSheetCategoryEntityTypeConfiguration()
                {
                    this.ToTable("BudgetSheetCategory", "BudgetGAAP");
                    this.HasKey<int>(t => t.BalanceSheetCategoryId);

                }
            }

            public class CostOfServiceSoldEntityTypeConfiguration : EntityTypeConfiguration<CostOfServiceSold>
            {

                public CostOfServiceSoldEntityTypeConfiguration()
                {
                    this.ToTable("CostOfServiceSold", "BudgetGAAP");
                    this.HasKey<int>(t => t.CostOfServiceSoldId);

                }
            }

            public class ProfitLossStatementCategoryEntityTypeConfiguration : EntityTypeConfiguration<ProfitLossStatementCategory>
            {

                public ProfitLossStatementCategoryEntityTypeConfiguration()
                {
                    this.ToTable("ProfitLossStatementCategory", "BudgetGAAP");
                    this.HasKey<int>(t => t.ProfitLossStatementCategoryId);

                }
            }

            public class ProfitLossStatementEntityTypeConfiguration : EntityTypeConfiguration<ProfitLossStatement>
            {

                public ProfitLossStatementEntityTypeConfiguration()
                {
                    this.ToTable("ProfitLossStatement", "BudgetGAAP");
                    this.HasKey<int>(t => t.ProfitLossStatementId);

                }
            }
            public class InvestmentPlanEntityTypeConfiguration : EntityTypeConfiguration<InvestmentPlan>
            {

                public InvestmentPlanEntityTypeConfiguration()
                {
                    this.ToTable("InvestmentPlan", "BudgetGAAP");
                    this.HasKey<int>(t => t.InvestmentPlanId);

                }
            }

            public class ForcastedIncomeStatementEntityTypeConfiguration : EntityTypeConfiguration<ForcastedIncomeStatement>
            {

                public ForcastedIncomeStatementEntityTypeConfiguration()
                {
                    this.ToTable("ForcastedIncomeStatement", "BudgetGAAP");
                    this.HasKey<int>(t => t.ForcastedIncomeStatementId);

                }
            }

            public class ExpenditureEntityTypeConfiguration : EntityTypeConfiguration<Expenditure>
            {

                public ExpenditureEntityTypeConfiguration()
                {
                    this.ToTable("Expenditure", "BudgetGAAP");
                    this.HasKey<int>(t => t.ExpenditureId);

                }
            }

            #endregion


            
            public class BudgetBankAccountMapEntityTypeConfiguration : EntityTypeConfiguration<BudgetBankAccountMap>
            {

                public BudgetBankAccountMapEntityTypeConfiguration()
                {
                    this.ToTable("BudgetBankAccountMap", "Budget");
                    this.HasKey<int>(t => t.BudgetBankAccountMapId);

                }
            }
                 public class BudgetTypeEntityTypeConfiguration : EntityTypeConfiguration<BudgetType>
            {

                public BudgetTypeEntityTypeConfiguration()
                {
                    this.ToTable("BudgetType", "Budget");
                    this.HasKey<int>(t => t.BudgetTypeId);

                }
            }
            public class BudgetRequestTransferDistributionEntityTypeConfiguration : EntityTypeConfiguration<BudgetRequestTransferDistribution>
            {

                public BudgetRequestTransferDistributionEntityTypeConfiguration()
                {
                    this.ToTable("BudgetRequestTransferDistribution", "Budget");
                    this.HasKey<int>(t => t.BudgetRequestTransferDistributionId);

                }
            }

            public class BudgetRequestTransferLogEntityTypeConfiguration : EntityTypeConfiguration<BudgetRequestTransferLog>
            {

                public BudgetRequestTransferLogEntityTypeConfiguration()
                {
                    this.ToTable("BudgetRequestTransferLog", "Budget");
                    this.HasKey<int>(t => t.BudgetRequestTransferLogId);

                }
            }



            //public class BudgetTypeEntityTypeConfiguration : EntityTypeConfiguration<BudgetType>
            //{

            //    public BudgetTypeEntityTypeConfiguration()
            //    {
            //        this.ToTable("BudgetType", "Budget");
            //        this.HasKey<int>(t => t.BudgetTypeId);

            //    }
            //}


            public class TaskPlanEntityTypeConfiguration : EntityTypeConfiguration<TaskPlan>
            {

                public TaskPlanEntityTypeConfiguration()
                {
                    this.ToTable("TaskPlan", "Budget");
                    this.HasKey<int>(t => t.TaskPlanId);

                }
            }
            public class TaskCategoryEntityTypeConfiguration : EntityTypeConfiguration<TaskCategory>
            {

                public TaskCategoryEntityTypeConfiguration()
                {
                    this.ToTable("TaskCategory", "Budget");
                    this.HasKey<int>(t => t.TaskCategoryId);

                }
            }
            public class TaskUnitEntityTypeConfiguration : EntityTypeConfiguration<TaskUnit>
            {

                public TaskUnitEntityTypeConfiguration()
                {
                    this.ToTable("TaskUnit", "Budget");
                    this.HasKey<int>(t => t.TaskUnitId);

                }
            }

            public class TaskPerformanceEntityTypeConfiguration : EntityTypeConfiguration<TaskPerformance>
            {

                public TaskPerformanceEntityTypeConfiguration()
                {
                    this.ToTable("TaskPerformance", "Budget");
                    this.HasKey<int>(t => t.TaskPerformanceId);

                }
            }
            public class TaskPerformaceApprovalEntityTypeConfiguration : EntityTypeConfiguration<TaskPerformaceApproval>
            {

                public TaskPerformaceApprovalEntityTypeConfiguration()
                {
                    this.ToTable("TaskPerformaceApproval", "Budget");
                    this.HasKey<int>(t => t.TaskPerformaceApprovalId);

                }
            }

            public class TaskPlanMonthlyEntityTypeConfiguration : EntityTypeConfiguration<TaskPlanMonthly>
            {

                public TaskPlanMonthlyEntityTypeConfiguration()
                {
                    this.ToTable("TaskPlanMonthly", "Budget");
                    this.HasKey<int>(t => t.TaskPlanMonthlyId);

                }
            }
            public class TaskPlanApprovalEntityTypeConfiguration : EntityTypeConfiguration<TaskPlanApproval>
            {

                public TaskPlanApprovalEntityTypeConfiguration()
                {
                    this.ToTable("TaskPlanApproval", "Budget");
                    this.HasKey<int>(t => t.TaskPlanApprovalId);

                }
            }

    

            public class PlanCategoryEntityTypeConfiguration : EntityTypeConfiguration<PlanCategory>
            {

                public PlanCategoryEntityTypeConfiguration()
                {
                    this.ToTable("PlanCategory", "Budget");
                    this.HasKey<int>(t => t.PlanCategoryID);

                }
            }

            public class BudgetControlEntityTypeConfiguration : EntityTypeConfiguration<BudgetControl>
            {

                public BudgetControlEntityTypeConfiguration()
                {
                    this.ToTable("BudgetControl", "Budget");
                    this.HasKey<int>(t => t.BudgetControlId);

                }
            }
            public class BudgetRequestLineEntityTypeConfiguration : EntityTypeConfiguration<BudgetRequestLine>
            {

                public BudgetRequestLineEntityTypeConfiguration()
                {
                    this.ToTable("BudgetRequestLine", "Budget");
                    this.HasKey<int>(t => t.BudgetRequestLineId);

                }
            }
            public class RequestedMonthEntityTypeConfiguration : EntityTypeConfiguration<RequestedMonth>
            {

                public RequestedMonthEntityTypeConfiguration()
                {
                    this.ToTable("RequestedMonth", "Budget");
                    this.HasKey<int>(t => t.RequestedMonthID);
                   

                }
            }
      
            public class BudgetPlanEntityTypeConfiguration : EntityTypeConfiguration<BudgetPlan>
        {

            public BudgetPlanEntityTypeConfiguration()
            {
                this.ToTable("BudgetPlan", "Budget");
                this.HasKey<int>(t => t.BudgetPlanID);

            }
        }


            public class BudgetAllocationEntityTypeConfiguration : EntityTypeConfiguration<BudgetAllocation>
            {

                public BudgetAllocationEntityTypeConfiguration()
                {
                    this.ToTable("BudgetAllocation", "Budget");
                    this.HasKey<int>(t => t.BudgetAllocationID);

                }
            }


            public class BudgetAllocationApprovalEntityTypeConfiguration : EntityTypeConfiguration<BudgetAllocationApproval>
            {

                public BudgetAllocationApprovalEntityTypeConfiguration()
                {
                    this.ToTable("BudgetAllocationApproval", "Budget");
                    this.HasKey<int>(t => t.BudgetAllocationApprovalId);

                }
            }

            public class BudgetCategoryEntityTypeConfiguration : EntityTypeConfiguration<BudgetCategory>
            {

                public BudgetCategoryEntityTypeConfiguration()
                {
                    this.ToTable("BudgetCategory", "Budget");
                    this.HasKey<int>(t => t.BudgetCategoryID);

                }
            }

            public class BudgetCurrencyEntityTypeConfiguration : EntityTypeConfiguration<BudgetCurrency>
            {

                public BudgetCurrencyEntityTypeConfiguration()
                {
                    this.ToTable("BudgetCurrency", "Budget");
                    this.HasKey<int>(t => t.BudgetCurrencyId);

                }
            }

            public class BudgetPlanApprovalEntityTypeConfiguration : EntityTypeConfiguration<BudgetPlanApproval>
            {

                public BudgetPlanApprovalEntityTypeConfiguration()
                {
                    this.ToTable("BudgetPlanApproval", "Budget");
                    this.HasKey<int>(t => t.BudgetPlanApprovalId);

                }
            }

            public class BudgetPlanMonthlyEntityTypeConfiguration : EntityTypeConfiguration<BudgetPlanMonthly>
            {

                public BudgetPlanMonthlyEntityTypeConfiguration()
                {
                    this.ToTable("BudgetPlanMonthly", "Budget");
                    this.HasKey<int>(t => t.BudgetPlanMonthlyID);
                    this.HasRequired<BudgetPlan>(x => x.BudgetPlan).WithMany().WillCascadeOnDelete(false);
                }
            }

            public class BudgetRequestEntityTypeConfiguration : EntityTypeConfiguration<BudgetRequest>
            {

                public BudgetRequestEntityTypeConfiguration()
                {
                    this.ToTable("BudgetRequest", "Budget");
                    this.HasKey<int>(t => t.BudgetRequestID);

                }
            }

            public class BudgetRequestApprovalEntityTypeConfiguration : EntityTypeConfiguration<BudgetRequestApproval>
            {

                public BudgetRequestApprovalEntityTypeConfiguration()
                {
                    this.ToTable("BudgetRequestApproval", "Budget");
                    this.HasKey<int>(t => t.BudgetRequestApprovalId);

                }
            }

            public class BudgetTransferEntityTypeConfiguration : EntityTypeConfiguration<BudgetTransfer>
            {

                public BudgetTransferEntityTypeConfiguration()
                {
                    this.ToTable("BudgetTransfer", "Budget");
                    this.HasKey<int>(t => t.BudgetTransferID);

                }
            }

            public class BudgetTransferApprovalEntityTypeConfiguration : EntityTypeConfiguration<BudgetTransferApproval>
            {

                public BudgetTransferApprovalEntityTypeConfiguration()
                {
                    this.ToTable("BudgetTransferApproval", "Budget");
                    this.HasKey<int>(t => t.BudgetTransferApprovalId);

                }
            }
            public class BudgetUsageEntityTypeConfiguration : EntityTypeConfiguration<BudgetUsage>
            {

                public BudgetUsageEntityTypeConfiguration()
                {
                    this.ToTable("BudgetUsage", "Budget");
                    this.HasKey<int>(t => t.BudgetUsageID);

                }
            }
            public class BudgetUsageApprovalEntityTypeConfiguration : EntityTypeConfiguration<BudgetUsageApproval>
            {

                public BudgetUsageApprovalEntityTypeConfiguration()
                {
                    this.ToTable("BudgetUsageApproval", "Budget");
                    this.HasKey<int>(t => t.BudgetUsageApprovalId);

                }
            }

          
    }
}
