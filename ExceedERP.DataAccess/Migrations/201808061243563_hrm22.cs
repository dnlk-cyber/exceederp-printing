namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm22 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.CourseTracking", "NoOfTrainee", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("HRM.CourseTracking", "NoOfTrainee");
        }
    }
}
