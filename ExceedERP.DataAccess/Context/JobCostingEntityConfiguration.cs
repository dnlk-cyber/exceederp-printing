using ExceedERP.Core.Domain.JobCosting;
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
