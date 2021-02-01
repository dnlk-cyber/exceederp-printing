namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrmerge10 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Health.FixedAssetAccidentRecord",
                c => new
                    {
                        FixedAssetAccidentRecordId = c.Int(nullable: false, identity: true),
                        GlFiscalYearId = c.Int(nullable: false),
                        AssetNumber = c.String(maxLength: 128),
                        PlateNumber = c.String(),
                        Amount = c.Double(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FixedAssetAccidentRecordId)
                .ForeignKey("EXCEED.FleetEquipmentRegistration", t => t.AssetNumber)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: true)
                .Index(t => t.GlFiscalYearId)
                .Index(t => t.AssetNumber);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Health.FixedAssetAccidentRecord", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("Health.FixedAssetAccidentRecord", "AssetNumber", "EXCEED.FleetEquipmentRegistration");
            DropIndex("Health.FixedAssetAccidentRecord", new[] { "AssetNumber" });
            DropIndex("Health.FixedAssetAccidentRecord", new[] { "GlFiscalYearId" });
            DropTable("Health.FixedAssetAccidentRecord");
        }
    }
}
