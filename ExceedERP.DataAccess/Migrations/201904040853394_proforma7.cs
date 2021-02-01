namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class proforma7 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.ProformPassFail", "ItmCode", c => c.String());
            AlterColumn("EXCEED.ProformTechnical", "ItmCode", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.ProformTechnical", "ItmCode", c => c.String(nullable: false));
            AlterColumn("EXCEED.ProformPassFail", "ItmCode", c => c.String(nullable: false));
        }
    }
}
