namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class paymenttax : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ApPayment", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ApPayment", "GLTaxRateID", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.ApPayment", "GLTaxRateID", c => c.Int(nullable: false));
            DropColumn("EXCEED.ApPayment", "TaxAmount");
        }
    }
}
