namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class foreignpurchase1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ForeignPurchaseCost", "Payee", c => c.String());
            AddColumn("EXCEED.ForeignPurchaseItemCost", "Payee", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ForeignPurchaseItemCost", "Payee");
            DropColumn("EXCEED.ForeignPurchaseCost", "Payee");
        }
    }
}
