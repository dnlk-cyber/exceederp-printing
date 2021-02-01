namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class travel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.ApTravelOrderDistribution", "ApPettyCashId", "EXCEED.ApPettyCash");
            DropForeignKey("EXCEED.ApTravelOrderDistribution", "ApTravelOrder_ApTravelOrderId", "EXCEED.ApTravelOrder");
            DropIndex("EXCEED.ApTravelOrderDistribution", new[] { "ApPettyCashId" });
            DropIndex("EXCEED.ApTravelOrderDistribution", new[] { "ApTravelOrder_ApTravelOrderId" });
            RenameColumn(table: "EXCEED.ApTravelOrderDistribution", name: "ApTravelOrder_ApTravelOrderId", newName: "ApTravelOrderId");
            AlterColumn("EXCEED.ApTravelOrderDistribution", "ApTravelOrderId", c => c.Int(nullable: false));
            CreateIndex("EXCEED.ApTravelOrderDistribution", "ApTravelOrderId");
            AddForeignKey("EXCEED.ApTravelOrderDistribution", "ApTravelOrderId", "EXCEED.ApTravelOrder", "ApTravelOrderId", cascadeDelete: true);
            DropColumn("EXCEED.ApTravelOrderDistribution", "ApPettyCashId");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ApTravelOrderDistribution", "ApPettyCashId", c => c.Int(nullable: false));
            DropForeignKey("EXCEED.ApTravelOrderDistribution", "ApTravelOrderId", "EXCEED.ApTravelOrder");
            DropIndex("EXCEED.ApTravelOrderDistribution", new[] { "ApTravelOrderId" });
            AlterColumn("EXCEED.ApTravelOrderDistribution", "ApTravelOrderId", c => c.Int());
            RenameColumn(table: "EXCEED.ApTravelOrderDistribution", name: "ApTravelOrderId", newName: "ApTravelOrder_ApTravelOrderId");
            CreateIndex("EXCEED.ApTravelOrderDistribution", "ApTravelOrder_ApTravelOrderId");
            CreateIndex("EXCEED.ApTravelOrderDistribution", "ApPettyCashId");
            AddForeignKey("EXCEED.ApTravelOrderDistribution", "ApTravelOrder_ApTravelOrderId", "EXCEED.ApTravelOrder", "ApTravelOrderId");
            AddForeignKey("EXCEED.ApTravelOrderDistribution", "ApPettyCashId", "EXCEED.ApPettyCash", "ApPettyCashId", cascadeDelete: true);
        }
    }
}
