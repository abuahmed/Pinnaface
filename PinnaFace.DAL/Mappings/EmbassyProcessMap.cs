using System.Data.Entity.ModelConfiguration;
using PinnaFace.Core.Models;

namespace PinnaFace.DAL.Mappings
{
    public class EmbassyProcessMap : EntityTypeConfiguration<EmbassyProcessDTO>
    {
        public EmbassyProcessMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.EnjazNumber)
               .IsRequired();

            // Table & Column Mappings
            ToTable("Processes_Embassy");

            //Relationships
            //HasRequired(t => t.Employee)
            // .WithMany(t => t.EmbassyProcesses)
            // .HasForeignKey(t => t.EmployeeId);
        }
    }
}
