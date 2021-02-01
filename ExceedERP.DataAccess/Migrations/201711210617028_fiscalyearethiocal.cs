namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fiscalyearethiocal : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.GlFiscalYear", "DateFromAmharic", c => c.String());
            AddColumn("EXCEED.GlFiscalYear", "DateToAmharic", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.GlFiscalYear", "DateToAmharic");
            DropColumn("EXCEED.GlFiscalYear", "DateFromAmharic");
        }
    }
}
