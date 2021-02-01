namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class procurement14 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.AnnualNeedAssesmentItem", "Price", c => c.String());
            AlterColumn("EXCEED.ContractManagement", "ContractAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.ContractManagement", "ContractAmount", c => c.String());
            DropColumn("EXCEED.AnnualNeedAssesmentItem", "Price");
        }
    }
}
