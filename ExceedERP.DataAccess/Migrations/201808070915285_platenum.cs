namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class platenum : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.StoreRequisition", "PlateNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.StoreRequisition", "PlateNo");
        }
    }
}
