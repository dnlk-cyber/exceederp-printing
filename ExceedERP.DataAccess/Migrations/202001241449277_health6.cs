namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class health6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.HealthRankAndValue", "RecoredDate", c => c.DateTime(nullable: false));
            AddColumn("EXCEED.HealthRankAndValue", "RecoredDateAmh", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.HealthRankAndValue", "RecoredDateAmh");
            DropColumn("EXCEED.HealthRankAndValue", "RecoredDate");
        }
    }
}
