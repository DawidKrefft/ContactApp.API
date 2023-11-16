using ContactApp.API.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContactApp.API.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(c => c.Name).IsRequired();

            //builder
            //    .HasMany(c => c.Subcategories)
            //    .WithOne(s => s.Category)
            //    .HasForeignKey(s => s.CategoryId)
            //    .OnDelete(DeleteBehavior.Restrict);

            //builder
            //    .HasMany(c => c.Contacts)
            //    .WithOne(contact => contact.Category)
            //    .HasForeignKey(contact => contact.CategoryId)
            //    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
