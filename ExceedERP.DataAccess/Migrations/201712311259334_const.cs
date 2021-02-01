namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _const : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Construction.DelayAnalysisTask",
                c => new
                    {
                        DelayAnalysisTaskId = c.Int(nullable: false, identity: true),
                        ID = c.Int(nullable: false),
                        ParentID = c.Int(),
                        OrderID = c.Int(nullable: false),
                        Title = c.String(),
                        Start = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                        PercentComplete = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Expanded = c.Boolean(nullable: false),
                        Summary = c.Boolean(nullable: false),
                        DelayDuration = c.Int(nullable: false),
                        ParentTask_DelayAnalysisTaskId = c.Int(),
                    })
                .PrimaryKey(t => t.DelayAnalysisTaskId)
                .ForeignKey("Construction.DelayAnalysisTask", t => t.ParentTask_DelayAnalysisTaskId)
                .ForeignKey("Task.Task", t => t.ID, cascadeDelete: true)
                .Index(t => t.ID)
                .Index(t => t.ParentTask_DelayAnalysisTaskId);
            
            CreateTable(
                "Construction.DelayTaskDependency",
                c => new
                    {
                        DelayTaskDependencyId = c.Int(nullable: false, identity: true),
                        ID = c.Int(nullable: false),
                        PredecessorID = c.Int(nullable: false),
                        SuccessorID = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DelayTaskDependencyId)
                .ForeignKey("Task.Dependency", t => t.ID, cascadeDelete: true)
                .Index(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Construction.DelayTaskDependency", "ID", "Task.Dependency");
            DropForeignKey("Construction.DelayAnalysisTask", "ID", "Task.Task");
            DropForeignKey("Construction.DelayAnalysisTask", "ParentTask_DelayAnalysisTaskId", "Construction.DelayAnalysisTask");
            DropIndex("Construction.DelayTaskDependency", new[] { "ID" });
            DropIndex("Construction.DelayAnalysisTask", new[] { "ParentTask_DelayAnalysisTaskId" });
            DropIndex("Construction.DelayAnalysisTask", new[] { "ID" });
            DropTable("Construction.DelayTaskDependency");
            DropTable("Construction.DelayAnalysisTask");
        }
    }
}
