using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.JobCosting;
using ExceedERP.Core.Domain.JobCosting.Setting;
using ExceedERP.Core.Domain.ProductionFollowUp;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.DataAccess.Context
{
   public class JobCostingEntityConfiguration
    {
        public class DailyLaborRateEntityTypeConfiguration : EntityTypeConfiguration<DailyLaborRate>
        {
            public DailyLaborRateEntityTypeConfiguration()
            {
                this.ToTable("DailyLaborRates", "JobCosting");
                this.HasKey<int>(t => t.DailyLaborRateId);
            }
        }
        public class DLRSalaryEntityTypeConfiguration : EntityTypeConfiguration<DLRSalary>
        {
            public DLRSalaryEntityTypeConfiguration()
            {
                this.ToTable("DLRSalaries", "JobCosting");
                this.HasKey<int>(t => t.DLRSalaryId);
            }
        }
        public class DLRHourEntityTypeConfiguration : EntityTypeConfiguration<DLRHour>
        {
            public DLRHourEntityTypeConfiguration()
            {
                this.ToTable("DLRHours", "JobCosting");
                this.HasKey<int>(t => t.DLRHourId);
            }
        }
        public class JobCostEntityTypeConfiguration : EntityTypeConfiguration<JobCost>
        {
            public JobCostEntityTypeConfiguration()
            {
                this.ToTable("JobCosts", "JobCosting");
                this.HasKey<int>(t => t.JobCostId);
            }
        }
        public class JobChangeOfOrderEntityTypeConfiguration : EntityTypeConfiguration<JobChangeOfOrder>
        {
            public JobChangeOfOrderEntityTypeConfiguration()
            {
                this.ToTable("JobChangeOfOrders", "JobCosting");
                this.HasKey<int>(t => t.JobChangeOfOrderId);
            }
        }
        public class JobLaborCostEntityTypeConfiguration : EntityTypeConfiguration<JobLaborCost>
        {
            public JobLaborCostEntityTypeConfiguration()
            {
                this.ToTable("JobLaborCosts", "JobCosting");
                this.HasKey<int>(t => t.JobLaborCostId);
            }
        }
        public class JobMaterialCostEntityTypeConfiguration : EntityTypeConfiguration<JobMaterialCost>
        {
            public JobMaterialCostEntityTypeConfiguration()
            {
                this.ToTable("JobMaterialCosts", "JobCosting");
                this.HasKey<int>(t => t.JobMaterialCostId);
            }
        }
        public class JobMaterialreturnEntityTypeConfiguration : EntityTypeConfiguration<JobMaterialReturn>
        {
            public JobMaterialreturnEntityTypeConfiguration()
            {
                this.ToTable("JobMaterialReturns", "JobCosting");
                this.HasKey<int>(t => t.JobMaterialReturnId);
            }
        }
        public class FinishedJobEntityTypeConfiguration : EntityTypeConfiguration<FinishedJob>
        {
            public FinishedJobEntityTypeConfiguration()
            {
                this.ToTable("FinishedJobs", "JobCosting");
                this.HasKey<int>(t => t.FinishedJobId);
            }
        }
        public class DeliveredJobEntityTypeConfiguration : EntityTypeConfiguration<DeliveredJob>
        {
            public DeliveredJobEntityTypeConfiguration()
            {
                this.ToTable("DeliveredJobs", "JobCosting");
                this.HasKey<int>(t => t.DeliveredJobId);
            }
        }
        public class IdleTimeEntityTypeConfiguration : EntityTypeConfiguration<IdleTime>
        {
            public IdleTimeEntityTypeConfiguration()
            {
                this.ToTable("IdleTimes", "JobCosting");
                this.HasKey<int>(t => t.IdleTimeID);
            }
        }
        public class IdleTimeOTEntityTypeConfiguration : EntityTypeConfiguration<IdleTimeOT>
        {
            public IdleTimeOTEntityTypeConfiguration()
            {
                this.ToTable("IdleTimeOTs", "JobCosting");
                this.HasKey<int>(t => t.IdleTimeOTID);
            }
        }
        public class IdleTimeProductiveEntityTypeConfiguration : EntityTypeConfiguration<IdleTimeProductive>
        {
            public IdleTimeProductiveEntityTypeConfiguration()
            {
                this.ToTable("IdleTimeProductives", "JobCosting");
                this.HasKey<int>(t => t.IdleTimeProductiveID);
            }
        }
        public class IdleTimeNonProductiveControllableEntityTypeConfiguration : EntityTypeConfiguration<IdleTimeNonProductiveControllable>
        {
            public IdleTimeNonProductiveControllableEntityTypeConfiguration()
            {
                this.ToTable("IdleTimeNonProductives", "JobCosting");
                this.HasKey<int>(t => t.IdleTimeNonProductiveControllableID);
            }
        }
        public class IdleTimeNonProductiveNonControllableEntityTypeConfiguration : EntityTypeConfiguration<IdleTimeNonProductiveNonControllable>
        {
            public IdleTimeNonProductiveNonControllableEntityTypeConfiguration()
            {
                this.ToTable("IdleTimeNonProductiveNonControllables", "JobCosting");
                this.HasKey<int>(t => t.IdleTimeNonProductiveNonControllableID);
            }
        }
        //public class GLChartOfAccountCostCenterEntityTypeConfiguration : EntityTypeConfiguration<GLChartOfAccountCostCenter>
        //{
        //    public GLChartOfAccountCostCenterEntityTypeConfiguration()
        //    {
        //        this.ToTable("GLChartOfAccountCostCentres", "JobCosting");
        //        this.HasKey<int>(t => t.CostCenterID);
        //    }
        //}
        //public class GLChartOfAccountAccountEntityTypeConfiguration : EntityTypeConfiguration<GLChartOfAccountAccount>
        //{
        //    public GLChartOfAccountAccountEntityTypeConfiguration()
        //    {
        //        this.ToTable("GLChartOfAccountAccounts", "JobCosting");
        //        this.HasKey<int>(t => t.GLChartOfAccountAccountID);
        //    }
        //}
        //public class GLFiscalYearEntityTypeConfiguration : EntityTypeConfiguration<GlFiscalYear>
        //{
        //    public GLFiscalYearEntityTypeConfiguration()
        //    {
        //        this.ToTable("GLFiscalYears", "JobCosting");
        //        this.HasKey<int>(t => t.GlFiscalYearId);
        //    }
        //}
        //public class GLPeriodEntityTypeConfiguration : EntityTypeConfiguration<GLPeriod>
        //{
        //    public GLPeriodEntityTypeConfiguration()
        //    {
        //        this.ToTable("GLPeriods", "JobCosting");
        //        this.HasKey<int>(t => t.GLPeriodId);
        //    }
        //}
        //public class GLTaxRateEntityTypeConfiguration : EntityTypeConfiguration<GLTaxRate>
        //{
        //    public GLTaxRateEntityTypeConfiguration()
        //    {
        //        this.ToTable("GLTaxRates", "JobCosting");
        //        this.HasKey<int>(t => t.GLTaxRateID);
        //    }
        //}
        //public class PensionEntityTypeConfiguration : EntityTypeConfiguration<Pension>
        //{
        //    public PensionEntityTypeConfiguration()
        //    {
        //        this.ToTable("Pensions", "JobCosting");
        //        this.HasKey<int>(t => t.PensionId);
        //    }
        //}
        public class YearlyOverHeadRateEntityTypeConfiguration : EntityTypeConfiguration<YearlyOverHeadRate>
        {
            public YearlyOverHeadRateEntityTypeConfiguration()
            {
                this.ToTable("YearlyOverHeadRates", "JobCosting");
                this.HasKey<int>(t => t.YearlyOverHeadRateId);
            }
        }
        //public class MachineTypeEntityTypeConfiguration : EntityTypeConfiguration<MachineType>
        //{
        //    public MachineTypeEntityTypeConfiguration()
        //    {
        //        this.ToTable("MachineTypes", "JobCosting");
        //        this.HasKey<int>(t => t.MachineTypeId);
        //    }
        //}
        //public class MainCompanyEntityTypeConfiguration : EntityTypeConfiguration<MainCompany>
        //{
        //    public MainCompanyEntityTypeConfiguration()
        //    {
        //        this.ToTable("MainCompanies", "JobCosting");
        //        this.HasKey<int>(t => t.MainCompanyId);
        //    }
        //}
        public class MachineEntityTypeConfiguration : EntityTypeConfiguration<Machine>
        {
            public MachineEntityTypeConfiguration()
            {
                this.ToTable("Machines", "JobCosting");
                this.HasKey<int>(t => t.MachineId);
            }
        }
        public class UnitEntityTypeConfiguration : EntityTypeConfiguration<Unit>
        {
            public UnitEntityTypeConfiguration()
            {
                this.ToTable("Units", "JobCosting");
                this.HasKey<int>(t => t.UnitId);
            }
        }

        public class JobCategoryEntityTypeConfiguration : EntityTypeConfiguration<JobCategory>
        {
            public JobCategoryEntityTypeConfiguration()
            {
                this.ToTable("JobCategories", "JobCosting");
                this.HasKey<int>(t => t.JobCategoryId);
            }
        }  public class IdleTimeCategoryEntityTypeConfiguration : EntityTypeConfiguration<IdleTimeCategory>
        {
            public IdleTimeCategoryEntityTypeConfiguration()
            {
                this.ToTable("IdleTimeCategories", "JobCosting");
                this.HasKey<int>(t => t.IdleTimeCategoryId);
            }
        }
        public class JobOrderTimeRegistrationEntityTypeConfiguration : EntityTypeConfiguration<JobOrderTimeRegistration>
        {
            public JobOrderTimeRegistrationEntityTypeConfiguration()
            {
                this.ToTable("JobOrderTimeRegistration", "JobCosting");
                this.HasKey<int>(t => t.JobOrderTimeRegistrationId);
            }
        }
        public class JobOrderTimeSheetEntityTypeConfiguration : EntityTypeConfiguration<JobOrderTimeSheet>
        {
            public JobOrderTimeSheetEntityTypeConfiguration()
            {
                this.ToTable("JobOrderTimeSheet", "JobCosting");
                this.HasKey<int>(t => t.JobOrderTimeSheetId);
            }
        }
        public class JobOrderSummaryEntityTypeConfiguration : EntityTypeConfiguration<JobOrderSummary>
        {
            public JobOrderSummaryEntityTypeConfiguration()
            {
                this.ToTable("JobOrderSummary", "JobCosting");
                this.HasKey<int>(t => t.JobOrderSummaryId);
            }
        }
        public class JobOrderSummaryCostCenterEntityTypeConfiguration : EntityTypeConfiguration<JobOrderSummaryCostCenter>
        {
            public JobOrderSummaryCostCenterEntityTypeConfiguration()
            {
                this.ToTable("JobOrderSummaryCostCenter", "JobCosting");
                this.HasKey<int>(t => t.JobOrderSummaryCostCenterId);
            }
        }
        public class JobOrderSummaryCostCenterDetailEntityTypeConfiguration : EntityTypeConfiguration<JobOrderSummaryCostCenterDetail>
        {
            public JobOrderSummaryCostCenterDetailEntityTypeConfiguration()
            {
                this.ToTable("JobOrderSummaryCostCenterDetail", "JobCosting");
                this.HasKey<int>(t => t.JobOrderSummaryCostCenterDetailId);
            }
        }
    }
}
