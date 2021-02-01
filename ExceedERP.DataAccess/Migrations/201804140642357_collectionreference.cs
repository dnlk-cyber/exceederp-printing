namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class collectionreference : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ArStandardCollection", "Reference", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ArStandardCollection", "Reference");
        }
    }
}
