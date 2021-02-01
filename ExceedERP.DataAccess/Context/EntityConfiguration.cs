using ExceedERP.Core.Domain.Common;

using System.Data.Entity.ModelConfiguration;
using ExceedERP.Core.Domain.Common.HRM;

using Attachment = ExceedERP.Core.Domain.Common.HRM.Attachment;

using BankReference = ExceedERP.Core.Domain.Common.HRM.BankReference;

using Country = ExceedERP.Core.Domain.Common.HRM.Country;



namespace ExceedERP.DataAccess.Context
{
    class ConfigurationEntityTypeConfiguration : EntityTypeConfiguration<Configuration>
    {

        public ConfigurationEntityTypeConfiguration()
        {
            this.ToTable("Configuration", "Common");
            this.HasKey<int>(vd => vd.ConfigurationId);

        }
    }
    class AddressEntityTypeConfiguration : EntityTypeConfiguration<Address>
    {

        public AddressEntityTypeConfiguration()
        {
            this.ToTable("Address", "Common");
            this.HasKey<int>(vd => vd.AddressId);

        }
    }
    class LookupEntityTypeConfiguration : EntityTypeConfiguration<Lookup>
    {

        public LookupEntityTypeConfiguration()
        {
            this.ToTable("Lookup", "Common");
            this.HasKey<int>(vd => vd.LookupId);

        }
    }
    class PeriodEntityTypeConfiguration : EntityTypeConfiguration<Period>
    {

        public PeriodEntityTypeConfiguration()
        {
            this.ToTable("Period", "Common");
            this.HasKey<int>(vd => vd.PeriodId);

        }
    }
    class IndustryInvolvedEntityTypeConfiguration : EntityTypeConfiguration<IndustryInvolved>
    {

        public IndustryInvolvedEntityTypeConfiguration()
        {
            this.ToTable("IndustryInvolved", "Common");
            this.HasKey<int>(vd => vd.IndustryInvolvedID);

        }
    }

    class AttachmentEntityTypeConfiguration : EntityTypeConfiguration<Attachment>
    {

        public AttachmentEntityTypeConfiguration()
        {
            this.ToTable("Attachment", "Common");
            this.HasKey<int>(vd => vd.AttachmentId);

        }
    }
    class IdentificationEntityTypeConfiguration : EntityTypeConfiguration<Identification>
    {

        public IdentificationEntityTypeConfiguration()
        {
            this.ToTable("Identification", "Common");
            this.HasKey<int>(vd => vd.IdentificationId);

        }
    }
    class LanguageEntityTypeConfiguration : EntityTypeConfiguration<Language>
    {

        public LanguageEntityTypeConfiguration()
        {
            this.ToTable("Language", "Common");
            this.HasKey<int>(vd => vd.LanguageId);

        }
    }
    class LanguageProficiencyEntityTypeConfiguration : EntityTypeConfiguration<LanguageProficiency>
    {

        public LanguageProficiencyEntityTypeConfiguration()
        {
            this.ToTable("LanguageProficiency", "Common");
            this.HasKey<int>(vd => vd.LanguageProficiencyId);

        }
    }
    class RelationEntityTypeConfiguration : EntityTypeConfiguration<Relation>
    {

        public RelationEntityTypeConfiguration()
        {
            this.ToTable("Relation", "Common");
            this.HasKey<int>(vd => vd.RelationId);

        }
    }
    class CountryEntityTypeConfiguration : EntityTypeConfiguration<Country>
    {

        public CountryEntityTypeConfiguration()
        {
            this.ToTable("Country", "Common");
            this.HasKey<int>(vd => vd.CountryId);

        }
    }
    class BankEntityTypeConfiguration : EntityTypeConfiguration<Bank>
    {

        public BankEntityTypeConfiguration()
        {
            this.ToTable("Bank", "Common");
            this.HasKey<int>(vd => vd.BankId);

        }
    }
    class BankReferenceEntityTypeConfiguration : EntityTypeConfiguration<BankReference>
    {

        public BankReferenceEntityTypeConfiguration()
        {
            this.ToTable("BankReference", "Common");
            this.HasKey<int>(vd => vd.BankReferenceId);

        }
    }




}



