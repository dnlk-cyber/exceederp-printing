namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class furnituremanufacturing : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Manufacturing.FurnitureBillOfQuantity",
                c => new
                    {
                        FurnitureBillOfQuantityId = c.Int(nullable: false, identity: true),
                        JobTypeId = c.Int(nullable: false),
                        CustomerId = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Size = c.String(),
                        Status = c.String(),
                        Remark = c.String(),
                        IsVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        IsOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        IsOnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        SendForApprovalBy = c.String(),
                        SendForApprovalTime = c.DateTime(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureBillOfQuantityId);
            
            CreateTable(
                "Manufacturing.FurnitureBOQLabor",
                c => new
                    {
                        FurnitureBOQLaborId = c.Int(nullable: false, identity: true),
                        WorkShopType = c.Int(nullable: false),
                        NumberOfEmloyees = c.Int(nullable: false),
                        EstimatedHour = c.Single(nullable: false),
                        FurnitureBillOfQuantityId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureBOQLaborId)
                .ForeignKey("Manufacturing.FurnitureBillOfQuantity", t => t.FurnitureBillOfQuantityId, cascadeDelete: true)
                .Index(t => t.FurnitureBillOfQuantityId);
            
            CreateTable(
                "Manufacturing.FurnitureBOQMaterial",
                c => new
                    {
                        FurnitureBOQMaterialId = c.Int(nullable: false, identity: true),
                        ItemCode = c.Int(nullable: false),
                        UnitOfMeasurement = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        WorkshopType = c.Int(nullable: false),
                        FurnitureBillOfQuantityId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FurnitureBOQMaterialId)
                .ForeignKey("Manufacturing.FurnitureBillOfQuantity", t => t.FurnitureBillOfQuantityId, cascadeDelete: true)
                .Index(t => t.FurnitureBillOfQuantityId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Manufacturing.FurnitureBOQMaterial", "FurnitureBillOfQuantityId", "Manufacturing.FurnitureBillOfQuantity");
            DropForeignKey("Manufacturing.FurnitureBOQLabor", "FurnitureBillOfQuantityId", "Manufacturing.FurnitureBillOfQuantity");
            DropIndex("Manufacturing.FurnitureBOQMaterial", new[] { "FurnitureBillOfQuantityId" });
            DropIndex("Manufacturing.FurnitureBOQLabor", new[] { "FurnitureBillOfQuantityId" });
            DropTable("Manufacturing.FurnitureBOQMaterial");
            DropTable("Manufacturing.FurnitureBOQLabor");
            DropTable("Manufacturing.FurnitureBillOfQuantity");
        }
    }
}
