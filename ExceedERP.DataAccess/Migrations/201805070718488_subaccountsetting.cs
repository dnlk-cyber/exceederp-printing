namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class subaccountsetting : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.EmployeeSubAccount",
                c => new
                    {
                        EmployeeSubAccountId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        SubAccountCode = c.String(nullable: false),
                        Enable = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EmployeeSubAccountId);
            
            CreateTable(
                "EXCEED.SubAccountRange",
                c => new
                    {
                        SubAccountRangeId = c.Int(nullable: false, identity: true),
                        From = c.String(nullable: false),
                        To = c.String(nullable: false),
                        Type = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SubAccountRangeId);
            
            AlterColumn("EXCEED.ApTravelOrder", "Name", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.ApTravelOrder", "Name", c => c.String(nullable: false));
            DropTable("EXCEED.SubAccountRange");
            DropTable("EXCEED.EmployeeSubAccount");
        }
    }
}
