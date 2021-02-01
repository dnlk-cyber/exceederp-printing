namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedassetname2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.PurchaseRequisitionReassignmentItem", "Name", c => c.String());
            AddColumn("EXCEED.PurchaseRequisitionItem", "Name", c => c.String());
            AlterColumn("EXCEED.ArInvoice", "SourceInvoiceId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.ArInvoice", "SourceInvoiceId", c => c.Int(nullable: false));
            DropColumn("EXCEED.PurchaseRequisitionItem", "Name");
            DropColumn("EXCEED.PurchaseRequisitionReassignmentItem", "Name");
        }
    }
}
