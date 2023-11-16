using ContactApp.API.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Globalization;

namespace ContactApp.API.Data.Configurations
{
    public class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            //Required
            builder.Property(c => c.FirstName).IsRequired();
            builder.Property(c => c.LastName).IsRequired();
            builder.Property(c => c.Email).IsRequired();
            builder.Property(c => c.Password).IsRequired();
            builder.Property(c => c.PhoneNumber).IsRequired();
            builder.Property(c => c.DateOfBirth).IsRequired();
            // Unique
            builder.HasIndex(c => c.Email).IsUnique();
            builder.HasIndex(c => c.PhoneNumber).IsUnique();

            //builder
            //    .HasOne(c => c.Category)
            //    .WithMany(category => category.Contacts)
            //    .HasForeignKey(c => c.CategoryId)
            //    .OnDelete(DeleteBehavior.Restrict);

            //builder
            //    .HasOne(c => c.Subcategory)
            //    .WithMany(subcategory => subcategory.Contacts)
            //    .HasForeignKey(c => c.SubcategoryId)
            //    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
