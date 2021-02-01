namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class budget1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Budget.BudgetBankAccountMap",
                c => new
                    {
                        BudgetBankAccountMapId = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        IsHeadOffice = c.Boolean(nullable: false),
                        AccountCode = c.String(),
                        BankNumber = c.String(),
                        Address = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BudgetBankAccountMapId);
            
        }
        
        public override void Down()
        {
            DropTable("Budget.BudgetBankAccountMap");
        }
    }
}
