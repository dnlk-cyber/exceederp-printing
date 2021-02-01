namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class arsalesoerder : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ArMiscelaneousCollectionLine", "Remark", c => c.String());
            AddColumn("EXCEED.ArSalesOrderLine", "SubItem", c => c.String());
            DropColumn("EXCEED.ApMiscellaneousPaymentLine", "OrgBranchName");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ApMiscellaneousPaymentLine", "OrgBranchName", c => c.String());
            DropColumn("EXCEED.ArSalesOrderLine", "SubItem");
            DropColumn("EXCEED.ArMiscelaneousCollectionLine", "Remark");
        }
    }
}
