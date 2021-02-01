namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class issue3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.Issue", "StoreRequisitionID", "EXCEED.StoreRequisition");
            DropIndex("EXCEED.Issue", new[] { "StoreRequisitionID" });
        }
        
        public override void Down()
        {
            CreateIndex("EXCEED.Issue", "StoreRequisitionID");
            AddForeignKey("EXCEED.Issue", "StoreRequisitionID", "EXCEED.StoreRequisition", "StoreRequisitionID", cascadeDelete: true);
        }
    }
}
