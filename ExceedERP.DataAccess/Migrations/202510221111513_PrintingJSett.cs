namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PrintingJSett : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "PrintingJobSetting.PrintingJobSetting",
                c => new
                    {
                        PrintingJobSettingId = c.Int(nullable: false, identity: true),
                        Remark = c.String(),
                        IsFromPrinting = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.PrintingJobSettingId);
            
            AddColumn("JobCosting.JobLaborCosts", "IsFromPrinting", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("JobCosting.JobLaborCosts", "IsFromPrinting");
            DropTable("PrintingJobSetting.PrintingJobSetting");
        }
    }
}
