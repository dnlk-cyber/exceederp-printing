namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedcatdep : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.CategoryDepreciationValue",
                c => new
                    {
                        CategoryDepreciationValueId = c.Int(nullable: false, identity: true),
                        FixedAssetCategoryID = c.Int(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        DepreciationAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CategoryDepreciationValueId);
            
        }
        
        public override void Down()
        {
            DropTable("EXCEED.CategoryDepreciationValue");
        }
    }
}
