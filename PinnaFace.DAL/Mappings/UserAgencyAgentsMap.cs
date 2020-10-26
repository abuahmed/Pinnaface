using System.Data.Entity.ModelConfiguration;
using PinnaFace.Core.Models;

namespace PinnaFace.DAL.Mappings
{
    public class UserAgencyAgentsMap : EntityTypeConfiguration<UserAgencyAgentDTO>
    {
        public UserAgencyAgentsMap()
        {
            // Table & Column Mappings
            ToTable("UserAgencyAgents");

            //Relationships
            HasRequired(t => t.User)
                .WithMany(e => e.AgenciesWithAgents)
                .HasForeignKey(t => t.UserId);

            HasRequired(t => t.AgencyAgent)
                .WithMany(e => e.Users)
                .HasForeignKey(t => t.AgencyWithAgentId);

        }
    }
}