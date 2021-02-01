namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class prassignmentitem : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.PurchaseRequisitionReassignmentItem", "SerialNo", c => c.String());
            AddColumn("EXCEED.PurchaseRequisitionReassignmentItem", "ChasisNo", c => c.String());
            AddColumn("EXCEED.PurchaseRequisitionReassignmentItem", "ItemSpecification", c => c.String());
            AddColumn("EXCEED.PurchaseRequisitionReassignmentItem", "UoM", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.PurchaseRequisitionReassignmentItem", "UoM");
            DropColumn("EXCEED.PurchaseRequisitionReassignmentItem", "ItemSpecification");
            DropColumn("EXCEED.PurchaseRequisitionReassignmentItem", "ChasisNo");
            DropColumn("EXCEED.PurchaseRequisitionReassignmentItem", "SerialNo");
        }
    }
}
