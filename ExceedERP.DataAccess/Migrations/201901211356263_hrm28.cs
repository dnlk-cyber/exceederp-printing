namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm28 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("HRM.RecruitmentPlan", "PlannedSalary", c => c.String());
            AlterColumn("HRM.JobTitleMappingDetail", "PlannedSalary", c => c.String());
            AlterColumn("HRM.JobTitleMappingDetailTemp", "PlannedSalary", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("HRM.JobTitleMappingDetailTemp", "PlannedSalary", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("HRM.JobTitleMappingDetail", "PlannedSalary", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("HRM.RecruitmentPlan", "PlannedSalary", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
