namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inventorysetting : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.InventorySetting",
                c => new
                    {
                        InventorySettingId = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Value = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.InventorySettingId);
            
        }
        
        public override void Down()
        {
            DropTable("EXCEED.InventorySetting");
        }
    }
}
