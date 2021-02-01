namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class supplier : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.SupplierBlackList",
                c => new
                    {
                        SupplierBlackListId = c.Int(nullable: false, identity: true),
                        ProcurementSupplierID = c.Int(nullable: false),
                        Reason = c.String(nullable: false),
                        BlackListType = c.Int(nullable: false),
                        DurationDays = c.Int(nullable: false),
                        DateBlackListed = c.DateTime(nullable: false),
                        DateBlackListedExpire = c.DateTime(nullable: false),
                        Remark = c.String(),
                        Status = c.String(),
                        isOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SupplierBlackListId)
                .ForeignKey("EXCEED.ProcurementSupplier", t => t.ProcurementSupplierID, cascadeDelete: true)
                .Index(t => t.ProcurementSupplierID);
            
            CreateTable(
                "EXCEED.SupplierBlackListValidation",
                c => new
                    {
                        SupplierBlackListValidationId = c.Int(nullable: false, identity: true),
                        SupplierBlackListId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SupplierBlackListValidationId)
                .ForeignKey("EXCEED.SupplierBlackList", t => t.SupplierBlackListId, cascadeDelete: true)
                .Index(t => t.SupplierBlackListId);
            
            CreateTable(
                "EXCEED.SupplierPerformance",
                c => new
                    {
                        SupplierPerformanceId = c.Int(nullable: false, identity: true),
                        ProcurementSupplierID = c.Int(nullable: false),
                        ProfurementRef = c.String(nullable: false),
                        Quality = c.String(nullable: false),
                        PlannedDeliveryPeriod = c.String(nullable: false),
                        ActualDeliveryPeriod = c.String(nullable: false),
                        Remark = c.String(),
                        Status = c.String(),
                        isOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SupplierPerformanceId)
                .ForeignKey("EXCEED.ProcurementSupplier", t => t.ProcurementSupplierID, cascadeDelete: true)
                .Index(t => t.ProcurementSupplierID);
            
            CreateTable(
                "EXCEED.SupplierPerformanceValidation",
                c => new
                    {
                        SupplierPerformanceValidationId = c.Int(nullable: false, identity: true),
                        SupplierPerformanceId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SupplierPerformanceValidationId)
                .ForeignKey("EXCEED.SupplierPerformance", t => t.SupplierPerformanceId, cascadeDelete: true)
                .Index(t => t.SupplierPerformanceId);
            
            CreateTable(
                "EXCEED.SupplierRemoveBlackList",
                c => new
                    {
                        SupplierRemoveBlackListId = c.Int(nullable: false, identity: true),
                        ProcurementSupplierID = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                        DateBlackListed = c.DateTime(nullable: false),
                        ClearedOn = c.DateTime(nullable: false),
                        Remark = c.String(),
                        Status = c.String(),
                        isOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        isVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SupplierRemoveBlackListId)
                .ForeignKey("EXCEED.ProcurementSupplier", t => t.ProcurementSupplierID, cascadeDelete: true)
                .Index(t => t.ProcurementSupplierID);
            
            CreateTable(
                "EXCEED.SupplierRemoveBlackListValidation",
                c => new
                    {
                        SupplierRemoveBlackListValidationId = c.Int(nullable: false, identity: true),
                        SupplierRemoveBlackListId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SupplierRemoveBlackListValidationId)
                .ForeignKey("EXCEED.SupplierRemoveBlackList", t => t.SupplierRemoveBlackListId, cascadeDelete: true)
                .Index(t => t.SupplierRemoveBlackListId);
            
            AddColumn("EXCEED.ProcurementSupplier", "TIN", c => c.String());
            AddColumn("EXCEED.ProcurementSupplier", "Telephone", c => c.String());
            AddColumn("EXCEED.ProcurementSupplier", "Fax", c => c.String());
            AddColumn("EXCEED.ProcurementSupplier", "Email", c => c.String());
            AddColumn("EXCEED.ProcurementSupplier", "IsBlackListed", c => c.Boolean(nullable: false));
            DropColumn("EXCEED.ProcurementSupplier", "FormerContractPerformance");
            DropColumn("EXCEED.ProcurementSupplier", "Account");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ProcurementSupplier", "Account", c => c.String());
            AddColumn("EXCEED.ProcurementSupplier", "FormerContractPerformance", c => c.String());
            DropForeignKey("EXCEED.SupplierRemoveBlackListValidation", "SupplierRemoveBlackListId", "EXCEED.SupplierRemoveBlackList");
            DropForeignKey("EXCEED.SupplierRemoveBlackList", "ProcurementSupplierID", "EXCEED.ProcurementSupplier");
            DropForeignKey("EXCEED.SupplierPerformanceValidation", "SupplierPerformanceId", "EXCEED.SupplierPerformance");
            DropForeignKey("EXCEED.SupplierPerformance", "ProcurementSupplierID", "EXCEED.ProcurementSupplier");
            DropForeignKey("EXCEED.SupplierBlackListValidation", "SupplierBlackListId", "EXCEED.SupplierBlackList");
            DropForeignKey("EXCEED.SupplierBlackList", "ProcurementSupplierID", "EXCEED.ProcurementSupplier");
            DropIndex("EXCEED.SupplierRemoveBlackListValidation", new[] { "SupplierRemoveBlackListId" });
            DropIndex("EXCEED.SupplierRemoveBlackList", new[] { "ProcurementSupplierID" });
            DropIndex("EXCEED.SupplierPerformanceValidation", new[] { "SupplierPerformanceId" });
            DropIndex("EXCEED.SupplierPerformance", new[] { "ProcurementSupplierID" });
            DropIndex("EXCEED.SupplierBlackListValidation", new[] { "SupplierBlackListId" });
            DropIndex("EXCEED.SupplierBlackList", new[] { "ProcurementSupplierID" });
            DropColumn("EXCEED.ProcurementSupplier", "IsBlackListed");
            DropColumn("EXCEED.ProcurementSupplier", "Email");
            DropColumn("EXCEED.ProcurementSupplier", "Fax");
            DropColumn("EXCEED.ProcurementSupplier", "Telephone");
            DropColumn("EXCEED.ProcurementSupplier", "TIN");
            DropTable("EXCEED.SupplierRemoveBlackListValidation");
            DropTable("EXCEED.SupplierRemoveBlackList");
            DropTable("EXCEED.SupplierPerformanceValidation");
            DropTable("EXCEED.SupplierPerformance");
            DropTable("EXCEED.SupplierBlackListValidation");
            DropTable("EXCEED.SupplierBlackList");
        }
    }
}
