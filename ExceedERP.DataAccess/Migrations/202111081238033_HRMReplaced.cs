namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HRMReplaced : DbMigration
    {
        public override void Up()
        {
            MoveTable(name: "EXCEED.AttendanceNew", newSchema: "HRM");
            DropForeignKey("HRM.Vacancy", "BranchId", "HRM.Branch");
            DropForeignKey("HRM.Vacancy", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("HRM.Vacancy", "LookupId", "Common.Lookup");
            DropForeignKey("HRM.Vacancy", "OrgStructureId", "HRM.MainCompanyStructure");
            DropForeignKey("HRM.RecruitmentPlan", "OrgStructureId", "HRM.MainCompanyStructure");
            DropForeignKey("HRM.Vacancy", "RecruitmentPlanId", "HRM.RecruitmentPlan");
            DropForeignKey("HRM.ApplicantRegistration", "VacancyId", "HRM.Vacancy");
            DropForeignKey("HRM.ApplicantEducation", "ApplicantRegistrationId", "HRM.ApplicantRegistration");
            DropForeignKey("HRM.ApplicantExperience", "ApplicantRegistrationId", "HRM.ApplicantRegistration");
            DropForeignKey("HRM.ApplicantScreeningResult", "ApplicantRegistrationId", "HRM.ApplicantRegistration");
            DropForeignKey("HRM.CourseTracking", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("HRM.CourseTracking", "LookupId", "Common.Lookup");
            DropForeignKey("HRM.CourseTracking", "MainCompanyId", "HRM.MainCompany");
            DropForeignKey("HRM.TrainingPlan", "MainCompanyId", "HRM.MainCompany");
            DropForeignKey("HRM.CourseTracking", "TrainingPlanId", "HRM.TrainingPlan");
            DropForeignKey("HRM.DisciplineMeasuresType", "MeasuresId", "HRM.DisciplineMeasures");
            DropForeignKey("HRM.Emergency", "LookupId", "Common.Lookup");
            DropForeignKey("Health.FixedAssetAccidentRecord", "BranchId", "HRM.Branch");
            DropForeignKey("Health.FixedAssetAccidentRecord", "FleetsID", "FleetManagement.Fleets");
            DropForeignKey("Health.FixedAssetAccidentRecord", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("HRM.Placement", "BranchId", "HRM.Branch");
            DropForeignKey("HRM.Placement", "EmployeeId", "HRM.Employee");
            DropForeignKey("HRM.Placement", "LookupId", "Common.Lookup");
            DropForeignKey("HRM.Placement", "OrgStructureId", "HRM.MainCompanyStructure");
            DropForeignKey("HRM.Placement", "RankId", "HRM.Rank");
            DropForeignKey("HRM.Placement", "StepId", "HRM.Step");
            DropForeignKey("Insurance.VehicleAccidentDetail", "FixedAssetAccidentRecordId", "Health.FixedAssetAccidentRecord");
            DropIndex("HRM.ApplicantEducation", new[] { "ApplicantRegistrationId" });
            DropIndex("HRM.ApplicantRegistration", new[] { "VacancyId" });
            DropIndex("HRM.Vacancy", new[] { "RecruitmentPlanId" });
            DropIndex("HRM.Vacancy", new[] { "LookupId" });
            DropIndex("HRM.Vacancy", new[] { "BranchId" });
            DropIndex("HRM.Vacancy", new[] { "GlFiscalYearId" });
            DropIndex("HRM.Vacancy", new[] { "OrgStructureId" });
            DropIndex("HRM.RecruitmentPlan", new[] { "OrgStructureId" });
            DropIndex("HRM.ApplicantExperience", new[] { "ApplicantRegistrationId" });
            DropIndex("HRM.ApplicantScreeningResult", new[] { "ApplicantRegistrationId" });
            DropIndex("HRM.CourseTracking", new[] { "TrainingPlanId" });
            DropIndex("HRM.CourseTracking", new[] { "GlFiscalYearId" });
            DropIndex("HRM.CourseTracking", new[] { "LookupId" });
            DropIndex("HRM.CourseTracking", new[] { "MainCompanyId" });
            DropIndex("HRM.TrainingPlan", new[] { "MainCompanyId" });
            DropIndex("HRM.DisciplineMeasuresType", new[] { "MeasuresId" });
            DropIndex("HRM.Emergency", new[] { "LookupId" });
            DropIndex("Health.FixedAssetAccidentRecord", new[] { "GlFiscalYearId" });
            DropIndex("Health.FixedAssetAccidentRecord", new[] { "FleetsID" });
            DropIndex("Health.FixedAssetAccidentRecord", new[] { "BranchId" });
            DropIndex("HRM.Placement", new[] { "EmployeeId" });
            DropIndex("HRM.Placement", new[] { "LookupId" });
            DropIndex("HRM.Placement", new[] { "OrgStructureId" });
            DropIndex("HRM.Placement", new[] { "RankId" });
            DropIndex("HRM.Placement", new[] { "StepId" });
            DropIndex("HRM.Placement", new[] { "BranchId" });
            DropIndex("Insurance.VehicleAccidentDetail", new[] { "FixedAssetAccidentRecordId" });
            CreateTable(
                "EXCEED.ApplicantCertefication",
                c => new
                    {
                        ApplicantCerteficationId = c.Int(nullable: false, identity: true),
                        ApplicantDetailId = c.Int(nullable: false),
                        ApplicantRegistrationId = c.Int(nullable: false),
                        CerteficationName = c.String(),
                        CerteficationType = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ApplicantCerteficationId);
            
            CreateTable(
                "EXCEED.ApplicantDetail",
                c => new
                    {
                        ApplicantDetailId = c.Int(nullable: false, identity: true),
                        ApplicantRegistrationId = c.Int(nullable: false),
                        Sex = c.Int(nullable: false),
                        Age = c.Int(nullable: false),
                        ApplicantName = c.String(),
                        Address = c.String(),
                        Training = c.String(),
                        VacancyCode = c.String(),
                        Description = c.String(),
                        Language = c.Int(nullable: false),
                        FilePath = c.String(),
                        FileName = c.String(),
                    })
                .PrimaryKey(t => t.ApplicantDetailId);
            
            CreateTable(
                "EXCEED.ApplicantScreening",
                c => new
                    {
                        ApplicantScreeningId = c.Int(nullable: false, identity: true),
                        ApplicantDetailId = c.Int(nullable: false),
                        ApplicantScreenResultId = c.Int(nullable: false),
                        Sex = c.Int(nullable: false),
                        Age = c.Int(nullable: false),
                        ApplicantName = c.String(),
                        Address = c.String(),
                        IsProfileGenerated = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ApplicantScreeningId);
            
            CreateTable(
                "EXCEED.ApplicantScreenResult",
                c => new
                    {
                        ApplicantScreenResultId = c.Int(nullable: false, identity: true),
                        VacancyId = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ApplicantScreenResultId);
            
            CreateTable(
                "EXCEED.CheckUp",
                c => new
                    {
                        CheckUpId = c.Int(nullable: false, identity: true),
                        CriminalCheck = c.Boolean(nullable: false),
                        HealthCheck = c.Boolean(nullable: false),
                        Offer = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CheckUpId);
            
            CreateTable(
                "HRM.EmployeesInAssessment",
                c => new
                    {
                        EmployeesInAssessmentId = c.Int(nullable: false, identity: true),
                        AssesmentId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        JobtitleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EmployeesInAssessmentId);
            
            CreateTable(
                "EXCEED.ExamTypeSetting",
                c => new
                    {
                        ExamTypeSettingId = c.Int(nullable: false, identity: true),
                        ExamSource = c.Int(nullable: false),
                        ExamType = c.Int(nullable: false),
                        SettingVlaue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ExamTypeSettingId);
            
            CreateTable(
                "EXCEED.ExperienceJobTitle",
                c => new
                    {
                        ExperienceJobTitleId = c.Int(nullable: false, identity: true),
                        JobTitle = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ExperienceJobTitleId);
            
            CreateTable(
                "EXCEED.ExperienceOrganizationTypeSetting",
                c => new
                    {
                        ExperienceOrganizationTypeSettingId = c.Int(nullable: false, identity: true),
                        OrganizationType = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ExperienceOrganizationTypeSettingId);
            
            CreateTable(
                "EXCEED.Freelancer",
                c => new
                    {
                        FreelancerId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        From = c.DateTime(nullable: false),
                        To = c.DateTime(nullable: false),
                        FromAm = c.String(),
                        ToAm = c.String(),
                    })
                .PrimaryKey(t => t.FreelancerId);
            
            CreateTable(
                "EXCEED.InternalApplicantExamResult",
                c => new
                    {
                        InternalApplicantExamResultId = c.Int(nullable: false, identity: true),
                        InternalApplicantScreeningId = c.Int(nullable: false),
                        ExamType = c.Int(nullable: false),
                        SettingValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CurrentResult = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.InternalApplicantExamResultId);
            
            CreateTable(
                "EXCEED.InternalApplicants",
                c => new
                    {
                        InternalApplicantsId = c.Int(nullable: false, identity: true),
                        ApplicationFormId = c.Int(nullable: false),
                        EmployeeId = c.Int(),
                        ApplicantName = c.String(),
                        Gender = c.Int(nullable: false),
                        Age = c.Int(),
                        Address = c.String(),
                        Training = c.String(),
                        VacancyCode = c.String(),
                        Description = c.String(),
                        Language = c.Int(nullable: false),
                        FilePath = c.String(),
                        FileName = c.String(),
                    })
                .PrimaryKey(t => t.InternalApplicantsId);
            
            CreateTable(
                "EXCEED.InternalApplicantScreeningResult",
                c => new
                    {
                        InternalApplicantScreeningResultId = c.Int(nullable: false, identity: true),
                        ApplicationFormId = c.Int(nullable: false),
                        VacancyId = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.InternalApplicantScreeningResultId);
            
            CreateTable(
                "EXCEED.InternalApplicantScreening",
                c => new
                    {
                        InternalApplicantScreeningId = c.Int(nullable: false, identity: true),
                        InternalApplicantScreeningResultId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.InternalApplicantScreeningId);
            
            CreateTable(
                "HRM.LeaveNewAwajStartYear",
                c => new
                    {
                        LeaveNewAwajStartYearId = c.Int(nullable: false, identity: true),
                        GlFisicalYearId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LeaveNewAwajStartYearId);
            
            CreateTable(
                "EXCEED.LeaveUsageTrackingForVoid",
                c => new
                    {
                        LeaveUsageTrackingForVoidId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        GlFisicalYearId = c.Int(nullable: false),
                        Index = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LeaveRequestId = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.LeaveUsageTrackingForVoidId);
            
            CreateTable(
                "HRM.PlacementAfterSystem",
                c => new
                    {
                        PlacementAfterSystemId = c.Int(nullable: false, identity: true),
                        PlacementId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        RankId = c.Int(nullable: false),
                        StepId = c.Int(nullable: false),
                        Salary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        From = c.DateTime(nullable: false),
                        FromAm = c.String(),
                        To = c.DateTime(nullable: false),
                        ToAm = c.String(),
                        AssignmentType = c.Int(nullable: false),
                        ResponsibilityLevel = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        IsHead = c.Boolean(nullable: false),
                        IsCurrent = c.Boolean(nullable: false),
                        EmployementType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PlacementAfterSystemId);
            
            CreateTable(
                "EXCEED.RecruitmentApplicantPassSetting",
                c => new
                    {
                        RecruitmentApplicantPassSettingId = c.Int(nullable: false, identity: true),
                        TotalValue = c.Double(nullable: false),
                        PassScore = c.Double(nullable: false),
                        VacancyId = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.RecruitmentApplicantPassSettingId);
            
            CreateTable(
                "EXCEED.RecruitmetnInterviewerPerseon",
                c => new
                    {
                        RecruitmetnInterviewerPerseonId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        VacancyId = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.RecruitmetnInterviewerPerseonId);
            
            CreateTable(
                "EXCEED.ScreeningEducationField",
                c => new
                    {
                        ScreeningEducationFieldId = c.Int(nullable: false, identity: true),
                        ScreeingSettingId = c.Int(nullable: false),
                        EducationFieldId = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ScreeningEducationFieldId);
            
            CreateTable(
                "EXCEED.ScreeningEducationLevel",
                c => new
                    {
                        ScreeningEducationLevelId = c.Int(nullable: false, identity: true),
                        ScreeingSettingId = c.Int(nullable: false),
                        EducationLevelId = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ScreeningEducationLevelId);
            
            CreateTable(
                "EXCEED.ScreeningExperienceJobtitleForInternal",
                c => new
                    {
                        ScreeningExperienceJobtitleForInternalId = c.Int(nullable: false, identity: true),
                        ScreeingSettingId = c.Int(nullable: false),
                        ExperienceJobTitleId = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ScreeningExperienceJobtitleForInternalId);
            
            CreateTable(
                "EXCEED.ScreeningExperienceJobtitle",
                c => new
                    {
                        ScreeningExperienceJobtitleId = c.Int(nullable: false, identity: true),
                        ScreeingSettingId = c.Int(nullable: false),
                        ExperienceJobTitleId = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ScreeningExperienceJobtitleId);
            
            CreateTable(
                "EXCEED.ScreeningSetting",
                c => new
                    {
                        ScreeningSettingId = c.Int(nullable: false, identity: true),
                        VacancyId = c.Int(nullable: false),
                        Gender = c.Int(nullable: false),
                        OrganizationType = c.Int(nullable: false),
                        ExperienceYear = c.Int(nullable: false),
                        Language = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ScreeningSettingId);
            
            CreateTable(
                "HRM.TrainingEvaluationCriteriaCategory",
                c => new
                    {
                        TrainingEvaluationSettingCategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.TrainingEvaluationSettingCategoryId);
            
            CreateTable(
                "HRM.TrainingEvaluationCriteria",
                c => new
                    {
                        TrainingEvaluationCriteriaId = c.Int(nullable: false, identity: true),
                        TrainingEvaluationSettingCategoryId = c.Int(nullable: false),
                        Criteria = c.String(),
                    })
                .PrimaryKey(t => t.TrainingEvaluationCriteriaId);
            
            CreateTable(
                "HRM.TrainingEvaluation",
                c => new
                    {
                        TrainingEvaluationId = c.Int(nullable: false, identity: true),
                        CriteriaId = c.Int(nullable: false),
                        Score = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        CourseTrackingId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TrainingEvaluationId);
            
            CreateTable(
                "EXCEED.TrainingFeedback",
                c => new
                    {
                        TrainingFeedbackId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        FeedBack = c.String(),
                        CourseTrackingId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TrainingFeedbackId);
            
            AddColumn("HRM.Employee", "PhoneNo", c => c.String());
            AddColumn("HRM.Employee", "City", c => c.String());
            AddColumn("HRM.Employee", "SubCity", c => c.String());
            AddColumn("HRM.Employee", "Wereda", c => c.String());
            AddColumn("HRM.Education", "CourseTrackingId", c => c.Int());
            AddColumn("HRM.Experience", "JobTitleId", c => c.Int(nullable: false));
            AddColumn("HRM.Training", "CourseTrackingId", c => c.Int());
            AddColumn("Common.Lookup", "DescriptionEng", c => c.String());
            AddColumn("HRM.DisciplinaryCase", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("HRM.ApplicantEducation", "ApplicantDetailId", c => c.Int(nullable: false));
            AddColumn("HRM.ApplicantEducation", "EducationLevelSettingId", c => c.Int(nullable: false));
            AddColumn("HRM.ApplicantEducation", "EducationFieldSettingId", c => c.Int(nullable: false));
            AddColumn("HRM.Vacancy", "JobTitle", c => c.Int(nullable: false));
            AddColumn("HRM.Vacancy", "Department", c => c.Int(nullable: false));
            AddColumn("HRM.Vacancy", "NoOfVacant", c => c.Int(nullable: false));
            AddColumn("HRM.Vacancy", "WrittenExamSchedule", c => c.DateTime());
            AddColumn("HRM.Vacancy", "InterviewSchedule", c => c.DateTime());
            AddColumn("HRM.Vacancy", "Location", c => c.Int(nullable: false));
            AddColumn("HRM.Vacancy", "Salary", c => c.String());
            AddColumn("HRM.Vacancy", "ExperienceLevel", c => c.Int(nullable: false));
            AddColumn("HRM.Vacancy", "TotalYearOfExperience", c => c.Int(nullable: false));
            AddColumn("HRM.Vacancy", "JobRequirement", c => c.String());
            AddColumn("HRM.Vacancy", "Responsibility", c => c.String());
            AddColumn("HRM.Vacancy", "EthioJobsCode", c => c.String());
            AddColumn("HRM.Vacancy", "Language", c => c.Int(nullable: false));
            AddColumn("HRM.Vacancy", "Gender", c => c.Int(nullable: false));
            AddColumn("HRM.ApplicantExamResult", "ApplicantScreeningId", c => c.Int(nullable: false));
            AddColumn("HRM.ApplicantExamResult", "ExamType", c => c.Int(nullable: false));
            AddColumn("HRM.ApplicantExperience", "ApplicantDetailId", c => c.Int(nullable: false));
            AddColumn("HRM.ApplicantExperience", "OrganizationType", c => c.Int(nullable: false));
            AddColumn("HRM.ApplicantExperience", "JobTitleId", c => c.Int(nullable: false));
            AddColumn("HRM.ApplicantExperience", "Jobtitle", c => c.Int());
            AddColumn("HRM.ApplicantExperience", "Year", c => c.Int());
            AddColumn("HRM.ApplicantExperience", "FromAm", c => c.String());
            AddColumn("HRM.ApplicantExperience", "ToAm", c => c.String());
            AddColumn("HRM.ApplicantExperience", "TotalExperience", c => c.String());
            AddColumn("HRM.CourseTracking", "TrainingRequestType", c => c.Int(nullable: false));
            AddColumn("HRM.DisciplineMeasuresType", "DetailMeasuresId", c => c.Int(nullable: false));
            AddColumn("HRM.ExperienceRequest", "IsDirect", c => c.Boolean(nullable: false));
            AddColumn("HRM.LeaveReturn", "LeaveRequestId", c => c.Int(nullable: false));
            AddColumn("HRM.NeedAssessment", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("HRM.PlacementHistory", "LookupId", c => c.Int(nullable: false));
            AddColumn("HRM.Placement", "IsApprove", c => c.Boolean(nullable: false));
            AddColumn("HRM.Placement", "ApprovedByEmpId", c => c.Int(nullable: false));
            AddColumn("HRM.Placement", "PreparedByEmpId", c => c.Int(nullable: false));
            AddColumn("HRM.Template", "Document", c => c.String());
            AddColumn("HRM.TrainingApplicant", "TrainingRequestType", c => c.Int(nullable: false));
            AddColumn("HRM.TrainingApplicant", "GlFisicalYearId", c => c.Int(nullable: false));
            AlterColumn("HRM.ApplicantEducation", "EducationInstitution", c => c.String());
            AlterColumn("HRM.ApplicantExperience", "OrganizationName", c => c.String());
            AlterColumn("HRM.ApplicantExperience", "From", c => c.DateTime());
            AlterColumn("HRM.ApplicantExperience", "To", c => c.DateTime());
            AlterColumn("HRM.CourseTracking", "TrainingPlanId", c => c.Int());
            AlterColumn("HRM.ExperienceRequest", "ApplicationDateAm", c => c.String());
            AlterColumn("HRM.ManPowerPlanningRequest", "LookupId", c => c.Int());
            AlterColumn("HRM.PlacementHistory", "FromAmharic", c => c.String());
            AlterColumn("HRM.PlacementHistory", "ToAmharic", c => c.String());
            AlterColumn("HRM.PlacementHistory", "JobTitle", c => c.String());
            AlterColumn("HRM.PlacementHistory", "Rank", c => c.String());
            AlterColumn("HRM.PlacementHistory", "Step", c => c.String());
            DropColumn("HRM.ApplicantEducation", "EduLevel");
            DropColumn("HRM.ApplicantEducation", "EducField");
            DropColumn("HRM.ApplicantEducation", "To");
            DropColumn("HRM.ApplicantEducation", "ToAmahric");
            DropColumn("HRM.ApplicantRegistration", "ApplicantName");
            DropColumn("HRM.ApplicantRegistration", "Age");
            DropColumn("HRM.ApplicantRegistration", "Sex");
            DropColumn("HRM.ApplicantRegistration", "RegistrationDate");
            DropColumn("HRM.ApplicantRegistration", "RegistrationDateAm");
            DropColumn("HRM.ApplicantRegistration", "RegistrationPalce");
            DropColumn("HRM.ApplicantRegistration", "RegisterBy");
            DropColumn("HRM.ApplicantRegistration", "LookupId");
            DropColumn("HRM.ApplicantRegistration", "FirstName");
            DropColumn("HRM.ApplicantRegistration", "MiddleName");
            DropColumn("HRM.ApplicantRegistration", "LastName");
            DropColumn("HRM.Vacancy", "LookupId");
            DropColumn("HRM.Vacancy", "WorkUnitId");
            DropColumn("HRM.Vacancy", "GlFiscalYearId");
            DropColumn("HRM.Vacancy", "OrgStructureId");
            DropColumn("HRM.Vacancy", "IsComplete");
            DropColumn("HRM.ApplicantExamResult", "ApplicantRegistrationId");
            DropColumn("HRM.ApplicantExamResult", "Criteria");
            DropColumn("HRM.ApplicantExamResult", "Total");
            DropColumn("HRM.ApplicantExamResult", "ReferenceType");
            DropColumn("HRM.ApplicantExperience", "EduLevel");
            DropColumn("HRM.ApplicantExperience", "EducField");
            DropColumn("HRM.ApplicantExperience", "Postion");
            DropColumn("HRM.ApplicantExperience", "Experience");
            DropColumn("HRM.ApplicantExperience", "FromAmharic");
            DropColumn("HRM.ApplicantExperience", "ToAmahric");
            DropColumn("HRM.LeaveReturn", "IsForOther");
            DropTable("HRM.ApplicantScreeningResult");
            DropTable("Health.FixedAssetAccidentRecord");
            DropTable("Insurance.VehicleAccidentDetail");
        }
        
        public override void Down()
        {
            CreateTable(
                "Insurance.VehicleAccidentDetail",
                c => new
                    {
                        VehicleAccidentDetailId = c.Int(nullable: false, identity: true),
                        AccidentType = c.Int(nullable: false),
                        IsThirdPartAccident = c.Boolean(nullable: false),
                        AccidentLevel = c.Int(nullable: false),
                        FixedAssetAccidentRecordId = c.Int(nullable: false),
                        Remark = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.VehicleAccidentDetailId);
            
            CreateTable(
                "Health.FixedAssetAccidentRecord",
                c => new
                    {
                        FixedAssetAccidentRecordId = c.Int(nullable: false, identity: true),
                        GlFiscalYearId = c.Int(nullable: false),
                        FleetsID = c.Guid(nullable: false),
                        PlateNumber = c.String(),
                        Amount = c.Double(nullable: false),
                        BranchId = c.Int(nullable: false),
                        Remark = c.String(maxLength: 200),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FixedAssetAccidentRecordId);
            
            CreateTable(
                "HRM.ApplicantScreeningResult",
                c => new
                    {
                        ScreeningResultId = c.Int(nullable: false, identity: true),
                        ApplicantRegistrationId = c.Int(nullable: false),
                        Criteria = c.String(nullable: false),
                        SettingValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CurrentResult = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReferenceType = c.String(),
                        Remark = c.String(),
                        TotalResult = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Rank = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ScreeningResultId);
            
            AddColumn("HRM.LeaveReturn", "IsForOther", c => c.Boolean(nullable: false));
            AddColumn("HRM.ApplicantExperience", "ToAmahric", c => c.String());
            AddColumn("HRM.ApplicantExperience", "FromAmharic", c => c.String());
            AddColumn("HRM.ApplicantExperience", "Experience", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.ApplicantExperience", "Postion", c => c.String(nullable: false));
            AddColumn("HRM.ApplicantExperience", "EducField", c => c.Int(nullable: false));
            AddColumn("HRM.ApplicantExperience", "EduLevel", c => c.Int(nullable: false));
            AddColumn("HRM.ApplicantExamResult", "ReferenceType", c => c.String(nullable: false));
            AddColumn("HRM.ApplicantExamResult", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("HRM.ApplicantExamResult", "Criteria", c => c.String(nullable: false));
            AddColumn("HRM.ApplicantExamResult", "ApplicantRegistrationId", c => c.Int(nullable: false));
            AddColumn("HRM.Vacancy", "IsComplete", c => c.Boolean(nullable: false));
            AddColumn("HRM.Vacancy", "OrgStructureId", c => c.Int(nullable: false));
            AddColumn("HRM.Vacancy", "GlFiscalYearId", c => c.Int(nullable: false));
            AddColumn("HRM.Vacancy", "WorkUnitId", c => c.Int(nullable: false));
            AddColumn("HRM.Vacancy", "LookupId", c => c.Int(nullable: false));
            AddColumn("HRM.ApplicantRegistration", "LastName", c => c.String());
            AddColumn("HRM.ApplicantRegistration", "MiddleName", c => c.String());
            AddColumn("HRM.ApplicantRegistration", "FirstName", c => c.String());
            AddColumn("HRM.ApplicantRegistration", "LookupId", c => c.Int(nullable: false));
            AddColumn("HRM.ApplicantRegistration", "RegisterBy", c => c.String(nullable: false));
            AddColumn("HRM.ApplicantRegistration", "RegistrationPalce", c => c.String(nullable: false));
            AddColumn("HRM.ApplicantRegistration", "RegistrationDateAm", c => c.String());
            AddColumn("HRM.ApplicantRegistration", "RegistrationDate", c => c.DateTime(nullable: false));
            AddColumn("HRM.ApplicantRegistration", "Sex", c => c.Int(nullable: false));
            AddColumn("HRM.ApplicantRegistration", "Age", c => c.Int(nullable: false));
            AddColumn("HRM.ApplicantRegistration", "ApplicantName", c => c.String(nullable: false));
            AddColumn("HRM.ApplicantEducation", "ToAmahric", c => c.String());
            AddColumn("HRM.ApplicantEducation", "To", c => c.DateTime(nullable: false));
            AddColumn("HRM.ApplicantEducation", "EducField", c => c.Int(nullable: false));
            AddColumn("HRM.ApplicantEducation", "EduLevel", c => c.Int(nullable: false));
            AlterColumn("HRM.PlacementHistory", "Step", c => c.String(nullable: false));
            AlterColumn("HRM.PlacementHistory", "Rank", c => c.String(nullable: false));
            AlterColumn("HRM.PlacementHistory", "JobTitle", c => c.String(nullable: false));
            AlterColumn("HRM.PlacementHistory", "ToAmharic", c => c.String(nullable: false));
            AlterColumn("HRM.PlacementHistory", "FromAmharic", c => c.String(nullable: false));
            AlterColumn("HRM.ManPowerPlanningRequest", "LookupId", c => c.Int(nullable: false));
            AlterColumn("HRM.ExperienceRequest", "ApplicationDateAm", c => c.String(nullable: false));
            AlterColumn("HRM.CourseTracking", "TrainingPlanId", c => c.Int(nullable: false));
            AlterColumn("HRM.ApplicantExperience", "To", c => c.DateTime(nullable: false));
            AlterColumn("HRM.ApplicantExperience", "From", c => c.DateTime(nullable: false));
            AlterColumn("HRM.ApplicantExperience", "OrganizationName", c => c.String(nullable: false));
            AlterColumn("HRM.ApplicantEducation", "EducationInstitution", c => c.String(nullable: false));
            DropColumn("HRM.TrainingApplicant", "GlFisicalYearId");
            DropColumn("HRM.TrainingApplicant", "TrainingRequestType");
            DropColumn("HRM.Template", "Document");
            DropColumn("HRM.Placement", "PreparedByEmpId");
            DropColumn("HRM.Placement", "ApprovedByEmpId");
            DropColumn("HRM.Placement", "IsApprove");
            DropColumn("HRM.PlacementHistory", "LookupId");
            DropColumn("HRM.NeedAssessment", "IsSendForApproval");
            DropColumn("HRM.LeaveReturn", "LeaveRequestId");
            DropColumn("HRM.ExperienceRequest", "IsDirect");
            DropColumn("HRM.DisciplineMeasuresType", "DetailMeasuresId");
            DropColumn("HRM.CourseTracking", "TrainingRequestType");
            DropColumn("HRM.ApplicantExperience", "TotalExperience");
            DropColumn("HRM.ApplicantExperience", "ToAm");
            DropColumn("HRM.ApplicantExperience", "FromAm");
            DropColumn("HRM.ApplicantExperience", "Year");
            DropColumn("HRM.ApplicantExperience", "Jobtitle");
            DropColumn("HRM.ApplicantExperience", "JobTitleId");
            DropColumn("HRM.ApplicantExperience", "OrganizationType");
            DropColumn("HRM.ApplicantExperience", "ApplicantDetailId");
            DropColumn("HRM.ApplicantExamResult", "ExamType");
            DropColumn("HRM.ApplicantExamResult", "ApplicantScreeningId");
            DropColumn("HRM.Vacancy", "Gender");
            DropColumn("HRM.Vacancy", "Language");
            DropColumn("HRM.Vacancy", "EthioJobsCode");
            DropColumn("HRM.Vacancy", "Responsibility");
            DropColumn("HRM.Vacancy", "JobRequirement");
            DropColumn("HRM.Vacancy", "TotalYearOfExperience");
            DropColumn("HRM.Vacancy", "ExperienceLevel");
            DropColumn("HRM.Vacancy", "Salary");
            DropColumn("HRM.Vacancy", "Location");
            DropColumn("HRM.Vacancy", "InterviewSchedule");
            DropColumn("HRM.Vacancy", "WrittenExamSchedule");
            DropColumn("HRM.Vacancy", "NoOfVacant");
            DropColumn("HRM.Vacancy", "Department");
            DropColumn("HRM.Vacancy", "JobTitle");
            DropColumn("HRM.ApplicantEducation", "EducationFieldSettingId");
            DropColumn("HRM.ApplicantEducation", "EducationLevelSettingId");
            DropColumn("HRM.ApplicantEducation", "ApplicantDetailId");
            DropColumn("HRM.DisciplinaryCase", "IsActive");
            DropColumn("Common.Lookup", "DescriptionEng");
            DropColumn("HRM.Training", "CourseTrackingId");
            DropColumn("HRM.Experience", "JobTitleId");
            DropColumn("HRM.Education", "CourseTrackingId");
            DropColumn("HRM.Employee", "Wereda");
            DropColumn("HRM.Employee", "SubCity");
            DropColumn("HRM.Employee", "City");
            DropColumn("HRM.Employee", "PhoneNo");
            DropTable("EXCEED.TrainingFeedback");
            DropTable("HRM.TrainingEvaluation");
            DropTable("HRM.TrainingEvaluationCriteria");
            DropTable("HRM.TrainingEvaluationCriteriaCategory");
            DropTable("EXCEED.ScreeningSetting");
            DropTable("EXCEED.ScreeningExperienceJobtitle");
            DropTable("EXCEED.ScreeningExperienceJobtitleForInternal");
            DropTable("EXCEED.ScreeningEducationLevel");
            DropTable("EXCEED.ScreeningEducationField");
            DropTable("EXCEED.RecruitmetnInterviewerPerseon");
            DropTable("EXCEED.RecruitmentApplicantPassSetting");
            DropTable("HRM.PlacementAfterSystem");
            DropTable("EXCEED.LeaveUsageTrackingForVoid");
            DropTable("HRM.LeaveNewAwajStartYear");
            DropTable("EXCEED.InternalApplicantScreening");
            DropTable("EXCEED.InternalApplicantScreeningResult");
            DropTable("EXCEED.InternalApplicants");
            DropTable("EXCEED.InternalApplicantExamResult");
            DropTable("EXCEED.Freelancer");
            DropTable("EXCEED.ExperienceOrganizationTypeSetting");
            DropTable("EXCEED.ExperienceJobTitle");
            DropTable("EXCEED.ExamTypeSetting");
            DropTable("HRM.EmployeesInAssessment");
            DropTable("EXCEED.CheckUp");
            DropTable("EXCEED.ApplicantScreenResult");
            DropTable("EXCEED.ApplicantScreening");
            DropTable("EXCEED.ApplicantDetail");
            DropTable("EXCEED.ApplicantCertefication");
            CreateIndex("Insurance.VehicleAccidentDetail", "FixedAssetAccidentRecordId");
            CreateIndex("HRM.Placement", "BranchId");
            CreateIndex("HRM.Placement", "StepId");
            CreateIndex("HRM.Placement", "RankId");
            CreateIndex("HRM.Placement", "OrgStructureId");
            CreateIndex("HRM.Placement", "LookupId");
            CreateIndex("HRM.Placement", "EmployeeId");
            CreateIndex("Health.FixedAssetAccidentRecord", "BranchId");
            CreateIndex("Health.FixedAssetAccidentRecord", "FleetsID");
            CreateIndex("Health.FixedAssetAccidentRecord", "GlFiscalYearId");
            CreateIndex("HRM.Emergency", "LookupId");
            CreateIndex("HRM.DisciplineMeasuresType", "MeasuresId");
            CreateIndex("HRM.TrainingPlan", "MainCompanyId");
            CreateIndex("HRM.CourseTracking", "MainCompanyId");
            CreateIndex("HRM.CourseTracking", "LookupId");
            CreateIndex("HRM.CourseTracking", "GlFiscalYearId");
            CreateIndex("HRM.CourseTracking", "TrainingPlanId");
            CreateIndex("HRM.ApplicantScreeningResult", "ApplicantRegistrationId");
            CreateIndex("HRM.ApplicantExperience", "ApplicantRegistrationId");
            CreateIndex("HRM.RecruitmentPlan", "OrgStructureId");
            CreateIndex("HRM.Vacancy", "OrgStructureId");
            CreateIndex("HRM.Vacancy", "GlFiscalYearId");
            CreateIndex("HRM.Vacancy", "BranchId");
            CreateIndex("HRM.Vacancy", "LookupId");
            CreateIndex("HRM.Vacancy", "RecruitmentPlanId");
            CreateIndex("HRM.ApplicantRegistration", "VacancyId");
            CreateIndex("HRM.ApplicantEducation", "ApplicantRegistrationId");
            AddForeignKey("Insurance.VehicleAccidentDetail", "FixedAssetAccidentRecordId", "Health.FixedAssetAccidentRecord", "FixedAssetAccidentRecordId", cascadeDelete: true);
            AddForeignKey("HRM.Placement", "StepId", "HRM.Step", "StepId", cascadeDelete: true);
            AddForeignKey("HRM.Placement", "RankId", "HRM.Rank", "RankId", cascadeDelete: true);
            AddForeignKey("HRM.Placement", "OrgStructureId", "HRM.MainCompanyStructure", "OrgStructureId", cascadeDelete: true);
            AddForeignKey("HRM.Placement", "LookupId", "Common.Lookup", "LookupId", cascadeDelete: true);
            AddForeignKey("HRM.Placement", "EmployeeId", "HRM.Employee", "EmployeeId", cascadeDelete: true);
            AddForeignKey("HRM.Placement", "BranchId", "HRM.Branch", "BranchId", cascadeDelete: true);
            AddForeignKey("Health.FixedAssetAccidentRecord", "GlFiscalYearId", "EXCEED.GlFiscalYear", "GlFiscalYearId", cascadeDelete: true);
            AddForeignKey("Health.FixedAssetAccidentRecord", "FleetsID", "FleetManagement.Fleets", "FleetsID", cascadeDelete: true);
            AddForeignKey("Health.FixedAssetAccidentRecord", "BranchId", "HRM.Branch", "BranchId", cascadeDelete: true);
            AddForeignKey("HRM.Emergency", "LookupId", "Common.Lookup", "LookupId", cascadeDelete: true);
            AddForeignKey("HRM.DisciplineMeasuresType", "MeasuresId", "HRM.DisciplineMeasures", "MeasuresId", cascadeDelete: true);
            AddForeignKey("HRM.CourseTracking", "TrainingPlanId", "HRM.TrainingPlan", "TrainingPlanId", cascadeDelete: true);
            AddForeignKey("HRM.TrainingPlan", "MainCompanyId", "HRM.MainCompany", "MainCompanyId", cascadeDelete: true);
            AddForeignKey("HRM.CourseTracking", "MainCompanyId", "HRM.MainCompany", "MainCompanyId", cascadeDelete: true);
            AddForeignKey("HRM.CourseTracking", "LookupId", "Common.Lookup", "LookupId", cascadeDelete: true);
            AddForeignKey("HRM.CourseTracking", "GlFiscalYearId", "EXCEED.GlFiscalYear", "GlFiscalYearId", cascadeDelete: true);
            AddForeignKey("HRM.ApplicantScreeningResult", "ApplicantRegistrationId", "HRM.ApplicantRegistration", "ApplicantRegistrationId", cascadeDelete: true);
            AddForeignKey("HRM.ApplicantExperience", "ApplicantRegistrationId", "HRM.ApplicantRegistration", "ApplicantRegistrationId", cascadeDelete: true);
            AddForeignKey("HRM.ApplicantEducation", "ApplicantRegistrationId", "HRM.ApplicantRegistration", "ApplicantRegistrationId", cascadeDelete: true);
            AddForeignKey("HRM.ApplicantRegistration", "VacancyId", "HRM.Vacancy", "VacancyId", cascadeDelete: true);
            AddForeignKey("HRM.Vacancy", "RecruitmentPlanId", "HRM.RecruitmentPlan", "RecruitmentPlanId", cascadeDelete: true);
            AddForeignKey("HRM.RecruitmentPlan", "OrgStructureId", "HRM.MainCompanyStructure", "OrgStructureId", cascadeDelete: true);
            AddForeignKey("HRM.Vacancy", "OrgStructureId", "HRM.MainCompanyStructure", "OrgStructureId", cascadeDelete: true);
            AddForeignKey("HRM.Vacancy", "LookupId", "Common.Lookup", "LookupId", cascadeDelete: true);
            AddForeignKey("HRM.Vacancy", "GlFiscalYearId", "EXCEED.GlFiscalYear", "GlFiscalYearId", cascadeDelete: true);
            AddForeignKey("HRM.Vacancy", "BranchId", "HRM.Branch", "BranchId", cascadeDelete: true);
            MoveTable(name: "HRM.AttendanceNew", newSchema: "EXCEED");
        }
    }
}
