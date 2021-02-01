namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class productcode : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ArProductService", "Code", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ArProductService", "Code");
        }
    }
}
