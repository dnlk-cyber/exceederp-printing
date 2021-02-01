namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrmerge5 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "HRM.Branch", name: "LookupId", newName: "Lookup_LookupId");
            RenameIndex(table: "HRM.Branch", name: "IX_LookupId", newName: "IX_Lookup_LookupId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "HRM.Branch", name: "IX_Lookup_LookupId", newName: "IX_LookupId");
            RenameColumn(table: "HRM.Branch", name: "Lookup_LookupId", newName: "LookupId");
        }
    }
}
