using System.Data.Entity.ModelConfiguration;
using PinnaFace.Core.Models;

namespace PinnaFace.DAL.Mappings
{
    public class EmployeeExperienceMap : EntityTypeConfiguration<EmployeeExperienceDTO>
    {
        public EmployeeExperienceMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            

            // Table & Column Mappings
            ToTable("Employee_Experiences");

            //Relationships
            //HasRequired(t => t.Employee)
            // .WithMany(t => t.EmployeeApplications)
            // .HasForeignKey(t => t.EmployeeId);
        }
    }
}
