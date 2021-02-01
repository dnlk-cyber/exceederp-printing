namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inventory3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.StoreRequisition", "IsSecondOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.StoreRequisition", "SecondOnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.StoreRequisition", "SecondOnlineApprovedTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.StoreRequisition", "SecondOnlineApprovedTime");
            DropColumn("EXCEED.StoreRequisition", "SecondOnlineApprovedBy");
            DropColumn("EXCEED.StoreRequisition", "IsSecondOnlineApproved");
        }
    }
}
