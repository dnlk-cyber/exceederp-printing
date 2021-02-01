namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class financeupdate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ArSalesOrderStatus",
                c => new
                    {
                        ArSalesOrderStatusId = c.Int(nullable: false, identity: true),
                        ArSalesOrderId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        OrderStatus = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ArSalesOrderStatusId)
                .ForeignKey("EXCEED.ArSalesOrder", t => t.ArSalesOrderId, cascadeDelete: true)
                .Index(t => t.ArSalesOrderId);
            
            AddColumn("EXCEED.ApPaybleInvoices", "IsManual", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ArSalesOrder", "OrderType", c => c.Int(nullable: false));
            AddColumn("EXCEED.ArSalesOrderLine", "AccountCode", c => c.String(nullable: false));
            AddColumn("EXCEED.ArSalesOrderLine", "AccountDesc", c => c.String());
            AddColumn("EXCEED.GLSegmentSetup", "SegmentType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.ArSalesOrderStatus", "ArSalesOrderId", "EXCEED.ArSalesOrder");
            DropIndex("EXCEED.ArSalesOrderStatus", new[] { "ArSalesOrderId" });
            DropColumn("EXCEED.GLSegmentSetup", "SegmentType");
            DropColumn("EXCEED.ArSalesOrderLine", "AccountDesc");
            DropColumn("EXCEED.ArSalesOrderLine", "AccountCode");
            DropColumn("EXCEED.ArSalesOrder", "OrderType");
            DropColumn("EXCEED.ApPaybleInvoices", "IsManual");
            DropTable("EXCEED.ArSalesOrderStatus");
        }
    }
}
