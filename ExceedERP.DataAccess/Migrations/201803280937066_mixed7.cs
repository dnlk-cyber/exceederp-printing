namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mixed7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ApPayment", "PoTypeEnum", c => c.Int(nullable: true));
            AddColumn("HRM.Placement", "HaveFixedAsset", c => c.Boolean(nullable: false));
            AlterColumn("EXCEED.ApPayment", "PoType", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.ApPayment", "PoType", c => c.Int(nullable: false));
            DropColumn("HRM.Placement", "HaveFixedAsset");
            DropColumn("EXCEED.ApPayment", "PoTypeEnum");
        }
    }
}
