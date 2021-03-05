namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CMSModification_TraineeCourseAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "CMS.CMSTraineeCourses",
                c => new
                    {
                        CMSTraineeCourseId = c.Int(nullable: false, identity: true),
                        CMSCourseId = c.Int(nullable: false),
                        CmsTraineeId = c.Int(nullable: false),
                        TheoreticalExamResult = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PracticalExamResult = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TrainingStatus = c.Int(nullable: false),
                        PlaceOfTrainingOrExamination = c.String(),
                        StartOfTraining = c.DateTime(nullable: false),
                        EndOfTraining = c.DateTime(nullable: false),
                        TargetAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CMSTraineeCourseId)
                .ForeignKey("CMS.CMSCourses", t => t.CMSCourseId, cascadeDelete: true)
                .ForeignKey("CMS.CmsTrainees", t => t.CmsTraineeId, cascadeDelete: true)
                .Index(t => t.CMSCourseId)
                .Index(t => t.CmsTraineeId);
            
            AddColumn("CMS.CMSCourses", "CMSProgramId", c => c.Int(nullable: false));
            AddColumn("CMS.CmsTraineeIdentifications", "DateOfIssue", c => c.DateTime());
            AddColumn("CMS.CmsTraineeIdentifications", "DateOfExpiry", c => c.DateTime());
            AddColumn("CMS.CmsTrainees", "PlaceOfBirth", c => c.String());
            AddColumn("CMS.CmsTrainees", "ZipCode", c => c.String());
            AddColumn("CMS.CmsTrainees", "Requirement", c => c.String());
            AlterColumn("CMS.CmsTrainees", "DateOfBirth", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropForeignKey("CMS.CMSTraineeCourses", "CmsTraineeId", "CMS.CmsTrainees");
            DropForeignKey("CMS.CMSTraineeCourses", "CMSCourseId", "CMS.CMSCourses");
            DropIndex("CMS.CMSTraineeCourses", new[] { "CmsTraineeId" });
            DropIndex("CMS.CMSTraineeCourses", new[] { "CMSCourseId" });
            AlterColumn("CMS.CmsTrainees", "DateOfBirth", c => c.DateTime(nullable: false));
            DropColumn("CMS.CmsTrainees", "Requirement");
            DropColumn("CMS.CmsTrainees", "ZipCode");
            DropColumn("CMS.CmsTrainees", "PlaceOfBirth");
            DropColumn("CMS.CmsTraineeIdentifications", "DateOfExpiry");
            DropColumn("CMS.CmsTraineeIdentifications", "DateOfIssue");
            DropColumn("CMS.CMSCourses", "CMSProgramId");
            DropTable("CMS.CMSTraineeCourses");
        }
    }
}
