namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class finance : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.Purchaser",
                c => new
                    {
                        PurchaserId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        Enable = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PurchaserId);
            
            AddColumn("EXCEED.ApMiscellaneousPayment", "PaymentType", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApPaybleInvoices", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApPaybleInvoices", "PrNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ApPaybleInvoices", "PrNo");
            DropColumn("EXCEED.ApPaybleInvoices", "EmployeeId");
            DropColumn("EXCEED.ApMiscellaneousPayment", "PaymentType");
            DropTable("EXCEED.Purchaser");
        }
    }
}
