namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class furniturejobprod : DbMigration
    {
        public override void Up()
        {
            MoveTable(name: "EXCEED.FurnitureJobAssignment", newSchema: "Manufacturing");
            MoveTable(name: "EXCEED.FurnitureJobOrderProduction", newSchema: "Manufacturing");
            MoveTable(name: "EXCEED.FurnitureProductionBillOfLabor", newSchema: "Manufacturing");
            MoveTable(name: "EXCEED.FurnitureProductionBillOfMaterial", newSchema: "Manufacturing");
            CreateTable(
                "Manufacturing.FurnitureDailyProductionFollowUp",
                c => new
                    {
                        FurnitureDailyProductionFollowUpId = c.Int(nullable: false, identity: true),
                        WorkShop = c.Int(nullable: false),
                        ManifucturingTaskCategoryId = c.Int(nullable: false),
                        EmployeeId = c.Int(nullable: false),
                        DateExcuted = c.DateTime(),
                        DailyProductionPlan = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DailyProductionActual = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FurnitureJobOrderProductionId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureDailyProductionFollowUpId);
            
            CreateTable(
                "Manufacturing.FurnitureProductionDailyFollowUpDelayReason",
                c => new
                    {
                        FurnitureProductionDailyFollowUpDelayReasonId = c.Int(nullable: false, identity: true),
                        ProductionDelayReason = c.Int(nullable: false),
                        Remark = c.String(),
                        FurnitureDailyProductionFollowUpId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FurnitureProductionDailyFollowUpDelayReasonId)
                .ForeignKey("Manufacturing.FurnitureDailyProductionFollowUp", t => t.FurnitureDailyProductionFollowUpId, cascadeDelete: true)
                .Index(t => t.FurnitureDailyProductionFollowUpId);
            
            CreateTable(
                "Manufacturing.FurnitureUnfinishedMaterialTransferForm",
                c => new
                    {
                        FurnitureUnfinishedMaterialTransferFormId = c.Int(nullable: false, identity: true),
                        JobTypeId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        TransferDate = c.DateTime(nullable: false),
                        TransfererId = c.Int(nullable: false),
                        ReceiverId = c.Int(nullable: false),
                        QualityGrade = c.Int(nullable: false),
                        WorkProgressInPercentage = c.Double(nullable: false),
                        IsReceived = c.Boolean(nullable: false),
                        ReceivedBy = c.String(),
                        ReceivedDate = c.DateTime(),
                        FurnitureJobOrderProductionId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureUnfinishedMaterialTransferFormId);
            
            AddColumn("Manufacturing.FurnitureJobAssignment", "DailyProductionPlan", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Manufacturing.FurnitureJobAssignment", "DateCreated", c => c.DateTime());
            AddColumn("Manufacturing.FurnitureJobAssignment", "LastModified", c => c.DateTime());
            AddColumn("Manufacturing.FurnitureJobAssignment", "CreatedBy", c => c.String());
            AddColumn("Manufacturing.FurnitureJobAssignment", "ModifiedBy", c => c.String());
            AddColumn("Manufacturing.FurnitureJobOrderProduction", "IsSendForFinishing", c => c.Boolean(nullable: false));
            AddColumn("Manufacturing.FurnitureJobOrderProduction", "SendForFinishingBy", c => c.String());
            AddColumn("Manufacturing.FurnitureJobOrderProduction", "SendForFinishingDate", c => c.DateTime());
            AddColumn("Manufacturing.FurnitureJobOrderProduction", "IsClosed", c => c.Boolean(nullable: false));
            AddColumn("Manufacturing.FurnitureJobOrderProduction", "ClosedBy", c => c.String());
            AddColumn("Manufacturing.FurnitureJobOrderProduction", "DateClosed", c => c.DateTime());
            AddColumn("Manufacturing.FurnitureJobOrderProduction", "Status", c => c.String());
            AddColumn("Manufacturing.FurnitureJobOrderProduction", "Remark", c => c.String());
            AddColumn("Manufacturing.FurnitureJobOrderProduction", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("Manufacturing.FurnitureJobOrderProduction", "VoidBy", c => c.String());
            AddColumn("Manufacturing.FurnitureJobOrderProduction", "VoidTime", c => c.DateTime());
            AddColumn("Manufacturing.FurnitureJobOrderProduction", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("Manufacturing.FurnitureJobOrderProduction", "OnlineApprovedBy", c => c.String());
            AddColumn("Manufacturing.FurnitureJobOrderProduction", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("Manufacturing.FurnitureJobOrderProduction", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("Manufacturing.FurnitureJobOrderProduction", "IsOnlineTransferredBy", c => c.String());
            AddColumn("Manufacturing.FurnitureJobOrderProduction", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("Manufacturing.FurnitureJobOrderProduction", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Manufacturing.FurnitureJobOrderProduction", "OrgBranchName", c => c.String());
            AddColumn("Manufacturing.FurnitureJobOrderProduction", "SendForApprovalBy", c => c.String());
            AddColumn("Manufacturing.FurnitureJobOrderProduction", "SendForApprovalTime", c => c.DateTime());
            AddColumn("Manufacturing.FurnitureJobOrderProduction", "DateCreated", c => c.DateTime());
            AddColumn("Manufacturing.FurnitureJobOrderProduction", "LastModified", c => c.DateTime());
            AddColumn("Manufacturing.FurnitureJobOrderProduction", "CreatedBy", c => c.String());
            AddColumn("Manufacturing.FurnitureJobOrderProduction", "ModifiedBy", c => c.String());
            AddColumn("Manufacturing.FurnitureStandardJobType", "JobCode", c => c.String());
            DropColumn("Manufacturing.FurnitureStandardJobType", "JobNo");
        }
        
        public override void Down()
        {
            AddColumn("Manufacturing.FurnitureStandardJobType", "JobNo", c => c.String());
            DropForeignKey("Manufacturing.FurnitureProductionDailyFollowUpDelayReason", "FurnitureDailyProductionFollowUpId", "Manufacturing.FurnitureDailyProductionFollowUp");
            DropIndex("Manufacturing.FurnitureProductionDailyFollowUpDelayReason", new[] { "FurnitureDailyProductionFollowUpId" });
            DropColumn("Manufacturing.FurnitureStandardJobType", "JobCode");
            DropColumn("Manufacturing.FurnitureJobOrderProduction", "ModifiedBy");
            DropColumn("Manufacturing.FurnitureJobOrderProduction", "CreatedBy");
            DropColumn("Manufacturing.FurnitureJobOrderProduction", "LastModified");
            DropColumn("Manufacturing.FurnitureJobOrderProduction", "DateCreated");
            DropColumn("Manufacturing.FurnitureJobOrderProduction", "SendForApprovalTime");
            DropColumn("Manufacturing.FurnitureJobOrderProduction", "SendForApprovalBy");
            DropColumn("Manufacturing.FurnitureJobOrderProduction", "OrgBranchName");
            DropColumn("Manufacturing.FurnitureJobOrderProduction", "IsSendForApproval");
            DropColumn("Manufacturing.FurnitureJobOrderProduction", "OnlineTransferredTime");
            DropColumn("Manufacturing.FurnitureJobOrderProduction", "IsOnlineTransferredBy");
            DropColumn("Manufacturing.FurnitureJobOrderProduction", "IsOnlineTransferred");
            DropColumn("Manufacturing.FurnitureJobOrderProduction", "OnlineApprovedTime");
            DropColumn("Manufacturing.FurnitureJobOrderProduction", "OnlineApprovedBy");
            DropColumn("Manufacturing.FurnitureJobOrderProduction", "IsOnlineApproved");
            DropColumn("Manufacturing.FurnitureJobOrderProduction", "VoidTime");
            DropColumn("Manufacturing.FurnitureJobOrderProduction", "VoidBy");
            DropColumn("Manufacturing.FurnitureJobOrderProduction", "IsVoid");
            DropColumn("Manufacturing.FurnitureJobOrderProduction", "Remark");
            DropColumn("Manufacturing.FurnitureJobOrderProduction", "Status");
            DropColumn("Manufacturing.FurnitureJobOrderProduction", "DateClosed");
            DropColumn("Manufacturing.FurnitureJobOrderProduction", "ClosedBy");
            DropColumn("Manufacturing.FurnitureJobOrderProduction", "IsClosed");
            DropColumn("Manufacturing.FurnitureJobOrderProduction", "SendForFinishingDate");
            DropColumn("Manufacturing.FurnitureJobOrderProduction", "SendForFinishingBy");
            DropColumn("Manufacturing.FurnitureJobOrderProduction", "IsSendForFinishing");
            DropColumn("Manufacturing.FurnitureJobAssignment", "ModifiedBy");
            DropColumn("Manufacturing.FurnitureJobAssignment", "CreatedBy");
            DropColumn("Manufacturing.FurnitureJobAssignment", "LastModified");
            DropColumn("Manufacturing.FurnitureJobAssignment", "DateCreated");
            DropColumn("Manufacturing.FurnitureJobAssignment", "DailyProductionPlan");
            DropTable("Manufacturing.FurnitureUnfinishedMaterialTransferForm");
            DropTable("Manufacturing.FurnitureProductionDailyFollowUpDelayReason");
            DropTable("Manufacturing.FurnitureDailyProductionFollowUp");
            MoveTable(name: "Manufacturing.FurnitureProductionBillOfMaterial", newSchema: "EXCEED");
            MoveTable(name: "Manufacturing.FurnitureProductionBillOfLabor", newSchema: "EXCEED");
            MoveTable(name: "Manufacturing.FurnitureJobOrderProduction", newSchema: "EXCEED");
            MoveTable(name: "Manufacturing.FurnitureJobAssignment", newSchema: "EXCEED");
        }
    }
}
