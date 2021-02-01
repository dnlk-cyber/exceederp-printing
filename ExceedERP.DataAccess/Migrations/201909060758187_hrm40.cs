namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm40 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "HRM.Template",
                c => new
                    {
                        TemplateId = c.Int(nullable: false, identity: true),
                        TempOne = c.String(),
                        TemplateType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TemplateId);
            
            AddColumn("HRM.LeaveEntitle", "BranchId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("HRM.LeaveEntitle", "BranchId");
            DropTable("HRM.Template");
        }
    }
}
