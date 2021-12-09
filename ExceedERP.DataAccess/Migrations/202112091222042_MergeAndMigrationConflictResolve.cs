namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MergeAndMigrationConflictResolve : DbMigration
    {
        public override void Up()
        {
            AddColumn("ProductFollowUp.Job", "JT", c => c.String());
            AddColumn("ProductFollowUp.Job", "DateCreated", c => c.DateTime());
            AddColumn("ProductFollowUp.Job", "LastModified", c => c.DateTime());
            AddColumn("ProductFollowUp.Job", "CreatedBy", c => c.String());
            AddColumn("ProductFollowUp.Job", "ModifiedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("ProductFollowUp.Job", "ModifiedBy");
            DropColumn("ProductFollowUp.Job", "CreatedBy");
            DropColumn("ProductFollowUp.Job", "LastModified");
            DropColumn("ProductFollowUp.Job", "DateCreated");
            DropColumn("ProductFollowUp.Job", "JT");
        }
    }
}
