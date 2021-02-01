namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class storeassignment2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.BranchItemAssignment",
                c => new
                    {
                        BranchItemAssignmentId = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        ItemCategoryCode = c.String(nullable: false, maxLength: 128),
                        ItemCode = c.String(nullable: false, maxLength: 128),
                        ReOrderPoint = c.Single(nullable: false),
                        MinPoint = c.Single(nullable: false),
                        MaxPoint = c.Single(nullable: false),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AvgPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BranchItemAssignmentId)
                .ForeignKey("HRM.Branch", t => t.BranchId, cascadeDelete: true)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode, cascadeDelete: true)
                .ForeignKey("EXCEED.ItemCategory", t => t.ItemCategoryCode, cascadeDelete: true)
                .Index(t => t.BranchId)
                .Index(t => t.ItemCategoryCode)
                .Index(t => t.ItemCode);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.BranchItemAssignment", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.BranchItemAssignment", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("EXCEED.BranchItemAssignment", "BranchId", "HRM.Branch");
            DropIndex("EXCEED.BranchItemAssignment", new[] { "ItemCode" });
            DropIndex("EXCEED.BranchItemAssignment", new[] { "ItemCategoryCode" });
            DropIndex("EXCEED.BranchItemAssignment", new[] { "BranchId" });
            DropTable("EXCEED.BranchItemAssignment");
        }
    }
}
