using System.Data.Entity.ModelConfiguration;
using PinnaFace.Core.Models;

namespace PinnaFace.DAL.Mappings
{
    public class AttachmentMap : EntityTypeConfiguration<AttachmentDTO>
    {
        public AttachmentMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties

            // Table & Column Mappings
            ToTable("Attachments");
            Property(t => t.Id).HasColumnName("Id");

        }
    }
}