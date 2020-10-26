using System.Data.Entity.ModelConfiguration;
using PinnaFace.Core.Models;

namespace PinnaFace.DAL.Mappings
{
    public class EmployeeEducationMap : EntityTypeConfiguration<EmployeeEducationDTO>
    {
        public EmployeeEducationMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties

            // Table & Column Mappings
            ToTable("Employee_Educations");

            //Relationships    
            //HasRequired(t => t.Employee)
            //  .WithMany(t => t.EmployeeEducations)
            //  .HasForeignKey(t => t.EmployeeId);
        }
    }
}
