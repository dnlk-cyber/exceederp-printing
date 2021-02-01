namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salespartner : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ArSalesOrderBroadcastTime",
                c => new
                    {
                        ArSalesOrderBroadcastTimeId = c.Int(nullable: false, identity: true),
                        ArSalesOrderId = c.Int(nullable: false),
                        BroadcastDateTime = c.DateTime(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ArSalesOrderBroadcastTimeId)
                .ForeignKey("EXCEED.ArSalesOrder", t => t.ArSalesOrderId, cascadeDelete: true)
                .Index(t => t.ArSalesOrderId);
            
            CreateTable(
                "EXCEED.Partner",
                c => new
                    {
                        PartnerId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        PartnerTypeId = c.Int(nullable: false),
                        ContactName = c.String(nullable: false),
                        PhoneNumber = c.String(),
                        Address = c.String(nullable: false),
                        Country = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PartnerId);
            
            CreateTable(
                "EXCEED.PartnerType",
                c => new
                    {
                        PartnerTypeId = c.Int(nullable: false, identity: true),
                        CTypeName = c.String(nullable: false),
                        Description = c.String(),
                        Enable = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PartnerTypeId);
            
            AddColumn("EXCEED.ArSalesOrder", "SalesPersonId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ArSalesOrder", "PartnerId", c => c.Int(nullable: false));
            DropColumn("EXCEED.ArSalesOrder", "OrderNumber");
            DropColumn("EXCEED.ArSalesOrder", "BroadcastDateTime");
            DropColumn("EXCEED.ArSalesOrder", "SalesPerson");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ArSalesOrder", "SalesPerson", c => c.String());
            AddColumn("EXCEED.ArSalesOrder", "BroadcastDateTime", c => c.DateTime(nullable: false));
            AddColumn("EXCEED.ArSalesOrder", "OrderNumber", c => c.String());
            DropForeignKey("EXCEED.ArSalesOrderBroadcastTime", "ArSalesOrderId", "EXCEED.ArSalesOrder");
            DropIndex("EXCEED.ArSalesOrderBroadcastTime", new[] { "ArSalesOrderId" });
            DropColumn("EXCEED.ArSalesOrder", "PartnerId");
            DropColumn("EXCEED.ArSalesOrder", "SalesPersonId");
            DropTable("EXCEED.PartnerType");
            DropTable("EXCEED.Partner");
            DropTable("EXCEED.ArSalesOrderBroadcastTime");
        }
    }
}
