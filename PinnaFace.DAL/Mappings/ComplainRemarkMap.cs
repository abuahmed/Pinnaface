using System.Data.Entity.ModelConfiguration;
using PinnaFace.Core.Models;

namespace PinnaFace.DAL.Mappings
{
    public class ComplainRemarkMap : EntityTypeConfiguration<ComplainRemarkDTO>
    {
        public ComplainRemarkMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Remark)
               .IsRequired();

            // Table & Column Mappings
            ToTable("Complain_Remarks");

            //Relationships
            HasRequired(t => t.Complain)
             .WithMany(c=>c.Remarks)
             .HasForeignKey(t => t.ComplainId)
             .WillCascadeOnDelete(false);
        }
    }
}
