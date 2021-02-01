namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class servicereq : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ServiceRequisitionItem",
                c => new
                    {
                        ServiceRequisitionItemId = c.Int(nullable: false, identity: true),
                        PurchaseRequisitionID = c.Int(nullable: false),
                        ServiceCategoryCode = c.String(nullable: false, maxLength: 128),
                        ServiceItemCode = c.String(nullable: false, maxLength: 128),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsApproved = c.Boolean(nullable: false),
                        ItemSpecification = c.String(),
                        UoM = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ServiceRequisitionItemId)
                .ForeignKey("EXCEED.PurchaseRequisition", t => t.PurchaseRequisitionID, cascadeDelete: true)
                .ForeignKey("EXCEED.ServiceCategory", t => t.ServiceCategoryCode, cascadeDelete: true)
                .ForeignKey("EXCEED.ServiceItem", t => t.ServiceItemCode, cascadeDelete: true)
                .Index(t => t.PurchaseRequisitionID)
                .Index(t => t.ServiceCategoryCode)
                .Index(t => t.ServiceItemCode);
            
            CreateTable(
                "EXCEED.ServiceCategory",
                c => new
                    {
                        ServiceCategoryCode = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        AccountCode = c.String(nullable: false),
                        IssueAccountCode = c.String(nullable: false),
                        HaveParent = c.Boolean(nullable: false),
                        ParentName = c.String(),
                        Abbreviation = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ServiceCategoryCode);
            
            CreateTable(
                "EXCEED.ServiceItem",
                c => new
                    {
                        ServiceItemCode = c.String(nullable: false, maxLength: 128),
                        ServiceCategoryCode = c.String(maxLength: 128),
                        Name = c.String(nullable: false),
                        Type = c.Int(nullable: false),
                        UoMName = c.String(maxLength: 128),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AvgPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HasTransaction = c.Boolean(nullable: false),
                        Enable = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ServiceItemCode)
                .ForeignKey("EXCEED.ServiceCategory", t => t.ServiceCategoryCode)
                .ForeignKey("EXCEED.UoM", t => t.UoMName)
                .Index(t => t.ServiceCategoryCode)
                .Index(t => t.UoMName);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.ServiceRequisitionItem", "ServiceItemCode", "EXCEED.ServiceItem");
            DropForeignKey("EXCEED.ServiceRequisitionItem", "ServiceCategoryCode", "EXCEED.ServiceCategory");
            DropForeignKey("EXCEED.ServiceItem", "UoMName", "EXCEED.UoM");
            DropForeignKey("EXCEED.ServiceItem", "ServiceCategoryCode", "EXCEED.ServiceCategory");
            DropForeignKey("EXCEED.ServiceRequisitionItem", "PurchaseRequisitionID", "EXCEED.PurchaseRequisition");
            DropIndex("EXCEED.ServiceItem", new[] { "UoMName" });
            DropIndex("EXCEED.ServiceItem", new[] { "ServiceCategoryCode" });
            DropIndex("EXCEED.ServiceRequisitionItem", new[] { "ServiceItemCode" });
            DropIndex("EXCEED.ServiceRequisitionItem", new[] { "ServiceCategoryCode" });
            DropIndex("EXCEED.ServiceRequisitionItem", new[] { "PurchaseRequisitionID" });
            DropTable("EXCEED.ServiceItem");
            DropTable("EXCEED.ServiceCategory");
            DropTable("EXCEED.ServiceRequisitionItem");
        }
    }
}
