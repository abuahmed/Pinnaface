using System.Data.Entity.ModelConfiguration;
using PinnaFace.Core.Models;

namespace PinnaFace.DAL.Mappings
{
    public class ListMap : EntityTypeConfiguration<ListDTO>
    {
        public ListMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.DisplayName)
               .IsRequired()
               .HasMaxLength(75);
                        
            // Table & Column Mappings
            ToTable("Lists");
            Property(t => t.Id).HasColumnName("Id");

            //Relationships
        }


    }
}

