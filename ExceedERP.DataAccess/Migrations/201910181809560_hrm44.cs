namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm44 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "HRM.LeaveReturn",
                c => new
                    {
                        LeaveReturnId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GlFiscalYearId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LeaveReturnId);
            
            AddColumn("HRM.Complain", "CC", c => c.Int(nullable: false));
            AddColumn("HRM.Complain", "SendTo", c => c.Int(nullable: false));
            AddColumn("HRM.LeaveRequest", "LeaveActionTypes", c => c.Int(nullable: false));
            AddColumn("HRM.Leave", "LeaveActionTypes", c => c.Int(nullable: false));
            AddColumn("HRM.LeaveEntitle", "ExpiredBy", c => c.String());
            AddColumn("HRM.LeaveEntitle", "ExpiredDate", c => c.DateTime());
            AddColumn("HRM.LeaveEntitle", "ReversedBy", c => c.String());
            AddColumn("HRM.LeaveEntitle", "ReversedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("HRM.LeaveEntitle", "ReversedDate");
            DropColumn("HRM.LeaveEntitle", "ReversedBy");
            DropColumn("HRM.LeaveEntitle", "ExpiredDate");
            DropColumn("HRM.LeaveEntitle", "ExpiredBy");
            DropColumn("HRM.Leave", "LeaveActionTypes");
            DropColumn("HRM.LeaveRequest", "LeaveActionTypes");
            DropColumn("HRM.Complain", "SendTo");
            DropColumn("HRM.Complain", "CC");
            DropTable("HRM.LeaveReturn");
        }
    }
}
