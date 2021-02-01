namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dessie : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.DeprciationReverse",
                c => new
                    {
                        DeprciationReverseId = c.Int(nullable: false, identity: true),
                        RegistrationID = c.Int(nullable: false),
                        StoreCode = c.String(),
                        Period = c.String(),
                        FiscalYear = c.String(),
                        BookValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DeprciationReverseId);
            
            CreateTable(
                "EXCEED.DepreciationPeriod",
                c => new
                    {
                        DepreciationPeriodId = c.Int(nullable: false, identity: true),
                        Period = c.Int(nullable: false),
                        FiscalYear = c.Int(nullable: false),
                        StoreCode = c.String(),
                        Status = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DepreciationPeriodId);
            
            AddColumn("EXCEED.DepreciationRule", "AccomodateAccount", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.DepreciationRule", "AccomodateAccount");
            DropTable("EXCEED.DepreciationPeriod");
            DropTable("EXCEED.DeprciationReverse");
        }
    }
}
