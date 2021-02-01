using System.Data.Entity.ModelConfiguration;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Inventory.ArchiveModel;
using ExceedERP.Core.Domain.Procurement.PR;

namespace ExceedERP.DataAccess.Context
{
    #region inventory_archive
    class StoreRequisitionArchiveEntityConfiguration : EntityTypeConfiguration<StoreRequisitionArchive>
    {
        public StoreRequisitionArchiveEntityConfiguration()
        {
            ToTable("StoreRequisitionArchive", "Inventory_Archive");
            HasKey(vd => vd.StoreRequisitionArchiveId);
        }
    }
    class StoreRequisitionItemArchiveEntityConfiguration : EntityTypeConfiguration<StoreRequisitionItemArchive>
    {
        public StoreRequisitionItemArchiveEntityConfiguration()
        {
            ToTable("StoreRequisitionItemArchive", "Inventory_Archive");
            HasKey(vd => vd.StoreRequisitionItemArchiveId);
        }
    }
    class StoreRequisitionValidationArchiveEntityConfiguration : EntityTypeConfiguration<StoreRequisitionValidationArchive>
    {
        public StoreRequisitionValidationArchiveEntityConfiguration()
        {
            ToTable("StoreRequisitionValidationArchive", "Inventory_Archive");
            HasKey(vd => vd.StoreRequisitionValidationArchiveId);
        }
    }
    class StoreRequisitionPropertyValidationArchiveEntityConfiguration : EntityTypeConfiguration<StoreRequisitionPropertyValidationArchive>
    {
        public StoreRequisitionPropertyValidationArchiveEntityConfiguration()
        {
            ToTable("StoreRequisitionPropertyValidationArchive", "Inventory_Archive");
            HasKey(vd => vd.StoreRequisitionPropertyValidationArchiveId);
        }
    }

    class BinCardLineArchiveEntityConfiguration : EntityTypeConfiguration<BinCardLineArchive>
    {
        public BinCardLineArchiveEntityConfiguration()
        {
            ToTable("BinCardLineArchive", "Inventory_Archive");
            HasKey(vd => vd.BinCardLineArchiveId);
           Property(e => e.IssuedQTY).HasPrecision(18, 4);
           Property(e => e.IssuedUPrice).HasPrecision(18, 4);

           Property(e => e.IssuedTPrice).HasPrecision(18, 4);

           Property(e => e.ReceivedQTY).HasPrecision(18, 4);

           Property(e => e.ReceivedUPrice).HasPrecision(18, 4);

           Property(e => e.ReceivedTPrice).HasPrecision(18, 4);

           Property(e => e.BalanceUPrice).HasPrecision(18, 4);

           Property(e => e.BalanceTPrice).HasPrecision(18, 4);

           Property(e => e.BalanceQTY).HasPrecision(18, 4);
        }
    }
    #endregion
    class GoodReceiveEntityConfiguration : EntityTypeConfiguration<GoodReceive>
        {
        public GoodReceiveEntityConfiguration()
            {
                ToTable("GoodRecive", "Inventory");
                HasKey(vd => vd.GoodReceiveId);
            }
         }
    class GoodReceiveItemEntityConfiguration : EntityTypeConfiguration<GoodReceiveItem>
        {
        public GoodReceiveItemEntityConfiguration()
            {
                ToTable("GoodReceiveItem", "Inventory");
                HasKey(vd => vd.GoodReceiveItemID);
            }
         } 
    class GoodReceiveDistributionEntityConfiguration : EntityTypeConfiguration<GoodReceiveDistribution>
        {
        public GoodReceiveDistributionEntityConfiguration()
            {
                ToTable("GoodReceiveDistribution", "Inventory");
                HasKey(vd => vd.GoodReceiveDistributionID);
            }
         } 
    class GoodReceiveValidationEntityConfiguration : EntityTypeConfiguration<GoodReceiveValidation>
        {
        public GoodReceiveValidationEntityConfiguration()
            {
                ToTable("GoodReceiveValidation", "Inventory");
                HasKey(vd => vd.GoodReceiveValidationID);
            }
         } 
    class GoodReceiveDonationEntityConfiguration : EntityTypeConfiguration<GoodReceiveDonation>
        {
        public GoodReceiveDonationEntityConfiguration()
            {
                ToTable("GoodReceiveDonation", "Inventory");
                HasKey(vd => vd.GoodReceiveDonationId);
            }
         }
    class GoodReceiveDonationItemEntityConfiguration : EntityTypeConfiguration<GoodReceiveDonationItem>
        {
        public GoodReceiveDonationItemEntityConfiguration()
            {
                ToTable("GoodReceiveDonationItem", "Inventory");
                HasKey(vd => vd.GoodReceiveDonationItemID);
            }
         }
    class GoodReciveDonationDistributionEntityConfiguration : EntityTypeConfiguration<GoodReciveDonationDistribution>
        {
        public GoodReciveDonationDistributionEntityConfiguration()
            {
                ToTable("GoodReciveDonationDistribution", "Inventory");
                HasKey(vd => vd.GoodReciveDonationDistributionID);
            }
         }
    class GoodReciveDonationValidationEntityConfiguration : EntityTypeConfiguration<GoodReciveDonationValidation>
        {
        public GoodReciveDonationValidationEntityConfiguration()
            {
                ToTable("GoodReciveDonationValidation", "Inventory");
                HasKey(vd => vd.GoodReciveDonationValidationID);
            }
         }
    class GoodSellingAndLendingEntityConfiguration : EntityTypeConfiguration<GoodSellingAndLending>
        {
        public GoodSellingAndLendingEntityConfiguration()
            {
                ToTable("GoodSellingAndLending", "Inventory");
                HasKey(vd => vd.GoodSellingAndLendingID);
            }
         }
    class GoodSellingAndLendingItemEntityConfiguration : EntityTypeConfiguration<GoodSellingAndLendingItem>
        {
        public GoodSellingAndLendingItemEntityConfiguration()
            {
                ToTable("GoodSellingAndLendingItem", "Inventory");
                HasKey(vd => vd.GoodSellingAndLendingItemID);
            }
         } 
    class GoodSellingAndLendingDistributionEntityConfiguration : EntityTypeConfiguration<GoodSellingAndLendingDistribution>
        {
        public GoodSellingAndLendingDistributionEntityConfiguration()
            {
                ToTable("GoodSellingAndLendingDistribution", "Inventory");
                HasKey(vd => vd.GoodSellingAndLendingDistributionID);
            }
         } 
    class GoodSellingAndLendingValidationEntityConfiguration : EntityTypeConfiguration<GoodSellingAndLendingValidation>
        {
        public GoodSellingAndLendingValidationEntityConfiguration()
            {
                ToTable("GoodSellingAndLendingValidation", "Inventory");
                HasKey(vd => vd.GoodSellingAndLendingValidationID);
            }
         }
    class InventoryItemEntityConfiguration : EntityTypeConfiguration<InventoryItem>
        {
        public InventoryItemEntityConfiguration()
            {
                ToTable("InventoryItem", "Inventory");
                HasKey(vd => vd.ItemCode);
            }
         }
    class InventoryStoreEntityConfiguration : EntityTypeConfiguration<InventoryStore>
        {
        public InventoryStoreEntityConfiguration()
            {
                ToTable("InventoryStore", "Inventory");
                HasKey(vd => vd.StoreCode);
            }
         }
    class IssueEntityConfiguration : EntityTypeConfiguration<Issue>
    {
        public IssueEntityConfiguration()
        {
            ToTable("Issue", "Inventory");
            HasKey(vd => vd.IssueID);
        }
    }
    class IssueItemEntityConfiguration : EntityTypeConfiguration<IssueItem>
    {
        public IssueItemEntityConfiguration()
        {
            ToTable("IssueItem", "Inventory");
            HasKey(vd => vd.IssueItemID);
        }
    }
    class IssueDistributionEntityConfiguration : EntityTypeConfiguration<IssueDistribution>
    {
        public IssueDistributionEntityConfiguration()
        {
            ToTable("IssueDistribution", "Inventory");
            HasKey(vd => vd.IssueDistributionID);
        }
    } 
    class IssueValidationEntityConfiguration : EntityTypeConfiguration<IssueValidation>
    {
        public IssueValidationEntityConfiguration()
        {
            ToTable("IssueValidation", "Inventory");
            HasKey(vd => vd.IssueValidationID);
        }
    } 
   
    class ItemInspectionEntityConfiguration : EntityTypeConfiguration<ItemInspection>
    {
        public ItemInspectionEntityConfiguration()
        {
            ToTable("ItemInspection", "Inventory");
            HasKey(vd => vd.ItemInspectionId);
        }
    }
    class ItemTransferEntityConfiguration : EntityTypeConfiguration<ItemTransfer>
    {
        public ItemTransferEntityConfiguration()
        {
            ToTable("ItemTransfer", "Inventory");
            HasKey(vd => vd.ItemTransferID);
        }
    }
    class ItemTransferItemEntityConfiguration : EntityTypeConfiguration<ItemTransferItem>
    {
        public ItemTransferItemEntityConfiguration()
        {
            ToTable("ItemTransferItem", "Inventory");
            HasKey(vd => vd.ItemTransferItemID);
        }
    }
    class ItemTransferDistributionEntityConfiguration : EntityTypeConfiguration<ItemTransferDistribution>
    {
        public ItemTransferDistributionEntityConfiguration()
        {
            ToTable("ItemTransferDistribution", "Inventory");
            HasKey(vd => vd.ItemTransferDistributionID);
        }
    }
    class ItemTransferValidationEntityConfiguration : EntityTypeConfiguration<ItemTransferValidation>
    {
        public ItemTransferValidationEntityConfiguration()
        {
            ToTable("ItemTransferValidation", "Inventory");
            HasKey(vd => vd.ItemTransferValidationID);
        }
    }
    class ProjectStoresEntityConfiguration : EntityTypeConfiguration<ProjectStores>
    {
        public ProjectStoresEntityConfiguration()
        {
            ToTable("ProjectStores", "Inventory");
            HasKey(vd => vd.ProjectStoresId);
        }
    }
    
    class PropertyCountEntityConfiguration : EntityTypeConfiguration<PropertyCount>
    {
        public PropertyCountEntityConfiguration()
        {
            ToTable("PropertyCount", "Inventory");
            HasKey(vd => vd.PropertyCountId);
        }
    }
    class PropertyCountItemEntityConfiguration : EntityTypeConfiguration<PropertyCountItem>
    {
        public PropertyCountItemEntityConfiguration()
        {
            ToTable("PropertyCountItem", "Inventory");
            HasKey(vd => vd.PropertyCountItemId);
        }
    }
    class PurchaseRequisitionEntityConfiguration : EntityTypeConfiguration<PurchaseRequisition>
    {
        public PurchaseRequisitionEntityConfiguration()
        {
            ToTable("PurchaseRequisition", "Inventory");
            HasKey(vd => vd.PurchaseRequisitionID);
        }
    }
    class PurchaseRequisitionItemEntityConfiguration : EntityTypeConfiguration<PurchaseRequisitionItem>
    {
        public PurchaseRequisitionItemEntityConfiguration()
        {
            ToTable("PurchaseRequisitionItem", "Inventory");
            HasKey(vd => vd.PurchaseRequisitionItemID);
        }
    }
    //class PurchaseRequisitionDistributionEntityConfiguration : EntityTypeConfiguration<PurchaseRequisitionDistribution>
    //{
    //    public PurchaseRequisitionDistributionEntityConfiguration()
    //    {
    //        ToTable("PurchaseRequisitionDistribution", "Inventory");
    //        HasKey(vd => vd.PurchaseRequisitionDistributionID );
    //    }
    //} 
    class PurchaseRequisitionPurchaseDepartmentValidationEntityConfiguration : EntityTypeConfiguration<PurchaseRequisitionPurchaseDepartmentValidation>
    {
        public PurchaseRequisitionPurchaseDepartmentValidationEntityConfiguration()
        {
            ToTable("PurchaseRequisitionPurchaseDepartmentValidation", "Inventory");
            HasKey(vd => vd.PurchaseRequisitionPurchaseDepartmentValidationID);
        }
    } 
    class PurchaseRequisitionUserDepartmentValidationEntityConfiguration : EntityTypeConfiguration<PurchaseRequisitionUserDepartmentValidation>
    {
        public PurchaseRequisitionUserDepartmentValidationEntityConfiguration()
        {
            ToTable("PurchaseRequisitionUserDepartmentValidation", "Inventory");
            HasKey(vd => vd.PurchaseRequisitionUserDepartmentValidationID);
        }
    }
    class PurchaseReturnEntityConfiguration : EntityTypeConfiguration<PurchaseReturn>
    {
        public PurchaseReturnEntityConfiguration()
        {
            ToTable("PurchaseReturn", "Inventory");
            HasKey(vd => vd.PurchaseReturnID);
        }
    }
    class PurchaseReturnDistributionEntityConfiguration : EntityTypeConfiguration<PurchaseReturnDistribution>
    {
        public PurchaseReturnDistributionEntityConfiguration()
        {
            ToTable("PurchaseReturnDistribution", "Inventory");
            HasKey(vd => vd.PurchaseReturnDistributionID);
        }
    }
    class PurchaseReturnItemsEntityConfiguration : EntityTypeConfiguration<PurchaseReturnItems>
    {
        public PurchaseReturnItemsEntityConfiguration()
        {
            ToTable("PurchaseReturnItems", "Inventory");
            HasKey(vd => vd.PurchaseReturnItemsID);
        }
    }
    class PurchaseReturnValidationEntityConfiguration : EntityTypeConfiguration<PurchaseReturnValidation>
    {
        public PurchaseReturnValidationEntityConfiguration()
        {
            ToTable("PurchaseReturnValidation", "Inventory");
            HasKey(vd => vd.PurchaseReturnValidationID);
        }
    }
    class StockAdjustmentEntityConfiguration : EntityTypeConfiguration<StockAdjustment>
    {
        public StockAdjustmentEntityConfiguration()
        {
            ToTable("StockAdjustment", "Inventory");
            HasKey(vd => vd.StockAdjustmentID);
        }
    }
    class StockAdjustmentItemsEntityConfiguration : EntityTypeConfiguration<StockAdjustmentItems>
    {
        public StockAdjustmentItemsEntityConfiguration()
        {
            ToTable("StockAdjustmentItems", "Inventory");
            HasKey(vd => vd.StockAdjustmentItemsID);
        }
    } 
    class StockAdjustmentDistributionEntityConfiguration : EntityTypeConfiguration<StockAdjustmentDistribution>
    {
        public StockAdjustmentDistributionEntityConfiguration()
        {
            ToTable("StockAdjustmentDistribution", "Inventory");
            HasKey(vd => vd.StockAdjustmentDistributionID);
        }
    } 
    class StockAdjustmentValidationEntityConfiguration : EntityTypeConfiguration<StockAdjustmentValidation>
    {
        public StockAdjustmentValidationEntityConfiguration()
        {
            ToTable("StockAdjustmentValidation", "Inventory");
            HasKey(vd => vd.StockAdjustmentValidationID);
        }
    } 
    class StoreKeeperAssignmentEntityConfiguration : EntityTypeConfiguration<StoreKeeperAssignment>
    {
        public StoreKeeperAssignmentEntityConfiguration()
        {
            ToTable("StoreKeeperAssignment", "Inventory");
            HasKey(vd => vd.StoreKeeperAssignmentID);
        }
    }
    class StoreItemAssignmentEntityConfiguration : EntityTypeConfiguration<StoreItemAssignment>
    {
        public StoreItemAssignmentEntityConfiguration()
        {
            ToTable("StoreItemAssignment", "Inventory");
            HasKey(vd => vd.StoreItemAssignmentID);
        }
    } 
    class BinCardLineEntityConfiguration : EntityTypeConfiguration<BinCardLine>
    {
        public BinCardLineEntityConfiguration()
        {
            ToTable("BinCardLine", "Inventory");
            HasKey(vd => vd.BinCardLineID);
        }
    } 
    class StoreRequisitionEntityConfiguration : EntityTypeConfiguration<StoreRequisition>
    {
        public StoreRequisitionEntityConfiguration()
        {
            ToTable("StoreRequisition", "Inventory");
            HasKey(vd => vd.StoreRequisitionID);
        }
    }
    class StoreRequisitionItemEntityConfiguration : EntityTypeConfiguration<StoreRequisitionItem>
    {
        public StoreRequisitionItemEntityConfiguration()
        {
            ToTable("StoreRequisitionItem", "Inventory");
            HasKey(vd => vd.StoreRequisitionItemID);
        }
    }
    
    class StoreRequisitionValidationEntityConfiguration : EntityTypeConfiguration<StoreRequisitionValidation>
    {
        public StoreRequisitionValidationEntityConfiguration()
        {
            ToTable("StoreRequisitionValidation", "Inventory");
            HasKey(vd => vd.StoreRequisitionValidationID);
        }
    } 
    class StoreReturnEntityConfiguration : EntityTypeConfiguration<StoreReturn>
    {
        public StoreReturnEntityConfiguration()
        {
            ToTable("StoreReturn", "Inventory");
            HasKey(vd => vd.StoreReturnID);
        }
    }
    class StoreReturnItemsEntityConfiguration : EntityTypeConfiguration<StoreReturnItems>
    {
        public StoreReturnItemsEntityConfiguration()
        {
            ToTable("StoreReturnItems", "Inventory");
            HasKey(vd => vd.StoreReturnItemsID);
        }
    }
    class StoreReturnDistributionEntityConfiguration : EntityTypeConfiguration<StoreReturnDistribution>
    {
        public StoreReturnDistributionEntityConfiguration()
        {
            ToTable("StoreReturnDistribution", "Inventory");
            HasKey(vd => vd.StoreReturnDistributionID );
        }
    }
    class StoreReturnValidationEntityConfiguration : EntityTypeConfiguration<StoreReturnValidation>
    {
        public StoreReturnValidationEntityConfiguration()
        {
            ToTable("StoreReturnValidation", "Inventory");
            HasKey(vd => vd.StoreReturnValidationID);
        }
    }
    class TempGoodReceiveEntityConfiguration : EntityTypeConfiguration<TempGoodReceive>
    {
        public TempGoodReceiveEntityConfiguration()
        {
            ToTable("TempGoodReceive", "Inventory");
            HasKey(vd => vd.TempGoodReceiveID);
        }
    }
    class TempGoodReceiveItemEntityConfiguration : EntityTypeConfiguration<TempGoodReceiveItem>
    {
        public TempGoodReceiveItemEntityConfiguration()
        {
            ToTable("TempGoodReceiveItem", "Inventory");
            HasKey(vd => vd.TempGoodReceiveItemID);
        }
    }
    class TempGoodReceiveValidationEntityConfiguration : EntityTypeConfiguration<TempGoodReceiveValidation>
    {
        public TempGoodReceiveValidationEntityConfiguration()
        {
            ToTable("TempGoodReceiveValidation", "Inventory");
            HasKey(vd => vd.TempGoodReceiveValidationID);
        }
    } 

}
