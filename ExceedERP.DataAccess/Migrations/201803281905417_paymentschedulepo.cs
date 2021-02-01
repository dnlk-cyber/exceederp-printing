namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class paymentschedulepo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.PoPaymentSchedule",
                c => new
                    {
                        PoPaymentScheduleId = c.Int(nullable: false, identity: true),
                        PurchaseOrderID = c.Int(nullable: false),
                        PaymentType = c.Int(nullable: false),
                        Percentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PoPaymentScheduleId)
                .ForeignKey("EXCEED.PurchaseOrder", t => t.PurchaseOrderID, cascadeDelete: true)
                .Index(t => t.PurchaseOrderID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.PoPaymentSchedule", "PurchaseOrderID", "EXCEED.PurchaseOrder");
            DropIndex("EXCEED.PoPaymentSchedule", new[] { "PurchaseOrderID" });
            DropTable("EXCEED.PoPaymentSchedule");
        }
    }
}
