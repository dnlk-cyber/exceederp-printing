namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inven4 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.StoreRequisitionPropertyValidation", "Status", c => c.Int(nullable: false));
            AlterColumn("EXCEED.StoreRequisitionValidation", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.StoreRequisitionValidation", "Status", c => c.String());
            AlterColumn("EXCEED.StoreRequisitionPropertyValidation", "Status", c => c.String());
        }
    }
}
