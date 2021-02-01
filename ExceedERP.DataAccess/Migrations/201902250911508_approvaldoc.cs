namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class approvaldoc : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ApprovalDelegation",
                c => new
                    {
                        ApprovalDelegationId = c.Int(nullable: false, identity: true),
                        DelegatingEmployeeId = c.Int(nullable: false),
                        DelegatedEmployeeId = c.Int(nullable: false),
                        From = c.DateTime(nullable: false),
                        To = c.DateTime(),
                        Reason = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApprovalDelegationId);
            
            AddColumn("Global.DocumentApprovalRule", "HierarchyType", c => c.Int(nullable: false));
            AddColumn("Global.AssignApprovalGroup", "EmployeeId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Global.AssignApprovalGroup", "EmployeeId");
            DropColumn("Global.DocumentApprovalRule", "HierarchyType");
            DropTable("EXCEED.ApprovalDelegation");
        }
    }
}
