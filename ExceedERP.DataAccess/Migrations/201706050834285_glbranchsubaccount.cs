namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class glbranchsubaccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.Branch", "SubAccount", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("HRM.Branch", "SubAccount");
        }
    }
}
