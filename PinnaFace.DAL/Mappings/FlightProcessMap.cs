using System.Data.Entity.ModelConfiguration;
using PinnaFace.Core.Models;

namespace PinnaFace.DAL.Mappings
{
    public class FlightProcessMap : EntityTypeConfiguration<FlightProcessDTO>
    {
        public FlightProcessMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties

            // Table & Column Mappings
            ToTable("Processes_Flight");

            //Relationships
            //HasRequired(t => t.Employee)
            // .WithMany(e => e.FlightProcesses)
            // .HasForeignKey(t => t.EmployeeId);
        }
    }
}
