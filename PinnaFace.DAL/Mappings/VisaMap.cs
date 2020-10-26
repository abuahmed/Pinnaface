using System.Data.Entity.ModelConfiguration;
using PinnaFace.Core.Models;

namespace PinnaFace.DAL.Mappings
{
    public class VisaMap : EntityTypeConfiguration<VisaDTO>
    {
        public VisaMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties

            // Table & Column Mappings
            ToTable("Visas");

            //Relationships
            HasRequired(t => t.Agent)
             .WithMany(t => t.Visas)
             .HasForeignKey(t => t.ForeignAgentId)
             .WillCascadeOnDelete(false);

        }
    }
}
