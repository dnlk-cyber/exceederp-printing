namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixeddeprule1 : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "EXCEED.DepreciationRule",
            //    c => new
            //        {
            //            DepreciationRuleId = c.Int(nullable: false, identity: true),
            //            CategoryCode = c.String(nullable: false),
            //            AccountCode = c.String(),
            //            DepreciationCode = c.String(),
            //            ClearingAccount = c.String(),
            //            AccumulatedAccount = c.String(),
            //            FixedAssetGroupID = c.Int(nullable: false),
            //        })
            //    .PrimaryKey(t => t.DepreciationRuleId)
            //    .ForeignKey("EXCEED.FixedAssetGroup", t => t.FixedAssetGroupID, cascadeDelete: true)
            //    .Index(t => t.FixedAssetGroupID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.DepreciationRule", "FixedAssetGroupID", "EXCEED.FixedAssetGroup");
            DropIndex("EXCEED.DepreciationRule", new[] { "FixedAssetGroupID" });
            DropTable("EXCEED.DepreciationRule");
        }
    }
}
