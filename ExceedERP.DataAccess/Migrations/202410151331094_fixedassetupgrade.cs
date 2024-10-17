namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedassetupgrade : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.FixedAssetIssue", "FixedAssetRequisitionId", "EXCEED.FixedAssetRequisition");
            DropIndex("EXCEED.FixedAssetIssue", new[] { "FixedAssetRequisitionId" });
            CreateTable(
                "EXCEED.FixedAssetRequisitionAttachment",
                c => new
                    {
                        FixedAssetRequisitionAttachmentId = c.Int(nullable: false, identity: true),
                        FixedAssetRequisitionId = c.Int(nullable: false),
                        FileName = c.String(),
                        FilePath = c.String(),
                    })
                .PrimaryKey(t => t.FixedAssetRequisitionAttachmentId)
                .ForeignKey("EXCEED.FixedAssetRequisition", t => t.FixedAssetRequisitionId, cascadeDelete: true)
                .Index(t => t.FixedAssetRequisitionId);
            
            AddColumn("EXCEED.AnnualDepreciation", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.AssetTransfer", "FixedAssetRequisitionId", c => c.Int());
            AddColumn("EXCEED.AssetTransfer", "Reference", c => c.String());
            AddColumn("EXCEED.AssetTransfer", "WithOutRequest", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.AssetTransfer", "FromGRN", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.AssetTransfer", "GRNNo", c => c.Int());
            AddColumn("EXCEED.FixedAssetCountItem", "PartNo", c => c.String());
            AddColumn("EXCEED.FixedAssetCountItem", "ModelNo", c => c.String());
            AddColumn("EXCEED.FixedAssetCountItem", "SerialNo", c => c.String());
            AddColumn("EXCEED.FixedAssetCountItem", "EngineNo", c => c.String());
            AddColumn("EXCEED.FixedAssetCountItem", "ChasisNo", c => c.String());
            AddColumn("EXCEED.FixedAssetCountItem", "PlateNo", c => c.String());
            AddColumn("EXCEED.FixedAssetIssueItem", "FixedAssetNameID", c => c.Int(nullable: false));
            AddColumn("EXCEED.FixedAssetIssue", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.FixedAssetIssue", "IsManual", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.FixedAssetRequisition", "CreatedAtOrgStructureId", c => c.Int(nullable: false));
            AddColumn("EXCEED.FixedAssetRequisitionItem", "AssetRegistrationId", c => c.Int());
            AddColumn("EXCEED.FixedAssetRequisitionItem", "TagNo", c => c.String());
            AddColumn("EXCEED.FixedAssetRequisitionItem", "EngineNo", c => c.String());
            AddColumn("EXCEED.FixedAssetRequisitionItem", "PlateNo", c => c.String());
            AddColumn("EXCEED.FixedAssetRequisitionItem", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.AssetRegistration", "BusinessUnitId", c => c.Int(nullable: false));
            CreateIndex("EXCEED.DisposalItem", "AssetRegistrationId");
            AddForeignKey("EXCEED.DisposalItem", "AssetRegistrationId", "EXCEED.AssetRegistration", "AssetRegistrationId", cascadeDelete: false);
            DropColumn("EXCEED.AssetTransfer", "RequestNumber");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.AssetTransfer", "RequestNumber", c => c.String());
            DropForeignKey("EXCEED.FixedAssetRequisitionAttachment", "FixedAssetRequisitionId", "EXCEED.FixedAssetRequisition");
            DropForeignKey("EXCEED.DisposalItem", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropIndex("EXCEED.FixedAssetRequisitionAttachment", new[] { "FixedAssetRequisitionId" });
            DropIndex("EXCEED.DisposalItem", new[] { "AssetRegistrationId" });
            AlterColumn("EXCEED.AssetRegistration", "BusinessUnitId", c => c.Int());
            DropColumn("EXCEED.FixedAssetRequisitionItem", "Price");
            DropColumn("EXCEED.FixedAssetRequisitionItem", "PlateNo");
            DropColumn("EXCEED.FixedAssetRequisitionItem", "EngineNo");
            DropColumn("EXCEED.FixedAssetRequisitionItem", "TagNo");
            DropColumn("EXCEED.FixedAssetRequisitionItem", "AssetRegistrationId");
            DropColumn("EXCEED.FixedAssetRequisition", "CreatedAtOrgStructureId");
            DropColumn("EXCEED.FixedAssetIssue", "IsManual");
            DropColumn("EXCEED.FixedAssetIssue", "BusinessUnitId");
            DropColumn("EXCEED.FixedAssetIssueItem", "FixedAssetNameID");
            DropColumn("EXCEED.FixedAssetCountItem", "PlateNo");
            DropColumn("EXCEED.FixedAssetCountItem", "ChasisNo");
            DropColumn("EXCEED.FixedAssetCountItem", "EngineNo");
            DropColumn("EXCEED.FixedAssetCountItem", "SerialNo");
            DropColumn("EXCEED.FixedAssetCountItem", "ModelNo");
            DropColumn("EXCEED.FixedAssetCountItem", "PartNo");
            DropColumn("EXCEED.AssetTransfer", "GRNNo");
            DropColumn("EXCEED.AssetTransfer", "FromGRN");
            DropColumn("EXCEED.AssetTransfer", "WithOutRequest");
            DropColumn("EXCEED.AssetTransfer", "Reference");
            DropColumn("EXCEED.AssetTransfer", "FixedAssetRequisitionId");
            DropColumn("EXCEED.AnnualDepreciation", "BusinessUnitId");
            DropTable("EXCEED.FixedAssetRequisitionAttachment");
            CreateIndex("EXCEED.FixedAssetIssue", "FixedAssetRequisitionId");
            AddForeignKey("EXCEED.FixedAssetIssue", "FixedAssetRequisitionId", "EXCEED.FixedAssetRequisition", "FixedAssetRequisitionId", cascadeDelete: true);
        }
    }
}
