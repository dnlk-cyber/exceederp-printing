namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HRMTriaingChangesMigration : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("HRM.NeedAssessment", "OrgStructureId", "HRM.MainCompanyStructure");
            DropIndex("HRM.NeedAssessment", new[] { "OrgStructureId" });
            AddColumn("HRM.NeedAssessment", "TotalDays", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("HRM.NeedAssessment", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("HRM.NeedAssessment", "From", c => c.DateTime());
            AddColumn("HRM.NeedAssessment", "To", c => c.DateTime());
            AddColumn("HRM.NeedAssessment", "FromAmharic", c => c.String());
            AddColumn("HRM.NeedAssessment", "ToAmharic", c => c.String());
            AddColumn("HRM.NeedAssessment", "Role", c => c.String());
            AddColumn("HRM.NeedAssessment", "IsHRLevel", c => c.Boolean(nullable: false));
            AddColumn("HRM.NeedAssessment", "BranchId", c => c.Int());
            AddColumn("HRM.NeedAssessmentSummary", "IsNotified", c => c.Boolean(nullable: false));
            AddColumn("HRM.NeedAssessmentSummary", "NotifiedDate", c => c.DateTime());
            AddColumn("HRM.NeedAssessmentSummary", "NotifiedBy", c => c.String());
            AddColumn("EXCEED.StoreRequisition", "NextApprover", c => c.String());
            AddColumn("HRM.TrainingNeedRequests", "IsForAll", c => c.Boolean(nullable: false));
            AddColumn("HRM.TrainingEvaluationCriteria", "TrainingCourseId", c => c.Int());
            AddColumn("HRM.TrainingEvaluationCriteria", "TrainingCourse_LookupId", c => c.Int());
            AddColumn("HRM.TrainingPlan", "BusinessUnitId", c => c.Int());
            AlterColumn("HRM.LeavePlan", "FromAmharic", c => c.String());
            AlterColumn("HRM.LeavePlan", "ToAmharic", c => c.String());
            AlterColumn("HRM.NeedAssessment", "OrgStructureId", c => c.Int());
            AlterColumn("HRM.NeedAssessment", "RequiredTraining", c => c.Int());
            AlterColumn("HRM.TrainingNeedRequests", "Prerequest", c => c.String());
            AlterColumn("HRM.TrainingNeedRequests", "EmployeePerformanceGap", c => c.String());
            AlterColumn("HRM.TrainingNeedRequests", "TrainingNeedForEmployeePerformanceGap", c => c.String());
            CreateIndex("HRM.NeedAssessment", "OrgStructureId");
            CreateIndex("HRM.TrainingEvaluationCriteria", "TrainingCourse_LookupId");
            AddForeignKey("HRM.TrainingEvaluationCriteria", "TrainingCourse_LookupId", "Common.Lookup", "LookupId");
            AddForeignKey("HRM.NeedAssessment", "OrgStructureId", "HRM.MainCompanyStructure", "OrgStructureId");
        }
        
        public override void Down()
        {
            DropForeignKey("HRM.NeedAssessment", "OrgStructureId", "HRM.MainCompanyStructure");
            DropForeignKey("HRM.TrainingEvaluationCriteria", "TrainingCourse_LookupId", "Common.Lookup");
            DropIndex("HRM.TrainingEvaluationCriteria", new[] { "TrainingCourse_LookupId" });
            DropIndex("HRM.NeedAssessment", new[] { "OrgStructureId" });
            AlterColumn("HRM.TrainingNeedRequests", "TrainingNeedForEmployeePerformanceGap", c => c.String(nullable: false));
            AlterColumn("HRM.TrainingNeedRequests", "EmployeePerformanceGap", c => c.String(nullable: false));
            AlterColumn("HRM.TrainingNeedRequests", "Prerequest", c => c.String(nullable: false));
            AlterColumn("HRM.NeedAssessment", "RequiredTraining", c => c.Int(nullable: false));
            AlterColumn("HRM.NeedAssessment", "OrgStructureId", c => c.Int(nullable: false));
            AlterColumn("HRM.LeavePlan", "ToAmharic", c => c.String(nullable: false));
            AlterColumn("HRM.LeavePlan", "FromAmharic", c => c.String(nullable: false));
            DropColumn("HRM.TrainingPlan", "BusinessUnitId");
            DropColumn("HRM.TrainingEvaluationCriteria", "TrainingCourse_LookupId");
            DropColumn("HRM.TrainingEvaluationCriteria", "TrainingCourseId");
            DropColumn("HRM.TrainingNeedRequests", "IsForAll");
            DropColumn("EXCEED.StoreRequisition", "NextApprover");
            DropColumn("HRM.NeedAssessmentSummary", "NotifiedBy");
            DropColumn("HRM.NeedAssessmentSummary", "NotifiedDate");
            DropColumn("HRM.NeedAssessmentSummary", "IsNotified");
            DropColumn("HRM.NeedAssessment", "BranchId");
            DropColumn("HRM.NeedAssessment", "IsHRLevel");
            DropColumn("HRM.NeedAssessment", "Role");
            DropColumn("HRM.NeedAssessment", "ToAmharic");
            DropColumn("HRM.NeedAssessment", "FromAmharic");
            DropColumn("HRM.NeedAssessment", "To");
            DropColumn("HRM.NeedAssessment", "From");
            DropColumn("HRM.NeedAssessment", "BusinessUnitId");
            DropColumn("HRM.NeedAssessment", "TotalDays");
            CreateIndex("HRM.NeedAssessment", "OrgStructureId");
            AddForeignKey("HRM.NeedAssessment", "OrgStructureId", "HRM.MainCompanyStructure", "OrgStructureId", cascadeDelete: true);
        }
    }
}
