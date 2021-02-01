namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction16 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Construction.MaterialPriceIndex", "BranchId", "HRM.Branch");
            DropIndex("Construction.MaterialPriceIndex", new[] { "BranchId" });
            AddColumn("EXCEED.ApPettyCash", "ForwardedFrom", c => c.String());
            AddColumn("EXCEED.ApPettyCash", "ForwardedTo", c => c.String());
            AddColumn("EXCEED.ApPettyCash", "IsFinalApproval", c => c.Boolean(nullable: false));
            AddColumn("Construction.AreaWorkItem", "ItemCode", c => c.String());
            AddColumn("Construction.SubContractWorkItemEstmn", "ItemCode", c => c.String());
            AddColumn("Construction.EstimationAreaWorkItem", "ItemCode", c => c.String());
            AddColumn("Construction.SubContractWorkItem", "ItemCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("Construction.SubContractWorkItem", "ItemCode");
            DropColumn("Construction.EstimationAreaWorkItem", "ItemCode");
            DropColumn("Construction.SubContractWorkItemEstmn", "ItemCode");
            DropColumn("Construction.AreaWorkItem", "ItemCode");
            DropColumn("EXCEED.ApPettyCash", "IsFinalApproval");
            DropColumn("EXCEED.ApPettyCash", "ForwardedTo");
            DropColumn("EXCEED.ApPettyCash", "ForwardedFrom");
            CreateIndex("Construction.MaterialPriceIndex", "BranchId");
            AddForeignKey("Construction.MaterialPriceIndex", "BranchId", "HRM.Branch", "BranchId", cascadeDelete: true);
        }
    }
}
