using System.Data.Entity.ModelConfiguration;
using PinnaFace.Core.Models;

namespace PinnaFace.DAL.Mappings
{
    public class AgencyMap : EntityTypeConfiguration<AgencyDTO>
    {
        public AgencyMap() {

            // Primary Key
            HasKey(t => t.Id);

            // Properties

            // Table & Column Mappings
            ToTable("Agencies");

            //Relationships
            HasOptional(t => t.Address)
              .WithMany()
              .HasForeignKey(t => t.AddressId);
        }
    }
}
