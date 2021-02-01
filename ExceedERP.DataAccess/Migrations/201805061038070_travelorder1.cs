namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class travelorder1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.ApTravelExpenseClaim", "ApTravelOrderId", "EXCEED.ApTravelOrder");
            DropIndex("EXCEED.ApTravelExpenseClaim", new[] { "ApTravelOrderId" });
            AddColumn("EXCEED.ApTravelExpenseClaim", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApTravelExpenseClaim", "IsForOther", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApTravelOrder", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApTravelOrder", "IsForOther", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ApTravelOrder", "IsForOther");
            DropColumn("EXCEED.ApTravelOrder", "EmployeeId");
            DropColumn("EXCEED.ApTravelExpenseClaim", "IsForOther");
            DropColumn("EXCEED.ApTravelExpenseClaim", "EmployeeId");
            CreateIndex("EXCEED.ApTravelExpenseClaim", "ApTravelOrderId");
            AddForeignKey("EXCEED.ApTravelExpenseClaim", "ApTravelOrderId", "EXCEED.ApTravelOrder", "ApTravelOrderId", cascadeDelete: true);
        }
    }
}
