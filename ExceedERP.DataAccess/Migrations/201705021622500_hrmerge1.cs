namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrmerge1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.DisciplinaryCase", "OrgBranchName", c => c.String());
            AddColumn("HRM.DisciplinaryCase", "DateCreated", c => c.DateTime());
            AddColumn("HRM.DisciplinaryCase", "LastModified", c => c.DateTime());
            AddColumn("HRM.DisciplinaryCase", "CreatedBy", c => c.String());
            AddColumn("HRM.DisciplinaryCase", "ModifiedBy", c => c.String());
            AddColumn("HRM.CourseTracking", "OrgBranchName", c => c.String());
            AddColumn("HRM.CourseTracking", "DateCreated", c => c.DateTime());
            AddColumn("HRM.CourseTracking", "LastModified", c => c.DateTime());
            AddColumn("HRM.CourseTracking", "CreatedBy", c => c.String());
            AddColumn("HRM.CourseTracking", "ModifiedBy", c => c.String());
            AddColumn("Performance.EmployeeMeasurementDefinition", "OrgBranchName", c => c.String());
            AddColumn("Performance.EmployeeMeasurementDefinition", "DateCreated", c => c.DateTime());
            AddColumn("Performance.EmployeeMeasurementDefinition", "LastModified", c => c.DateTime());
            AddColumn("Performance.EmployeeMeasurementDefinition", "CreatedBy", c => c.String());
            AddColumn("Performance.EmployeeMeasurementDefinition", "ModifiedBy", c => c.String());
            AddColumn("Payroll.FixedContribution", "ApplyToAll", c => c.Boolean(nullable: false));
            AddColumn("HRM.Incomming", "OrgBranchName", c => c.String());
            AddColumn("HRM.Incomming", "DateCreated", c => c.DateTime());
            AddColumn("HRM.Incomming", "LastModified", c => c.DateTime());
            AddColumn("HRM.Incomming", "CreatedBy", c => c.String());
            AddColumn("HRM.Incomming", "ModifiedBy", c => c.String());
            AddColumn("HRM.NeedAssessment", "OrgBranchName", c => c.String());
            AddColumn("HRM.NeedAssessment", "DateCreated", c => c.DateTime());
            AddColumn("HRM.NeedAssessment", "LastModified", c => c.DateTime());
            AddColumn("HRM.NeedAssessment", "CreatedBy", c => c.String());
            AddColumn("HRM.NeedAssessment", "ModifiedBy", c => c.String());
            AddColumn("HRM.Outgoing", "OrgBranchName", c => c.String());
            AddColumn("HRM.Outgoing", "DateCreated", c => c.DateTime());
            AddColumn("HRM.Outgoing", "LastModified", c => c.DateTime());
            AddColumn("HRM.Outgoing", "CreatedBy", c => c.String());
            AddColumn("HRM.Outgoing", "ModifiedBy", c => c.String());
            AddColumn("Payroll.RegularContribution", "ApplyToAll", c => c.Boolean(nullable: false));
            AddColumn("Payroll.RegularContribution", "OrgBranchName", c => c.String());
            AddColumn("Payroll.RegularContribution", "DateCreated", c => c.DateTime());
            AddColumn("Payroll.RegularContribution", "LastModified", c => c.DateTime());
            AddColumn("Payroll.RegularContribution", "CreatedBy", c => c.String());
            AddColumn("Payroll.RegularContribution", "ModifiedBy", c => c.String());
            AddColumn("HRM.Termination", "OrgBranchName", c => c.String());
            AddColumn("HRM.Termination", "DateCreated", c => c.DateTime());
            AddColumn("HRM.Termination", "LastModified", c => c.DateTime());
            AddColumn("HRM.Termination", "CreatedBy", c => c.String());
            AddColumn("HRM.Termination", "ModifiedBy", c => c.String());
            AddColumn("HRM.TrainingPlan", "OrgBranchName", c => c.String());
            AddColumn("HRM.TrainingPlan", "DateCreated", c => c.DateTime());
            AddColumn("HRM.TrainingPlan", "LastModified", c => c.DateTime());
            AddColumn("HRM.TrainingPlan", "CreatedBy", c => c.String());
            AddColumn("HRM.TrainingPlan", "ModifiedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("HRM.TrainingPlan", "ModifiedBy");
            DropColumn("HRM.TrainingPlan", "CreatedBy");
            DropColumn("HRM.TrainingPlan", "LastModified");
            DropColumn("HRM.TrainingPlan", "DateCreated");
            DropColumn("HRM.TrainingPlan", "OrgBranchName");
            DropColumn("HRM.Termination", "ModifiedBy");
            DropColumn("HRM.Termination", "CreatedBy");
            DropColumn("HRM.Termination", "LastModified");
            DropColumn("HRM.Termination", "DateCreated");
            DropColumn("HRM.Termination", "OrgBranchName");
            DropColumn("Payroll.RegularContribution", "ModifiedBy");
            DropColumn("Payroll.RegularContribution", "CreatedBy");
            DropColumn("Payroll.RegularContribution", "LastModified");
            DropColumn("Payroll.RegularContribution", "DateCreated");
            DropColumn("Payroll.RegularContribution", "OrgBranchName");
            DropColumn("Payroll.RegularContribution", "ApplyToAll");
            DropColumn("HRM.Outgoing", "ModifiedBy");
            DropColumn("HRM.Outgoing", "CreatedBy");
            DropColumn("HRM.Outgoing", "LastModified");
            DropColumn("HRM.Outgoing", "DateCreated");
            DropColumn("HRM.Outgoing", "OrgBranchName");
            DropColumn("HRM.NeedAssessment", "ModifiedBy");
            DropColumn("HRM.NeedAssessment", "CreatedBy");
            DropColumn("HRM.NeedAssessment", "LastModified");
            DropColumn("HRM.NeedAssessment", "DateCreated");
            DropColumn("HRM.NeedAssessment", "OrgBranchName");
            DropColumn("HRM.Incomming", "ModifiedBy");
            DropColumn("HRM.Incomming", "CreatedBy");
            DropColumn("HRM.Incomming", "LastModified");
            DropColumn("HRM.Incomming", "DateCreated");
            DropColumn("HRM.Incomming", "OrgBranchName");
            DropColumn("Payroll.FixedContribution", "ApplyToAll");
            DropColumn("Performance.EmployeeMeasurementDefinition", "ModifiedBy");
            DropColumn("Performance.EmployeeMeasurementDefinition", "CreatedBy");
            DropColumn("Performance.EmployeeMeasurementDefinition", "LastModified");
            DropColumn("Performance.EmployeeMeasurementDefinition", "DateCreated");
            DropColumn("Performance.EmployeeMeasurementDefinition", "OrgBranchName");
            DropColumn("HRM.CourseTracking", "ModifiedBy");
            DropColumn("HRM.CourseTracking", "CreatedBy");
            DropColumn("HRM.CourseTracking", "LastModified");
            DropColumn("HRM.CourseTracking", "DateCreated");
            DropColumn("HRM.CourseTracking", "OrgBranchName");
            DropColumn("HRM.DisciplinaryCase", "ModifiedBy");
            DropColumn("HRM.DisciplinaryCase", "CreatedBy");
            DropColumn("HRM.DisciplinaryCase", "LastModified");
            DropColumn("HRM.DisciplinaryCase", "DateCreated");
            DropColumn("HRM.DisciplinaryCase", "OrgBranchName");
        }
    }
}
