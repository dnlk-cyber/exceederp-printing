namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tenderresult : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.TenderEvaluationItemResult",
                c => new
                    {
                        TenderEvaluationItemResultId = c.Int(nullable: false, identity: true),
                        TenderEvaluationResultID = c.Int(nullable: false),
                        Financial = c.Single(nullable: false),
                        Technical = c.Single(nullable: false),
                        Pass = c.Int(nullable: false),
                        Total = c.Single(nullable: false),
                        CatCode = c.String(),
                        CategoryName = c.String(),
                        ItmCode = c.String(),
                        ItemName = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TenderEvaluationItemResultId)
                .ForeignKey("EXCEED.TenderEvaluationResult", t => t.TenderEvaluationResultID, cascadeDelete: true)
                .Index(t => t.TenderEvaluationResultID);
            
            DropColumn("EXCEED.TenderEvaluationResult", "CatCode");
            DropColumn("EXCEED.TenderEvaluationResult", "CategoryName");
            DropColumn("EXCEED.TenderEvaluationResult", "ItmCode");
            DropColumn("EXCEED.TenderEvaluationResult", "ItemName");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.TenderEvaluationResult", "ItemName", c => c.String());
            AddColumn("EXCEED.TenderEvaluationResult", "ItmCode", c => c.String());
            AddColumn("EXCEED.TenderEvaluationResult", "CategoryName", c => c.String());
            AddColumn("EXCEED.TenderEvaluationResult", "CatCode", c => c.String());
            DropForeignKey("EXCEED.TenderEvaluationItemResult", "TenderEvaluationResultID", "EXCEED.TenderEvaluationResult");
            DropIndex("EXCEED.TenderEvaluationItemResult", new[] { "TenderEvaluationResultID" });
            DropTable("EXCEED.TenderEvaluationItemResult");
        }
    }
}
