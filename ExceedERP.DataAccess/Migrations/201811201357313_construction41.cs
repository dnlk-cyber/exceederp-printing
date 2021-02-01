namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction41 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Construction.ConstructionProjectUserAssignment",
                c => new
                    {
                        ConstructionProjectUserAssignmentId = c.Int(nullable: false, identity: true),
                        ProjectInformationId = c.Int(nullable: false),
                        UserName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ConstructionProjectUserAssignmentId)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ProjectInformationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Construction.ConstructionProjectUserAssignment", "ProjectInformationId", "Construction.ProjectInformation");
            DropIndex("Construction.ConstructionProjectUserAssignment", new[] { "ProjectInformationId" });
            DropTable("Construction.ConstructionProjectUserAssignment");
        }
    }
}
