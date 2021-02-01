namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class travel2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.ApTravelExpenseValidation", "ApTravelOrderId", "EXCEED.ApTravelOrder");
            DropForeignKey("EXCEED.ApTravelExpenseValidation", "ApTravelExpenseClaim_ApTravelExpenseClaimId", "EXCEED.ApTravelExpenseClaim");
            DropIndex("EXCEED.ApTravelExpenseValidation", new[] { "ApTravelOrderId" });
            DropIndex("EXCEED.ApTravelExpenseValidation", new[] { "ApTravelExpenseClaim_ApTravelExpenseClaimId" });
            RenameColumn(table: "EXCEED.ApTravelExpenseValidation", name: "ApTravelExpenseClaim_ApTravelExpenseClaimId", newName: "ApTravelExpenseClaimId");
            AlterColumn("EXCEED.ApTravelExpenseValidation", "ApTravelExpenseClaimId", c => c.Int(nullable: false));
            CreateIndex("EXCEED.ApTravelExpenseValidation", "ApTravelExpenseClaimId");
            AddForeignKey("EXCEED.ApTravelExpenseValidation", "ApTravelExpenseClaimId", "EXCEED.ApTravelExpenseClaim", "ApTravelExpenseClaimId", cascadeDelete: true);
            DropColumn("EXCEED.ApTravelExpenseValidation", "ApTravelOrderId");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ApTravelExpenseValidation", "ApTravelOrderId", c => c.Int(nullable: false));
            DropForeignKey("EXCEED.ApTravelExpenseValidation", "ApTravelExpenseClaimId", "EXCEED.ApTravelExpenseClaim");
            DropIndex("EXCEED.ApTravelExpenseValidation", new[] { "ApTravelExpenseClaimId" });
            AlterColumn("EXCEED.ApTravelExpenseValidation", "ApTravelExpenseClaimId", c => c.Int());
            RenameColumn(table: "EXCEED.ApTravelExpenseValidation", name: "ApTravelExpenseClaimId", newName: "ApTravelExpenseClaim_ApTravelExpenseClaimId");
            CreateIndex("EXCEED.ApTravelExpenseValidation", "ApTravelExpenseClaim_ApTravelExpenseClaimId");
            CreateIndex("EXCEED.ApTravelExpenseValidation", "ApTravelOrderId");
            AddForeignKey("EXCEED.ApTravelExpenseValidation", "ApTravelExpenseClaim_ApTravelExpenseClaimId", "EXCEED.ApTravelExpenseClaim", "ApTravelExpenseClaimId");
            AddForeignKey("EXCEED.ApTravelExpenseValidation", "ApTravelOrderId", "EXCEED.ApTravelOrder", "ApTravelOrderId", cascadeDelete: true);
        }
    }
}
