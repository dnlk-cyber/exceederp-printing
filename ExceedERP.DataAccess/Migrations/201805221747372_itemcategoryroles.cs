namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class itemcategoryroles : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ItemCategoryUserRole",
                c => new
                    {
                        ItemCategoryUserRoleId = c.Int(nullable: false, identity: true),
                        ItemCategoryCode = c.String(maxLength: 128),
                        RoleName = c.String(),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ItemCategoryUserRoleId)
                .ForeignKey("EXCEED.ItemCategory", t => t.ItemCategoryCode)
                .Index(t => t.ItemCategoryCode);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.ItemCategoryUserRole", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropIndex("EXCEED.ItemCategoryUserRole", new[] { "ItemCategoryCode" });
            DropTable("EXCEED.ItemCategoryUserRole");
        }
    }
}
