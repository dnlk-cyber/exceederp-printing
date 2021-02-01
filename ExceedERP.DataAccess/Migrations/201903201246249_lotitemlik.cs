namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lotitemlik : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.BidAwardedSupplierItem", "LotItemID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.BidAwardedSupplierItem", "LotItemID");
        }
    }
}
