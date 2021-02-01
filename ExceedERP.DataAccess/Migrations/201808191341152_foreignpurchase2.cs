namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class foreignpurchase2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ForeignPurchaseAccount",
                c => new
                    {
                        ForeignPurchaseAccountId = c.Int(nullable: false, identity: true),
                        ForeignPurchaseId = c.Int(nullable: false),
                        AccountCode = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ForeignPurchaseAccountId)
                .ForeignKey("EXCEED.ForeignPurchase", t => t.ForeignPurchaseId, cascadeDelete: true)
                .Index(t => t.ForeignPurchaseId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.ForeignPurchaseAccount", "ForeignPurchaseId", "EXCEED.ForeignPurchase");
            DropIndex("EXCEED.ForeignPurchaseAccount", new[] { "ForeignPurchaseId" });
            DropTable("EXCEED.ForeignPurchaseAccount");
        }
    }
}
