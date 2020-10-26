using System.Data.Entity.ModelConfiguration;
using PinnaFace.Core.Models;

namespace PinnaFace.DAL.Mappings
{
    public class VisaSponsorMap : EntityTypeConfiguration<VisaSponsorDTO>
    {
        public VisaSponsorMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties

            // Table & Column Mappings
            ToTable("Visa_Sponsors");

            //Relationships
            HasOptional(t => t.Address)
              .WithMany()
              .HasForeignKey(t => t.AddressId);
        }
    }
}
