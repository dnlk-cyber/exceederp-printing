namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class businessunit1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.BusinessUnit", "Accountcode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("HRM.BusinessUnit", "Accountcode");
        }
    }
}
