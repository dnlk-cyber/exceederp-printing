namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class priningjobfollowup : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "Printing.JobStatuses", newName: "JobStatus");
            MoveTable(name: "Printing.JobStatus", newSchema: "EXCEED");
            CreateTable(
                "Printing.PressDailyFollowUpWastage",
                c => new
                    {
                        PressDailyFollowUpWastageId = c.Int(nullable: false, identity: true),
                        UOM = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PrintingProductionDailyFollowUpDetailId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PressDailyFollowUpWastageId);
            
            CreateTable(
                "Printing.PrintingDailyFollowUpLogBook",
                c => new
                    {
                        PrintingDailyFollowUpLogBookId = c.Int(nullable: false, identity: true),
                        ActivityType = c.String(),
                        TimeSpent = c.Double(nullable: false),
                        PrintingProductionDailyFollowUpDetailId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PrintingDailyFollowUpLogBookId);
            
            CreateTable(
                "Printing.PrintingDailyFollowUpMachineMaintenance",
                c => new
                    {
                        PrintingDailyFollowUpMachineMaintenanceId = c.Int(nullable: false, identity: true),
                        Reason = c.String(),
                        IssueReportedTime = c.DateTime(nullable: false),
                        MaintenanceStartTime = c.DateTime(),
                        MaintenanceEndTime = c.DateTime(),
                        MaintenanceTime = c.Double(nullable: false),
                        TechnicianId = c.Int(nullable: false),
                        Remark = c.String(),
                        PrintingProductionDailyFollowUpDetailId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PrintingDailyFollowUpMachineMaintenanceId);
            
            CreateTable(
                "Printing.PrintingTaskIssue",
                c => new
                    {
                        PrintingTaskIssueId = c.Int(nullable: false, identity: true),
                        IssueType = c.Int(nullable: false),
                        Description = c.String(),
                        ReportedTime = c.DateTime(nullable: false),
                        ResolvedTime = c.DateTime(),
                        Resolution = c.String(),
                        TimeSpentInMinute = c.Double(nullable: false),
                        PrintingProductionDailyFollowUpDetailId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PrintingTaskIssueId);
            
            AddColumn("Printing.PrintingProductionFollowUp", "PrintingOperationStatus", c => c.Int(nullable: false));
            AddColumn("Printing.PrintingProductionFollowUp", "IsPrePressChecked", c => c.Boolean(nullable: false));
            AddColumn("Printing.PrintingProductionFollowUp", "IsPressChecked", c => c.Boolean(nullable: false));
            AddColumn("Printing.PrintingProductionFollowUp", "IsPostPressChecked", c => c.Boolean(nullable: false));
            AddColumn("Printing.PrintingProductionFollowUp", "PrePressCheckedBy", c => c.String());
            AddColumn("Printing.PrintingProductionFollowUp", "PressCheckedBy", c => c.String());
            AddColumn("Printing.PrintingProductionFollowUp", "PostPressCheckedBy", c => c.String());
            AddColumn("Printing.PrintingProductionFollowUp", "PrePressCheckedTime", c => c.DateTime());
            AddColumn("Printing.PrintingProductionFollowUp", "PressCheckedTime", c => c.DateTime());
            AddColumn("Printing.PrintingProductionFollowUp", "PostPressCheckedTime", c => c.DateTime());
            AddColumn("Printing.PrintingProductionFollowUp", "IsPrePressApproved", c => c.Boolean(nullable: false));
            AddColumn("Printing.PrintingProductionFollowUp", "IsPressApproved", c => c.Boolean(nullable: false));
            AddColumn("Printing.PrintingProductionFollowUp", "IsPostPressApproved", c => c.Boolean(nullable: false));
            AddColumn("Printing.PrintingProductionFollowUp", "PrePressApprovedBy", c => c.String());
            AddColumn("Printing.PrintingProductionFollowUp", "PressApprovedBy", c => c.String());
            AddColumn("Printing.PrintingProductionFollowUp", "PostPressApprovedBy", c => c.String());
            AddColumn("Printing.PrintingProductionFollowUp", "PrePressApprovedTime", c => c.DateTime());
            AddColumn("Printing.PrintingProductionFollowUp", "PressApprovedTime", c => c.DateTime());
            AddColumn("Printing.PrintingProductionFollowUp", "PostPressApprovedTime", c => c.DateTime());
            DropColumn("Printing.PrintingProductionFollowUp", "OperationStatus");
            DropTable("Printing.PrintingDailyFollowUpDelayReason");
        }
        
        public override void Down()
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
            
            AddColumn("Printing.PrintingProductionFollowUp", "OperationStatus", c => c.Int(nullable: false));
            DropColumn("Printing.PrintingProductionFollowUp", "PostPressApprovedTime");
            DropColumn("Printing.PrintingProductionFollowUp", "PressApprovedTime");
            DropColumn("Printing.PrintingProductionFollowUp", "PrePressApprovedTime");
            DropColumn("Printing.PrintingProductionFollowUp", "PostPressApprovedBy");
            DropColumn("Printing.PrintingProductionFollowUp", "PressApprovedBy");
            DropColumn("Printing.PrintingProductionFollowUp", "PrePressApprovedBy");
            DropColumn("Printing.PrintingProductionFollowUp", "IsPostPressApproved");
            DropColumn("Printing.PrintingProductionFollowUp", "IsPressApproved");
            DropColumn("Printing.PrintingProductionFollowUp", "IsPrePressApproved");
            DropColumn("Printing.PrintingProductionFollowUp", "PostPressCheckedTime");
            DropColumn("Printing.PrintingProductionFollowUp", "PressCheckedTime");
            DropColumn("Printing.PrintingProductionFollowUp", "PrePressCheckedTime");
            DropColumn("Printing.PrintingProductionFollowUp", "PostPressCheckedBy");
            DropColumn("Printing.PrintingProductionFollowUp", "PressCheckedBy");
            DropColumn("Printing.PrintingProductionFollowUp", "PrePressCheckedBy");
            DropColumn("Printing.PrintingProductionFollowUp", "IsPostPressChecked");
            DropColumn("Printing.PrintingProductionFollowUp", "IsPressChecked");
            DropColumn("Printing.PrintingProductionFollowUp", "IsPrePressChecked");
            DropColumn("Printing.PrintingProductionFollowUp", "PrintingOperationStatus");
            DropTable("Printing.PrintingTaskIssue");
            DropTable("Printing.PrintingDailyFollowUpMachineMaintenance");
            DropTable("Printing.PrintingDailyFollowUpLogBook");
            DropTable("Printing.PressDailyFollowUpWastage");
            MoveTable(name: "EXCEED.JobStatus", newSchema: "Printing");
            RenameTable(name: "Printing.JobStatus", newName: "JobStatuses");
        }
    }
}
