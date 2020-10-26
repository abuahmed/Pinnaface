using System.Data.Entity.ModelConfiguration;
using PinnaFace.Core.Models;

namespace PinnaFace.DAL.Mappings
{
    public class AgentMap : EntityTypeConfiguration<AgentDTO>
    {
        public AgentMap() {

            // Primary Key
            HasKey(t => t.Id);

            // Properties

            // Table & Column Mappings
            ToTable("Agents");

            //Relationships
            HasOptional(t => t.Address)
              .WithMany()
              .HasForeignKey(t => t.AddressId);

        } 
    }
}
