using System.Data.Entity.ModelConfiguration;
using PinnaFace.Core.Models;

namespace PinnaFace.DAL.Mappings
{
    public class ProductActivationMap : EntityTypeConfiguration<ProductActivationDTO>
    {
        public ProductActivationMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.ProductKey)
               .IsRequired();

            Property(t => t.RegisteredBiosSn)
               .IsRequired();

            // Table & Column Mappings
            ToTable("ProductActivation");

            //Relationships
           
        }
    }
}
