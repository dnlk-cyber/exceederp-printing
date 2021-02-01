namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fa6 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.StoreReturnValidation", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.StoreReturnValidation", "Status", c => c.String());
        }
    }
}
