namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class proforma8 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.WinningSupplierItem", "CatCode", c => c.String());
            AlterColumn("EXCEED.WinningSupplierItem", "ItmCode", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.WinningSupplierItem", "ItmCode", c => c.String(nullable: false));
            AlterColumn("EXCEED.WinningSupplierItem", "CatCode", c => c.String(nullable: false));
        }
    }
}
