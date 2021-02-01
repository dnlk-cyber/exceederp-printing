namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class clearingaccount : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.StockClearingAccount",
                c => new
                    {
                        StockClearingAccountId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        AccountCode = c.String(),
                        Enable = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.StockClearingAccountId);
            
        }
        
        public override void Down()
        {
            DropTable("EXCEED.StockClearingAccount");
        }
    }
}
