namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class procurement11 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.BidAwardedSupplierItem", "CatCode", c => c.String());
            AlterColumn("EXCEED.BidAwardedSupplierItem", "ItmCode", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.BidAwardedSupplierItem", "ItmCode", c => c.String(nullable: false));
            AlterColumn("EXCEED.BidAwardedSupplierItem", "CatCode", c => c.String(nullable: false));
        }
    }
}
