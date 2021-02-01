namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm27 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Performance.MeasurementDefinition", "OrgStuructureId", c => c.Int(nullable: false));
            AddColumn("Performance.MeasurementDefinition", "IsDepartmentLevel", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Performance.MeasurementDefinition", "IsDepartmentLevel");
            DropColumn("Performance.MeasurementDefinition", "OrgStuructureId");
        }
    }
}
