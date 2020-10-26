using System.Data.Entity.ModelConfiguration;
using PinnaFace.Core.Models;

namespace PinnaFace.DAL.Mappings
{
    public class SettingMap : EntityTypeConfiguration<SettingDTO>
    {
        public SettingMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties

            // Table & Column Mappings
            ToTable("Settings");

            //Relationships
            
        }
    }
}
