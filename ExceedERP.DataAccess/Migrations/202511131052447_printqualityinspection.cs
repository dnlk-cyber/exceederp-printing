namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printqualityinspection : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Printing.PrintingQualityInspection",
                c => new
                    {
                        PrintingQualityInspectionId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        PrintQualityGrade = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        CreatedBy = c.String(),
                        DateModified = c.DateTime(),
                        ModifiedBy = c.String(),
                        PrintingProductionFollowUpId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PrintingQualityInspectionId);
            
        }
        
        public override void Down()
        {
            DropTable("Printing.PrintingQualityInspection");
        }
    }
}
