namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class finance3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.BudgetAccountSheduleLine", "BudgetAccountSheduleId", "EXCEED.BudgetAccountShedule");
            DropIndex("EXCEED.BudgetAccountSheduleLine", new[] { "BudgetAccountSheduleId" });
            CreateTable(
                "EXCEED.BudgetAccountSheduleSubCategory",
                c => new
                    {
                        BudgetAccountSheduleSubCategoryId = c.Int(nullable: false, identity: true),
                        BudgetAccountSheduleId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BudgetAccountSheduleSubCategoryId)
                .ForeignKey("EXCEED.BudgetAccountShedule", t => t.BudgetAccountSheduleId, cascadeDelete: true)
                .Index(t => t.BudgetAccountSheduleId);
            
            AddColumn("EXCEED.BudgetAccountSheduleLine", "BudgetAccountSheduleSubCategoryId", c => c.Int(nullable: false));
            CreateIndex("EXCEED.BudgetAccountSheduleLine", "BudgetAccountSheduleSubCategoryId");
            AddForeignKey("EXCEED.BudgetAccountSheduleLine", "BudgetAccountSheduleSubCategoryId", "EXCEED.BudgetAccountSheduleSubCategory", "BudgetAccountSheduleSubCategoryId", cascadeDelete: true);
            DropColumn("EXCEED.BudgetAccountSheduleLine", "BudgetAccountSheduleId");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.BudgetAccountSheduleLine", "BudgetAccountSheduleId", c => c.Int(nullable: false));
            DropForeignKey("EXCEED.BudgetAccountSheduleLine", "BudgetAccountSheduleSubCategoryId", "EXCEED.BudgetAccountSheduleSubCategory");
            DropForeignKey("EXCEED.BudgetAccountSheduleSubCategory", "BudgetAccountSheduleId", "EXCEED.BudgetAccountShedule");
            DropIndex("EXCEED.BudgetAccountSheduleSubCategory", new[] { "BudgetAccountSheduleId" });
            DropIndex("EXCEED.BudgetAccountSheduleLine", new[] { "BudgetAccountSheduleSubCategoryId" });
            DropColumn("EXCEED.BudgetAccountSheduleLine", "BudgetAccountSheduleSubCategoryId");
            DropTable("EXCEED.BudgetAccountSheduleSubCategory");
            CreateIndex("EXCEED.BudgetAccountSheduleLine", "BudgetAccountSheduleId");
            AddForeignKey("EXCEED.BudgetAccountSheduleLine", "BudgetAccountSheduleId", "EXCEED.BudgetAccountShedule", "BudgetAccountSheduleId", cascadeDelete: true);
        }
    }
}
