namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm8 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.ApplicantionForm", "IsForOther", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("HRM.ApplicantionForm", "IsForOther");
        }
    }
}
