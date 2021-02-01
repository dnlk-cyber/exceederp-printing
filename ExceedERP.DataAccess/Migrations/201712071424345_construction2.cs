namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Construction.ConstructionPeriod",
                c => new
                    {
                        ConstructionPeriodId = c.Int(nullable: false, identity: true),
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
                .PrimaryKey(t => t.ConstructionPeriodId);
            
            AddColumn("Construction.LaborRateIndexFactorAttribute", "ApprovedIndex", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("Construction.LaborRateIndexFactorAttribute", "ApprovedIndex");
            DropTable("Construction.ConstructionPeriod");
        }
    }
}
