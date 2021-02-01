namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction39 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Construction.ResourceUtilizationWorkItem", "IsSubCategory", c => c.Boolean(nullable: false));
            AddColumn("Construction.ResourceUtilizationWorkItem", "ParentId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Construction.ResourceUtilizationWorkItem", "ParentId");
            DropColumn("Construction.ResourceUtilizationWorkItem", "IsSubCategory");
        }
    }
}
