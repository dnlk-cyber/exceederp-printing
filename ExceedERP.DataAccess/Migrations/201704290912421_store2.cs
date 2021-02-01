namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class store2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.InventoryStore", "BranchId", c => c.Int(nullable: false));
            CreateIndex("EXCEED.InventoryStore", "BranchId");
            AddForeignKey("EXCEED.InventoryStore", "BranchId", "HRM.Branch", "BranchId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.InventoryStore", "BranchId", "HRM.Branch");
            DropIndex("EXCEED.InventoryStore", new[] { "BranchId" });
            DropColumn("EXCEED.InventoryStore", "BranchId");
        }
    }
}
