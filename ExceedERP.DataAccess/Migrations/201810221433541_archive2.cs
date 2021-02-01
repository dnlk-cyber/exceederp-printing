namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class archive2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Archive.InstitutionArchiveNo", "BranchId", c => c.Int(nullable: false));
            AddColumn("Archive.InstitutionArchiveNo", "RoomNo", c => c.String());
            AddColumn("Archive.PersonalArchiveNo", "BranchId", c => c.Int(nullable: false));
            AddColumn("Archive.PersonalArchiveNo", "RoomNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("Archive.PersonalArchiveNo", "RoomNo");
            DropColumn("Archive.PersonalArchiveNo", "BranchId");
            DropColumn("Archive.InstitutionArchiveNo", "RoomNo");
            DropColumn("Archive.InstitutionArchiveNo", "BranchId");
        }
    }
}
