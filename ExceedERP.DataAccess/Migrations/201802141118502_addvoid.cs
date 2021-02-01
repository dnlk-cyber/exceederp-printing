namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addvoid : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.FixedAssetRequisition", "IsSecondVoid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.FixedAssetRequisition", "SecondVoidBy", c => c.String());
            AddColumn("EXCEED.FixedAssetRequisition", "SecondVoidTime", c => c.DateTime());
            AddColumn("EXCEED.PurchaseRequisition", "IsSecondVoid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchaseRequisition", "SecondVoidBy", c => c.String());
            AddColumn("EXCEED.PurchaseRequisition", "SecondVoidTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PurchaseRequisition", "SecondVoidTime");
            DropColumn("EXCEED.PurchaseRequisition", "SecondVoidBy");
            DropColumn("EXCEED.PurchaseRequisition", "IsSecondVoid");
            DropColumn("EXCEED.FixedAssetRequisition", "SecondVoidTime");
            DropColumn("EXCEED.FixedAssetRequisition", "SecondVoidBy");
            DropColumn("EXCEED.FixedAssetRequisition", "IsSecondVoid");
        }
    }
}
