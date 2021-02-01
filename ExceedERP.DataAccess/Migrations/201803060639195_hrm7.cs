namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm7 : DbMigration
    {
        public override void Up()
        {
            DropColumn("HRM.CourseTracking", "EducationLevel");
        }
        
        public override void Down()
        {
            AddColumn("HRM.CourseTracking", "EducationLevel", c => c.Int(nullable: false));
        }
    }
}
