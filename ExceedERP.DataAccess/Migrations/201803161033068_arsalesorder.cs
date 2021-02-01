namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class arsalesorder : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ArSalesOrderContract",
                c => new
                    {
                        ArSalesOrderContractId = c.Int(nullable: false, identity: true),
                        ArSalesOrderId = c.Int(nullable: false),
                        FileName = c.String(),
                        FilePath = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ArSalesOrderContractId)
                .ForeignKey("EXCEED.ArSalesOrder", t => t.ArSalesOrderId, cascadeDelete: true)
                .Index(t => t.ArSalesOrderId);
            
            CreateTable(
                "EXCEED.ArSalesOrderPaymentSchedule",
                c => new
                    {
                        ArSalesOrderPaymentScheduleId = c.Int(nullable: false, identity: true),
                        ArSalesOrderId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Amount = c.String(),
                        Description = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ArSalesOrderPaymentScheduleId)
                .ForeignKey("EXCEED.ArSalesOrder", t => t.ArSalesOrderId, cascadeDelete: true)
                .Index(t => t.ArSalesOrderId);
            
            AddColumn("EXCEED.ArSalesOrder", "IsNewCustomer", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ArSalesOrder", "CustomerName", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.ArSalesOrderPaymentSchedule", "ArSalesOrderId", "EXCEED.ArSalesOrder");
            DropForeignKey("EXCEED.ArSalesOrderContract", "ArSalesOrderId", "EXCEED.ArSalesOrder");
            DropIndex("EXCEED.ArSalesOrderPaymentSchedule", new[] { "ArSalesOrderId" });
            DropIndex("EXCEED.ArSalesOrderContract", new[] { "ArSalesOrderId" });
            DropColumn("EXCEED.ArSalesOrder", "CustomerName");
            DropColumn("EXCEED.ArSalesOrder", "IsNewCustomer");
            DropTable("EXCEED.ArSalesOrderPaymentSchedule");
            DropTable("EXCEED.ArSalesOrderContract");
        }
    }
}
