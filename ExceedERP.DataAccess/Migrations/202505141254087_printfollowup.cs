namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printfollowup : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Printing.PrintingDailyFollowUpDelayReason",
                c => new
                    {
                        PrintingDailyFollowUpDelayReasonId = c.Int(nullable: false, identity: true),
                        Reason = c.String(),
                        Description = c.String(),
                        PrintingProductionDailyFollowUpDetailId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PrintingDailyFollowUpDelayReasonId);
            
            CreateTable(
                "Printing.PrintingProductionAllottedRawMaterial",
                c => new
                    {
                        PrintingProductionAllottedRawMaterialId = c.Int(nullable: false, identity: true),
                        PrintingMaterialCategoryId = c.Int(nullable: false),
                        PrintingMaterialCategoryItemId = c.Int(nullable: false),
                        UnitOfMeasurment = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PrintingProductionFollowUpId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PrintingProductionAllottedRawMaterialId);
            
            CreateTable(
                "Printing.PrintingProductionDailyFollowUpDetail",
                c => new
                    {
                        PrintingProductionDailyFollowUpDetailId = c.Int(nullable: false, identity: true),
                        OperatorId = c.Int(nullable: false),
                        ProductionStartTime = c.DateTime(nullable: false),
                        ProductionEndTime = c.DateTime(nullable: false),
                        UoM = c.String(),
                        ActualOutput = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TimeSpentHours = c.Double(nullable: false),
                        Remark = c.String(),
                        PrintingProductionFollowUpTaskId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PrintingProductionDailyFollowUpDetailId);
            
            CreateTable(
                "Printing.PrintingProductionFollowUp",
                c => new
                    {
                        PrintingProductionFollowUpId = c.Int(nullable: false, identity: true),
                        JobOrderId = c.Int(nullable: false),
                        CustomerId = c.Int(nullable: false),
                        PrintJobType = c.Int(nullable: false),
                        BindingStyle = c.Int(nullable: false),
                        TextPaperSizeId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        Pages = c.Int(nullable: false),
                        JobDescription = c.String(),
                        OperationStatus = c.Int(nullable: false),
                        IsPrePressChecked = c.Boolean(nullable: false),
                        IsPressChecked = c.Boolean(nullable: false),
                        IsPostPressChecked = c.Boolean(nullable: false),
                        PrePressCheckedBy = c.String(),
                        PressCheckedBy = c.String(),
                        PostPressCheckedBy = c.String(),
                        PrePressCheckedTime = c.DateTime(),
                        PressCheckedTime = c.DateTime(),
                        PostPressCheckedTime = c.DateTime(),
                        IsPrePressApproved = c.Boolean(nullable: false),
                        IsPressApproved = c.Boolean(nullable: false),
                        IsPostPressApproved = c.Boolean(nullable: false),
                        PrePressApprovedBy = c.String(),
                        PressApprovedBy = c.String(),
                        PostPressApprovedBy = c.String(),
                        PrePressApprovedTime = c.DateTime(),
                        PressApprovedTime = c.DateTime(),
                        PostPressApprovedTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.PrintingProductionFollowUpId);
            
            CreateTable(
                "Printing.PrintingProductionFollowUpTask",
                c => new
                    {
                        PrintingProductionFollowUpTaskId = c.Int(nullable: false, identity: true),
                        PressCategory = c.Int(nullable: false),
                        LabourActivity = c.Int(nullable: false),
                        PrintingmachineTypeId = c.Int(),
                        OperatorId = c.Int(nullable: false),
                        UoM = c.String(),
                        PlannedOutput = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EstimatedHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        PrintingProductionFollowUpId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PrintingProductionFollowUpTaskId);
            
            AddColumn("Printing.PrintingProductionLaborCost", "LabourActivity", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Printing.PrintingProductionLaborCost", "LabourActivity");
            DropTable("Printing.PrintingProductionFollowUpTask");
            DropTable("Printing.PrintingProductionFollowUp");
            DropTable("Printing.PrintingProductionDailyFollowUpDetail");
            DropTable("Printing.PrintingProductionAllottedRawMaterial");
            DropTable("Printing.PrintingDailyFollowUpDelayReason");
        }
    }
}
