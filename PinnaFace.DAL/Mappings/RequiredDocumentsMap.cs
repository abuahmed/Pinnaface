using System.Data.Entity.ModelConfiguration;
using PinnaFace.Core.Models;

namespace PinnaFace.DAL.Mappings
{
    public class RequiredDocumentsMap : EntityTypeConfiguration<RequiredDocumentsDTO>
    {
        public RequiredDocumentsMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties

            // Table & Column Mappings
            ToTable("Employee_Documents");
            Property(t => t.Id).HasColumnName("Id");
        }
    }
}