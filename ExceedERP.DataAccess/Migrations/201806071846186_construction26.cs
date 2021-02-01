namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction26 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Construction.ProjectContract", "SiteHandoverTime", c => c.DateTime(nullable: false));
            AddColumn("Construction.ExtensionOfTime", "EoTRequesteDate", c => c.DateTime(nullable: false));
            AddColumn("Construction.ExtensionOfTime", "EoTApproveDate", c => c.DateTime(nullable: false));
            AddColumn("Construction.ExtensionOfTime", "OrgBranchName", c => c.String());
            AddColumn("Construction.ExtensionOfTime", "DateCreated", c => c.DateTime());
            AddColumn("Construction.ExtensionOfTime", "LastModified", c => c.DateTime());
            AddColumn("Construction.ExtensionOfTime", "CreatedBy", c => c.String());
            AddColumn("Construction.ExtensionOfTime", "ModifiedBy", c => c.String());
            AddColumn("Construction.ContractVariationOrder", "DaysAdded", c => c.Int(nullable: false));
            AddColumn("Construction.ContractVariationOrder", "OrgBranchName", c => c.String());
            AddColumn("Construction.ContractVariationOrder", "DateCreated", c => c.DateTime());
            AddColumn("Construction.ContractVariationOrder", "LastModified", c => c.DateTime());
            AddColumn("Construction.ContractVariationOrder", "CreatedBy", c => c.String());
            AddColumn("Construction.ContractVariationOrder", "ModifiedBy", c => c.String());
            AddColumn("Construction.VariationOrderType", "Type", c => c.Int(nullable: false));
            AddColumn("Construction.VariationOrderType", "OrgBranchName", c => c.String());
            AddColumn("Construction.VariationOrderType", "DateCreated", c => c.DateTime());
            AddColumn("Construction.VariationOrderType", "LastModified", c => c.DateTime());
            AddColumn("Construction.VariationOrderType", "CreatedBy", c => c.String());
            AddColumn("Construction.VariationOrderType", "ModifiedBy", c => c.String());
            AlterColumn("Construction.ContractVariationOrder", "ChangeCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("Construction.ContractVariationOrder", "OrderStatus");
            DropColumn("Construction.ContractVariationOrder", "Priority");
            DropColumn("Construction.ContractVariationOrder", "DateIssued");
            DropColumn("Construction.ContractVariationOrder", "IssuedBy");
        }
        
        public override void Down()
        {
            AddColumn("Construction.ContractVariationOrder", "IssuedBy", c => c.String());
            AddColumn("Construction.ContractVariationOrder", "DateIssued", c => c.DateTime(nullable: false));
            AddColumn("Construction.ContractVariationOrder", "Priority", c => c.String());
            AddColumn("Construction.ContractVariationOrder", "OrderStatus", c => c.String());
            AlterColumn("Construction.ContractVariationOrder", "ChangeCost", c => c.Double(nullable: false));
            DropColumn("Construction.VariationOrderType", "ModifiedBy");
            DropColumn("Construction.VariationOrderType", "CreatedBy");
            DropColumn("Construction.VariationOrderType", "LastModified");
            DropColumn("Construction.VariationOrderType", "DateCreated");
            DropColumn("Construction.VariationOrderType", "OrgBranchName");
            DropColumn("Construction.VariationOrderType", "Type");
            DropColumn("Construction.ContractVariationOrder", "ModifiedBy");
            DropColumn("Construction.ContractVariationOrder", "CreatedBy");
            DropColumn("Construction.ContractVariationOrder", "LastModified");
            DropColumn("Construction.ContractVariationOrder", "DateCreated");
            DropColumn("Construction.ContractVariationOrder", "OrgBranchName");
            DropColumn("Construction.ContractVariationOrder", "DaysAdded");
            DropColumn("Construction.ExtensionOfTime", "ModifiedBy");
            DropColumn("Construction.ExtensionOfTime", "CreatedBy");
            DropColumn("Construction.ExtensionOfTime", "LastModified");
            DropColumn("Construction.ExtensionOfTime", "DateCreated");
            DropColumn("Construction.ExtensionOfTime", "OrgBranchName");
            DropColumn("Construction.ExtensionOfTime", "EoTApproveDate");
            DropColumn("Construction.ExtensionOfTime", "EoTRequesteDate");
            DropColumn("Construction.ProjectContract", "SiteHandoverTime");
        }
    }
}
