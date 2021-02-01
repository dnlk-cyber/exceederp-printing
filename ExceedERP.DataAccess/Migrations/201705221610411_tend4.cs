namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tend4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.Education", "FromEAmharic", c => c.String());
            AddColumn("HRM.Education", "ToEAmharic", c => c.String());
            DropColumn("HRM.Education", "FromAmharic");
            DropColumn("HRM.Education", "ToAmharic");
        }
        
        public override void Down()
        {
            AddColumn("HRM.Education", "ToAmharic", c => c.String());
            AddColumn("HRM.Education", "FromAmharic", c => c.String());
            DropColumn("HRM.Education", "ToEAmharic");
            DropColumn("HRM.Education", "FromEAmharic");
        }
    }
}
