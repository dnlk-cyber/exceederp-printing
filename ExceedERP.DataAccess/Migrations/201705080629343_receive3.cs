namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class receive3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.PropertyCount", "BranchId", c => c.Int(nullable: false));
            CreateIndex("EXCEED.PropertyCount", "BranchId");
            AddForeignKey("EXCEED.PropertyCount", "BranchId", "HRM.Branch", "BranchId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.PropertyCount", "BranchId", "HRM.Branch");
            DropIndex("EXCEED.PropertyCount", new[] { "BranchId" });
            DropColumn("EXCEED.PropertyCount", "BranchId");
        }
    }
}
