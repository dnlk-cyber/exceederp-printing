namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class procurement1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ProformaPurchase", "StoreRequisitionID", c => c.Int(nullable: false));
            //AlterColumn("EXCEED.Notification", "DeadLine", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.Notification", "DeadLine", c => c.DateTime());
            DropColumn("EXCEED.ProformaPurchase", "StoreRequisitionID");
        }
    }
}
