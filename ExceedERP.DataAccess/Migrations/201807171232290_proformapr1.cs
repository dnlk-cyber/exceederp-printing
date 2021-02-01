namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class proformapr1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ProformaPurchase", "PurchaseRequisitionAssignmentId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ProformaPurchase", "PurchaseRequisitionAssignmentId");
        }
    }
}
