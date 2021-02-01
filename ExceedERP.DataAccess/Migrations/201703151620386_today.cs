namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class today : DbMigration
    {
        public override void Up()
        {
            MoveTable(name: "Common.Bank", newSchema: "EXCEED");
            DropForeignKey("HRM.RecruitmentPlan", "RankId", "HRM.Rank");
            DropIndex("HRM.RecruitmentPlan", new[] { "RankId" });
            AlterColumn("HRM.RecruitmentPlan", "RecruiTime", c => c.DateTime());
            AlterColumn("HRM.RecruitmentPlan", "From", c => c.DateTime());
            AlterColumn("HRM.RecruitmentPlan", "To", c => c.DateTime());
            AlterColumn("HRM.RecruitmentPlan", "RankId", c => c.Int());
            CreateIndex("HRM.RecruitmentPlan", "RankId");
            AddForeignKey("HRM.RecruitmentPlan", "RankId", "HRM.Rank", "RankId");
        }
        
        public override void Down()
        {
            DropForeignKey("HRM.RecruitmentPlan", "RankId", "HRM.Rank");
            DropIndex("HRM.RecruitmentPlan", new[] { "RankId" });
            AlterColumn("HRM.RecruitmentPlan", "RankId", c => c.Int(nullable: false));
            AlterColumn("HRM.RecruitmentPlan", "To", c => c.DateTime(nullable: false));
            AlterColumn("HRM.RecruitmentPlan", "From", c => c.DateTime(nullable: false));
            AlterColumn("HRM.RecruitmentPlan", "RecruiTime", c => c.DateTime(nullable: false));
            CreateIndex("HRM.RecruitmentPlan", "RankId");
            AddForeignKey("HRM.RecruitmentPlan", "RankId", "HRM.Rank", "RankId", cascadeDelete: true);
            MoveTable(name: "EXCEED.Bank", newSchema: "Common");
        }
    }
}
