namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class distvoucher : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.FixedAssetReceiveDistribution", "AdditionalDescription", c => c.String());
            AddColumn("EXCEED.FixedAssetReceiveDistribution", "OrgBranchName", c => c.String());
            AddColumn("EXCEED.FixedAssetReceiveDistribution", "DateCreated", c => c.DateTime());
            AddColumn("EXCEED.FixedAssetReceiveDistribution", "LastModified", c => c.DateTime());
            AddColumn("EXCEED.FixedAssetReceiveDistribution", "CreatedBy", c => c.String());
            AddColumn("EXCEED.FixedAssetReceiveDistribution", "ModifiedBy", c => c.String());
            AddColumn("EXCEED.GoodReciveDonationDistribution", "AdditionalDescription", c => c.String());
            AddColumn("EXCEED.GoodSellingAndLendingDistribution", "AdditionalDescription", c => c.String());
            AddColumn("EXCEED.GoodSellingAndLendingDistribution", "OrgBranchName", c => c.String());
            AddColumn("EXCEED.GoodSellingAndLendingDistribution", "DateCreated", c => c.DateTime());
            AddColumn("EXCEED.GoodSellingAndLendingDistribution", "LastModified", c => c.DateTime());
            AddColumn("EXCEED.GoodSellingAndLendingDistribution", "CreatedBy", c => c.String());
            AddColumn("EXCEED.GoodSellingAndLendingDistribution", "ModifiedBy", c => c.String());
            AddColumn("EXCEED.PurchaseOrderDistribution", "AdditionalDescription", c => c.String());
            AddColumn("EXCEED.PurchaseReturnDistribution", "AdditionalDescription", c => c.String());
            AddColumn("EXCEED.PurchaseReturnDistribution", "OrgBranchName", c => c.String());
            AddColumn("EXCEED.PurchaseReturnDistribution", "DateCreated", c => c.DateTime());
            AddColumn("EXCEED.PurchaseReturnDistribution", "LastModified", c => c.DateTime());
            AddColumn("EXCEED.PurchaseReturnDistribution", "CreatedBy", c => c.String());
            AddColumn("EXCEED.PurchaseReturnDistribution", "ModifiedBy", c => c.String());
            AddColumn("EXCEED.ServiceReceiveDistribution", "AdditionalDescription", c => c.String());
            AddColumn("EXCEED.ServiceReceiveDistribution", "OrgBranchName", c => c.String());
            AddColumn("EXCEED.ServiceReceiveDistribution", "DateCreated", c => c.DateTime());
            AddColumn("EXCEED.ServiceReceiveDistribution", "LastModified", c => c.DateTime());
            AddColumn("EXCEED.ServiceReceiveDistribution", "CreatedBy", c => c.String());
            AddColumn("EXCEED.ServiceReceiveDistribution", "ModifiedBy", c => c.String());
            DropColumn("EXCEED.GoodReciveDonationDistribution", "GoodReceiveItemIDTrackBack");
            DropColumn("EXCEED.GoodSellingAndLendingDistribution", "GoodSellingAndLendingItemIDTrackBack");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.GoodSellingAndLendingDistribution", "GoodSellingAndLendingItemIDTrackBack", c => c.Int(nullable: false));
            AddColumn("EXCEED.GoodReciveDonationDistribution", "GoodReceiveItemIDTrackBack", c => c.Int(nullable: false));
            DropColumn("EXCEED.ServiceReceiveDistribution", "ModifiedBy");
            DropColumn("EXCEED.ServiceReceiveDistribution", "CreatedBy");
            DropColumn("EXCEED.ServiceReceiveDistribution", "LastModified");
            DropColumn("EXCEED.ServiceReceiveDistribution", "DateCreated");
            DropColumn("EXCEED.ServiceReceiveDistribution", "OrgBranchName");
            DropColumn("EXCEED.ServiceReceiveDistribution", "AdditionalDescription");
            DropColumn("EXCEED.PurchaseReturnDistribution", "ModifiedBy");
            DropColumn("EXCEED.PurchaseReturnDistribution", "CreatedBy");
            DropColumn("EXCEED.PurchaseReturnDistribution", "LastModified");
            DropColumn("EXCEED.PurchaseReturnDistribution", "DateCreated");
            DropColumn("EXCEED.PurchaseReturnDistribution", "OrgBranchName");
            DropColumn("EXCEED.PurchaseReturnDistribution", "AdditionalDescription");
            DropColumn("EXCEED.PurchaseOrderDistribution", "AdditionalDescription");
            DropColumn("EXCEED.GoodSellingAndLendingDistribution", "ModifiedBy");
            DropColumn("EXCEED.GoodSellingAndLendingDistribution", "CreatedBy");
            DropColumn("EXCEED.GoodSellingAndLendingDistribution", "LastModified");
            DropColumn("EXCEED.GoodSellingAndLendingDistribution", "DateCreated");
            DropColumn("EXCEED.GoodSellingAndLendingDistribution", "OrgBranchName");
            DropColumn("EXCEED.GoodSellingAndLendingDistribution", "AdditionalDescription");
            DropColumn("EXCEED.GoodReciveDonationDistribution", "AdditionalDescription");
            DropColumn("EXCEED.FixedAssetReceiveDistribution", "ModifiedBy");
            DropColumn("EXCEED.FixedAssetReceiveDistribution", "CreatedBy");
            DropColumn("EXCEED.FixedAssetReceiveDistribution", "LastModified");
            DropColumn("EXCEED.FixedAssetReceiveDistribution", "DateCreated");
            DropColumn("EXCEED.FixedAssetReceiveDistribution", "OrgBranchName");
            DropColumn("EXCEED.FixedAssetReceiveDistribution", "AdditionalDescription");
        }
    }
}
