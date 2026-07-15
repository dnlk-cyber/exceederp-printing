namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class severancepayable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "HRM.SeveranceCalculationSettings",
                c => new
                    {
                        SeveranceCalculationSettingId = c.Int(nullable: false, identity: true),
                        FormulaType = c.Int(nullable: false),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SeveranceCalculationSettingId);
            
            AddColumn("HRM.SeverancePayableSettings", "MinYearsOfService", c => c.Int(nullable: false));
            AddColumn("HRM.SeverancePayableSettings", "MaxYearsOfService", c => c.Int());
            AddColumn("HRM.SeverancePayableSettings", "YearsPaidWithFullSalary", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("HRM.SeverancePayableSettings", "YearsPaidWithFullSalary");
            DropColumn("HRM.SeverancePayableSettings", "MaxYearsOfService");
            DropColumn("HRM.SeverancePayableSettings", "MinYearsOfService");
            DropTable("HRM.SeveranceCalculationSettings");
        }
    }
}
