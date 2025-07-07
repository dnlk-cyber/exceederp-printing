namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InsuranceUpdates : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Insurance.InsurancePremiumSettings",
                c => new
                    {
                        InsurancePremiumSettingId = c.Int(nullable: false, identity: true),
                        Premium = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalMonths = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.InsurancePremiumSettingId);
            
        }
        
        public override void Down()
        {
            DropTable("Insurance.InsurancePremiumSettings");
        }
    }
}
