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
        public class ProformaEntityTypeConfiguration : EntityTypeConfiguration<Proforma>
        {
            public ProformaEntityTypeConfiguration()
            {
                this.ToTable("Proforma", "ProductFollowUp");
                this.HasKey<int>(t => t.ProformaId);
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
        
        public class JobTypeEntityTypeConfiguration : EntityTypeConfiguration<JobType>
        {
            public JobTypeEntityTypeConfiguration()
            {
                this.ToTable("JobType", "ProductFollowUp");
                this.HasKey<int>(t => t.JobTypeId);
            }
        }
        public class JobTypeItemEntityTypeConfiguration : EntityTypeConfiguration<JobTypeItem>
        {
            public JobTypeItemEntityTypeConfiguration()
            {
                this.ToTable("JobTypeItem", "ProductFollowUp");
                this.HasKey<int>(t => t.JobTypeItemId);
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

        public class FinishedGoodReceivingEntityTypeConfiguration : EntityTypeConfiguration<FinishedGoodReceiving>
        {
            public FinishedGoodReceivingEntityTypeConfiguration()
            {
                this.ToTable("FinishedGoodReceiving", "ProductFollowUp");
                this.HasKey<int>(t => t.FinishedGoodReceivingId);
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
