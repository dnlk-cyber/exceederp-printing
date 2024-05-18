namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printingjob : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Printing.PrintingJobType",
                c => new
                    {
                        PrintingJobTypeId = c.Int(nullable: false, identity: true),
                        JobTypeName = c.String(),
                        NoOfPagesFactor = c.Int(nullable: false),
                        DefaultNoOfPages = c.Int(nullable: false),
                        IsSpinRequired = c.Boolean(nullable: false),
                        IsBindingRequired = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PrintingJobTypeId);
            
            AddColumn("Printing.PrintingProductProcess", "DateCreated", c => c.DateTime());
            AddColumn("Printing.PrintingProductProcess", "LastModified", c => c.DateTime());
            AddColumn("Printing.PrintingProductProcess", "CreatedBy", c => c.String());
            AddColumn("Printing.PrintingProductProcess", "ModifiedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("Printing.PrintingProductProcess", "ModifiedBy");
            DropColumn("Printing.PrintingProductProcess", "CreatedBy");
            DropColumn("Printing.PrintingProductProcess", "LastModified");
            DropColumn("Printing.PrintingProductProcess", "DateCreated");
            DropTable("Printing.PrintingJobType");
        }
    }
}
