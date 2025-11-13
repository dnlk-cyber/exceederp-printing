using ExceedERP.Core.Domain.printing.PrintingEstimation;
using ExceedERP.Core.Domain.Printing;
using ExceedERP.Core.Domain.Printing.PrintingEstimation;
using ExceedERP.Core.Domain.Printing.PrintingProduction;
using ExceedERP.Core.Domain.Printing.ProductionFollowUp;
using System.Data.Entity.ModelConfiguration;

namespace ExceedERP.DataAccess.Context
{
    public class PrintingQualityInspectionEntityTypeConfiguration : EntityTypeConfiguration<PrintingQualityInspection>
    {
        public PrintingQualityInspectionEntityTypeConfiguration()
        {
            this.ToTable("PrintingQualityInspection", "Printing");
            this.HasKey<int>(p => p.PrintingQualityInspectionId);
        }
    }
    public class PrintingDailyFollowUpMachineMaintenanceEntityTypeConfiguration : EntityTypeConfiguration<PrintingDailyFollowUpMachineMaintenance>
    {

        public PrintingDailyFollowUpMachineMaintenanceEntityTypeConfiguration()
        {
            this.ToTable("PrintingDailyFollowUpMachineMaintenance", "Printing");
            this.HasKey<int>(p => p.PrintingDailyFollowUpMachineMaintenanceId);
        }
    }
    public class PrintingDailyFollowUpLogBookEntityTypeConfiguration : EntityTypeConfiguration<PrintingDailyFollowUpLogBook>
    {
        public PrintingDailyFollowUpLogBookEntityTypeConfiguration()
        {
            this.ToTable("PrintingDailyFollowUpLogBook", "Printing");
            this.HasKey<int>(p => p.PrintingDailyFollowUpLogBookId);
        }
    }
    public class PressDailyFollowUpWastageEntityTypeConfiguration : EntityTypeConfiguration<PressDailyFollowUpWastage>
    {
        public PressDailyFollowUpWastageEntityTypeConfiguration()
        {
            this.ToTable("PressDailyFollowUpWastage", "Printing");
            this.HasKey<int>(p => p.PressDailyFollowUpWastageId);
        }
    }
    public class PrintingSummaryCommentEntityTypeCOnfiguration : EntityTypeConfiguration<PrintingSummaryComment>
    {
        public PrintingSummaryCommentEntityTypeCOnfiguration()
        {
            this.ToTable("PrintingSummaryComment", "Printing");
            this.HasKey<int>(p => p.PrintingSummaryCommentId);
        }
    }
    public class PrintingProductionFollowUpEntityTypeConfiguration : EntityTypeConfiguration<PrintingProductionFollowUp>
    {
        public PrintingProductionFollowUpEntityTypeConfiguration()
        {
            this.ToTable("PrintingProductionFollowUp", "Printing");
            this.HasKey<int>(p => p.PrintingProductionFollowUpId);
        }
    }
    public class PrintingProductionFollowUpTaskEntityTypeConfiguration : EntityTypeConfiguration<PrintingProductionFollowUpTask>
    {
        public PrintingProductionFollowUpTaskEntityTypeConfiguration()
        {
            this.ToTable("PrintingProductionFollowUpTask", "Printing");
            this.HasKey<int>(p => p.PrintingProductionFollowUpTaskId);
        }
    }
    public class PrintingProductionDailyFollowUpDetailEntityTypeConfiguration : EntityTypeConfiguration<PrintingProductionDailyFollowUpDetail>
    {
        public PrintingProductionDailyFollowUpDetailEntityTypeConfiguration()
        {
            this.ToTable("PrintingProductionDailyFollowUpDetail", "Printing");
            this.HasKey<int>(p => p.PrintingProductionDailyFollowUpDetailId);
        }
    }
    public class PrintingTaskIssueEntityTypeConfiguration : EntityTypeConfiguration<PrintingTaskIssue>
    {
        public PrintingTaskIssueEntityTypeConfiguration()
        {
            this.ToTable("PrintingTaskIssue", "Printing");
            this.HasKey<int>(p => p.PrintingTaskIssueId);
        }
    }
    public class PrintingProductionAllottedRawMaterialEntityTypeConfiguration : EntityTypeConfiguration<PrintingProductionAllottedRawMaterial>
    { 
        public PrintingProductionAllottedRawMaterialEntityTypeConfiguration()
        {
            this.ToTable("PrintingProductionAllottedRawMaterial", "Printing");
            this.HasKey<int>(p => p.PrintingProductionAllottedRawMaterialId);
        }
    }
    public class PrintingMachineDetailEntityTypeConfiguration : EntityTypeConfiguration<PrintingMachineDetail>
    {
        public PrintingMachineDetailEntityTypeConfiguration()
        {
            this.ToTable("PrintingMachineDetail", "Printing");
            this.HasKey<int>(p => p.PrintingMachineDetailId);
        }
    }
    public class PrintingJobOrderJobDesignEntityTypeConfiguration : EntityTypeConfiguration<PrintingJobOrderJobDesign>
    {
        public PrintingJobOrderJobDesignEntityTypeConfiguration()
        {
            this.ToTable("PrintingJobOrderJobDesign", "Printing");
            this.HasKey<int>(p => p.PrintingJobOrderJobDesignId);
        }
    }
    public class PrintingJobOrderProductionJobDesignEntityTypeConfiguration : EntityTypeConfiguration<PrintingJobOrderProductionJobDesign>
    {
        public PrintingJobOrderProductionJobDesignEntityTypeConfiguration()
        {
            this.ToTable("PrintingJobOrderProductionJobDesign", "Printing");
            this.HasKey<int>(p => p.PrintingJobOrderProductionJobDesignId);
        }
    }
    public class PrintingEstimationMarginEntityTypeConfiguration : EntityTypeConfiguration<PrintingEstimationMargin>
    {
        public PrintingEstimationMarginEntityTypeConfiguration()
        {
            this.ToTable("PrintingEstimationMargin", "Printing");
            this.HasKey<int>(p => p.PrintingEstimationMarginId);
        }
    }
    public class PrintingJobOrderProductionEntityTypeConfiguration: EntityTypeConfiguration<PrintingJobOrderProduction>
    {
        public PrintingJobOrderProductionEntityTypeConfiguration()
        {
            this.ToTable("PrintingJobOrderProduction", "Printing");
            this.HasKey<int>(p => p.PrintingJobOrderProductionId);
        }
    }
    public class PrintingProductionMaterialCostEntityTypeConfiguration : EntityTypeConfiguration<PrintingProductionMaterialCost>
    {
        public PrintingProductionMaterialCostEntityTypeConfiguration()
        {
            this.ToTable("PrintingProductionMaterialCost", "Printing");
            this.HasKey<int>(p => p.PrintingProductionMaterialCostId);
        }
    }
    public class PrintingProductionLaborCostEntityTypeConfiguration : EntityTypeConfiguration<PrintingProductionLaborCost>
    {
        public PrintingProductionLaborCostEntityTypeConfiguration()
        {
            this.ToTable("PrintingProductionLaborCost", "Printing");
            this.HasKey<int>(p => p.PrintingProductionLaborCostId);
        }
    }
    public class PrintingJobOrderEntityTypeConfiguration : EntityTypeConfiguration<PrintingJobOrder>
    {
        public PrintingJobOrderEntityTypeConfiguration()
        {
            this.ToTable("PrintingJobOrder", "Printing");
            this.HasKey<int>(p => p.PrintingJobOrderId);
        }
    }
    public class PrintingJobOrderItemEntityTypeConfiguration : EntityTypeConfiguration<PrintingJobOrderItem>
    {
        public PrintingJobOrderItemEntityTypeConfiguration()
        {
            this.ToTable("PrintingJobOrderItem", "Printing");
            this.HasKey<int>(p => p.PrintingJobOrderItemId);
        }
    }
    public class PrintingProformaInvoiceEntityTypeConfiguration : EntityTypeConfiguration<PrintingProformaInvoice>
    {
        public PrintingProformaInvoiceEntityTypeConfiguration()
        {
            this.ToTable("PrintingProformaInvoice", "Printing");
            this.HasKey<int>(p => p.PrintingProformaInvoiceId);
        }
    }
    public class PrintingProformaInvoiceItemEntityTypeConfiguration : EntityTypeConfiguration<PrintingProformaInvoiceItem>
    {
        public PrintingProformaInvoiceItemEntityTypeConfiguration()
        {
            this.ToTable("PrintingProformaInvoiceItem", "Printing");
            this.HasKey<int>(p => p.PrintingProformaInvoiceItemId);
        }
    }
    public class PrintingEstimationMaterialCostsEntityTypeConfiguration : EntityTypeConfiguration<PrintingEstimationMaterialCost>
    {
        public PrintingEstimationMaterialCostsEntityTypeConfiguration()
        {
            this.ToTable("PrintingEstimationMaterialCost", "Printing");
            this.HasKey<int>(p => p.PrintingEstimationMaterialCostId);
        }
    }
    public class PrintingEstimationSummaryEntityTypeConfiguration : EntityTypeConfiguration<PrintingEstimationSummary>
    {
        public PrintingEstimationSummaryEntityTypeConfiguration()
        {
            this.ToTable("PrintingEstimationSummary", "Printing");
            this.HasKey<int>(p => p.PrintingEstimationSummaryId);
        }
    }
    public class PrintingEstimationDetailEntityTypeConfiguration : EntityTypeConfiguration<PrintingEstimationDetail>
    {
        public PrintingEstimationDetailEntityTypeConfiguration()
        {
            this.ToTable("PrintingEstimationDetail", "Printing");
            this.HasKey<int>(p => p.PrintingEstimationDetailId);
        }
    }
    public class PrintingEstimationCostSummaryEntityTypeConfiguration : EntityTypeConfiguration<PrintingEstimationCostSummary>
    {
        public PrintingEstimationCostSummaryEntityTypeConfiguration()
        {
            this.ToTable("PrintingEstimationCostSummary", "Printing");
            this.HasKey<int>(p => p.PrintingEstimationCostSummaryId);
        }
    }
    public class PrintingEstimationLaborCostEntityTypeConfiguration : EntityTypeConfiguration<PrintingEstimationLaborCost>
    {
        public PrintingEstimationLaborCostEntityTypeConfiguration()
        {
            this.ToTable("PrintingEstimationLaborCost", "Printing");
            this.HasKey<int>(p => p.PrintingEstimationLaborCostId);
        }
    }
    public class PrintingOverAllCostEntityTypeConfiguration : EntityTypeConfiguration<PrintingOverAllCost>
    {
        public PrintingOverAllCostEntityTypeConfiguration()
        {
            this.ToTable("PrintingOverAllCost", "Printing");
            this.HasKey<int>(p => p.PrintingOverAllCostId);
        }
    }
    public class PrintingMaterialCategoryEntityTypeConfiguration : EntityTypeConfiguration<PrintingMaterialCategory>
    {
        public PrintingMaterialCategoryEntityTypeConfiguration()
        {
            this.ToTable("PrintingMaterialCategory", "Printing");
            this.HasKey<int>(p => p.PrintingMaterialCategoryId);
        }
    }
    public class PrintingMaterialCategoryItemEntityTypeConfiguration : EntityTypeConfiguration<PrintingMaterialCategoryItem>
    {
        public PrintingMaterialCategoryItemEntityTypeConfiguration()
        {
            this.ToTable("PrintingMaterialCategoryItem", "Printing");
            this.HasKey<int>(p => p.PrintingMaterialCategoryItemId);
        }
    }
    public class PrintingBindingStyleEntityTypeConfiguration : EntityTypeConfiguration<PrintingBindingStyle>
    {
        public PrintingBindingStyleEntityTypeConfiguration()
        {
            this.ToTable("PrintingBindingStyle", "Printing");
            this.HasKey<int>(p => p.PrintingBindingStyleId);
        }
    }
   
    public class PrintingPaperSizeEntityTypeConfiguration : EntityTypeConfiguration<PrintingPaperSize>
    {
        public PrintingPaperSizeEntityTypeConfiguration()
        {
            this.ToTable("PrintingPaperSize", "Printing");
            this.HasKey<int>(p => p.PrintingPaperSizeId);
        }
    }
    public class PrintingProductProcessEntityTypeConfiguration : EntityTypeConfiguration<PrintingProductProcess>
    {
        public PrintingProductProcessEntityTypeConfiguration()
        {
            this.ToTable("PrintingProductProcess", "Printing");
            this.HasKey<int>(p => p.PrintingProductProcessId);
        }
    }
    public class PrintingCostEstimationEntityTypeConfiguration : EntityTypeConfiguration<PrintingCostEstimation>
    {
        public PrintingCostEstimationEntityTypeConfiguration()
        {
            this.ToTable("PrintingCostEstimation", "Printing");
            this.HasKey<int>(p => p.PrintingCostEstimationId);
        }
    }
    public class PrintCostCenterEntityTypeConfiguration: EntityTypeConfiguration<PrintCostCenter>
    {
        public PrintCostCenterEntityTypeConfiguration()
        {
            this.ToTable("PrintCostCenter", "Printing");
            this.HasKey<int>(p => p.PrintCostCenterId);
        }
    }
    public class PrintingMachineTypeEntityTypeConfiguration : EntityTypeConfiguration<PrintingMachineType>
    {
        public PrintingMachineTypeEntityTypeConfiguration()
        {
            this.ToTable("PrintingMachineType", "Printing");
            this.HasKey<int>(p => p.PrintingMachineTypeId);
        }
    }

    public class PrintingProcessEntityTypeConfiguration : EntityTypeConfiguration<PrintingProcess>
    {
        public PrintingProcessEntityTypeConfiguration()
        {
            this.ToTable("PrintingProcess", "Printing");
            this.HasKey<int>(p => p.PrintingProcessId);
        }
    }
    public class PrintItemCategoryEntityTypeConfiguration : EntityTypeConfiguration<PrintItemCategory>
    {
        public PrintItemCategoryEntityTypeConfiguration()
        {
            this.ToTable("PrintItemCategory", "Printing");
            this.HasKey<int>(p => p.PrintItemCategoryId);
        }
    }

    public class PrintLabourRateEntityTypeConfiguration : EntityTypeConfiguration<PrintLabourRate>
    {
        public PrintLabourRateEntityTypeConfiguration()
        {
            this.ToTable("PrintLabourRate", "Printing");
            this.HasKey<int>(p => p.PrintLaborRateId);
        }
    }
    public class PrintMachinePaperListEntityTypeConfiguration : EntityTypeConfiguration<PrintMachinePaperList>
    {
        public PrintMachinePaperListEntityTypeConfiguration()
        {
            this.ToTable("PrintMachinePaperList", "Printing");
            this.HasKey<int>(p => p.PrintMachinePaperListId);
        }
    }
    public class PrintWorkOrderTypeEntityTypeConfiguration : EntityTypeConfiguration<PrintWorkOrderType>
    {
        public PrintWorkOrderTypeEntityTypeConfiguration()
        {
            this.ToTable("PrintWorkOrderType", "Printing");
            this.HasKey<int>(p => p.PrintWorkOrderTypeId);
        }
    }
    #region printing Estimation
    class OverallCostEntityConfiguration : EntityTypeConfiguration<OverallCost>
    {
        public OverallCostEntityConfiguration()
        {
            this.ToTable("OverallCosts", "Printing");
            this.HasKey<int>(t => t.OverallCostId);
        }
    }
    class EstimationFormEntityConfiguration : EntityTypeConfiguration<EstimationForm>
    {
        public EstimationFormEntityConfiguration()
        {
            this.ToTable("EstimationForms", "Printing");
            this.HasKey<int>(t => t.EstimationFormId);
        }
    }
    class MaterialCostEntityConfiguration : EntityTypeConfiguration<MaterialCost>
    {
        public MaterialCostEntityConfiguration()
        {
            this.ToTable("MaterialCosts", "Printing");
            this.HasKey<int>(t => t.MaterialCostId);
        }
    }

    class LaborCostEntityConfiguration : EntityTypeConfiguration<LaborCost>
    {
        public LaborCostEntityConfiguration()
        {
            this.ToTable("LaborCosts", "Printing");
            this.HasKey<int>(t => t.LaborCostId);
        }
    }
   
    class PrintingStepEntityConfiguration : EntityTypeConfiguration<PrintingStep>
    {
        public PrintingStepEntityConfiguration()
        {
            this.ToTable("PrintingSteps", "Printing");
            this.HasKey<int>(t => t.PrintingStepId);
        }
    }
    public class PrintingCriteriaCatagoryEntityTypeConfiguration : EntityTypeConfiguration<PrintingCriteriaCategory>
    {
        public PrintingCriteriaCatagoryEntityTypeConfiguration()
        {
            this.ToTable("PrintingCriteriaCatagory", "Printing");
            this.HasKey<int>(t => t.PrintingCriteriaCategoryId);
        }
    }
    #endregion
   
    #region Product Felow Up
    public class YearlyProductionPlanEntityTypeConfiguration : EntityTypeConfiguration<YearlyProductionPlan>
    {

        public YearlyProductionPlanEntityTypeConfiguration()
        {
            this.ToTable("YearlyProductionPlans", "Printing");
            this.HasKey<int>(t => t.YearlyProductionPlanId);

        }
    }
    public class HalfYearlyProductionPlanEntityTypeConfiguration : EntityTypeConfiguration<HalfYearlyProductionPlan>
    {

        public HalfYearlyProductionPlanEntityTypeConfiguration()
        {
            this.ToTable("HalfYearlyProductionPlans", "Printing");
            this.HasKey<int>(t => t.HalfYearlyProductionPlanId);

        }
    }
    public class QuarterlyProductionPlanEntityTypeConfiguration : EntityTypeConfiguration<QuarterlyProductionPlan>
    {

        public QuarterlyProductionPlanEntityTypeConfiguration()
        {
            this.ToTable("QuarterlyProductionPlans", "Printing");
            this.HasKey<int>(t => t.QuarterlyProductionPlanId);

        }
    }
    public class MonthlyProductionPlanEntityTypeConfiguration : EntityTypeConfiguration<MonthlyProductionPlan>
    {

        public MonthlyProductionPlanEntityTypeConfiguration()
        {
            this.ToTable("MonthlyProductionPlans", "Printing");
            this.HasKey<int>(t => t.MonthlyProductionPlanId);

        }
    }
    public class WeeklyProductionPlanEntityTypeConfiguration : EntityTypeConfiguration<WeeklyProductionPlan>
    {

        public WeeklyProductionPlanEntityTypeConfiguration()
        {
            this.ToTable("WeeklyProductionPlans", "Printing");
            this.HasKey<int>(t => t.WeeklyProductionPlanId);

        }
    }
    public class ProductionJobTypePlanEntityTypeConfiguration : EntityTypeConfiguration<ProductionJobTypePlan>
    {

        public ProductionJobTypePlanEntityTypeConfiguration()
        {
            this.ToTable("ProductionJobTypePlans", "Printing");
            this.HasKey<int>(t => t.ProductionJobTypePlanId);

        }
    }
    public class ProductionCustomerTypeEntityTypeConfiguration : EntityTypeConfiguration<ProductionCustomerType>
    {
        public ProductionCustomerTypeEntityTypeConfiguration()
        {
            this.ToTable("ProductionCustomerTypes", "Printing");
            this.HasKey<int>(t => t.ProductionCustomerTypeId);
        }
    }
    public class ProductionMonthlyJobTypePlanEntityTypeConfiguration : EntityTypeConfiguration<ProductionMonthlyJobTypePlan>
    {
        public ProductionMonthlyJobTypePlanEntityTypeConfiguration()
        {
            this.ToTable("ProductionMonthlyJobTypePlans", "Printing");
            this.HasKey<int>(t => t.ProductionMonthlyJobTypePlanId);
        }
    }
    public class ProductionMonthlyPlanEntityTypeConfiguration : EntityTypeConfiguration<ProductionMonthlyPlan>
    {
        public ProductionMonthlyPlanEntityTypeConfiguration()
        {
            this.ToTable("ProductionMonthlyPlans", "Printing");
            this.HasKey<int>(t => t.ProductionMonthlyPlanId);
        }
    }
    public class ProductionYearlyPlanEntityTypeConfiguration : EntityTypeConfiguration<ProductionYearlyPlan>
    {
        public ProductionYearlyPlanEntityTypeConfiguration()
        {
            this.ToTable("ProductionYearlyPlans", "Printing");
            this.HasKey<int>(t => t.ProductionYearlyPlanId);
        }
    }
    public class JobStatusLineItemEntityTypeConfiguration : EntityTypeConfiguration<JobStatusLineItem>
    {
        public JobStatusLineItemEntityTypeConfiguration()
        {
            this.ToTable("JobStatusLineItems", "Printing");
            this.HasKey<int>(t => t.JobStatusLineItemId);
        }
    }
    
    public class JobCategoryEntityTypeConfiguration : EntityTypeConfiguration<JobCategory>
    {
        public JobCategoryEntityTypeConfiguration()
        {
            this.ToTable("JobCategories", "Printing");
            this.HasKey<int>(t => t.JobCategoryId);
        }
    }

    public class ProformaEntityTypeConfiguration : EntityTypeConfiguration<Proforma>
    {
        public ProformaEntityTypeConfiguration()
        {
            this.ToTable("Proforma", "Printing");
            this.HasKey<int>(t => t.ProformaId);
        }
    }
    public class PFProformaItemsEntityTypeConfiguration : EntityTypeConfiguration<PFProformaItems>
    {
        public PFProformaItemsEntityTypeConfiguration()
        {
            this.ToTable("PFProformaItems", "Printing");
            this.HasKey<int>(t => t.PFProformaItemsId);
        }
    }



    public class GraphicEntityTypeConfiguration : EntityTypeConfiguration<Graphic>
    {
        public GraphicEntityTypeConfiguration()
        {
            this.ToTable("Graphic", "Printing");
            this.HasKey<int>(t => t.GraphicID);
        }
    }


    public class JobEntityTypeConfiguration : EntityTypeConfiguration<Job>
    {
        public JobEntityTypeConfiguration()
        {
            this.ToTable("Job", "Printing");
            this.HasKey<int>(t => t.JobId);
        }
    }
    //public class JobStatusEntityTypeConfiguration : EntityTypeConfiguration<JobStatus>
    //{
    //    public JobStatusEntityTypeConfiguration()
    //    {
    //        this.ToTable("JobStatuses", "Printing");
    //        this.HasKey<int>(t => t.JobStatusId);
    //    }
    //}



   


    public class JobOrderMaterialEntityTypeConfiguration : EntityTypeConfiguration<JobOrderMaterial>
    {
        public JobOrderMaterialEntityTypeConfiguration()
        {
            this.ToTable("JobOrderMaterial", "Printing");
            this.HasKey<int>(t => t.JobOrderMaterialId);
        }
    }

    public class PrintingProductionFinishedGoodsStoreReceiveEntityTypeConfiguration : EntityTypeConfiguration<PrintingProductionFinishedGoodsStoreReceive>
    {
        public PrintingProductionFinishedGoodsStoreReceiveEntityTypeConfiguration()
        {
            this.ToTable("FinishedGoodsStoreReceive", "Printing");
            this.HasKey<int>(t => t.FinishedGoodsStoreReceiveId);
        }
    }
    public class PrintingProductionFinishedGoodsStoreReceiveItemEntityTypeConfiguration : EntityTypeConfiguration<PrintingProductionFinishedGoodsStoreReceiveItem>
    {
        public PrintingProductionFinishedGoodsStoreReceiveItemEntityTypeConfiguration()
        {
            this.ToTable("FinishedGoodsStoreReceiveItem", "Printing");
            this.HasKey<int>(t => t.FinishedGoodsStoreReceiveItemId);
        }
    }
    public class PrintingProductionFinishedGoodsStoreReceiveValidationEntityTypeConfiguration : EntityTypeConfiguration<PrintingProductionFinishedGoodsStoreReceiveValidation>
    {
        public PrintingProductionFinishedGoodsStoreReceiveValidationEntityTypeConfiguration()
        {
            this.ToTable("FinishedGoodsStoreReceiveValidation", "Printing");
            this.HasKey<int>(t => t.FinishedGoodsStoreReceiveValidationId);
        }
    }
    public class PrintingProductionFinishedGoodsStoreReceiveDistributionEntityTypeConfiguration : EntityTypeConfiguration<PrintingProductionFinishedGoodsStoreReceiveDistribution>
    {
        public PrintingProductionFinishedGoodsStoreReceiveDistributionEntityTypeConfiguration()
        {
            this.ToTable("FinishedGoodsStoreReceiveDistribution", "Printing");
            this.HasKey<int>(t => t.FinishedGoodsStoreReceiveDistributionId);
        }
    }

    public class FinishedGoodsEntityTypeConfiguration : EntityTypeConfiguration<FinishedGoods>
    {
        public FinishedGoodsEntityTypeConfiguration()
        {
            this.ToTable("FinishedGoodItems", "Printing");
            this.HasKey<int>(t => t.FinishedGoodsId);
        }
    }

    public class ProductDeliveryEntityTypeConfiguration : EntityTypeConfiguration<ProductDelivery>
    {
        public ProductDeliveryEntityTypeConfiguration()
        {
            this.ToTable("ProductDelivery", "Printing");
            this.HasKey<int>(t => t.ProductDeliveryId);
        }
    }

   
  
    #endregion




}
