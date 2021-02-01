namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bincard1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.BinCardLine", "SupplierName", c => c.String());
            AddColumn("EXCEED.BinCardLine", "IssuedTo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.BinCardLine", "IssuedTo");
            DropColumn("EXCEED.BinCardLine", "SupplierName");
        }
    }
}
