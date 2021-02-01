namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class req1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.StoreRequisition", "EmployeeId", c => c.Int(nullable: false));
            CreateIndex("EXCEED.StoreRequisition", "EmployeeId");
            AddForeignKey("EXCEED.StoreRequisition", "EmployeeId", "HRM.Employee", "EmployeeId", cascadeDelete: true);
            DropColumn("EXCEED.StoreRequisition", "PersonID");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.StoreRequisition", "PersonID", c => c.String());
            DropForeignKey("EXCEED.StoreRequisition", "EmployeeId", "HRM.Employee");
            DropIndex("EXCEED.StoreRequisition", new[] { "EmployeeId" });
            DropColumn("EXCEED.StoreRequisition", "EmployeeId");
        }
    }
}
