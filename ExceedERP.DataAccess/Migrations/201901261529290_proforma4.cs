namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class proforma4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ProformTechnical", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ProformTechnical", "Name");
        }
    }
}
