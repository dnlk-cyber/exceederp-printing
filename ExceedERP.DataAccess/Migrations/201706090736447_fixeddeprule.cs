namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixeddeprule : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.DepreciationRule", "FixedAssetGroupID", "EXCEED.FixedAssetGroup");
            DropIndex("EXCEED.DepreciationRule", new[] { "FixedAssetGroupID" });
            //DropTable("EXCEED.DepreciationRule");
        }
        
        public override void Down()
        {
            CreateTable(
                "EXCEED.DepreciationRule",
                c => new
                    {
                        DepreciationRuleId = c.Int(nullable: false, identity: true),
                        CategoryCode = c.String(nullable: false),
                        AccountCode = c.String(),
                        DepreciationCode = c.String(),
                        ClearingAccount = c.String(),
                        AccumulatedAccount = c.String(),
                        FixedAssetGroupID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DepreciationRuleId);
            
            CreateIndex("EXCEED.DepreciationRule", "FixedAssetGroupID");
            AddForeignKey("EXCEED.DepreciationRule", "FixedAssetGroupID", "EXCEED.FixedAssetGroup", "FixedAssetGroupID", cascadeDelete: true);
        }
    }
}
