namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction28 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Construction.PEVCostSetting",
                c => new
                    {
                        PEVCostSettingId = c.Int(nullable: false, identity: true),
                        Attribute = c.Int(nullable: false),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 7),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PEVCostSettingId);
            
            AddColumn("Construction.ProjectWorkBreakDown", "ConfigurationId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Construction.ProjectWorkBreakDown", "ConfigurationId");
            DropTable("Construction.PEVCostSetting");
        }
    }
}
