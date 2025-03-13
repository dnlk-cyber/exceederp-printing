namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class esimationdesign : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.PrintingJobOrderJobDesign",
                c => new
                    {
                        PrintingJobOrderJobDesignId = c.Int(nullable: false, identity: true),
                        ReferenceId = c.Int(nullable: false),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.PrintingJobOrderJobDesignId);
            
            CreateTable(
                "EXCEED.PrintingJobOrderProductionJobDesign",
                c => new
                    {
                        PrintingJobOrderProductionJobDesignId = c.Int(nullable: false, identity: true),
                        ReferenceId = c.Int(nullable: false),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.PrintingJobOrderProductionJobDesignId);
            
        }
        
        public override void Down()
        {
            DropTable("EXCEED.PrintingJobOrderProductionJobDesign");
            DropTable("EXCEED.PrintingJobOrderJobDesign");
        }
    }
}
