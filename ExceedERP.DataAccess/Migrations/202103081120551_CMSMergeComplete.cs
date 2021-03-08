namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CMSMergeComplete : DbMigration
    {
        public override void Up()
        {
            AddColumn("CMS.CmsTrainees", "CertificateDesignationCode", c => c.String());
            AddColumn("CMS.WelderPerformanceQualification", "RangeOfGrooveBaseMetalPlateOrPipe", c => c.String());
            AddColumn("CMS.WelderPerformanceQualification", "RangeOfFilletBaseMetalPlateOrPipe", c => c.String());
            AddColumn("CMS.WelderPerformanceQualification", "Auxiliaries", c => c.String());
            AddColumn("CMS.WelderPerformanceQualification", "RangeOfAuxiliaries", c => c.String());
            DropColumn("CMS.WelderPerformanceQualification", "ConsumableInsert");
            DropColumn("CMS.WelderPerformanceQualification", "PenetrationEnhanceFlux");
        }
        
        public override void Down()
        {
            AddColumn("CMS.WelderPerformanceQualification", "PenetrationEnhanceFlux", c => c.Int(nullable: false));
            AddColumn("CMS.WelderPerformanceQualification", "ConsumableInsert", c => c.Int(nullable: false));
            DropColumn("CMS.WelderPerformanceQualification", "RangeOfAuxiliaries");
            DropColumn("CMS.WelderPerformanceQualification", "Auxiliaries");
            DropColumn("CMS.WelderPerformanceQualification", "RangeOfFilletBaseMetalPlateOrPipe");
            DropColumn("CMS.WelderPerformanceQualification", "RangeOfGrooveBaseMetalPlateOrPipe");
            DropColumn("CMS.CmsTrainees", "CertificateDesignationCode");
        }
    }
}
