namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class documentapproval : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ApprovalGroupRule",
                c => new
                    {
                        ApprovalGroupRuleId = c.Int(nullable: false, identity: true),
                        ApprovalGroupId = c.Int(nullable: false),
                        RuleObject = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        AmountLimit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LowValue = c.String(),
                        HighValue = c.String(),
                    })
                .PrimaryKey(t => t.ApprovalGroupRuleId)
                .ForeignKey("EXCEED.ApprovalGroup", t => t.ApprovalGroupId, cascadeDelete: true)
                .Index(t => t.ApprovalGroupId);
            
            CreateTable(
                "EXCEED.ApprovalGroup",
                c => new
                    {
                        ApprovalGroupId = c.Int(nullable: false, identity: true),
                        OpertatingUnit = c.String(),
                        Name = c.String(),
                        Enabled = c.Boolean(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ApprovalGroupId);
            
            CreateTable(
                "EXCEED.AssignApprovalGroupAssignment",
                c => new
                    {
                        AssignApprovalGroupAssignmentId = c.Int(nullable: false, identity: true),
                        AssignApprovalGroupId = c.Int(nullable: false),
                        DocumentType = c.Int(nullable: false),
                        ApprovalGroupId = c.Int(nullable: false),
                        From = c.DateTime(nullable: false),
                        To = c.DateTime(),
                    })
                .PrimaryKey(t => t.AssignApprovalGroupAssignmentId)
                .ForeignKey("EXCEED.ApprovalGroup", t => t.ApprovalGroupId, cascadeDelete: true)
                .ForeignKey("EXCEED.AssignApprovalGroup", t => t.AssignApprovalGroupId, cascadeDelete: true)
                .Index(t => t.AssignApprovalGroupId)
                .Index(t => t.ApprovalGroupId);
            
            CreateTable(
                "EXCEED.AssignApprovalGroup",
                c => new
                    {
                        AssignApprovalGroupId = c.Int(nullable: false, identity: true),
                        OpertatingUnit = c.String(),
                        Organization = c.String(),
                        DocumentType = c.Int(nullable: false),
                        Position = c.String(),
                        Job = c.String(),
                        AmountLimit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LowValue = c.String(),
                        HighValue = c.String(),
                    })
                .PrimaryKey(t => t.AssignApprovalGroupId);
            
            CreateTable(
                "EXCEED.DocumentApprovalRule",
                c => new
                    {
                        DocumentApprovalRuleId = c.Int(nullable: false, identity: true),
                        OwnerCanApprove = c.Boolean(nullable: false),
                        ApproverCanModify = c.Boolean(nullable: false),
                        CanChangeForwardTo = c.Boolean(nullable: false),
                        ApprovalWorkFlow = c.Int(nullable: false),
                        WorkFlowStartProcess = c.String(),
                        ForwardMethod = c.Int(nullable: false),
                        DefaultHierarchy = c.String(),
                        SecurityLevel = c.String(),
                        AccessLevel = c.String(),
                        ArchiveOn = c.String(),
                    })
                .PrimaryKey(t => t.DocumentApprovalRuleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.AssignApprovalGroupAssignment", "AssignApprovalGroupId", "EXCEED.AssignApprovalGroup");
            DropForeignKey("EXCEED.AssignApprovalGroupAssignment", "ApprovalGroupId", "EXCEED.ApprovalGroup");
            DropForeignKey("EXCEED.ApprovalGroupRule", "ApprovalGroupId", "EXCEED.ApprovalGroup");
            DropIndex("EXCEED.AssignApprovalGroupAssignment", new[] { "ApprovalGroupId" });
            DropIndex("EXCEED.AssignApprovalGroupAssignment", new[] { "AssignApprovalGroupId" });
            DropIndex("EXCEED.ApprovalGroupRule", new[] { "ApprovalGroupId" });
            DropTable("EXCEED.DocumentApprovalRule");
            DropTable("EXCEED.AssignApprovalGroup");
            DropTable("EXCEED.AssignApprovalGroupAssignment");
            DropTable("EXCEED.ApprovalGroup");
            DropTable("EXCEED.ApprovalGroupRule");
        }
    }
}
