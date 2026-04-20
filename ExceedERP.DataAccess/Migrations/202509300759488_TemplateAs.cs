namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TemplateAs : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.Template", "Title", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("HRM.Template", "Title");
        }
    }
}
