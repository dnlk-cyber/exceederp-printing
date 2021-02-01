namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class purchaserpayment : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ApPayment", "GLTaxRateID", c => c.Int(nullable: false));
            AlterColumn("EXCEED.ApPayment", "SupplierId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.ApPayment", "SupplierId", c => c.String(nullable: false));
            DropColumn("EXCEED.ApPayment", "GLTaxRateID");
        }
    }
}
