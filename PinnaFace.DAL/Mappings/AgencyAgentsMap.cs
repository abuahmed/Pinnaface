using System.Data.Entity.ModelConfiguration;
using PinnaFace.Core.Models;

namespace PinnaFace.DAL.Mappings
{
    public class AgencyAgentsMap : EntityTypeConfiguration<AgencyAgentDTO>
    {
        public AgencyAgentsMap()
        {
            // Table & Column Mappings
            ToTable("AgencyAgents");

            //Relationships
            HasRequired(t => t.Agency)
                .WithMany(e => e.Agents)
                .HasForeignKey(t => t.AgencyId);

            HasRequired(t => t.Agent)
                .WithMany(e => e.Agencies)
                .HasForeignKey(t => t.AgentId);
            
        }
    }
}