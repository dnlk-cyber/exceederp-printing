namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class manufactuiringupg : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Manufacturing.EstimationCostSummary",
                c => new
                    {
                        EstimationCostSummaryId = c.Int(nullable: false, identity: true),
                        CommulativeCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProfitMargin = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FinalPriceIncludingProfit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsApprovedMargin = c.Boolean(nullable: false),
                        EstimationSummaryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EstimationCostSummaryId);
            
            CreateTable(
                "Manufacturing.EstimationDetail",
                c => new
                    {
                        EstimationDetailId = c.Int(nullable: false, identity: true),
                        JobTypeId = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaterialCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LabourCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverHeadCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ManufacturingCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AdministrativeCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GrandTotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EstimationSummaryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EstimationDetailId);
            
            CreateTable(
                "Manufacturing.EstimationSummary",
                c => new
                    {
                        EstimationSummaryId = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        IsClosed = c.Boolean(nullable: false),
                        IsCalculated = c.Boolean(nullable: false),
                        IsMarginApproved = c.Boolean(nullable: false),
                        FurnitureEstimationId = c.Int(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        IsVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        IsOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        IsOnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        SendForApprovalBy = c.String(),
                        SendForApprovalTime = c.DateTime(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EstimationSummaryId);
            
            CreateTable(
                "Manufacturing.FurnitureBoqDocument",
                c => new
                    {
                        FurnitureBoqDocumentId = c.Int(nullable: false, identity: true),
                        FurnitureBoqRequestId = c.Int(nullable: false),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureBoqDocumentId);
            
            CreateTable(
                "Manufacturing.FurnitureBoqJobDesign",
                c => new
                    {
                        FurnitureBoqJobDesignId = c.Int(nullable: false, identity: true),
                        ReferenceId = c.Int(nullable: false),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                        JobReferenceId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FurnitureBoqJobDesignId);
            
            CreateTable(
                "Manufacturing.FurnitureBoqRequest",
                c => new
                    {
                        FurnitureBoqRequestId = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        JobType = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureBoqRequestId);
            
            CreateTable(
                "Manufacturing.FurnitureJobDesign",
                c => new
                    {
                        FurnitureJobDesignId = c.Int(nullable: false, identity: true),
                        FurnitureJobOrderProductionId = c.Int(nullable: false),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureJobDesignId);
            
            AddColumn("Manufacturing.FurnitureCostEstimations", "IsMarginAssigned", c => c.Boolean(nullable: false));
            AddColumn("Manufacturing.FurnitureCostEstimations", "JobCreated", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.FurnitureJobOrderForm", "JobStartDate", c => c.DateTime(nullable: false));
            AddColumn("Manufacturing.FurnitureJobOrderItem", "TransportCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Manufacturing.FurnitureJobOrderItem", "Remark", c => c.String());
            AddColumn("Manufacturing.FurnitureOverallCosts", "SellingPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Manufacturing.FurnitureProformaInvoiceItem", "TransportCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Manufacturing.FurnitureProformaInvoiceItem", "GLTaxRateID", c => c.Int(nullable: false));
            DropColumn("Manufacturing.FurnitureOverallCosts", "FinalPriceIncludingProfit");
        }
        
        public override void Down()
        {
            AddColumn("Manufacturing.FurnitureOverallCosts", "FinalPriceIncludingProfit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("Manufacturing.FurnitureProformaInvoiceItem", "GLTaxRateID");
            DropColumn("Manufacturing.FurnitureProformaInvoiceItem", "TransportCost");
            DropColumn("Manufacturing.FurnitureOverallCosts", "SellingPrice");
            DropColumn("Manufacturing.FurnitureJobOrderItem", "Remark");
            DropColumn("Manufacturing.FurnitureJobOrderItem", "TransportCost");
            DropColumn("EXCEED.FurnitureJobOrderForm", "JobStartDate");
            DropColumn("Manufacturing.FurnitureCostEstimations", "JobCreated");
            DropColumn("Manufacturing.FurnitureCostEstimations", "IsMarginAssigned");
            DropTable("Manufacturing.FurnitureJobDesign");
            DropTable("Manufacturing.FurnitureBoqRequest");
            DropTable("Manufacturing.FurnitureBoqJobDesign");
            DropTable("Manufacturing.FurnitureBoqDocument");
            DropTable("Manufacturing.EstimationSummary");
            DropTable("Manufacturing.EstimationDetail");
            DropTable("Manufacturing.EstimationCostSummary");
        }
    }
}
