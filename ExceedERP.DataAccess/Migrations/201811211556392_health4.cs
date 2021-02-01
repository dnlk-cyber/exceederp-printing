namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class health4 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("APTS.DrugReceivalModel19", "DrugListsId", "APTS.DrugLists");
            DropForeignKey("APTS.DrugReceivalModel19", "DrugUnitId", "APTS.DrugUnit");
            DropIndex("APTS.DrugReceivalModel19", new[] { "DrugListsId" });
            DropIndex("APTS.DrugReceivalModel19", new[] { "DrugUnitId" });
            CreateTable(
                "EXCEED.APTSValidation",
                c => new
                    {
                        APTSValidationId = c.Int(nullable: false, identity: true),
                        DrugReceivalModel19Id = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.APTSValidationId)
                .ForeignKey("APTS.DrugReceivalModel19", t => t.DrugReceivalModel19Id, cascadeDelete: true)
                .Index(t => t.DrugReceivalModel19Id);
            
            CreateTable(
                "APTS.DrugReceivalDetail",
                c => new
                    {
                        DrugReceivalDetailId = c.Int(nullable: false, identity: true),
                        DrugReceivalModel19Id = c.Int(nullable: false),
                        DrugListsId = c.Int(nullable: false),
                        DrugUnitId = c.Int(nullable: false),
                        QtyReceived = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BatchNo = c.String(),
                        ExpiryDate = c.DateTime(nullable: false),
                        ExpiryDateAmh = c.String(),
                        DateCreated = c.DateTime(),
                        CreatedBy = c.String(),
                        IsCurrent = c.Boolean(nullable: false),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.DrugReceivalDetailId);
            
            AddColumn("APTS.DrugReceivalModel19", "IsPostedToSecondLedger", c => c.Boolean(nullable: false));
            AddColumn("APTS.DrugReceivalModel19", "Status", c => c.String());
            AddColumn("APTS.DrugReceivalModel19", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("APTS.DrugReceivalModel19", "VoidBy", c => c.String());
            AddColumn("APTS.DrugReceivalModel19", "VoidTime", c => c.DateTime());
            AddColumn("APTS.DrugReceivalModel19", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("APTS.DrugReceivalModel19", "OnlineApprovedBy", c => c.String());
            AddColumn("APTS.DrugReceivalModel19", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("APTS.DrugReceivalModel19", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("APTS.DrugReceivalModel19", "IsOnlineTransferredBy", c => c.String());
            AddColumn("APTS.DrugReceivalModel19", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("APTS.DrugReceivalModel19", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("APTS.DrugReceivalModel19", "OrgBranchName", c => c.String());
            AddColumn("APTS.DrugReceivalModel19", "SendForApprovalBy", c => c.String());
            AddColumn("APTS.DrugReceivalModel19", "SendForApprovalTime", c => c.DateTime());
            AddColumn("APTS.DrugReceivalModel19", "LastModified", c => c.DateTime());
            AddColumn("APTS.DrugReceivalModel19", "ModifiedBy", c => c.String());
            AddColumn("APTS.PatientRegisterDetail", "IsUpdated", c => c.Boolean(nullable: false));
            AddColumn("APTS.PatientRegister", "IsDone", c => c.Boolean(nullable: false));
            DropColumn("APTS.DrugReceivalModel19", "DrugListsId");
            DropColumn("APTS.DrugReceivalModel19", "DrugUnitId");
            DropColumn("APTS.DrugReceivalModel19", "QtyReceived");
            DropColumn("APTS.DrugReceivalModel19", "UnitCost");
            DropColumn("APTS.DrugReceivalModel19", "TotalCost");
            DropColumn("APTS.DrugReceivalModel19", "BatchNo");
            DropColumn("APTS.DrugReceivalModel19", "ExpiryDate");
            DropColumn("APTS.DrugReceivalModel19", "ExpiryDateAmh");
            DropColumn("APTS.DrugReceivalModel19", "Balance");
        }
        
        public override void Down()
        {
            AddColumn("APTS.DrugReceivalModel19", "Balance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("APTS.DrugReceivalModel19", "ExpiryDateAmh", c => c.String());
            AddColumn("APTS.DrugReceivalModel19", "ExpiryDate", c => c.DateTime(nullable: false));
            AddColumn("APTS.DrugReceivalModel19", "BatchNo", c => c.String());
            AddColumn("APTS.DrugReceivalModel19", "TotalCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("APTS.DrugReceivalModel19", "UnitCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("APTS.DrugReceivalModel19", "QtyReceived", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("APTS.DrugReceivalModel19", "DrugUnitId", c => c.Int(nullable: false));
            AddColumn("APTS.DrugReceivalModel19", "DrugListsId", c => c.Int(nullable: false));
            DropForeignKey("EXCEED.APTSValidation", "DrugReceivalModel19Id", "APTS.DrugReceivalModel19");
            DropIndex("EXCEED.APTSValidation", new[] { "DrugReceivalModel19Id" });
            DropColumn("APTS.PatientRegister", "IsDone");
            DropColumn("APTS.PatientRegisterDetail", "IsUpdated");
            DropColumn("APTS.DrugReceivalModel19", "ModifiedBy");
            DropColumn("APTS.DrugReceivalModel19", "LastModified");
            DropColumn("APTS.DrugReceivalModel19", "SendForApprovalTime");
            DropColumn("APTS.DrugReceivalModel19", "SendForApprovalBy");
            DropColumn("APTS.DrugReceivalModel19", "OrgBranchName");
            DropColumn("APTS.DrugReceivalModel19", "IsSendForApproval");
            DropColumn("APTS.DrugReceivalModel19", "OnlineTransferredTime");
            DropColumn("APTS.DrugReceivalModel19", "IsOnlineTransferredBy");
            DropColumn("APTS.DrugReceivalModel19", "IsOnlineTransferred");
            DropColumn("APTS.DrugReceivalModel19", "OnlineApprovedTime");
            DropColumn("APTS.DrugReceivalModel19", "OnlineApprovedBy");
            DropColumn("APTS.DrugReceivalModel19", "IsOnlineApproved");
            DropColumn("APTS.DrugReceivalModel19", "VoidTime");
            DropColumn("APTS.DrugReceivalModel19", "VoidBy");
            DropColumn("APTS.DrugReceivalModel19", "IsVoid");
            DropColumn("APTS.DrugReceivalModel19", "Status");
            DropColumn("APTS.DrugReceivalModel19", "IsPostedToSecondLedger");
            DropTable("APTS.DrugReceivalDetail");
            DropTable("EXCEED.APTSValidation");
            CreateIndex("APTS.DrugReceivalModel19", "DrugUnitId");
            CreateIndex("APTS.DrugReceivalModel19", "DrugListsId");
            AddForeignKey("APTS.DrugReceivalModel19", "DrugUnitId", "APTS.DrugUnit", "DrugUnitId", cascadeDelete: true);
            AddForeignKey("APTS.DrugReceivalModel19", "DrugListsId", "APTS.DrugLists", "DrugListsId", cascadeDelete: true);
        }
    }
}
