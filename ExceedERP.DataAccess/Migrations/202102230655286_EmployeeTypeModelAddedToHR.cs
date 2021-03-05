namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmployeeTypeModelAddedToHR : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "HRM.EmployeeTypes",
                c => new
                    {
                        EmployeeTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EmployeeTypeId);
            
            AddColumn("HRM.Employee", "EmployeeTypeId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("HRM.Employee", "EmployeeTypeId");
            DropTable("HRM.EmployeeTypes");
        }
    }
}
