namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StoreCodeAddedToMaterialCost : DbMigration
    {
        public override void Up()
        {
            AddColumn("PrintingEstimation.MaterialCosts", "StoreCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("PrintingEstimation.MaterialCosts", "StoreCode");
        }
    }
}
