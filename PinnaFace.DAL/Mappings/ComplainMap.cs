using System.Data.Entity.ModelConfiguration;
using PinnaFace.Core.Models;

namespace PinnaFace.DAL.Mappings
{
    public class ComplainMap : EntityTypeConfiguration<ComplainDTO>
    {
        public ComplainMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Complain)
               .IsRequired();

            Property(p => p.RowVersion)
                .IsConcurrencyToken();

            // Table & Column Mappings
            ToTable("Complains");

            //Relationships
            HasRequired(t => t.Employee)
             .WithMany(t => t.Complains)
             .HasForeignKey(t => t.EmployeeId);

            //HasRequired(t => t.Agency)
            // .WithMany()
            // .HasForeignKey(t => t.AgencyId)
            // .WillCascadeOnDelete(false);
            
        }
    }
}
