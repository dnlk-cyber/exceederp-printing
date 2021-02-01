namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class glrule : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.GlRuleValue",
                c => new
                    {
                        GlRuleValueId = c.Int(nullable: false, identity: true),
                        NumberOfSegment = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GlRuleValueId);
            
        }
        
        public override void Down()
        {
            DropTable("EXCEED.GlRuleValue");
        }
    }
}
