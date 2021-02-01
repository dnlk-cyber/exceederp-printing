namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class apmisc : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ApMiscellaneousPayment", "GLTaxRateID", c => c.Int(nullable: false));
            AlterColumn("EXCEED.ApMiscellaneousPayment", "WithholdingType", c => c.Int(nullable: false));
            DropColumn("EXCEED.ApMiscellaneousPayment", "TaxType");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ApMiscellaneousPayment", "TaxType", c => c.String(nullable: false));
            AlterColumn("EXCEED.ApMiscellaneousPayment", "WithholdingType", c => c.String(nullable: false));
            DropColumn("EXCEED.ApMiscellaneousPayment", "GLTaxRateID");
        }
    }
}
