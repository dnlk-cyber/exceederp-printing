namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class commonsup : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("HRM.JobTitleMappingDetailTemp", "JobTitleMappingId", "HRM.JobTitleMappingTemp");
            DropForeignKey("HRM.JobTitleMappingMianTemp", "JobTitleMappingId", "HRM.JobTitleMappingTemp");
            DropIndex("HRM.JobTitleMappingDetailTemp", new[] { "JobTitleMappingId" });
            DropIndex("HRM.JobTitleMappingMianTemp", new[] { "JobTitleMappingId" });
            DropPrimaryKey("HRM.JobTitleMappingTemp");
            CreateTable(
                "EXCEED.ApSetupCommonSupplier",
                c => new
                    {
                        ApSetupCommonSupplierId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        AccountCode = c.String(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApSetupCommonSupplierId);
            
            AddColumn("HRM.JobTitleMappingDetailTemp", "JobTitleMapping_JobTitleMappingTempId", c => c.Int());
            AddColumn("HRM.JobTitleMappingTemp", "JobTitleMappingTempId", c => c.Int(nullable: false, identity: true));
            AddColumn("HRM.JobTitleMappingMianTemp", "JobTitleMapping_JobTitleMappingTempId", c => c.Int());
            AddPrimaryKey("HRM.JobTitleMappingTemp", "JobTitleMappingTempId");
            CreateIndex("HRM.JobTitleMappingDetailTemp", "JobTitleMapping_JobTitleMappingTempId");
            CreateIndex("HRM.JobTitleMappingMianTemp", "JobTitleMapping_JobTitleMappingTempId");
            AddForeignKey("HRM.JobTitleMappingDetailTemp", "JobTitleMapping_JobTitleMappingTempId", "HRM.JobTitleMappingTemp", "JobTitleMappingTempId");
            AddForeignKey("HRM.JobTitleMappingMianTemp", "JobTitleMapping_JobTitleMappingTempId", "HRM.JobTitleMappingTemp", "JobTitleMappingTempId");
            DropColumn("HRM.JobTitleMappingTemp", "JobTitleMappingId");
        }
        
        public override void Down()
        {
            AddColumn("HRM.JobTitleMappingTemp", "JobTitleMappingId", c => c.Int(nullable: false, identity: true));
            DropForeignKey("HRM.JobTitleMappingMianTemp", "JobTitleMapping_JobTitleMappingTempId", "HRM.JobTitleMappingTemp");
            DropForeignKey("HRM.JobTitleMappingDetailTemp", "JobTitleMapping_JobTitleMappingTempId", "HRM.JobTitleMappingTemp");
            DropIndex("HRM.JobTitleMappingMianTemp", new[] { "JobTitleMapping_JobTitleMappingTempId" });
            DropIndex("HRM.JobTitleMappingDetailTemp", new[] { "JobTitleMapping_JobTitleMappingTempId" });
            DropPrimaryKey("HRM.JobTitleMappingTemp");
            DropColumn("HRM.JobTitleMappingMianTemp", "JobTitleMapping_JobTitleMappingTempId");
            DropColumn("HRM.JobTitleMappingTemp", "JobTitleMappingTempId");
            DropColumn("HRM.JobTitleMappingDetailTemp", "JobTitleMapping_JobTitleMappingTempId");
            DropTable("EXCEED.ApSetupCommonSupplier");
            AddPrimaryKey("HRM.JobTitleMappingTemp", "JobTitleMappingId");
            CreateIndex("HRM.JobTitleMappingMianTemp", "JobTitleMappingId");
            CreateIndex("HRM.JobTitleMappingDetailTemp", "JobTitleMappingId");
            AddForeignKey("HRM.JobTitleMappingMianTemp", "JobTitleMappingId", "HRM.JobTitleMappingTemp", "JobTitleMappingId", cascadeDelete: true);
            AddForeignKey("HRM.JobTitleMappingDetailTemp", "JobTitleMappingId", "HRM.JobTitleMappingTemp", "JobTitleMappingId", cascadeDelete: true);
        }
    }
}
