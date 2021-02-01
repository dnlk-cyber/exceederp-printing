namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class propertyroad : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.DefaultBranch",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        BranchId = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "EXCEED.OrgBranch",
                c => new
                    {
                        OrgBranchID = c.Int(nullable: false, identity: true),
                        OrgBranchName = c.String(nullable: false),
                        OrgBranchAbbreviation = c.String(nullable: false),
                        BranchAccountCode = c.String(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.OrgBranchID);
            
            CreateTable(
                "EXCEED.ProcPurchaseValidation",
                c => new
                    {
                        ProcPurchaseValidationId = c.Int(nullable: false, identity: true),
                        PurchaseType = c.Int(nullable: false),
                        PurchaseRequisitionId = c.Int(nullable: false),
                        IsOnlineApproved = c.Boolean(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProcPurchaseValidationId)
                .ForeignKey("EXCEED.PurchaseRequisition", t => t.PurchaseRequisitionId, cascadeDelete: true)
                .Index(t => t.PurchaseRequisitionId);
            
            CreateTable(
                "EXCEED.PurchaseValidationItem",
                c => new
                    {
                        PurchaseValidationItemId = c.Int(nullable: false, identity: true),
                        ItemCode = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Unit = c.String(),
                        PartNo = c.String(),
                        ProcPurchaseValidationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PurchaseValidationItemId)
                .ForeignKey("EXCEED.ProcPurchaseValidation", t => t.ProcPurchaseValidationId, cascadeDelete: true)
                .Index(t => t.ProcPurchaseValidationId);
            
            CreateTable(
                "EXCEED.PurchaseValidationApproval",
                c => new
                    {
                        PurchaseValidationApprovalId = c.Int(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        ProcPurchaseValidationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PurchaseValidationApprovalId)
                .ForeignKey("EXCEED.ProcPurchaseValidation", t => t.ProcPurchaseValidationId, cascadeDelete: true)
                .Index(t => t.ProcPurchaseValidationId);
            
            AddColumn("EXCEED.TenderMessages", "SentDate", c => c.DateTime(nullable: false));
            AddColumn("EXCEED.TenderMessages", "ReciveDate", c => c.DateTime(nullable: false));
            DropColumn("EXCEED.OrgBranchUserMapping", "CreatedOn");
            DropColumn("EXCEED.OrgBranchUserMapping", "CreatedBy");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.OrgBranchUserMapping", "CreatedBy", c => c.String());
            AddColumn("EXCEED.OrgBranchUserMapping", "CreatedOn", c => c.DateTime(nullable: false));
            DropForeignKey("EXCEED.PurchaseValidationApproval", "ProcPurchaseValidationId", "EXCEED.ProcPurchaseValidation");
            DropForeignKey("EXCEED.PurchaseValidationItem", "ProcPurchaseValidationId", "EXCEED.ProcPurchaseValidation");
            DropForeignKey("EXCEED.ProcPurchaseValidation", "PurchaseRequisitionId", "EXCEED.PurchaseRequisition");
            DropIndex("EXCEED.PurchaseValidationApproval", new[] { "ProcPurchaseValidationId" });
            DropIndex("EXCEED.PurchaseValidationItem", new[] { "ProcPurchaseValidationId" });
            DropIndex("EXCEED.ProcPurchaseValidation", new[] { "PurchaseRequisitionId" });
            DropColumn("EXCEED.TenderMessages", "ReciveDate");
            DropColumn("EXCEED.TenderMessages", "SentDate");
            DropTable("EXCEED.PurchaseValidationApproval");
            DropTable("EXCEED.PurchaseValidationItem");
            DropTable("EXCEED.ProcPurchaseValidation");
            DropTable("EXCEED.OrgBranch");
            DropTable("EXCEED.DefaultBranch");
        }
    }
}
