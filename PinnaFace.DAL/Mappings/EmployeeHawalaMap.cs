using System.Data.Entity.ModelConfiguration;
using PinnaFace.Core.Models;

namespace PinnaFace.DAL.Mappings
{
    public class EmployeeHawalaMap : EntityTypeConfiguration<EmployeeHawalaDTO>
    {
        public EmployeeHawalaMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties

            // Table & Column Mappings
            ToTable("Employee_Hawalas");

            ////Relationships    
            //HasRequired(t => t.Employee)
            //  .WithMany(t => t.EmployeeHawalas)
            //  .HasForeignKey(t => t.EmployeeId);
        }
    }
}
