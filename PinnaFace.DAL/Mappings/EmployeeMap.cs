using System.Data.Entity.ModelConfiguration;
using PinnaFace.Core.Models;

namespace PinnaFace.DAL.Mappings
{
    public class EmployeeMap : EntityTypeConfiguration<EmployeeDTO>
    {
        public EmployeeMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            
            // Table & Column Mappings
            ToTable("Employees");

            //Relationships
            HasRequired(t => t.Address)
              .WithMany()
              .HasForeignKey(t => t.AddressId);

            //HasRequired(t => t.Agency)
            //  .WithMany()
            //  .HasForeignKey(t => t.AgencyId)
            //  .WillCascadeOnDelete(false);

            HasOptional(t => t.Visa)
              .WithMany(t=>t.Employees)
              .HasForeignKey(t => t.VisaId)
              .WillCascadeOnDelete(false);
        }
    }
}
