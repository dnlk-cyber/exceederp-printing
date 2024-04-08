using ExceedERP.Core.Domain.Manufacturing;
using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation;
using ExceedERP.Core.Domain.Manufacturing.JobCosting;
using ExceedERP.Core.Domain.Manufacturing.JobCosting.Setting;
using ExceedERP.Core.Domain.Manufacturing.Production;
using ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp;
using ExceedERP.Core.Domain.Manufacturing.Setting;
using System.Data.Entity.ModelConfiguration;

namespace ExceedERP.DataAccess.Context
{
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

    public class FurnitureDailyLaborRateEntityTypeConfiguration : EntityTypeConfiguration<FurnitureDailyLaborRate>
    {
        public FurnitureDailyLaborRateEntityTypeConfiguration()
        {
            this.ToTable("FurnitureDailyLaborRates", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureDailyLaborRateId);
        }
    }
    public class FurnitureDLRSalaryEntityTypeConfiguration : EntityTypeConfiguration<FurnitureDLRSalary>
    {
        public FurnitureDLRSalaryEntityTypeConfiguration()
        {
            this.ToTable("FurnitureDLRSalaries", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureDLRSalaryId);
        }
    }
    public class FurnitureDLRHourEntityTypeConfiguration : EntityTypeConfiguration<FurnitureDLRHour>
    {
        public FurnitureDLRHourEntityTypeConfiguration()
        {
            this.ToTable("FurnitureDLRHours", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureDLRHourId);
        }
    }
    public class FurnitureJobCostEntityTypeConfiguration : EntityTypeConfiguration<FurnitureJobCost>
    {
        public FurnitureJobCostEntityTypeConfiguration()
        {
            this.ToTable("FurnitureJobCosts", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureJobCostId);
        }
    }
    public class FurnitureJobChangeOfOrderEntityTypeConfiguration : EntityTypeConfiguration<FurnitureJobChangeOfOrder>
    {
        public FurnitureJobChangeOfOrderEntityTypeConfiguration()
        {
            this.ToTable("FurnitureJobChangeOfOrders", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureJobChangeOfOrderId);
        }
    }
    public class FurnitureJobLaborCostEntityTypeConfiguration : EntityTypeConfiguration<FurnitureJobLaborCost>
    {
        public FurnitureJobLaborCostEntityTypeConfiguration()
        {
            this.ToTable("FurnitureJobLaborCosts", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureJobLaborCostId);
        }
    }
    public class FurnitureJobMaterialCostEntityTypeConfiguration : EntityTypeConfiguration<FurnitureJobMaterialCost>
    {
        public FurnitureJobMaterialCostEntityTypeConfiguration()
        {
            this.ToTable("FurnitureJobMaterialCosts", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureJobMaterialCostId);
        }
    }
    public class FurnitureJobMaterialreturnEntityTypeConfiguration : EntityTypeConfiguration<FurnitureJobMaterialReturn>
    {
        public FurnitureJobMaterialreturnEntityTypeConfiguration()
        {
            this.ToTable("FurnitureJobMaterialReturns", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureJobMaterialReturnId);
        }
    }
    public class FurnitureFinishedJobEntityTypeConfiguration : EntityTypeConfiguration<FurnitureFinishedJob>
    {
        public FurnitureFinishedJobEntityTypeConfiguration()
        {
            this.ToTable("FurnitureFinishedJobs", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureFinishedJobId);
        }
    }
    public class FurnitureDeliveredJobEntityTypeConfiguration : EntityTypeConfiguration<FurnitureDeliveredJob>
    {
        public FurnitureDeliveredJobEntityTypeConfiguration()
        {
            this.ToTable("FurnitureDeliveredJobs", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureDeliveredJobId);
        }
    }
    public class FurnitureIdleTimeEntityTypeConfiguration : EntityTypeConfiguration<FurnitureIdleTime>
    {
        public FurnitureIdleTimeEntityTypeConfiguration()
        {
            this.ToTable("FurnitureIdleTimes", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureIdleTimeID);
        }
    }
    public class FurnitureIdleTimeOTEntityTypeConfiguration : EntityTypeConfiguration<FurnitureIdleTimeOT>
    {
        public FurnitureIdleTimeOTEntityTypeConfiguration()
        {
            this.ToTable("FurnitureIdleTimeOTs", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureIdleTimeOTID);
        }
    }
    public class FurnitureIdleTimeProductiveEntityTypeConfiguration : EntityTypeConfiguration<FurnitureIdleTimeProductive>
    {
        public FurnitureIdleTimeProductiveEntityTypeConfiguration()
        {
            this.ToTable("FurnitureIdleTimeProductives", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureIdleTimeProductiveID);
        }
    }
    public class FurnitureIdleTimeNonProductiveControllableEntityTypeConfiguration : EntityTypeConfiguration<FurnitureIdleTimeNonProductiveControllable>
    {
        public FurnitureIdleTimeNonProductiveControllableEntityTypeConfiguration()
        {
            this.ToTable("FurnitureIdleTimeNonProductives", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureIdleTimeNonProductiveControllableID);
        }
    }
    public class FurnitureIdleTimeNonProductiveNonControllableEntityTypeConfiguration : EntityTypeConfiguration<FurnitureIdleTimeNonProductiveNonControllable>
    {
        public FurnitureIdleTimeNonProductiveNonControllableEntityTypeConfiguration()
        {
            this.ToTable("FurnitureIdleTimeNonProductiveNonControllables", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureIdleTimeNonProductiveNonControllableID);
        }
    }
    public class FurnitureYearlyOverHeadRateEntityTypeConfiguration : EntityTypeConfiguration<FurnitureYearlyOverHeadRate>
    {
        public FurnitureYearlyOverHeadRateEntityTypeConfiguration()
        {
            this.ToTable("FurnitureYearlyOverHeadRates", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureYearlyOverHeadRateId);
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
    public class FurnitureMachineEntityTypeConfiguration : EntityTypeConfiguration<FurnitureMachine>
    {
        public FurnitureMachineEntityTypeConfiguration()
        {
            this.ToTable("FurnitureMachines", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureMachineId);
        }
    }


    public class FurnitureIdleTimeCategoryEntityTypeConfiguration : EntityTypeConfiguration<FurnitureIdleTimeCategory>
    {
        public FurnitureIdleTimeCategoryEntityTypeConfiguration()
        {
            this.ToTable("FurnitureIdleTimeCategories", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureIdleTimeCategoryId);
        }
    }
    public class FurnitureJcPensionEntityTypeConfiguration : EntityTypeConfiguration<FurnitureJcPension>
    {
        public FurnitureJcPensionEntityTypeConfiguration()
        {
            this.ToTable("FurnitureJcPensions", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureJcPensionId);
        }
    }
    public class FurnitureJobOrderTimeRegistrationEntityTypeConfiguration : EntityTypeConfiguration<FurnitureJobOrderTimeRegistration>
    {
        public FurnitureJobOrderTimeRegistrationEntityTypeConfiguration()
        {
            this.ToTable("FurnitureJobOrderTimeRegistration", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureJobOrderTimeRegistrationId);
        }
    }
    public class FurnitureJobOrderTimeSheetEntityTypeConfiguration : EntityTypeConfiguration<FurnitureJobOrderTimeSheet>
    {
        public FurnitureJobOrderTimeSheetEntityTypeConfiguration()
        {
            this.ToTable("FurnitureJobOrderTimeSheet", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureJobOrderTimeSheetId);
        }
    }
    public class FurnitureJobOrderSummaryEntityTypeConfiguration : EntityTypeConfiguration<FurnitureJobOrderSummary>
    {
        public FurnitureJobOrderSummaryEntityTypeConfiguration()
        {
            this.ToTable("FurnitureJobOrderSummary", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureJobOrderSummaryId);
        }
    }
    public class FurnitureJobOrderSummaryCostCenterEntityTypeConfiguration : EntityTypeConfiguration<FurnitureJobOrderSummaryCostCenter>
    {
        public FurnitureJobOrderSummaryCostCenterEntityTypeConfiguration()
        {
            this.ToTable("FurnitureJobOrderSummaryCostCenter", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureJobOrderSummaryCostCenterId);
        }
    }
    public class FurnitureJobOrderSummaryCostCenterDetailEntityTypeConfiguration : EntityTypeConfiguration<FurnitureJobOrderSummaryCostCenterDetail>
    {
        public FurnitureJobOrderSummaryCostCenterDetailEntityTypeConfiguration()
        {
            this.ToTable("FurnitureJobOrderSummaryCostCenterDetail", "Manufacturing");
            this.HasKey<int>(t => t.FurnitureJobOrderSummaryCostCenterDetailId);
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
