namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class invento2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.StoreRequisitionItem", "ApprovedQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.StoreRequisitionItem", "IssuedQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("EXCEED.StoreRequisitionItem", "FirstApprovalQuantity");
            DropColumn("EXCEED.StoreRequisitionItem", "SecondApprovalQuantity");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.StoreRequisitionItem", "SecondApprovalQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.StoreRequisitionItem", "FirstApprovalQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("EXCEED.StoreRequisitionItem", "IssuedQuantity");
            DropColumn("EXCEED.StoreRequisitionItem", "ApprovedQuantity");
        }
    }
}
