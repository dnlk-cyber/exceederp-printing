namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductionFUPStatusLineAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "ProductFollowUp.JobStatusLineItems",
                c => new
                    {
                        JobStatusLineItemId = c.Int(nullable: false, identity: true),
                        JobStatusId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Quantiy = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Invoice = c.String(),
                        SalesVoucherNo = c.String(),
                        Cash = c.Decimal(precision: 18, scale: 2),
                        Credit = c.Decimal(precision: 18, scale: 2),
                        CashCreditInvoice = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.JobStatusLineItemId);
            
            AddColumn("ProductFollowUp.JobStatusEntityTypeConfiguration", "DateCreated", c => c.DateTime());
            AddColumn("ProductFollowUp.JobStatusEntityTypeConfiguration", "LastModified", c => c.DateTime());
            AddColumn("ProductFollowUp.JobStatusEntityTypeConfiguration", "CreatedBy", c => c.String());
            AddColumn("ProductFollowUp.JobStatusEntityTypeConfiguration", "ModifiedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("ProductFollowUp.JobStatusEntityTypeConfiguration", "ModifiedBy");
            DropColumn("ProductFollowUp.JobStatusEntityTypeConfiguration", "CreatedBy");
            DropColumn("ProductFollowUp.JobStatusEntityTypeConfiguration", "LastModified");
            DropColumn("ProductFollowUp.JobStatusEntityTypeConfiguration", "DateCreated");
            DropTable("ProductFollowUp.JobStatusLineItems");
        }
    }
}
