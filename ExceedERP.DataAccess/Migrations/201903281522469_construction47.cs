namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction47 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Construction.ConsPaymentAgingPeriod",
                c => new
                    {
                        ConsPaymentAgingPeriodId = c.Int(nullable: false, identity: true),
                        Min = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Max = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TimeUnit = c.Int(nullable: false),
                        ConcatnatedRateDescription = c.String(),
                        ProvisionRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ConsPaymentAgingPeriodId);
            
        }
        
        public override void Down()
        {
            DropTable("Construction.ConsPaymentAgingPeriod");
        }
    }
}
