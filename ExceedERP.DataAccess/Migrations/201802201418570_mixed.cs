namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mixed : DbMigration
    {
        public override void Up()
        {
            AddColumn("Global.ApprovalGroupRule", "DateCreated", c => c.DateTime());
            AddColumn("Global.ApprovalGroupRule", "LastModified", c => c.DateTime());
            AddColumn("Global.ApprovalGroupRule", "CreatedBy", c => c.String());
            AddColumn("Global.ApprovalGroupRule", "ModifiedBy", c => c.String());
            AddColumn("Global.ApprovalGroup", "DateCreated", c => c.DateTime());
            AddColumn("Global.ApprovalGroup", "LastModified", c => c.DateTime());
            AddColumn("Global.ApprovalGroup", "CreatedBy", c => c.String());
            AddColumn("Global.ApprovalGroup", "ModifiedBy", c => c.String());
            AddColumn("Global.AssignApprovalGroupAssignment", "DateCreated", c => c.DateTime());
            AddColumn("Global.AssignApprovalGroupAssignment", "LastModified", c => c.DateTime());
            AddColumn("Global.AssignApprovalGroupAssignment", "CreatedBy", c => c.String());
            AddColumn("Global.AssignApprovalGroupAssignment", "ModifiedBy", c => c.String());
            AddColumn("Global.AssignApprovalGroup", "DateCreated", c => c.DateTime());
            AddColumn("Global.AssignApprovalGroup", "LastModified", c => c.DateTime());
            AddColumn("Global.AssignApprovalGroup", "CreatedBy", c => c.String());
            AddColumn("Global.AssignApprovalGroup", "ModifiedBy", c => c.String());
            AddColumn("Global.DocumentApprovalRule", "DateCreated", c => c.DateTime());
            AddColumn("Global.DocumentApprovalRule", "LastModified", c => c.DateTime());
            AddColumn("Global.DocumentApprovalRule", "CreatedBy", c => c.String());
            AddColumn("Global.DocumentApprovalRule", "ModifiedBy", c => c.String());
            AddColumn("HRM.NeedAssessment", "IsTransfered", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("HRM.NeedAssessment", "IsTransfered");
            DropColumn("Global.DocumentApprovalRule", "ModifiedBy");
            DropColumn("Global.DocumentApprovalRule", "CreatedBy");
            DropColumn("Global.DocumentApprovalRule", "LastModified");
            DropColumn("Global.DocumentApprovalRule", "DateCreated");
            DropColumn("Global.AssignApprovalGroup", "ModifiedBy");
            DropColumn("Global.AssignApprovalGroup", "CreatedBy");
            DropColumn("Global.AssignApprovalGroup", "LastModified");
            DropColumn("Global.AssignApprovalGroup", "DateCreated");
            DropColumn("Global.AssignApprovalGroupAssignment", "ModifiedBy");
            DropColumn("Global.AssignApprovalGroupAssignment", "CreatedBy");
            DropColumn("Global.AssignApprovalGroupAssignment", "LastModified");
            DropColumn("Global.AssignApprovalGroupAssignment", "DateCreated");
            DropColumn("Global.ApprovalGroup", "ModifiedBy");
            DropColumn("Global.ApprovalGroup", "CreatedBy");
            DropColumn("Global.ApprovalGroup", "LastModified");
            DropColumn("Global.ApprovalGroup", "DateCreated");
            DropColumn("Global.ApprovalGroupRule", "ModifiedBy");
            DropColumn("Global.ApprovalGroupRule", "CreatedBy");
            DropColumn("Global.ApprovalGroupRule", "LastModified");
            DropColumn("Global.ApprovalGroupRule", "DateCreated");
        }
    }
}
