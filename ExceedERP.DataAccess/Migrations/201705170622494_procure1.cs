namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class procure1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.Guarantee", "GuaranteeMode", c => c.String());
            AlterColumn("EXCEED.GuaranteeRule", "GuaranteeMode", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.GuaranteeRule", "GuaranteeMode", c => c.Int(nullable: false));
            AlterColumn("EXCEED.Guarantee", "GuaranteeMode", c => c.Int(nullable: false));
        }
    }
}
