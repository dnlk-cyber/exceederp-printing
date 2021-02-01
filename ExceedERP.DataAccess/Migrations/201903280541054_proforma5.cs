namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class proforma5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Construction.SubProjectType", "OrderIndex", c => c.Int(nullable: false));
            AlterColumn("EXCEED.ParticipatingSupplier", "CatCode", c => c.String());
            AlterColumn("EXCEED.ParticipatingSupplier", "ItmCode", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.ParticipatingSupplier", "ItmCode", c => c.String(nullable: false));
            AlterColumn("EXCEED.ParticipatingSupplier", "CatCode", c => c.String(nullable: false));
            DropColumn("Construction.SubProjectType", "OrderIndex");
        }
    }
}
