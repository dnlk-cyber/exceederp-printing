namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm52 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Payroll.BonusSetting",
                c => new
                    {
                        BonusSettingId = c.Int(nullable: false, identity: true),
                        BunusType = c.Int(nullable: false),
                        Multiplier = c.String(),
                        GlFiscalYearId = c.Int(),
                        CurrentValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.BonusSettingId);
            
            AlterColumn("Payroll.Bonus", "PerformanceRate", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("Payroll.Bonus", "PerformanceRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropTable("Payroll.BonusSetting");
        }
    }
}
