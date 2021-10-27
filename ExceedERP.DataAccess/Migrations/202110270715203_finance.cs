namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class finance : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.InvoiceArchive",
                c => new
                    {
                        InvoiceArchiveId = c.Int(nullable: false, identity: true),
                        PaidFor = c.String(),
                        InvoiceNo = c.String(),
                        VoucherNo = c.String(),
                        AmountPaid = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DatePaid = c.DateTime(nullable: false),
                        BoxFileNo = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.InvoiceArchiveId);
            
            AddColumn("EXCEED.ArInvoiceLines", "RemainingQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ArInvoiceLines", "RemainingQuantity");
            DropTable("EXCEED.InvoiceArchive");
        }
    }
}
