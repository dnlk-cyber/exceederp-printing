namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class businessunit : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "HRM.BusinessUnit",
                c => new
                    {
                        BusinessUnitId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BusinessUnitId);
            
            AddColumn("EXCEED.InventoryStore", "BusinessUnitId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.InventoryStore", "BusinessUnitId");
            DropTable("HRM.BusinessUnit");
        }
    }
}
