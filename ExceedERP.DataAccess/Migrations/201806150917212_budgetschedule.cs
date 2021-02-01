namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class budgetschedule : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.BudgetAccountSheduleCategory",
                c => new
                    {
                        BudgetAccountSheduleCategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BudgetAccountSheduleCategoryId);
            
            CreateTable(
                "EXCEED.BudgetAccountSheduleLine",
                c => new
                    {
                        BudgetAccountSheduleLineId = c.Int(nullable: false, identity: true),
                        BudgetAccountSheduleId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BudgetAccountSheduleLineId)
                .ForeignKey("EXCEED.BudgetAccountShedule", t => t.BudgetAccountSheduleId, cascadeDelete: true)
                .Index(t => t.BudgetAccountSheduleId);
            
            CreateTable(
                "EXCEED.BudgetAccountShedule",
                c => new
                    {
                        BudgetAccountSheduleId = c.Int(nullable: false, identity: true),
                        BudgetAccountSheduleCategoryId = c.Int(nullable: false),
                        Index = c.Int(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BudgetAccountSheduleId);
            
            CreateTable(
                "EXCEED.BudgetLineSheduleAccounts",
                c => new
                    {
                        BudgetLineSheduleAccountsId = c.Int(nullable: false, identity: true),
                        BudgetAccountSheduleLineId = c.Int(nullable: false),
                        FromAccountCode = c.String(nullable: false),
                        ToAccountCode = c.String(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BudgetLineSheduleAccountsId)
                .ForeignKey("EXCEED.BudgetAccountSheduleLine", t => t.BudgetAccountSheduleLineId, cascadeDelete: true)
                .Index(t => t.BudgetAccountSheduleLineId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.BudgetLineSheduleAccounts", "BudgetAccountSheduleLineId", "EXCEED.BudgetAccountSheduleLine");
            DropForeignKey("EXCEED.BudgetAccountSheduleLine", "BudgetAccountSheduleId", "EXCEED.BudgetAccountShedule");
            DropIndex("EXCEED.BudgetLineSheduleAccounts", new[] { "BudgetAccountSheduleLineId" });
            DropIndex("EXCEED.BudgetAccountSheduleLine", new[] { "BudgetAccountSheduleId" });
            DropTable("EXCEED.BudgetLineSheduleAccounts");
            DropTable("EXCEED.BudgetAccountShedule");
            DropTable("EXCEED.BudgetAccountSheduleLine");
            DropTable("EXCEED.BudgetAccountSheduleCategory");
        }
    }
}
