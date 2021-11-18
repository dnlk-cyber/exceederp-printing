using ExceedERP.Core.Domain.PrintingEstimation;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.DataAccess.Context
{
    class OverallCostEntityConfiguration : EntityTypeConfiguration<OverallCost>
    {
        public OverallCostEntityConfiguration()
        {
            this.ToTable("OverallCosts", "PrintingEstimation");
            this.HasKey<int>(t => t.OverallCostId);
        }
    }
    class EstimationFormEntityConfiguration : EntityTypeConfiguration<EstimationForm>
    {
        public EstimationFormEntityConfiguration()
        {
            this.ToTable("EstimationForms", "PrintingEstimation");
            this.HasKey<int>(t => t.EstimationFormId);
        }
    }
    class MaterialCostEntityConfiguration : EntityTypeConfiguration<MaterialCost>
    {
        public MaterialCostEntityConfiguration()
        {
            this.ToTable("MaterialCosts", "PrintingEstimation");
            this.HasKey<int>(t => t.MaterialCostId);
        }
    }

    class LaborCostEntityConfiguration : EntityTypeConfiguration<LaborCost>
    {
        public LaborCostEntityConfiguration()
        {
            this.ToTable("LaborCosts", "PrintingEstimation");
            this.HasKey<int>(t => t.LaborCostId);
        }
    }
   
    class PrintingStepEntityConfiguration : EntityTypeConfiguration<PrintingStep>
    {
        public PrintingStepEntityConfiguration()
        {
            this.ToTable("PrintingSteps", "PrintingEstimation");
            this.HasKey<int>(t => t.PrintingStepId);
        }
    }
    
}
