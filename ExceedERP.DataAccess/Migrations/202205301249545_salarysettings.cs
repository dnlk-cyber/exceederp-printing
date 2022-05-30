namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salarysettings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "HRM.SalarySetting",
                c => new
                    {
                        SalarySettingId = c.Int(nullable: false, identity: true),
                        IsJobtitleIncludedInSalary = c.Boolean(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.SalarySettingId);
            
        }
        
        public override void Down()
        {
            DropTable("HRM.SalarySetting");
        }
    }
}
