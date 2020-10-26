using System.Data.Entity.ModelConfiguration;
using PinnaFace.Core.Models;

namespace PinnaFace.DAL.Mappings
{
    public class InsuranceProcessMap : EntityTypeConfiguration<InsuranceProcessDTO>
    {
        public InsuranceProcessMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties

            // Table & Column Mappings
            ToTable("Processes_Insurance");

            //Relationships
            //HasRequired(t => t.Employee)
            // .WithMany(e => e.InsuranceProcesses)
            // .HasForeignKey(t => t.EmployeeId);
        }
    }
}
