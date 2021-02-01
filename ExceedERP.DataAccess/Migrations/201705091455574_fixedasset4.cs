namespace ExceedERP.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class fixedasset4 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "EXCEED.LocationTracking",
                    c => new
                    {
                        LocationTrackingId = c.Int(nullable: false, identity: true),
                        RegistrationId = c.Int(nullable: false),
                        LocationType = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDateDate = c.DateTime(),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LocationTrackingId);
                //.ForeignKey("EXCEED.AssetRegistration", t => t.RegistrationId, cascadeDelete: true)
               // .Index(t => t.RegistrationId);

            AddColumn("EXCEED.FixedAssetReceiveItem", "TagNo", c => c.String());
            AlterColumn("EXCEED.FixedAssetReceiveItem", "Quantity", c => c.Int(nullable: false));
            AlterColumn("EXCEED.FixedAssetIssueItem", "RequestedQuantity", c => c.Int(nullable: false));
            AlterColumn("EXCEED.FixedAssetIssueItem", "ApprovedQuantity", c => c.Int(nullable: false));
            AlterColumn("EXCEED.FixedAssetIssueItem", "IssuedQuantity", c => c.Int(nullable: false));
            AlterColumn("EXCEED.FixedAssetIssueItem", "TagNo", c => c.String(nullable: false));
        }

        public override void Down()
        {
            DropForeignKey("EXCEED.LocationTracking", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropIndex("EXCEED.LocationTracking", new[] { "AssetRegistrationId" });
            AlterColumn("EXCEED.FixedAssetIssueItem", "TagNo", c => c.String());
            AlterColumn("EXCEED.FixedAssetIssueItem", "IssuedQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.FixedAssetIssueItem", "ApprovedQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.FixedAssetIssueItem", "RequestedQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.FixedAssetReceiveItem", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("EXCEED.FixedAssetReceiveItem", "TagNo");
            DropTable("EXCEED.LocationTracking");
        }
    }
}
