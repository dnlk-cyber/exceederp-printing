namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class coatag : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.TagCodeSetting",
                c => new
                    {
                        TagCodeSettingId = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Index = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.TagCodeSettingId);
            
            AddColumn("EXCEED.GLSegmentSetup", "IsSubAccount", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.GLSegmentSetup", "IsCostCenter", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.GLSegmentSetup", "IsCostCenter");
            DropColumn("EXCEED.GLSegmentSetup", "IsSubAccount");
            DropTable("EXCEED.TagCodeSetting");
        }
    }
}
