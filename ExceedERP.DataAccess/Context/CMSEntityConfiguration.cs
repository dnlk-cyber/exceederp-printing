using ExceedERP.Core.Domain.CMS;
using System.Data.Entity.ModelConfiguration;

namespace ExceedERP.DataAccess.Context
{
    #region Settings
    public class CMSProgramEntityTypeConfiguration : EntityTypeConfiguration<CMSProgram>
    {
        public CMSProgramEntityTypeConfiguration()
        {
            this.ToTable("CMSPrograms", "CMS");
            this.HasKey<int>(p => p.CMSProgramId);
        }
    }

    public class CMSCourseEntityTypeConfiguration : EntityTypeConfiguration<CMSCourse>
    {
        public CMSCourseEntityTypeConfiguration()
        {
            this.ToTable("CMSCourses", "CMS");
            this.HasKey<int>(p => p.CMSCourseId);
        }
    }
    public class CityEntityTypeConfiguration : EntityTypeConfiguration<City>
    {
        public CityEntityTypeConfiguration()
        {
            this.ToTable("Cities", "CMS");
            this.HasKey<int>(p => p.CityId);
        }
    }
    public class RegistrationNumberPrefixEntityTypeConfiguration : EntityTypeConfiguration<RegistrationNumberPrefix>
    {
        public RegistrationNumberPrefixEntityTypeConfiguration()
        {
            this.ToTable("RegistrationNumberPrefixes", "CMS");
            this.HasKey<int>(p => p.RegistrationNumberPrefixId);
        }
    }
    public class ISOTypeEntityTypeConfiguration : EntityTypeConfiguration<ISOType>
    {
        public ISOTypeEntityTypeConfiguration()
        {
            this.ToTable("ISOTypes", "CMS");
            this.HasKey<int>(p => p.ISOTypeId);
        }
    }
    public class ISOChecklistEntityTypeConfiguration : EntityTypeConfiguration<ISOChecklist>
    {
        public ISOChecklistEntityTypeConfiguration()
        {
            this.ToTable("ISOChecklist", "CMS");
            this.HasKey<int>(p => p.ISOChecklistId);
        }
    }
    #endregion

    #region Trainee data
    public class CMSTraineeEntityTypeConfiguration : EntityTypeConfiguration<CmsTrainee>
    {
        public CMSTraineeEntityTypeConfiguration()
        {
            this.ToTable("CmsTrainees", "CMS");
            this.HasKey<int>(p => p.CmsTraineeId);
        }
    }
    public class TraineeIdNumberComponentEntityTypeConfiguration : EntityTypeConfiguration<TraineeIdNumberComponent>
    {
        public TraineeIdNumberComponentEntityTypeConfiguration()
        {
            this.ToTable("TraineeIdNumberComponents", "CMS");
            this.HasKey<int>(p => p.TraineeIdNumberComponentId);
        }
    }

    public class TraineeProgramEntityTypeConfiguration : EntityTypeConfiguration<TraineeProgram>
    {
        public TraineeProgramEntityTypeConfiguration()
        {
            this.ToTable("TraineePrograms", "CMS");
            this.HasKey<int>(p => p.TraineeProgramId);
        }
    }
    public class CMSTraineeCourseEntityTypeConfiguration : EntityTypeConfiguration<CMSTraineeCourse>
    {
        public CMSTraineeCourseEntityTypeConfiguration()
        {
            this.ToTable("CMSTraineeCourses", "CMS");
            this.HasKey<int>(p => p.CMSTraineeCourseId);
        }
    }
    public class CompanyEvaluationEntityTypeConfiguration : EntityTypeConfiguration<CompanyEvaluation>
    {
        public CompanyEvaluationEntityTypeConfiguration()
        {
            this.ToTable("CompanyEvaluations", "CMS");
            this.HasKey<int>(p => p.CompanyEvaluationId);
        }
    }
    public class CmsTraineeIdentificationEntityTypeConfiguration : EntityTypeConfiguration<CmsTraineeIdentification>
    {
        public CmsTraineeIdentificationEntityTypeConfiguration()
        {
            this.ToTable("CmsTraineeIdentifications", "CMS");
            this.HasKey<int>(p => p.CmsTraineeIdentificationId);
        }
    }
    public class WelderPerformanceQualificationEntityTypeConfiguration : EntityTypeConfiguration<WelderPerformanceQualification>
    {
        public WelderPerformanceQualificationEntityTypeConfiguration()
        {
            this.ToTable("WelderPerformanceQualification", "CMS");
            this.HasKey<int>(p => p.WelderPerformanceQualificationId);
        }
    }
    #endregion
}
