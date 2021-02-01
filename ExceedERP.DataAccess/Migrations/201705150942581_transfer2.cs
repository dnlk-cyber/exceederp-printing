namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class transfer2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ItemTransfer", "IsSecondOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ItemTransfer", "SecondOnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.ItemTransfer", "SecondOnlineApprovedTime", c => c.DateTime());
            AlterColumn("EXCEED.ItemTransferReciverValidation", "Status", c => c.Int(nullable: false));
            AlterColumn("EXCEED.ItemTransferValidation", "Status", c => c.Int(nullable: false));
            DropColumn("EXCEED.ItemTransferReciverValidation", "Approver");
            DropColumn("EXCEED.ItemTransferValidation", "Approver");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ItemTransferValidation", "Approver", c => c.String());
            AddColumn("EXCEED.ItemTransferReciverValidation", "Approver", c => c.String());
            AlterColumn("EXCEED.ItemTransferValidation", "Status", c => c.String());
            AlterColumn("EXCEED.ItemTransferReciverValidation", "Status", c => c.String());
            DropColumn("EXCEED.ItemTransfer", "SecondOnlineApprovedTime");
            DropColumn("EXCEED.ItemTransfer", "SecondOnlineApprovedBy");
            DropColumn("EXCEED.ItemTransfer", "IsSecondOnlineApproved");
        }
    }
}
