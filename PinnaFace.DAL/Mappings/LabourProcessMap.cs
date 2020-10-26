using System.Data.Entity.ModelConfiguration;
using PinnaFace.Core.Models;

namespace PinnaFace.DAL.Mappings
{
    public class LabourProcessMap : EntityTypeConfiguration<LabourProcessDTO>
    {
        public LabourProcessMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties

            // Table & Column Mappings
            ToTable("Processes_Labour");

            //Relationships
            //HasRequired(t => t.Employee)
            // .WithMany(e => e.LabourProcesses)
            // .HasForeignKey(t => t.EmployeeId);
        }
    }
}
