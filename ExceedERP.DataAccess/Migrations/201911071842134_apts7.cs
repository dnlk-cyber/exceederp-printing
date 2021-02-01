namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class apts7 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "APTS.EmergencyDrugRequestItem",
                c => new
                    {
                        EmergencyDrugRequestItemId = c.Int(nullable: false, identity: true),
                        EmergencyDrugRequestId = c.Int(nullable: false),
                        DrugListsId = c.Int(nullable: false),
                        DrugUnitId = c.Int(nullable: false),
                        BegStock = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReceivedQty = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Loss = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EndStock = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QtyRequested = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QtyApproved = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QtyAllowed = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.EmergencyDrugRequestItemId);
            
            CreateTable(
                "APTS.EmergencyDrugRequest",
                c => new
                    {
                        EmergencyDrugRequestId = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        RequestDate = c.DateTime(nullable: false),
                        RequestDateAm = c.String(),
                        RequestedBy = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        IsSendForApproval = c.Boolean(nullable: false),
                        IsConfirmed = c.Boolean(nullable: false),
                        IsClosed = c.Boolean(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.EmergencyDrugRequestId);
            
            CreateTable(
                "EXCEED.LaboratoryDrugRequest",
                c => new
                    {
                        LaboratoryDrugRequestId = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        RequestDate = c.DateTime(nullable: false),
                        RequestDateAm = c.String(),
                        RequestedBy = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        IsSendForApproval = c.Boolean(nullable: false),
                        IsConfirmed = c.Boolean(nullable: false),
                        IsClosed = c.Boolean(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.LaboratoryDrugRequestId);
            
            AddColumn("HRM.LeaveReturn", "From", c => c.DateTime(nullable: false));
            AddColumn("HRM.LeaveReturn", "FromAm", c => c.String());
            AddColumn("HRM.LeaveReturn", "To", c => c.DateTime(nullable: false));
            AddColumn("HRM.LeaveReturn", "ToAm", c => c.String());
            AddColumn("HRM.LeaveEntitle", "Void", c => c.String());
            AlterColumn("APTS.DrugReceivalModel19", "SendForApprovalTime", c => c.DateTime(nullable: false));
            DropColumn("APTS.DrugReceivalModel19", "IsPostedToSecondLedger");
            DropColumn("APTS.DrugReceivalModel19", "Remark");
            DropColumn("APTS.DrugReceivalModel19", "IsOnlineTransferred");
            DropColumn("APTS.DrugReceivalModel19", "IsOnlineTransferredBy");
            DropColumn("APTS.DrugReceivalModel19", "OnlineTransferredTime");
        }
        
        public override void Down()
        {
            AddColumn("APTS.DrugReceivalModel19", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("APTS.DrugReceivalModel19", "IsOnlineTransferredBy", c => c.String());
            AddColumn("APTS.DrugReceivalModel19", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("APTS.DrugReceivalModel19", "Remark", c => c.String());
            AddColumn("APTS.DrugReceivalModel19", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AlterColumn("APTS.DrugReceivalModel19", "SendForApprovalTime", c => c.DateTime());
            DropColumn("HRM.LeaveEntitle", "Void");
            DropColumn("HRM.LeaveReturn", "ToAm");
            DropColumn("HRM.LeaveReturn", "To");
            DropColumn("HRM.LeaveReturn", "FromAm");
            DropColumn("HRM.LeaveReturn", "From");
            DropTable("EXCEED.LaboratoryDrugRequest");
            DropTable("APTS.EmergencyDrugRequest");
            DropTable("APTS.EmergencyDrugRequestItem");
        }
    }
}
