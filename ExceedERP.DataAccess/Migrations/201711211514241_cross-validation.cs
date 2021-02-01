namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class crossvalidation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.GLCrossValidationRuleElement",
                c => new
                    {
                        GLCrossValidationRuleElementId = c.Int(nullable: false, identity: true),
                        GLSegmentCrossValidationId = c.Int(nullable: false),
                        SegmentType = c.String(nullable: false),
                        From = c.String(),
                        To = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GLCrossValidationRuleElementId)
                .ForeignKey("EXCEED.GLSegmentCrossValidation", t => t.GLSegmentCrossValidationId, cascadeDelete: true)
                .Index(t => t.GLSegmentCrossValidationId);
            
            CreateTable(
                "EXCEED.GLSegmentCrossValidation",
                c => new
                    {
                        GLSegmentCrossValidationId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        Enabled = c.Boolean(nullable: false),
                        ErrMsg = c.String(),
                        ErrorSegment = c.String(),
                        ErrorSegmentFrom = c.String(),
                        ErrorSegmentTo = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GLSegmentCrossValidationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.GLCrossValidationRuleElement", "GLSegmentCrossValidationId", "EXCEED.GLSegmentCrossValidation");
            DropIndex("EXCEED.GLCrossValidationRuleElement", new[] { "GLSegmentCrossValidationId" });
            DropTable("EXCEED.GLSegmentCrossValidation");
            DropTable("EXCEED.GLCrossValidationRuleElement");
        }
    }
}
