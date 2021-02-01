namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class secondledger1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ApMiscellaneousPayment", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApPaybleInvoices", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApPayment", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ArInvoice", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ARMiscelaneousCollection", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ArSalesOrder", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ArStandardCollection", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.GlRecordJournal", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.GlRecordJournal", "IsPostedToSecondLedger");
            DropColumn("EXCEED.ArStandardCollection", "IsPostedToSecondLedger");
            DropColumn("EXCEED.ArSalesOrder", "IsPostedToSecondLedger");
            DropColumn("EXCEED.ARMiscelaneousCollection", "IsPostedToSecondLedger");
            DropColumn("EXCEED.ArInvoice", "IsPostedToSecondLedger");
            DropColumn("EXCEED.ApPayment", "IsPostedToSecondLedger");
            DropColumn("EXCEED.ApPaybleInvoices", "IsPostedToSecondLedger");
            DropColumn("EXCEED.ApMiscellaneousPayment", "IsPostedToSecondLedger");
        }
    }
}
