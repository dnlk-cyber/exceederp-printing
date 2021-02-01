namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction37 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Construction.PerformanceEvaluationComponentWeight", "ValueForRoadProjects", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectEstmnBoqSummary", "SubProjectTypeId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectEstmnBoqSummary", "WorkCategory", c => c.String());
            AddColumn("Construction.ProjectEstmnBoqSummary", "StructureyType", c => c.String());
            AddColumn("Construction.ProjectEstmnBoqSummary", "SubProjectTypeName", c => c.String());
            AlterColumn("Construction.ProjectEstimnWorkItem", "TotalQuantityOfWork", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ProjectEstimnWorkItem", "OverHeadCost", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.ProjectEstimnWorkItem", "ProfitCost", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            DropColumn("Construction.ProjectEstmnBoqSummary", "Type");
            DropColumn("Construction.ProjectEstmnBoqSummary", "Category");
            DropColumn("Construction.ProjectEstmnBoqSummary", "BlockId");
            DropColumn("Construction.ProjectEstmnBoqSummary", "Block");
        }
        
        public override void Down()
        {
            AddColumn("Construction.ProjectEstmnBoqSummary", "Block", c => c.String());
            AddColumn("Construction.ProjectEstmnBoqSummary", "BlockId", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectEstmnBoqSummary", "Category", c => c.String());
            AddColumn("Construction.ProjectEstmnBoqSummary", "Type", c => c.String());
            AlterColumn("Construction.ProjectEstimnWorkItem", "ProfitCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ProjectEstimnWorkItem", "OverHeadCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.ProjectEstimnWorkItem", "TotalQuantityOfWork", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("Construction.ProjectEstmnBoqSummary", "SubProjectTypeName");
            DropColumn("Construction.ProjectEstmnBoqSummary", "StructureyType");
            DropColumn("Construction.ProjectEstmnBoqSummary", "WorkCategory");
            DropColumn("Construction.ProjectEstmnBoqSummary", "SubProjectTypeId");
            DropColumn("Construction.PerformanceEvaluationComponentWeight", "ValueForRoadProjects");
        }
    }
}
