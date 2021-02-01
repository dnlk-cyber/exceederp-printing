namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inven2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.IssueFuel", "Owner", c => c.Int(nullable: false));
            AddColumn("EXCEED.IssueFuel", "SupplierAccountCode", c => c.String());
            AddColumn("EXCEED.IssueFuel", "HasReading", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.IssueFuel", "HasReading");
            DropColumn("EXCEED.IssueFuel", "SupplierAccountCode");
            DropColumn("EXCEED.IssueFuel", "Owner");
        }
    }
}
