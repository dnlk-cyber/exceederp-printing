namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DTLoadingStty : DbMigration
    {
        public override void Up()
        {
            AddColumn("JobCosting.JobMaterialCosts", "MaterialType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("JobCosting.JobMaterialCosts", "MaterialType");
        }
    }
}
