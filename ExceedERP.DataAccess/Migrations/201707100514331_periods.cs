namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class periods : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.FixedAssetPeriod",
                c => new
                    {
                        FixedAssetPeriodId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DateFrom = c.DateTime(nullable: false),
                        DateFromAmharic = c.String(),
                        DateTo = c.DateTime(nullable: false),
                        DateToAmharic = c.String(),
                        Status = c.Int(nullable: false),
                        IsPermanentlyClosed = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FixedAssetPeriodId);
            
            CreateTable(
                "EXCEED.ProcurementPeriod",
                c => new
                    {
                        ProcurementPeriodId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DateFrom = c.DateTime(nullable: false),
                        DateFromAmharic = c.String(),
                        DateTo = c.DateTime(nullable: false),
                        DateToAmharic = c.String(),
                        Status = c.Int(nullable: false),
                        IsPermanentlyClosed = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProcurementPeriodId);
            
        }
        
        public override void Down()
        {
            DropTable("EXCEED.ProcurementPeriod");
            DropTable("EXCEED.FixedAssetPeriod");
        }
    }
}
