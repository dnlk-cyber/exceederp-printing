namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.Trainee", "EducationLevel", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("HRM.Trainee", "EducationLevel");
        }
    }
}
