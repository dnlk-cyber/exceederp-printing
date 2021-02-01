namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salesorder1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.ArSalesOrderPartial", "ArSalesOrderId", "EXCEED.ArSalesOrder");
            DropIndex("EXCEED.ArSalesOrderPartial", new[] { "ArSalesOrderId" });
            CreateTable(
                "EXCEED.ArSalesOrderPartialPrint",
                c => new
                    {
                        ArSalesOrderPartialPrintId = c.Int(nullable: false, identity: true),
                        ArSalesOrderId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Description = c.String(),
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
                .PrimaryKey(t => t.ArSalesOrderPartialPrintId)
                .ForeignKey("EXCEED.ArSalesOrder", t => t.ArSalesOrderId, cascadeDelete: true)
                .Index(t => t.ArSalesOrderId);
            
            AddColumn("EXCEED.ArSalesOrderPartial", "ArSalesOrderPartialPrintId", c => c.Int(nullable: false));
            CreateIndex("EXCEED.ArSalesOrderPartial", "ArSalesOrderPartialPrintId");
            AddForeignKey("EXCEED.ArSalesOrderPartial", "ArSalesOrderPartialPrintId", "EXCEED.ArSalesOrderPartialPrint", "ArSalesOrderPartialPrintId", cascadeDelete: true);
            DropColumn("EXCEED.ArSalesOrderPartial", "ArSalesOrderId");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ArSalesOrderPartial", "ArSalesOrderId", c => c.Int(nullable: false));
            DropForeignKey("EXCEED.ArSalesOrderPartial", "ArSalesOrderPartialPrintId", "EXCEED.ArSalesOrderPartialPrint");
            DropForeignKey("EXCEED.ArSalesOrderPartialPrint", "ArSalesOrderId", "EXCEED.ArSalesOrder");
            DropIndex("EXCEED.ArSalesOrderPartial", new[] { "ArSalesOrderPartialPrintId" });
            DropIndex("EXCEED.ArSalesOrderPartialPrint", new[] { "ArSalesOrderId" });
            DropColumn("EXCEED.ArSalesOrderPartial", "ArSalesOrderPartialPrintId");
            DropTable("EXCEED.ArSalesOrderPartialPrint");
            CreateIndex("EXCEED.ArSalesOrderPartial", "ArSalesOrderId");
            AddForeignKey("EXCEED.ArSalesOrderPartial", "ArSalesOrderId", "EXCEED.ArSalesOrder", "ArSalesOrderId", cascadeDelete: true);
        }
    }
}
