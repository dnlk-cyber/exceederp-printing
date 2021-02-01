namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uom : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.FixedAssetRequisitionItem", "UoM", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.FixedAssetRequisitionItem", "UoM");
        }
    }
}
