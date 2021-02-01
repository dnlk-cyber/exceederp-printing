namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class issue : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.Issue", "BranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.Issue", "OrgStructureId", c => c.Int(nullable: false));
            AddColumn("EXCEED.IssueFuel", "BranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.IssueFuel", "OrgStructureId", c => c.Int(nullable: false));
            DropColumn("EXCEED.IssueFuel", "RequestingDepartment");
            DropColumn("EXCEED.IssueFuel", "Project");
            DropColumn("EXCEED.IssueFuel", "FiscalYear");
            DropColumn("EXCEED.IssueFuel", "Period");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.IssueFuel", "Period", c => c.String());
            AddColumn("EXCEED.IssueFuel", "FiscalYear", c => c.String());
            AddColumn("EXCEED.IssueFuel", "Project", c => c.String());
            AddColumn("EXCEED.IssueFuel", "RequestingDepartment", c => c.String());
            DropColumn("EXCEED.IssueFuel", "OrgStructureId");
            DropColumn("EXCEED.IssueFuel", "BranchId");
            DropColumn("EXCEED.Issue", "OrgStructureId");
            DropColumn("EXCEED.Issue", "BranchId");
        }
    }
}
