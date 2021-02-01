namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class monthlydeprec : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.MonthlyDepreciationValidation",
                c => new
                    {
                        MonthlyDepreciationValidationId = c.Int(nullable: false, identity: true),
                        MonthlyDepreciationId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MonthlyDepreciationValidationId)
                .ForeignKey("EXCEED.MonthlyDepreciation", t => t.MonthlyDepreciationId, cascadeDelete: true)
                .Index(t => t.MonthlyDepreciationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.MonthlyDepreciationValidation", "MonthlyDepreciationId", "EXCEED.MonthlyDepreciation");
            DropIndex("EXCEED.MonthlyDepreciationValidation", new[] { "MonthlyDepreciationId" });
            DropTable("EXCEED.MonthlyDepreciationValidation");
        }
    }
}
