namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class po6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.CourseTracking", "EducationLevel", c => c.Int(nullable: false));
            AlterColumn("EXCEED.PurchaseOrder", "ProformaPurchaseID", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.PurchaseOrder", "ProformaPurchaseID", c => c.Int(nullable: false));
            DropColumn("HRM.CourseTracking", "EducationLevel");
        }
    }
}
