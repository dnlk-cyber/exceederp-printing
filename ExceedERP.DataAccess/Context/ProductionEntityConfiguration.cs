using ExceedERP.Core.Domain.Production;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.DataAccess.Context
{

    #region project information
    public class ProductionProjectEntityTypeConfiguration : EntityTypeConfiguration<ProductionProject>
    {
        public ProductionProjectEntityTypeConfiguration()
        {
            this.ToTable("ProductionProject", "Production");
            this.HasKey<int>(o => o.ProductionProjectId);
        }
    }
    public class ProductionSiteEntityTypeConfiguration : EntityTypeConfiguration<ProductionSite>
    {
        public ProductionSiteEntityTypeConfiguration()
        {
            this.ToTable("ProductionSite", "Production");
            this.HasKey<int>(o => o.ProductionSiteId);
        }
    }
    public class PlotEntityTypeConfiguration : EntityTypeConfiguration<Plot>
    {
        public PlotEntityTypeConfiguration()
        {
            this.ToTable("Plot", "Production");
            this.HasKey<int>(o => o.PlotId);
        }
    }
    #endregion

    #region Settings
    public class ProductionPeriodEntityTypeConfiguration : EntityTypeConfiguration<ProductionPeriod>
    {
        public ProductionPeriodEntityTypeConfiguration()
        {
            this.ToTable("ProductionPeriod", "Production");
            this.HasKey<int>(o => o.ProductionPeriodId);
        }
    }
    public class ProductionYearEntityTypeConfiguration : EntityTypeConfiguration<ProductionYear>
    {
        public ProductionYearEntityTypeConfiguration()
        {
            this.ToTable("ProductionYear", "Production");
            this.HasKey<int>(o => o.ProductionYearId);
        }
    }

    public class ProductionSettingEntityTypeConfiguration : EntityTypeConfiguration<ProductionSetting>
    {
        public ProductionSettingEntityTypeConfiguration()
        {
            this.ToTable("ProductionSetting", "Production");
            this.HasKey<int>(o => o.ProductionSettingId);
        }
    }
    public class CropEntityTypeConfiguration : EntityTypeConfiguration<Crop>
    {
        public CropEntityTypeConfiguration()
        {
            this.ToTable("Crop", "Production");
            this.HasKey<int>(o => o.CropId);
        }
    }

    public class NaturalGumEntityTypeConfiguration : EntityTypeConfiguration<NaturalGum>
    {
        public NaturalGumEntityTypeConfiguration()
        {
            this.ToTable("NaturalGum", "Production");
            this.HasKey<int>(o => o.NaturalGumId);
        }
    }
    public class ProductionItemCategoryEntityTypeConfiguration : EntityTypeConfiguration<ProductionItemCategory>
    {
        public ProductionItemCategoryEntityTypeConfiguration()
        {
            this.ToTable("ProductionItemCategory", "Production");
            this.HasKey<int>(o => o.ProductionItemCategoryId);
        }
    }
    public class ProductionAndProcessingCostCategoryEntityTypeConfiguration : EntityTypeConfiguration<ProductionAndProcessingCostCategory>
    {
        public ProductionAndProcessingCostCategoryEntityTypeConfiguration()
        {
            this.ToTable("ProductionAndProcessingCostCategory", "Production");
            this.HasKey<int>(o => o.ProductionAndProcessingCostCategoryId);
        }
    }

    public class InputMaterialApplicationRateEntityTypeConfiguration : EntityTypeConfiguration<InputMaterialApplicationRate>
    {
        public InputMaterialApplicationRateEntityTypeConfiguration()
        {
            this.ToTable("InputMaterialApplicationRate", "Production");
            this.HasKey<int>(o => o.InputMaterialApplicationRateId);
        }
    }
    public class InputMaterialApplicationRateItemEntityTypeConfiguration : EntityTypeConfiguration<InputMaterialApplicationRateItem>
    {
        public InputMaterialApplicationRateItemEntityTypeConfiguration()
        {
            this.ToTable("InputMaterialApplicationRateItem", "Production");
            this.HasKey<int>(o => o.InputMaterialApplicationRateItemId);
        }
    }
    public class RawProductFinishedGoodMappingEntityTypeConfiguration : EntityTypeConfiguration<RawProductFinishedGoodMapping>
    {
        public RawProductFinishedGoodMappingEntityTypeConfiguration()
        {
            this.ToTable("RawProductFinishedGoodMapping", "Production");
            this.HasKey<int>(o => o.RawProductFinishedGoodMappingId);
        }
    }
    #endregion

    #region production Activities
    public class ProductionActivityEntityTypeConfiguration : EntityTypeConfiguration<ProductionActivity>
    {
        public ProductionActivityEntityTypeConfiguration()
        {
            this.ToTable("ProductionActivity", "Production");
            this.HasKey<int>(o => o.ProductionActivityId);
        }
    }
    #endregion

    #region Production Planning
    public class SeedProductionPlanEntityTypeConfiguration : EntityTypeConfiguration<SeedProductionPlan>
    {
        public SeedProductionPlanEntityTypeConfiguration()
        {
            this.ToTable("SeedProductionPlan", "Production");
            this.HasKey<int>(o => o.SeedProductionPlanId);
        }
    }
    public class SeedProductionPlanYearEntityTypeConfiguration : EntityTypeConfiguration<SeedProductionPlanYear>
    {
        public SeedProductionPlanYearEntityTypeConfiguration()
        {
            this.ToTable("SeedProductionPlanYear", "Production");
            this.HasKey<int>(o => o.SeedProductionPlanYearId);
        }
    }
    #endregion

    #region Cost tracking
    public class CostTrackingPeriodEntityTypeConfiguration : EntityTypeConfiguration<CostTrackingPeriod>
    {
        public CostTrackingPeriodEntityTypeConfiguration()
        {
            this.ToTable("CostTrackingPeriod", "Production");
            this.HasKey<int>(o => o.CostTrackingPeriodId);
        }
    }
    public class ProjectCostByMonthEntityTypeConfiguration : EntityTypeConfiguration<ProjectCostByMonth>
    {
        public ProjectCostByMonthEntityTypeConfiguration()
        {
            this.ToTable("ProjectCostByMonth", "Production");
            this.HasKey<int>(o => o.ProjectCostByMonthId);
        }
    }
    public class ProjectCostByMonthDetailEntityTypeConfiguration : EntityTypeConfiguration<ProjectCostByMonthDetail>
    {
        public ProjectCostByMonthDetailEntityTypeConfiguration()
        {
            this.ToTable("ProjectCostByMonthDetail", "Production");
            this.HasKey<int>(o => o.ProjectCostByMonthDetailId);
        }
    }
    public class ProjectCostByMonthMaterialEntityTypeConfiguration : EntityTypeConfiguration<ProjectCostByMonthMaterial>
    {
        public ProjectCostByMonthMaterialEntityTypeConfiguration()
        {
            this.ToTable("ProjectCostByMonthMaterial", "Production");
            this.HasKey<int>(o => o.ProjectCostByMonthMaterialId);
        }
    }
    #endregion

    #region Production order
    //ProductionOrder
    public class ProductionOrderEntityTypeConfiguration : EntityTypeConfiguration<ProductionOrder>
    {
        public ProductionOrderEntityTypeConfiguration()
        {
            this.ToTable("ProductionOrder", "Production");
            this.HasKey<int>(o => o.ProductionOrderId);
        }
    }

    public class SeedProductionOrderItemEntityTypeConfiguration : EntityTypeConfiguration<SeedProductionOrderItem>
    {
        public SeedProductionOrderItemEntityTypeConfiguration()
        {
            this.ToTable("SeedProductionOrderItem", "Production");
            this.HasKey<int>(o => o.SeedProductionOrderItemId);
        }
    }

    public class GumProductionOrderItemEntityTypeConfiguration : EntityTypeConfiguration<GumProductionOrderItem>
    {
        public GumProductionOrderItemEntityTypeConfiguration()
        {
            this.ToTable("GumProductionOrderItem", "Production");
            this.HasKey<int>(o => o.GumProductionOrderItemId);
        }
    }

    public class ProductionOrderActivitiyEntityTypeConfiguration : EntityTypeConfiguration<ProductionOrderActivitiy>
    {
        public ProductionOrderActivitiyEntityTypeConfiguration()
        {
            this.ToTable("ProductionOrderActivitiy", "Production");
            this.HasKey<int>(o => o.ProductionOrderActivitiyId);
        }
    }
     public class MachineryMaintenanceEntityTypeConfiguration : EntityTypeConfiguration<MachineryMaintenance>
    {
        public MachineryMaintenanceEntityTypeConfiguration()
        {
            this.ToTable("MachineryMaintenance", "Production");
            this.HasKey<int>(o => o.MachineryMaintenanceId);
        }
    }
    public class ProductionRawProductPurchaseEntityTypeConfiguration : EntityTypeConfiguration<ProductionRawProductPurchase>
    {
        public ProductionRawProductPurchaseEntityTypeConfiguration()
        {
            this.ToTable("ProductionRawProductPurchase", "Production");
            this.HasKey<int>(o => o.ProductionRawProductPurchaseId);
        }
    }
    public class ProductionOrderInputMaterialEntityTypeConfiguration : EntityTypeConfiguration<ProductionOrderInputMaterial>
    {
        public ProductionOrderInputMaterialEntityTypeConfiguration()
        {
            this.ToTable("ProductionOrderInputMaterial", "Production");
            this.HasKey<int>(o => o.ProductionOrderInputMaterialId);
        }
    }
    #endregion

    #region Processing
    public class ProcessingPlantEntityTypeConfiguration : EntityTypeConfiguration<ProcessingPlant>
    {
        public ProcessingPlantEntityTypeConfiguration()
        {
            this.ToTable("ProcessingPlant", "Production");
            this.HasKey<int>(o => o.ProcessingPlantId);
        }
    }
    public class SeedProcessingOrderEntityTypeConfiguration : EntityTypeConfiguration<SeedProcessingOrder>
    {
        public SeedProcessingOrderEntityTypeConfiguration()
        {
            this.ToTable("SeedProcessingOrder", "Production");
            this.HasKey<int>(o => o.SeedProcessingOrderId);
        }
    }
    public class GumProcessingOrderEntityTypeConfiguration : EntityTypeConfiguration<GumProcessingOrder>
    {
        public GumProcessingOrderEntityTypeConfiguration()
        {
            this.ToTable("GumProcessingOrder", "Production");
            this.HasKey<int>(o => o.GumProcessingOrderId);
        }
    }
    public class SeedProcessingOrderItemEntityTypeConfiguration : EntityTypeConfiguration<SeedProcessingOrderItem>
    {
        public SeedProcessingOrderItemEntityTypeConfiguration()
        {
            this.ToTable("SeedProcessingOrderItem", "Production");
            this.HasKey<int>(o => o.SeedProcessingOrderItemId);
        }
    }
    public class GumProcessingOrderItemEntityTypeConfiguration : EntityTypeConfiguration<GumProcessingOrderItem>
    {
        public GumProcessingOrderItemEntityTypeConfiguration()
        {
            this.ToTable("GumProcessingOrderItem", "Production");
            this.HasKey<int>(o => o.GumProcessingOrderItemId);
        }
    }
    public class ProcessingOrderActivityEntityTypeConfiguration : EntityTypeConfiguration<ProcessingOrderActivity>
    {
        public ProcessingOrderActivityEntityTypeConfiguration()
        {
            this.ToTable("ProcessingOrderActivity", "Production");
            this.HasKey<int>(o => o.ProcessingOrderActivityId);
        }
    }
    public class ProcessingInputMaterialEntityTypeConfiguration : EntityTypeConfiguration<ProcessingInputMaterial>
    {
        public ProcessingInputMaterialEntityTypeConfiguration()
        {
            this.ToTable("ProcessingInputMaterial", "Production");
            this.HasKey<int>(o => o.ProcessingInputMaterialId);
        }
    }
    #endregion

    #region store
    public class ProductionStoreReceiveEntityTypeConfiguration : EntityTypeConfiguration<ProductionStoreReceive>
    {
        public ProductionStoreReceiveEntityTypeConfiguration()
        {
            this.ToTable("ProductionStoreReceive", "Production");
            this.HasKey<int>(o => o.ProductionStoreReceiveId);
        }
    }
    public class ProductionStoreReceiveItemEntityTypeConfiguration : EntityTypeConfiguration<ProductionStoreReceiveItem>
    {
        public ProductionStoreReceiveItemEntityTypeConfiguration()
        {
            this.ToTable("ProductionStoreReceiveItem", "Production");
            this.HasKey<int>(o => o.ProductionStoreReceiveItemId);
        }
    }
    public class ProductionStoreReceiveValidationEntityTypeConfiguration : EntityTypeConfiguration<ProductionStoreReceiveValidation>
    {
        public ProductionStoreReceiveValidationEntityTypeConfiguration()
        {
            this.ToTable("ProductionStoreReceiveValidation", "Production");
            this.HasKey<int>(o => o.ProductionStoreReceiveValidationId);
        }
    }
    public class ProductionStoreReceiveDistributionEntityTypeConfiguration : EntityTypeConfiguration<ProductionStoreReceiveDistribution>
    {
        public ProductionStoreReceiveDistributionEntityTypeConfiguration()
        {
            this.ToTable("ProductionStoreReceiveDistribution", "Production");
            this.HasKey<int>(o => o.ProductionStoreReceiveDistributionId);
        }
    }
    #region finished goods 
    public class ProductionFinishedGoodsStoreReceiveEntityTypeConfiguration : EntityTypeConfiguration<ProductionFinishedGoodsStoreReceive>
    {
        public ProductionFinishedGoodsStoreReceiveEntityTypeConfiguration()
        {
            this.ToTable("ProductionFinishedGoodsStoreReceive", "Production");
            this.HasKey<int>(o => o.ProductionFinishedGoodsStoreReceiveId);
        }
    }
    public class ProductionFinishedGoodsStoreReceiveItemEntityTypeConfiguration : EntityTypeConfiguration<ProductionFinishedGoodsStoreReceiveItem>
    {
        public ProductionFinishedGoodsStoreReceiveItemEntityTypeConfiguration()
        {
            this.ToTable("ProductionFinishedGoodsStoreReceiveItem", "Production");
            this.HasKey<int>(o => o.ProductionFinishedGoodsStoreReceiveItemId);
        }
    }
    public class ProductionFinishedGoodsStoreReceiveValidationEntityTypeConfiguration : EntityTypeConfiguration<ProductionFinishedGoodsStoreReceiveValidation>
    {
        public ProductionFinishedGoodsStoreReceiveValidationEntityTypeConfiguration()
        {
            this.ToTable("ProductionFinishedGoodsStoreReceiveValidation", "Production");
            this.HasKey<int>(o => o.ProductionFinishedGoodsStoreReceiveValidationId);
        }
    }
    public class ProductionFinishedGoodsStoreReceiveDistributionEntityTypeConfiguration : EntityTypeConfiguration<ProductionFinishedGoodsStoreReceiveDistribution>
    {
        public ProductionFinishedGoodsStoreReceiveDistributionEntityTypeConfiguration()
        {
            this.ToTable("ProductionFinishedGoodsStoreReceiveDistribution", "Production");
            this.HasKey<int>(o => o.ProductionFinishedGoodsStoreReceiveDistributionId);
        }
    }
    #endregion
    #endregion
    #region Issue
    public class ProductionIssueEntityTypeConfiguration : EntityTypeConfiguration<ProductionIssue>
    {
        public ProductionIssueEntityTypeConfiguration()
        {
            this.ToTable("ProductionIssue", "Production");
            this.HasKey<int>(o => o.ProductionIssueId);
        }
    }
    public class ProductionIssueItemEntityTypeConfiguration : EntityTypeConfiguration<ProductionIssueItem>
    {
        public ProductionIssueItemEntityTypeConfiguration()
        {
            this.ToTable("ProductionIssueItem", "Production");
            this.HasKey<int>(o => o.ProductionIssueItemId);
        }
    }
    public class ProductionIssueValidationEntityTypeConfiguration : EntityTypeConfiguration<ProductionIssueValidation>
    {
        public ProductionIssueValidationEntityTypeConfiguration()
        {
            this.ToTable("ProductionIssueValidation", "Production");
            this.HasKey<int>(o => o.ProductionIssueValidationId);
        }
    }
    public class ProductionIssueDistributionEntityTypeConfiguration : EntityTypeConfiguration<ProductionIssueDistribution>
    {
        public ProductionIssueDistributionEntityTypeConfiguration()
        {
            this.ToTable("ProductionIssueDistribution", "Production");
            this.HasKey<int>(o => o.ProductionIssueDistributionId);
        }
    }
    public class IssueAndProcessingOrderItemMappingEntityTypeConfiguration : EntityTypeConfiguration<IssueAndProcessingOrderItemMapping>
    {
        public IssueAndProcessingOrderItemMappingEntityTypeConfiguration()
        {
            this.ToTable("IssueAndProcessingOrderItemMapping", "Production");
            this.HasKey<int>(o => o.IssueAndProcessingOrderItemMappingId);
        }
    }
    #endregion
    #region Cleaned See/Gum Selling price
    public class CleanedSeedSellingPriceEntityTypeConfiguration : EntityTypeConfiguration<CleanedSeedSellingPrice>
    {
        public CleanedSeedSellingPriceEntityTypeConfiguration()
        {
            this.ToTable("CleanedSeedSellingPrice", "Production");
            this.HasKey<int>(o => o.CleanedSeedSellingPriceId);
        }
    }
    public class CleanedSeedSellingPriceItemEntityTypeConfiguration : EntityTypeConfiguration<CleanedSeedSellingPriceItem>
    {
        public CleanedSeedSellingPriceItemEntityTypeConfiguration()
        {
            this.ToTable("CleanedSeedSellingPriceItem", "Production");
            this.HasKey<int>(o => o.CleanedSeedSellingPriceItemId);
        }
    }
    public class CleanedNaturalGumSellingPriceEntityTypeConfiguration : EntityTypeConfiguration<CleanedNaturalGumSellingPrice>
    {
        public CleanedNaturalGumSellingPriceEntityTypeConfiguration()
        {
            this.ToTable("CleanedNaturalGumSellingPrice", "Production");
            this.HasKey<int>(o => o.CleanedNaturalGumSellingPriceId);
        }
    }
    public class CleanedNaturalGumSellingPriceItemEntityTypeConfiguration : EntityTypeConfiguration<CleanedNaturalGumSellingPriceItem>
    {
        public CleanedNaturalGumSellingPriceItemEntityTypeConfiguration()
        {
            this.ToTable("CleanedNaturalGumSellingPriceItem", "Production");
            this.HasKey<int>(o => o.CleanedNaturalGumSellingPriceItemId);
        }
    }
    #endregion

    #region Budget and Cash flow
    public class ProjectedMonthlyCashFlowEntityTypeConfiguration : EntityTypeConfiguration<ProjectedMonthlyCashFlow>
    {
        public ProjectedMonthlyCashFlowEntityTypeConfiguration()
        {
            this.ToTable("ProjectedMonthlyCashFlow", "Production");
            this.HasKey<int>(o => o.ProjectedMonthlyCashFlowId);
        }
    }
    public class ProjectedMonthlyCashFlowItemEntityTypeConfiguration : EntityTypeConfiguration<ProjectedMonthlyCashFlowItem>
    {
        public ProjectedMonthlyCashFlowItemEntityTypeConfiguration()
        {
            this.ToTable("ProjectedMonthlyCashFlowItem", "Production");
            this.HasKey<int>(o => o.ProjectedMonthlyCashFlowItemId);
        }
    }
    public class CleanedProductSaleEntityTypeConfiguration : EntityTypeConfiguration<CleanedProductSale>
    {
        public CleanedProductSaleEntityTypeConfiguration()
        {
            this.ToTable("CleanedProductSale", "Production");
            this.HasKey<int>(o => o.CleanedProductSaleId);
        }
    }
    public class CleanedProductSaleItemEntityTypeConfiguration : EntityTypeConfiguration<CleanedProductSaleItem>
    {
        public CleanedProductSaleItemEntityTypeConfiguration()
        {
            this.ToTable("CleanedProductSaleItem", "Production");
            this.HasKey<int>(o => o.CleanedProductSaleItemId);
        }
    }
    public class CleanedProductSaleItemPerMonthEntityTypeConfiguration : EntityTypeConfiguration<CleanedProductSaleItemPerMonth>
    {
        public CleanedProductSaleItemPerMonthEntityTypeConfiguration()
        {
            this.ToTable("CleanedProductSaleItemPerMonth", "Production");
            this.HasKey<int>(o => o.CleanedProductSaleItemPerMonthId);
        }
    }
    public class RawProductPurchaseEntityTypeConfiguration : EntityTypeConfiguration<RawProductPurchase>
    {
        public RawProductPurchaseEntityTypeConfiguration()
        {
            this.ToTable("RawProductPurchase", "Production");
            this.HasKey<int>(o => o.RawProductPurchaseId);
        }
    }
    public class RawProductPurchaseItemEntityTypeConfiguration : EntityTypeConfiguration<RawProductPurchaseItem>
    {
        public RawProductPurchaseItemEntityTypeConfiguration()
        {
            this.ToTable("RawProductPurchaseItem", "Production");
            this.HasKey<int>(o => o.RawProductPurchaseItemId);
        }
    }
    public class RawProductPurchaseItemPerMonthEntityTypeConfiguration : EntityTypeConfiguration<RawProductPurchaseItemPerMonth>
    {
        public RawProductPurchaseItemPerMonthEntityTypeConfiguration()
        {
            this.ToTable("RawProductPurchaseItemPerMonth", "Production");
            this.HasKey<int>(o => o.RawProductPurchaseItemPerMonthId);
        }
    }
    public class CustomCleaningAndStorageEntityTypeConfiguration : EntityTypeConfiguration<CustomCleaningAndStorage>
    {
        public CustomCleaningAndStorageEntityTypeConfiguration()
        {
            this.ToTable("CustomCleaningAndStorage", "Production");
            this.HasKey<int>(o => o.CustomCleaningAndStorageId);
        }
    }
    public class CustomCleaningAndStorageItemEntityTypeConfiguration : EntityTypeConfiguration<CustomCleaningAndStorageItem>
    {
        public CustomCleaningAndStorageItemEntityTypeConfiguration()
        {
            this.ToTable("CustomCleaningAndStorageItem", "Production");
            this.HasKey<int>(o => o.CustomCleaningAndStorageItemId);
        }
    }
    public class CustomCleaningAndStorageItemPerMonthEntityTypeConfiguration : EntityTypeConfiguration<CustomCleaningAndStorageItemPerMonth>
    {
        public CustomCleaningAndStorageItemPerMonthEntityTypeConfiguration()
        {
            this.ToTable("CustomCleaningAndStorageItemPerMonth", "Production");
            this.HasKey<int>(o => o.CustomCleaningAndStorageItemPerMonthId);
        }
    }
    #endregion
}
