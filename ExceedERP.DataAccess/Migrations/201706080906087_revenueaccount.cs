namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class revenueaccount : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.RevenueAccount",
                c => new
                    {
                        RevenueAccountId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        ApplyToAr = c.Boolean(nullable: false),
                        ApplyToAp = c.Boolean(nullable: false),
                        ApplyToInventory = c.Boolean(nullable: false),
                        ArAccountCode = c.String(),
                        ApAccountCode = c.String(),
                        InventoryAccountCode = c.String(),
                        Enable = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RevenueAccountId);
            
        }
        
        public override void Down()
        {
            DropTable("EXCEED.RevenueAccount");
        }
    }
}
