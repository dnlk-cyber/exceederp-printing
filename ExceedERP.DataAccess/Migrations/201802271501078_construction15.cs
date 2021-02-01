namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction15 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Construction.WorkItemPerProject", "ItemCode", c => c.String());
            AddColumn("Construction.ProjectEstimnWorkItem", "ItemCode", c => c.String());
            AddColumn("Construction.ProjectEstimationBoqDetail", "BillNumber", c => c.String());
            AddColumn("Construction.ProjectEstimationBoqDetail", "Index", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectTypeMapping", "CodeOrSeries", c => c.Int());
            AlterColumn("Construction.ProjectEstimnBoqBreakdown", "BillNumber", c => c.String(nullable: false));
            AlterColumn("Construction.ProjectEstimnBoqBreakdown", "Description", c => c.String(nullable: false));
            AlterColumn("Construction.ProjectEstimationBoqDetail", "Description", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("Construction.ProjectEstimationBoqDetail", "Description", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("Construction.ProjectEstimnBoqBreakdown", "Description", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("Construction.ProjectEstimnBoqBreakdown", "BillNumber", c => c.String(nullable: false, maxLength: 100));
            DropColumn("Construction.ProjectTypeMapping", "CodeOrSeries");
            DropColumn("Construction.ProjectEstimationBoqDetail", "Index");
            DropColumn("Construction.ProjectEstimationBoqDetail", "BillNumber");
            DropColumn("Construction.ProjectEstimnWorkItem", "ItemCode");
            DropColumn("Construction.WorkItemPerProject", "ItemCode");
        }
    }
}
