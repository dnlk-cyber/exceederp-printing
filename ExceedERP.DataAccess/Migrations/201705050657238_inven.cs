namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inven : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.AveragePriceSetting",
                c => new
                    {
                        AveragePriceSettingId = c.Int(nullable: false, identity: true),
                        Level = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AveragePriceSettingId);
            
            AlterColumn("EXCEED.IssueValidation", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.IssueValidation", "Status", c => c.String());
            DropTable("EXCEED.AveragePriceSetting");
        }
    }
}
