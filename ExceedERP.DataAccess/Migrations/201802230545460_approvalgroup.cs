namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class approvalgroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("Global.ApprovalGroup", "DocumentApprovalRuleId", c => c.Int(nullable: false));
            CreateIndex("Global.ApprovalGroup", "DocumentApprovalRuleId");
            AddForeignKey("Global.ApprovalGroup", "DocumentApprovalRuleId", "Global.DocumentApprovalRule", "DocumentApprovalRuleId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("Global.ApprovalGroup", "DocumentApprovalRuleId", "Global.DocumentApprovalRule");
            DropIndex("Global.ApprovalGroup", new[] { "DocumentApprovalRuleId" });
            DropColumn("Global.ApprovalGroup", "DocumentApprovalRuleId");
        }
    }
}
