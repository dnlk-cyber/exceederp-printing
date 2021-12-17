using ExceedERP.Core.Domain.ProductionFollowUp;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.DataAccess.Context
{
   public class ProductFollowUpEntityConfiguration
    {
        public class JobStatusLineItemEntityTypeConfiguration : EntityTypeConfiguration<JobStatusLineItem>
        {
            public JobStatusLineItemEntityTypeConfiguration()
            {
                this.ToTable("JobStatusLineItems", "ProductFollowUp");
                this.HasKey<int>(t => t.JobStatusLineItemId);
            }
        }
        public class MachineTypeEntityTypeConfiguration : EntityTypeConfiguration<Machine>
        {
            public MachineTypeEntityTypeConfiguration()
            {
                this.ToTable("MachineTypes", "ProductFollowUp");
                this.HasKey<int>(t => t.MachineId);
            }
        }
         public class JobCategoryEntityTypeConfiguration : EntityTypeConfiguration<JobCategory>
        {
            public JobCategoryEntityTypeConfiguration()
            {
                this.ToTable("JobCategories", "ProductFollowUp");
                this.HasKey<int>(t => t.JobCategoryId);
            }
        }

        public class ProformaEntityTypeConfiguration : EntityTypeConfiguration<Proforma>
        {
            public ProformaEntityTypeConfiguration()
            {
                this.ToTable("Proforma", "ProductFollowUp");
                this.HasKey<int>(t => t.ProformaId);
            }
        }
        public class PFProformaItemsEntityTypeConfiguration : EntityTypeConfiguration<PFProformaItems>
        {
            public PFProformaItemsEntityTypeConfiguration()
            {
                this.ToTable("PFProformaItems", "ProductFollowUp");
                this.HasKey<int>(t => t.PFProformaItemsId);
            }
        }

     

        public class GraphicEntityTypeConfiguration : EntityTypeConfiguration<Graphic>
        {
            public GraphicEntityTypeConfiguration()
            {
                this.ToTable("Graphic", "ProductFollowUp");
                this.HasKey<int>(t => t.GraphicID);
            }
        }


        public class JobEntityTypeConfiguration : EntityTypeConfiguration<Job>
        {
            public JobEntityTypeConfiguration()
            {
                this.ToTable("Job", "ProductFollowUp");
                this.HasKey<int>(t => t.JobId);
            }
        }
        public class JobStatusEntityTypeConfiguration : EntityTypeConfiguration<JobStatus>
        {
            public JobStatusEntityTypeConfiguration()
            {
                this.ToTable("JobStatusEntityTypeConfiguration", "ProductFollowUp");
                this.HasKey<int>(t => t.JobStatusId);
            }
        }

        

        public class SettingsEntityTypeConfiguration : EntityTypeConfiguration<Settings>
        {
            public SettingsEntityTypeConfiguration()
            {
                this.ToTable("Settings", "ProductFollowUp");
                this.HasKey<int>(t => t.SettingsId);
            }
        }
        
      

        public class JobOrderMaterialEntityTypeConfiguration : EntityTypeConfiguration<JobOrderMaterial>
        {
            public JobOrderMaterialEntityTypeConfiguration()
            {
                this.ToTable("JobOrderMaterial", "ProductFollowUp");
                this.HasKey<int>(t => t.JobOrderMaterialId);
            }
        }

        public class PrintingProductionFinishedGoodsStoreReceiveEntityTypeConfiguration : EntityTypeConfiguration<PrintingProductionFinishedGoodsStoreReceive>
        {
            public PrintingProductionFinishedGoodsStoreReceiveEntityTypeConfiguration()
            {
                this.ToTable("FinishedGoodsStoreReceive", "ProductFollowUp");
                this.HasKey<int>(t => t.FinishedGoodsStoreReceiveId);
            }
        }
        public class PrintingProductionFinishedGoodsStoreReceiveItemEntityTypeConfiguration : EntityTypeConfiguration<PrintingProductionFinishedGoodsStoreReceiveItem>
        {
            public PrintingProductionFinishedGoodsStoreReceiveItemEntityTypeConfiguration()
            {
                this.ToTable("FinishedGoodsStoreReceiveItem", "ProductFollowUp");
                this.HasKey<int>(t => t.FinishedGoodsStoreReceiveItemId);
            }
        }
        public class PrintingProductionFinishedGoodsStoreReceiveValidationEntityTypeConfiguration : EntityTypeConfiguration<PrintingProductionFinishedGoodsStoreReceiveValidation>
        {
            public PrintingProductionFinishedGoodsStoreReceiveValidationEntityTypeConfiguration()
            {
                this.ToTable("FinishedGoodsStoreReceiveValidation", "ProductFollowUp");
                this.HasKey<int>(t => t.FinishedGoodsStoreReceiveValidationId);
            }
        }
        public class PrintingProductionFinishedGoodsStoreReceiveDistributionEntityTypeConfiguration : EntityTypeConfiguration<PrintingProductionFinishedGoodsStoreReceiveDistribution>
        {
            public PrintingProductionFinishedGoodsStoreReceiveDistributionEntityTypeConfiguration()
            {
                this.ToTable("FinishedGoodsStoreReceiveDistribution", "ProductFollowUp");
                this.HasKey<int>(t => t.FinishedGoodsStoreReceiveDistributionId);
            }
        }

        public class FinishedGoodsEntityTypeConfiguration : EntityTypeConfiguration<FinishedGoods>
        {
            public FinishedGoodsEntityTypeConfiguration()
            {
                this.ToTable("FinishedGoodItems", "ProductFollowUp");
                this.HasKey<int>(t => t.FinishedGoodsId);
            }
        }

        public class ProductDeliveryEntityTypeConfiguration : EntityTypeConfiguration<ProductDelivery>
        {
            public ProductDeliveryEntityTypeConfiguration()
            {
                this.ToTable("ProductDelivery", "ProductFollowUp");
                this.HasKey<int>(t => t.ProductDeliveryId);
            }
        }

        public class ProductDeliveryItemsEntityTypeConfiguration : EntityTypeConfiguration<ProductDeliveryItems>
        {
            public ProductDeliveryItemsEntityTypeConfiguration()
            {
                this.ToTable("ProductDeliveryItems", "ProductFollowUp");
                this.HasKey<int>(t => t.ProductDeliveryItemsId);
            }
        }

    }
}
