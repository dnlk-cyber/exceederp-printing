namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class additionaldescription : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.GoodReceiveDistribution", "AdditionalDescription", c => c.String());
            AddColumn("EXCEED.GoodReceiveDistribution", "OrgBranchName", c => c.String());
            AddColumn("EXCEED.GoodReceiveDistribution", "DateCreated", c => c.DateTime());
            AddColumn("EXCEED.GoodReceiveDistribution", "LastModified", c => c.DateTime());
            AddColumn("EXCEED.GoodReceiveDistribution", "CreatedBy", c => c.String());
            AddColumn("EXCEED.GoodReceiveDistribution", "ModifiedBy", c => c.String());
            AddColumn("EXCEED.IssueDistribution", "AdditionalDescription", c => c.String());
            AddColumn("EXCEED.IssueDistribution", "OrgBranchName", c => c.String());
            AddColumn("EXCEED.IssueDistribution", "DateCreated", c => c.DateTime());
            AddColumn("EXCEED.IssueDistribution", "LastModified", c => c.DateTime());
            AddColumn("EXCEED.IssueDistribution", "CreatedBy", c => c.String());
            AddColumn("EXCEED.IssueDistribution", "ModifiedBy", c => c.String());
            AddColumn("EXCEED.ItemTransferDistribution", "AdditionalDescription", c => c.String());
            AddColumn("EXCEED.ItemTransferDistribution", "OrgBranchName", c => c.String());
            AddColumn("EXCEED.ItemTransferDistribution", "DateCreated", c => c.DateTime());
            AddColumn("EXCEED.ItemTransferDistribution", "LastModified", c => c.DateTime());
            AddColumn("EXCEED.ItemTransferDistribution", "CreatedBy", c => c.String());
            AddColumn("EXCEED.ItemTransferDistribution", "ModifiedBy", c => c.String());
            AddColumn("EXCEED.StoreReturnDistribution", "AdditionalDescription", c => c.String());
            AddColumn("EXCEED.StoreReturnDistribution", "OrgBranchName", c => c.String());
            AddColumn("EXCEED.StoreReturnDistribution", "DateCreated", c => c.DateTime());
            AddColumn("EXCEED.StoreReturnDistribution", "LastModified", c => c.DateTime());
            AddColumn("EXCEED.StoreReturnDistribution", "CreatedBy", c => c.String());
            AddColumn("EXCEED.StoreReturnDistribution", "ModifiedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.StoreReturnDistribution", "ModifiedBy");
            DropColumn("EXCEED.StoreReturnDistribution", "CreatedBy");
            DropColumn("EXCEED.StoreReturnDistribution", "LastModified");
            DropColumn("EXCEED.StoreReturnDistribution", "DateCreated");
            DropColumn("EXCEED.StoreReturnDistribution", "OrgBranchName");
            DropColumn("EXCEED.StoreReturnDistribution", "AdditionalDescription");
            DropColumn("EXCEED.ItemTransferDistribution", "ModifiedBy");
            DropColumn("EXCEED.ItemTransferDistribution", "CreatedBy");
            DropColumn("EXCEED.ItemTransferDistribution", "LastModified");
            DropColumn("EXCEED.ItemTransferDistribution", "DateCreated");
            DropColumn("EXCEED.ItemTransferDistribution", "OrgBranchName");
            DropColumn("EXCEED.ItemTransferDistribution", "AdditionalDescription");
            DropColumn("EXCEED.IssueDistribution", "ModifiedBy");
            DropColumn("EXCEED.IssueDistribution", "CreatedBy");
            DropColumn("EXCEED.IssueDistribution", "LastModified");
            DropColumn("EXCEED.IssueDistribution", "DateCreated");
            DropColumn("EXCEED.IssueDistribution", "OrgBranchName");
            DropColumn("EXCEED.IssueDistribution", "AdditionalDescription");
            DropColumn("EXCEED.GoodReceiveDistribution", "ModifiedBy");
            DropColumn("EXCEED.GoodReceiveDistribution", "CreatedBy");
            DropColumn("EXCEED.GoodReceiveDistribution", "LastModified");
            DropColumn("EXCEED.GoodReceiveDistribution", "DateCreated");
            DropColumn("EXCEED.GoodReceiveDistribution", "OrgBranchName");
            DropColumn("EXCEED.GoodReceiveDistribution", "AdditionalDescription");
        }
    }
}
