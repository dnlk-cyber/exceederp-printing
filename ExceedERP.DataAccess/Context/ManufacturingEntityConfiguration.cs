using ExceedERP.Core.Domain.Manufacturing;
using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation;
using ExceedERP.Core.Domain.Manufacturing.Production;
using ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp;
using ExceedERP.Core.Domain.Manufacturing.Setting;
using System.Data.Entity.ModelConfiguration;

namespace ExceedERP.DataAccess.Context
{
    class FurnitureBoqDocumentCommentEntityTypeConfiguration : EntityTypeConfiguration<FurnitureBoqDocumentComment>
    {
        public FurnitureBoqDocumentCommentEntityTypeConfiguration()
        {
            this.ToTable("FurnitureBoqDocumentComment", "Manufacturing");
            this.HasKey<int>(p => p.FurnitureBoqDocumentCommentId);
        }
    }
    class EstimationSummaryEntityTypeConfiguration : EntityTypeConfiguration<EstimationSummary>
    {
        public EstimationSummaryEntityTypeConfiguration()
        {
            this.ToTable("EstimationSummary", "Manufacturing");
            this.HasKey<int>(p => p.EstimationSummaryId);
        }
    }
    class EstimationDetailEntityTypeConfiguration : EntityTypeConfiguration<EstimationDetail>
    {
        public EstimationDetailEntityTypeConfiguration()
        {
            this.ToTable("EstimationDetail", "Manufacturing");
            this.HasKey<int>(p => p.EstimationDetailId);
        }
    }
    class EstimationCostSummaryEntityTypeConfiguration : EntityTypeConfiguration<EstimationCostSummary>
    {
        public EstimationCostSummaryEntityTypeConfiguration()
        {
            this.ToTable("EstimationCostSummary", "Manufacturing");
            this.HasKey<int>(p => p.EstimationCostSummaryId);
        }
    }
    class FurnitureBoqRequestEntityTypeConfiguration : EntityTypeConfiguration<FurnitureBoqRequest>
    {
        public FurnitureBoqRequestEntityTypeConfiguration()
        {
            this.ToTable("FurnitureBoqRequest", "Manufacturing");
            this.HasKey<int>(p => p.FurnitureBoqRequestId);
        }
    }
    class FurnitureBoqDocumentEntityTypeConfiguration : EntityTypeConfiguration<FurnitureBoqDocument>
    {
        public FurnitureBoqDocumentEntityTypeConfiguration()
        {
            this.ToTable("FurnitureBoqDocument", "Manufacturing");
            this.HasKey<int>(p => p.FurnitureBoqDocumentId);
        }
    }
    class FurnitureBoqJobDesignEntityTypeConfiguration : EntityTypeConfiguration<FurnitureBoqJobDesign>
    {
        public FurnitureBoqJobDesignEntityTypeConfiguration()
        {
            this.ToTable("FurnitureBoqJobDesign", "Manufacturing");
            this.HasKey<int>(p => p.FurnitureBoqJobDesignId);
        }
    }
    class FurnitureJobDesignEntityTypeConfiguration : EntityTypeConfiguration<FurnitureJobDesign>
    {
        public FurnitureJobDesignEntityTypeConfiguration()
        {
            this.ToTable("FurnitureJobDesign", "Manufacturing");
            this.HasKey<int>(p => p.FurnitureJobDesignId);
        }
    }
    class FurnitureJobOrderProductionEntityTypeConfiguration : EntityTypeConfiguration<FurnitureJobOrderProduction>
    {
        public FurnitureJobOrderProductionEntityTypeConfiguration()
        {
            this.ToTable("FurnitureJobOrderProduction", "Manufacturing");
            this.HasKey<int>(p => p.FurnitureJobOrderProductionId);
        }
    }
    class FurnitureProductionBillOfMaterialEntityTypeConfiguration : EntityTypeConfiguration<FurnitureProductionBillOfMaterial>
    {
        public FurnitureProductionBillOfMaterialEntityTypeConfiguration()
        {
            this.ToTable("FurnitureProductionBillOfMaterial", "Manufacturing");
            this.HasKey<int>(p => p.FurnitureProductionBillOfMaterialId);
        }
    }
    class FurnitureProductionBillOfLaborEntityTypeConfiguration : EntityTypeConfiguration<FurnitureProductionBillOfLabor>
    {
        public FurnitureProductionBillOfLaborEntityTypeConfiguration()
        {
            this.ToTable("FurnitureProductionBillOfLabor", "Manufacturing");
            this.HasKey<int>(p => p.FurnitureProductionBillOfLaborId);
        }
    }
    class FurnitureJobAssignmentEntityTypeConfiguration : EntityTypeConfiguration<FurnitureJobAssignment>
    {
        public FurnitureJobAssignmentEntityTypeConfiguration()
        {
            this.ToTable("FurnitureJobAssignment", "Manufacturing");
            this.HasKey<int>(p => p.FurnitureJobAssignmentId);
        }
    }
    class FurnitureDailyProductionFollowUpEntityTypeConfiguration : EntityTypeConfiguration<FurnitureDailyProductionFollowUp>
    {
        public FurnitureDailyProductionFollowUpEntityTypeConfiguration()
        {
            this.ToTable("FurnitureDailyProductionFollowUp", "Manufacturing");
            this.HasKey<int>(p => p.FurnitureDailyProductionFollowUpId);
        }
    }
    class FurnitureProductionDailyFollowUpDelayReasonEntityTypeConfiguration : EntityTypeConfiguration<FurnitureProductionDailyFollowUpDelayReason>
    {
        public FurnitureProductionDailyFollowUpDelayReasonEntityTypeConfiguration()
        {
            this.ToTable("FurnitureProductionDailyFollowUpDelayReason", "Manufacturing");
            this.HasKey<int>(p => p.FurnitureProductionDailyFollowUpDelayReasonId); 
        }
    }
    class FurnitureUnfinishedMaterialTransferFormEntityTypeConfiguration : EntityTypeConfiguration<FurnitureUnfinishedMaterialTransferForm>
    {
        public FurnitureUnfinishedMaterialTransferFormEntityTypeConfiguration() 
        {
            this.ToTable("FurnitureUnfinishedMaterialTransferForm", "Manufacturing");
            this.HasKey<int>(p => p.FurnitureUnfinishedMaterialTransferFormId);
        }
    }
    class FurnitureStandardJobTypeEntityTypeConfiguration : EntityTypeConfiguration<FurnitureStandardJobType>
    {
        public FurnitureStandardJobTypeEntityTypeConfiguration()
        {
            this.ToTable("FurnitureStandardJobType", "Manufacturing");
            this.HasKey<int>(p => p.FurnitureStandardJobTypeId);
        }
    }
    class StandardBillOfMaterialEntityTypeConfiguration : EntityTypeConfiguration<StandardBillOfMaterial>
    {
        public StandardBillOfMaterialEntityTypeConfiguration()
        {
            this.ToTable("StandardBillOfMaterial", "Manufacturing");
            this.HasKey<int>(p => p.StandardBillOfMaterialId);
        }
    }

    class StandardBillOfLaborEntityTypeConfiguration : EntityTypeConfiguration<StandardBillOfLabor>
    {
        public StandardBillOfLaborEntityTypeConfiguration()
        {
            this.ToTable("StandardBillOfLabor", "Manufacturing");
            this.HasKey<int>(p => p.StandardBillOfLaborId);
        }
    }
    class FurnitureJobOrderFormEntityTypeConfiguration : EntityTypeConfiguration<FurnitureJobOrderForm>
    {
        public FurnitureJobOrderFormEntityTypeConfiguration()
        {
            this.ToTable("FurnitureJobOrderForm", "Manufacturing");
            this.HasKey<int>(p => p.FurnitureJobOrderFormId);
        }
    }
    class FurnitureJobOrderItemEntityTypeConfiguration : EntityTypeConfiguration<FurnitureJobOrderItem>
    {
        public FurnitureJobOrderItemEntityTypeConfiguration()
        {
            this.ToTable("FurnitureJobOrderItem", "Manufacturing");
            this.HasKey<int>(p => p.FurnitureJobOrderItemId);
        }
    }
    class FurnitureProformaInvoiceEntityTypeConfiguration : EntityTypeConfiguration<FurnitureProformaInvoice>
    {
        public FurnitureProformaInvoiceEntityTypeConfiguration()
        {
            this.ToTable("FurnitureProformaInvoice", "Manufacturing");
            this.HasKey<int>(p => p.FurnitureProformaInvoiceId);
        }
    }
    class FurnitureProformaInvoiceItemEntityTypeConfiguration : EntityTypeConfiguration<FurnitureProformaInvoiceItem>
    {
        public FurnitureProformaInvoiceItemEntityTypeConfiguration()
        {
            this.ToTable("FurnitureProformaInvoiceItem", "Manufacturing");
            this.HasKey<int>(p => p.FurnitureProformaInvoiceItemId);
        }
    }
    class FurnitureBillOfQuantityEntityConfiguration : EntityTypeConfiguration<FurnitureBillOfQuantity>
    {
        public FurnitureBillOfQuantityEntityConfiguration()
        {
            this.ToTable("FurnitureBillOfQuantity", "Manufacturing");
            this.HasKey<int>(p => p.FurnitureBillOfQuantityId);
        }
    }
    class FurnitureBOQMaterialEntityTypeConfiguration : EntityTypeConfiguration<FurnitureBOQMaterial>
    {
        public FurnitureBOQMaterialEntityTypeConfiguration()
        {
            this.ToTable("FurnitureBOQMaterial", "Manufacturing");
            this.HasKey<int>(p => p.FurnitureBOQMaterialId);
        }
    }
    class FurnitureBOQLaborEntityTypeConfiguration : EntityTypeConfiguration<FurnitureBOQLabor>
    {
        public FurnitureBOQLaborEntityTypeConfiguration()
        {
            this.ToTable("FurnitureBOQLabor", "Manufacturing");
            this.HasKey<int>(p => p.FurnitureBOQLaborId);
        }
    }
    #region Furniture Estimation




    class FurnitureLaborCostEntityConfiguration : EntityTypeConfiguration<FurnitureLaborCost>
    {
        public FurnitureLaborCostEntityConfiguration()
        {
            this.ToTable("FurnitureLaborCosts", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureLaborCostId);
        }
    }

    class FurnitureStepEntityConfiguration : EntityTypeConfiguration<FurnitureStep>
    {
        public FurnitureStepEntityConfiguration()
        {
            this.ToTable("FurnitureSteps", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureStepId);
        }
    }

    class FurnitureCategoryEntityConfiguration : EntityTypeConfiguration<FurnitureCategory>
    {
        public FurnitureCategoryEntityConfiguration()
        {
            this.ToTable("FurnitureCategories", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureCategoryId);
        }
    }
    class FurnitureCostEstimationEntityConfiguration : EntityTypeConfiguration<FurnitureEstimationForm>
    {
        public FurnitureCostEstimationEntityConfiguration()
        {
            this.ToTable("FurnitureCostEstimations", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureEstimationId);
        }
    }
    class FurnitureMaterialCostEntityConfiguration : EntityTypeConfiguration<FurnitureMaterialCost>
    {
        public FurnitureMaterialCostEntityConfiguration()
        {
            this.ToTable("FurnitureMaterialCosts", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureMaterialCostId);
        }
    }
    class DirectLaborCostEntityConfiguration : EntityTypeConfiguration<DirectLaborCost>
    {
        public DirectLaborCostEntityConfiguration()
        {
            this.ToTable("DirectLaborCosts", "Manufacturing");
            this.HasKey<int>(t => t.DirectLaborCostId);
        }
    }
    class FurnitureOverallCostEntityConfiguration : EntityTypeConfiguration<FurnitureOverallCost>
    {
        public FurnitureOverallCostEntityConfiguration()
        {
            this.ToTable("FurnitureOverallCosts", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureOverallCostId);
        }
    }
    #endregion
    #region JobCosting

    
   
    public class FurnitureMachineEntityTypeConfiguration : EntityTypeConfiguration<FurnitureMachine>
    {
        public FurnitureMachineEntityTypeConfiguration()
        {
            this.ToTable("FurnitureMachines", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureMachineId);
        }
    }


    
    #endregion
    #region Product Felow Up
    public class FurnitureYearlyProductionPlanEntityTypeConfiguration : EntityTypeConfiguration<FurnitureJobProcuctionPlan>
    {

        public FurnitureYearlyProductionPlanEntityTypeConfiguration()
        {
            this.ToTable("FurnitureYearlyProductionPlans", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureJobProcuctionPlanId);

        }
    }
    public class FurnitureHalfYearlyProductionPlanEntityTypeConfiguration : EntityTypeConfiguration<FurnitureHalfYearlyProductionPlan>
    {

        public FurnitureHalfYearlyProductionPlanEntityTypeConfiguration()
        {
            this.ToTable("FurnitureHalfYearlyProductionPlans", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureHalfYearlyProductionPlanId);

        }
    }
    public class FurnitureQuarterlyProductionPlanEntityTypeConfiguration : EntityTypeConfiguration<FurnitureQuarterlyProductionPlan>
    {

        public FurnitureQuarterlyProductionPlanEntityTypeConfiguration()
        {
            this.ToTable("FurnitureQuarterlyProductionPlans", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureQuarterlyProductionPlanId);

        }
    }
    public class FurnitureMonthlyProductionPlanEntityTypeConfiguration : EntityTypeConfiguration<FurnitureMonthlyProductionPlan>
    {

        public FurnitureMonthlyProductionPlanEntityTypeConfiguration()
        {
            this.ToTable("FurnitureMonthlyProductionPlans", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureMonthlyProductionPlanId);

        }
    }
    public class FurnitureWeeklyProductionPlanEntityTypeConfiguration : EntityTypeConfiguration<FurnitureWeeklyProductionPlan>
    {

        public FurnitureWeeklyProductionPlanEntityTypeConfiguration()
        {
            this.ToTable("FurnitureWeeklyProductionPlans", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureWeeklyProductionPlanId);

        }
    }
    public class FurnitureProductionJobTypePlanEntityTypeConfiguration : EntityTypeConfiguration<FurnitureProductionJobTypePlan>
    {

        public FurnitureProductionJobTypePlanEntityTypeConfiguration()
        {
            this.ToTable("FurnitureProductionJobTypePlans", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureProductionJobTypePlanId);

        }
    }
    public class FurnitureProductionCustomerTypeEntityTypeConfiguration : EntityTypeConfiguration<FurnitureProductionCustomerType>
    {
        public FurnitureProductionCustomerTypeEntityTypeConfiguration()
        {
            this.ToTable("FurnitureProductionCustomerTypes", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureProductionCustomerTypeId);
        }
    }
    public class FurnitureProductionMonthlyJobTypePlanEntityTypeConfiguration : EntityTypeConfiguration<FurnitureProductionMonthlyJobTypePlan>
    {
        public FurnitureProductionMonthlyJobTypePlanEntityTypeConfiguration()
        {
            this.ToTable("FurnitureProductionMonthlyJobTypePlans", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureProductionMonthlyJobTypePlanId);
        }
    }
    public class FurnitureProductionMonthlyPlanEntityTypeConfiguration : EntityTypeConfiguration<FurnitureProductionMonthlyPlan>
    {
        public FurnitureProductionMonthlyPlanEntityTypeConfiguration()
        {
            this.ToTable("FurnitureProductionMonthlyPlans", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureProductionMonthlyPlanId);
        }
    }
    public class FurnitureProductionYearlyPlanEntityTypeConfiguration : EntityTypeConfiguration<FurnitureProductionYearlyPlan>
    {
        public FurnitureProductionYearlyPlanEntityTypeConfiguration()
        {
            this.ToTable("FurnitureProductionYearlyPlans", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureProductionYearlyPlanId);
        }
    }
    public class FurnitureJobStatusLineItemEntityTypeConfiguration : EntityTypeConfiguration<FurnitureJobStatusLineItem>
    {
        public FurnitureJobStatusLineItemEntityTypeConfiguration()
        {
            this.ToTable("FurnitureJobStatusLineItems", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureJobStatusLineItemId);
        }
    }
    public class FurnitureMachineTypeEntityTypeConfiguration : EntityTypeConfiguration<FurnitureMachine>
    {
        public FurnitureMachineTypeEntityTypeConfiguration()
        {
            this.ToTable("FurnitureMachineTypes", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureMachineId);
        }
    }
    public class FurnitureJobCategoryEntityTypeConfiguration : EntityTypeConfiguration<FurnitureJobCategory>
    {
        public FurnitureJobCategoryEntityTypeConfiguration()
        {
            this.ToTable("FurnitureJobCategories", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureJobCategoryId);
        }
    }

    public class FurnitureProformaEntityTypeConfiguration : EntityTypeConfiguration<FurnitureProforma>
    {
        public FurnitureProformaEntityTypeConfiguration()
        {
            this.ToTable("FurnitureProforma", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureProformaId);
        }
    }
    public class FurniturePFProformaItemsEntityTypeConfiguration : EntityTypeConfiguration<FurniturePFProformaItems>
    {
        public FurniturePFProformaItemsEntityTypeConfiguration()
        {
            this.ToTable("FurniturePFProformaItems", "Manufacturing");
            this.HasKey<int>(t => t.FurniturePFProformaItemsId);
        }
    }



    public class FurnitureGraphicEntityTypeConfiguration : EntityTypeConfiguration<FurnitureGraphic>
    {
        public FurnitureGraphicEntityTypeConfiguration()
        {
            this.ToTable(" FurnitureGraphic", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureGraphicID);
        }
    }


    public class FurnitureJobEntityTypeConfiguration : EntityTypeConfiguration<FurnitureJob>
    {
        public FurnitureJobEntityTypeConfiguration()
        {
            this.ToTable(" FurnitureJob", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureJobId);
        }
    }
    public class FurnitureJobStatusEntityTypeConfiguration : EntityTypeConfiguration<FurnitureJobStatus>
    {
        public FurnitureJobStatusEntityTypeConfiguration()
        {
            this.ToTable(" FurnitureJobStatuses", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureJobStatusId);
        }
    }



    public class FurnitureSettingsEntityTypeConfiguration : EntityTypeConfiguration<FurnitureSettings>
    {
        public FurnitureSettingsEntityTypeConfiguration()
        {
            this.ToTable(" FurnitureSettings", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureSettingsId);
        }
    }



    public class FurnitureJobOrderMaterialEntityTypeConfiguration : EntityTypeConfiguration<FurnitureJobOrderMaterial>
    {
        public FurnitureJobOrderMaterialEntityTypeConfiguration()
        {
            this.ToTable("FurnitureJobOrderMaterial", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureJobOrderMaterialId);
        }
    }

    //public class PrintingProductionFinishedGoodsStoreReceiveEntityTypeConfiguration : EntityTypeConfiguration<ManuProductionFinishedGoodsStoreReceive>
    //{
    //    public PrintingProductionFinishedGoodsStoreReceiveEntityTypeConfiguration()
    //    {
    //        this.ToTable("FinishedGoodsStoreReceive", "ProductFollowUp");
    //        this.HasKey<int>(t => t.FinishedGoodsStoreReceiveId);
    //    }
    //}
    //public class PrintingProductionFinishedGoodsStoreReceiveItemEntityTypeConfiguration : EntityTypeConfiguration<PrintingProductionFinishedGoodsStoreReceiveItem>
    //{
    //    public PrintingProductionFinishedGoodsStoreReceiveItemEntityTypeConfiguration()
    //    {
    //        this.ToTable("FinishedGoodsStoreReceiveItem", "ProductFollowUp");
    //        this.HasKey<int>(t => t.FinishedGoodsStoreReceiveItemId);
    //    }
    //}
    //public class PrintingProductionFinishedGoodsStoreReceiveValidationEntityTypeConfiguration : EntityTypeConfiguration<PrintingProductionFinishedGoodsStoreReceiveValidation>
    //{
    //    public PrintingProductionFinishedGoodsStoreReceiveValidationEntityTypeConfiguration()
    //    {
    //        this.ToTable("FinishedGoodsStoreReceiveValidation", "ProductFollowUp");
    //        this.HasKey<int>(t => t.FinishedGoodsStoreReceiveValidationId);
    //    }
    //}
    //public class PrintingProductionFinishedGoodsStoreReceiveDistributionEntityTypeConfiguration : EntityTypeConfiguration<PrintingProductionFinishedGoodsStoreReceiveDistribution>
    //{
    //    public PrintingProductionFinishedGoodsStoreReceiveDistributionEntityTypeConfiguration()
    //    {
    //        this.ToTable("FinishedGoodsStoreReceiveDistribution", "ProductFollowUp");
    //        this.HasKey<int>(t => t.FinishedGoodsStoreReceiveDistributionId);
    //    }
    //}

    public class FurnitureFinishedGoodsEntityTypeConfiguration : EntityTypeConfiguration<FurnitureFinishedGoods>
    {
        public FurnitureFinishedGoodsEntityTypeConfiguration()
        {
            this.ToTable("FurnitureFinishedGoodItems", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureFinishedGoodsId);
        }
    }

    public class FurnitureProductDeliveryEntityTypeConfiguration : EntityTypeConfiguration<FurnitureProductDelivery>
    {
        public FurnitureProductDeliveryEntityTypeConfiguration()
        {
            this.ToTable("FurnitureProductDelivery", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureProductDeliveryId);
        }
    }

    public class FurnitureProductDeliveryItemsEntityTypeConfiguration : EntityTypeConfiguration<FurnitureProductDeliveryItems>
    {
        public FurnitureProductDeliveryItemsEntityTypeConfiguration()
        {
            this.ToTable("FurnitureProductDeliveryItems", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureProductDeliveryItemsId);
        }
    }
    #endregion

    public class ManifucturingTaskCategoryEntityTypeConfiguration : EntityTypeConfiguration<ManifucturingTaskCategory>
    {
        public ManifucturingTaskCategoryEntityTypeConfiguration()
        {
            this.ToTable("ManifucturingTaskCategory", "Manufacturing");
            this.HasKey<int>(k => k.ManifucturingTaskCategoryId);
        }
    }
    public class ManufacturingMaterialCategoryEntityTypeConfiguration : EntityTypeConfiguration<ManufacturingMaterialCategory>
    {
        public ManufacturingMaterialCategoryEntityTypeConfiguration()
        {
            this.ToTable("ManufacturingMaterialCategory", "Manufacturing");
            this.HasKey<int>(k => k.ManufacturingMaterialCategoryId);
        }
    }
    public class ManufacturingMaterialCategoryItemEntityTypeConfiguration : EntityTypeConfiguration<ManufacturingMaterialCategoryItem>
    {
        public ManufacturingMaterialCategoryItemEntityTypeConfiguration()
        {
            this.ToTable("ManufacturingMaterialCategoryItem", "Manufacturing");
            this.HasKey<int>(k => k.ManufacturingMaterialCategoryItemId);
        }
    }

}
