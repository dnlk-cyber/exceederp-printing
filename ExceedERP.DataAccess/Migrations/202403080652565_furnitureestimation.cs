namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class furnitureestimation : DbMigration
    {
        public override void Up()
        {
            AddColumn("Manufacturing.FurnitureBOQLabor", "ManufacturingTaskCategoryId", c => c.Int(nullable: false));
            DropColumn("Manufacturing.DirectLaborCosts", "FurnitureCategoryId");
        }
        
        public override void Down()
        {
            AddColumn("Manufacturing.DirectLaborCosts", "FurnitureCategoryId", c => c.Int(nullable: false));
            DropColumn("Manufacturing.FurnitureBOQLabor", "ManufacturingTaskCategoryId");
        }
    }
}
