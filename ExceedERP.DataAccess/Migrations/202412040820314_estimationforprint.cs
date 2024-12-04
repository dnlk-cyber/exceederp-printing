namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class estimationforprint : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Printing.PrintingEstimationCostSummary",
                c => new
                    {
                        PrintingEstimationCostSummaryId = c.Int(nullable: false, identity: true),
                        CommulativeCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProfitMargin = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FinalPriceIncludingProfit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsApprovedMargin = c.Boolean(nullable: false),
                        PrintingEstimationSummaryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PrintingEstimationCostSummaryId);
            
            CreateTable(
                "Printing.PrintingEstimationDetail",
                c => new
                    {
                        PrintingEstimationDetailId = c.Int(nullable: false, identity: true),
                        JobTypeId = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaterialCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LabourCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverHeadCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ManufacturingCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AdministrativeCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GrandTotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PrintingEstimationSummaryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PrintingEstimationDetailId);
            
            CreateTable(
                "Printing.PrintingEstimationSummary",
                c => new
                    {
                        PrintingEstimationSummaryId = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        IsClosed = c.Boolean(nullable: false),
                        IsCalculated = c.Boolean(nullable: false),
                        PrintingCostEstimationId = c.Int(nullable: false),
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
                .PrimaryKey(t => t.PrintingEstimationSummaryId);
            
            AddColumn("Printing.PrintingCostEstimation", "IsMarginAssigned", c => c.Boolean(nullable: false));
            AddColumn("Printing.PrintingCostEstimation", "Status", c => c.String());
            AddColumn("Printing.PrintingCostEstimation", "Remark", c => c.String());
            AddColumn("Printing.PrintingCostEstimation", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("Printing.PrintingCostEstimation", "VoidBy", c => c.String());
            AddColumn("Printing.PrintingCostEstimation", "VoidTime", c => c.DateTime());
            AddColumn("Printing.PrintingCostEstimation", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("Printing.PrintingCostEstimation", "OnlineApprovedBy", c => c.String());
            AddColumn("Printing.PrintingCostEstimation", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("Printing.PrintingCostEstimation", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("Printing.PrintingCostEstimation", "IsOnlineTransferredBy", c => c.String());
            AddColumn("Printing.PrintingCostEstimation", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("Printing.PrintingCostEstimation", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Printing.PrintingCostEstimation", "OrgBranchName", c => c.String());
            AddColumn("Printing.PrintingCostEstimation", "SendForApprovalBy", c => c.String());
            AddColumn("Printing.PrintingCostEstimation", "SendForApprovalTime", c => c.DateTime());
            AddColumn("Printing.PrintingEstimationLaborCost", "PrintingMachineTypeId", c => c.Int());
            AddColumn("Printing.PrintingOverAllCost", "OverHeadCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Printing.PrintingOverAllCost", "SellingPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Printing.PrintingEstimationLaborCost", "PrintingProcessId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("Printing.PrintingEstimationLaborCost", "PrintingProcessId", c => c.Int(nullable: false));
            DropColumn("Printing.PrintingOverAllCost", "SellingPrice");
            DropColumn("Printing.PrintingOverAllCost", "OverHeadCost");
            DropColumn("Printing.PrintingEstimationLaborCost", "PrintingMachineTypeId");
            DropColumn("Printing.PrintingCostEstimation", "SendForApprovalTime");
            DropColumn("Printing.PrintingCostEstimation", "SendForApprovalBy");
            DropColumn("Printing.PrintingCostEstimation", "OrgBranchName");
            DropColumn("Printing.PrintingCostEstimation", "IsSendForApproval");
            DropColumn("Printing.PrintingCostEstimation", "OnlineTransferredTime");
            DropColumn("Printing.PrintingCostEstimation", "IsOnlineTransferredBy");
            DropColumn("Printing.PrintingCostEstimation", "IsOnlineTransferred");
            DropColumn("Printing.PrintingCostEstimation", "OnlineApprovedTime");
            DropColumn("Printing.PrintingCostEstimation", "OnlineApprovedBy");
            DropColumn("Printing.PrintingCostEstimation", "IsOnlineApproved");
            DropColumn("Printing.PrintingCostEstimation", "VoidTime");
            DropColumn("Printing.PrintingCostEstimation", "VoidBy");
            DropColumn("Printing.PrintingCostEstimation", "IsVoid");
            DropColumn("Printing.PrintingCostEstimation", "Remark");
            DropColumn("Printing.PrintingCostEstimation", "Status");
            DropColumn("Printing.PrintingCostEstimation", "IsMarginAssigned");
            DropTable("Printing.PrintingEstimationSummary");
            DropTable("Printing.PrintingEstimationDetail");
            DropTable("Printing.PrintingEstimationCostSummary");
        }
    }
}
