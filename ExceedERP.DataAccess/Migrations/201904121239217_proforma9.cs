namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class proforma9 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ProformaEvaluationItemResult",
                c => new
                    {
                        ProformaEvaluationItemResultId = c.Int(nullable: false, identity: true),
                        ProformaEvaluationResultID = c.Int(nullable: false),
                        Financial = c.Single(nullable: false),
                        Technical = c.Single(nullable: false),
                        Pass = c.Int(nullable: false),
                        Total = c.Single(nullable: false),
                        CatCode = c.String(),
                        CategoryName = c.String(),
                        ItmCode = c.String(),
                        ItemName = c.String(),
                        Name = c.String(),
                        ProformaItemID = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProformaEvaluationItemResultId)
                .ForeignKey("EXCEED.ProformaEvaluationResult", t => t.ProformaEvaluationResultID, cascadeDelete: true)
                .Index(t => t.ProformaEvaluationResultID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.ProformaEvaluationItemResult", "ProformaEvaluationResultID", "EXCEED.ProformaEvaluationResult");
            DropIndex("EXCEED.ProformaEvaluationItemResult", new[] { "ProformaEvaluationResultID" });
            DropTable("EXCEED.ProformaEvaluationItemResult");
        }
    }
}
