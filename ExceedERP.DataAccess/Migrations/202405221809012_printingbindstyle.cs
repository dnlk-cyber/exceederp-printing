namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printingbindstyle : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Printing.PrintingBindingStyle",
                c => new
                    {
                        PrintingBindingStyleId = c.Int(nullable: false, identity: true),
                        BindingStyleName = c.String(),
                        UnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PrintingBindingStyleId);
            
            AddColumn("Printing.PrintingJobType", "IsCoverRequired", c => c.Boolean(nullable: false));
            AddColumn("Printing.PrintingProductProcess", "PrintingProcessId", c => c.Int(nullable: false));
            AddColumn("Printing.PrintMachinePaperList", "PrintingPaperSizeId", c => c.Int(nullable: false));
            DropColumn("Printing.PrintMachinePaperList", "PaperSizeCode");
            DropColumn("Printing.PrintMachinePaperList", "PaperName");
        }
        
        public override void Down()
        {
            AddColumn("Printing.PrintMachinePaperList", "PaperName", c => c.String());
            AddColumn("Printing.PrintMachinePaperList", "PaperSizeCode", c => c.String());
            DropColumn("Printing.PrintMachinePaperList", "PrintingPaperSizeId");
            DropColumn("Printing.PrintingProductProcess", "PrintingProcessId");
            DropColumn("Printing.PrintingJobType", "IsCoverRequired");
            DropTable("Printing.PrintingBindingStyle");
        }
    }
}
