namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class employeeplaced : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.Employee", "IsPlaced", c => c.Boolean(nullable: false));
            AlterColumn("HRM.Employee", "BirthPlace", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("HRM.Employee", "BirthPlace", c => c.String(nullable: false));
            DropColumn("HRM.Employee", "IsPlaced");
        }
    }
}
