namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class documrntapproval : DbMigration
    {
        public override void Up()
        {
            DropColumn("Global.AssignApprovalGroup", "DocumentType");
            DropColumn("Global.AssignApprovalGroup", "AmountLimit");
            DropColumn("Global.AssignApprovalGroup", "LowValue");
            DropColumn("Global.AssignApprovalGroup", "HighValue");
        }
        
        public override void Down()
        {
            AddColumn("Global.AssignApprovalGroup", "HighValue", c => c.String());
            AddColumn("Global.AssignApprovalGroup", "LowValue", c => c.String());
            AddColumn("Global.AssignApprovalGroup", "AmountLimit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Global.AssignApprovalGroup", "DocumentType", c => c.Int(nullable: false));
        }
    }
}
