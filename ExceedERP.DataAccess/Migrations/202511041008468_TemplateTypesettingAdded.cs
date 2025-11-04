namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TemplateTypesettingAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "HRM.TemplateTypeSettings",
                c => new
                    {
                        TemplateTypeSettingId = c.Int(nullable: false, identity: true),
                        TemplateType = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TemplateTypeSettingId);
            
            AddColumn("HRM.Template", "TemplateTypeSettingId", c => c.Int());
            AddColumn("HRM.Template", "OrgBranchName", c => c.String());
            AddColumn("HRM.Template", "DateCreated", c => c.DateTime());
            AddColumn("HRM.Template", "LastModified", c => c.DateTime());
            AddColumn("HRM.Template", "CreatedBy", c => c.String());
            AddColumn("HRM.Template", "ModifiedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("HRM.Template", "ModifiedBy");
            DropColumn("HRM.Template", "CreatedBy");
            DropColumn("HRM.Template", "LastModified");
            DropColumn("HRM.Template", "DateCreated");
            DropColumn("HRM.Template", "OrgBranchName");
            DropColumn("HRM.Template", "TemplateTypeSettingId");
            DropTable("HRM.TemplateTypeSettings");
        }
    }
}
