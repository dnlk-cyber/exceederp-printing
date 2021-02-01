namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fix : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.AssetRegistration", "DepreciationRuleId", "EXCEED.DepreciationRule");
            DropIndex("EXCEED.AssetRegistration", new[] { "DepreciationRuleId" });
            DropColumn("EXCEED.AssetRegistration", "CostCenter");
            DropColumn("EXCEED.AssetRegistration", "DepreciationRuleId");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.AssetRegistration", "DepreciationRuleId", c => c.Int(nullable: false));
            AddColumn("EXCEED.AssetRegistration", "CostCenter", c => c.String());
            CreateIndex("EXCEED.AssetRegistration", "DepreciationRuleId");
            AddForeignKey("EXCEED.AssetRegistration", "DepreciationRuleId", "EXCEED.DepreciationRule", "DepreciationRuleId", cascadeDelete: true);
        }
    }
}
