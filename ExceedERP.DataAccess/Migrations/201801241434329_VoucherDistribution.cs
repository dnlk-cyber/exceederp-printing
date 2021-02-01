namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoucherDistribution : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ApMiscellaneousPaymentDistribution", "AdditionalDescription", c => c.String());
            AddColumn("EXCEED.ApPaybleInvoiceDistribution", "AdditionalDescription", c => c.String());
            AddColumn("EXCEED.ApPaymentDistribution", "AdditionalDescription", c => c.String());
            AddColumn("EXCEED.ArInvoiceDistribution", "AdditionalDescription", c => c.String());
            AddColumn("EXCEED.ARMiscelaneousCollectionDistribution", "AdditionalDescription", c => c.String());
            AddColumn("EXCEED.ArSalesOrderDistribution", "AdditionalDescription", c => c.String());
            AddColumn("EXCEED.ArStandardCollectionDistribution", "AdditionalDescription", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ArStandardCollectionDistribution", "AdditionalDescription");
            DropColumn("EXCEED.ArSalesOrderDistribution", "AdditionalDescription");
            DropColumn("EXCEED.ARMiscelaneousCollectionDistribution", "AdditionalDescription");
            DropColumn("EXCEED.ArInvoiceDistribution", "AdditionalDescription");
            DropColumn("EXCEED.ApPaymentDistribution", "AdditionalDescription");
            DropColumn("EXCEED.ApPaybleInvoiceDistribution", "AdditionalDescription");
            DropColumn("EXCEED.ApMiscellaneousPaymentDistribution", "AdditionalDescription");
        }
    }
}
