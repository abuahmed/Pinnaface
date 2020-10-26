using System.Data.Entity.ModelConfiguration;
using PinnaFace.Core.Models;

namespace PinnaFace.DAL.Mappings
{
    public class EmployeeRelativeMap : EntityTypeConfiguration<EmployeeRelativeDTO>
    {
        public EmployeeRelativeMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties

            // Table & Column Mappings
            ToTable("Employee_Relatives");

            //Relationships
            HasOptional(t => t.Address)
                .WithMany()
                .HasForeignKey(t => t.AddressId);

            HasOptional(t => t.Employee)
                .WithMany(t => t.EmployeeRelatives)
                .HasForeignKey(t => t.EmployeeId)
                .WillCascadeOnDelete(false);
        }
    }
}