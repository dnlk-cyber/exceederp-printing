namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class chartcatego : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.GlChartOfAccountCategory",
                c => new
                    {
                        GlChartOfAccountCategoryId = c.Int(nullable: false, identity: true),
                        AccountType = c.Int(nullable: false),
                        AccountSubType = c.String(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GlChartOfAccountCategoryId);
            
            //AlterColumn("EXCEED.GLChartOfAccountAccount", "AccountSubType", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.GLChartOfAccountAccount", "AccountSubType", c => c.Int(nullable: false));
            DropTable("EXCEED.GlChartOfAccountCategory");
        }
    }
}
