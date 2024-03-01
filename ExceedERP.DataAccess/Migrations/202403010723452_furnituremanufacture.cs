namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class furnituremanufacture : DbMigration
    {
        public override void Up()
        {
            AddColumn("Manufacturing.DirectLaborCosts", "TaskType", c => c.Int(nullable: false));
            AddColumn("Manufacturing.FurnitureMaterialCosts", "ManufacturingMaterialCategoryId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Manufacturing.FurnitureMaterialCosts", "ManufacturingMaterialCategoryId");
            DropColumn("Manufacturing.DirectLaborCosts", "TaskType");
        }
    }
}
