namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class gurantee : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.Guarantee", "Purpose", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.Guarantee", "Purpose");
        }
    }
}
