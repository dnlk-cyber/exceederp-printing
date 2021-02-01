namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class financevoid3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ARMiscelaneousCollection", "Remark", c => c.String());
            AddColumn("EXCEED.ARMiscelaneousCollection", "IsOnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.ArStandardCollection", "Remark", c => c.String());
            AddColumn("EXCEED.ArStandardCollection", "IsOnlineTransferredBy", c => c.String());
            DropColumn("EXCEED.ARMiscelaneousCollection", "OnlineTransferredBy");
            DropColumn("EXCEED.ArStandardCollection", "OnlineTransferredBy");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ArStandardCollection", "OnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.ARMiscelaneousCollection", "OnlineTransferredBy", c => c.String());
            DropColumn("EXCEED.ArStandardCollection", "IsOnlineTransferredBy");
            DropColumn("EXCEED.ArStandardCollection", "Remark");
            DropColumn("EXCEED.ARMiscelaneousCollection", "IsOnlineTransferredBy");
            DropColumn("EXCEED.ARMiscelaneousCollection", "Remark");
        }
    }
}
