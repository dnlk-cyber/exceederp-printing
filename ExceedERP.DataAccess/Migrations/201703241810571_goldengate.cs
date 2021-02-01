namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class goldengate : DbMigration
    {
        public override void Up()
        {
            //AddColumn("EXCEED.DepretiationRule", "AccomolatedAccount", c => c.String());
            //DropColumn("EXCEED.DepreciationRule", "AccomodateAccount");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.DepreciationRule", "AccomodateAccount", c => c.String());
            DropColumn("EXCEED.DepreciationRule", "AccomolatedAccount");
        }
    }
}
