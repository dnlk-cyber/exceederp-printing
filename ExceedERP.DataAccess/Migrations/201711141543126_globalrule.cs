namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class globalrule : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.GlobalRuleValue",
                c => new
                    {
                        GlobalRuleValueId = c.Int(nullable: false, identity: true),
                        Module = c.Int(nullable: false),
                        Key = c.Int(nullable: false),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.GlobalRuleValueId);
            
        }
        
        public override void Down()
        {
            DropTable("EXCEED.GlobalRuleValue");
        }
    }
}
