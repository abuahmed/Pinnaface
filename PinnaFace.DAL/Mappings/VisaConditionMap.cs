using System.Data.Entity.ModelConfiguration;
using PinnaFace.Core.Models;

namespace PinnaFace.DAL.Mappings
{
    public class VisaConditionMap : EntityTypeConfiguration<VisaConditionDTO>
    {
        public VisaConditionMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
           
            // Table & Column Mappings
            ToTable("Visa_Conditions");
        }
    }
}
