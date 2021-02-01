namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class apts11 : DbMigration
    {
        public override void Up()
        {
            AddColumn("APTS.DrugRequests", "IsDepartmentApproved", c => c.Boolean(nullable: false));
            AddColumn("APTS.DrugRequests", "IsRejected", c => c.Boolean(nullable: false));
            AddColumn("APTS.DrugRequests", "IsPending", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("APTS.DrugRequests", "IsPending");
            DropColumn("APTS.DrugRequests", "IsRejected");
            DropColumn("APTS.DrugRequests", "IsDepartmentApproved");
        }
    }
}
