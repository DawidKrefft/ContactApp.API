using ContactApp.API.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContactApp.API.Data.Configurations
{
    public class SubcategoryConfiguration : IEntityTypeConfiguration<Subcategory>
    {
        public void Configure(EntityTypeBuilder<Subcategory> builder)
        {
            // Required
            builder.Property(s => s.Name).IsRequired();
            builder.Property(s => s.CategoryId).IsRequired();

            //builder
            //    .HasMany(s => s.Contacts)
            //    .WithOne(contact => contact.Subcategory)
            //    .HasForeignKey(contact => contact.SubcategoryId)
            //    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
