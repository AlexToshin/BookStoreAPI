using BookStore.Core.Entities;
using BookStore.Core.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccess.Configurations
{
    internal class CategoryConfiguration : IEntityTypeConfiguration<CategoryEntity>
    {
        public void Configure(EntityTypeBuilder<CategoryEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(c => c.Name).HasMaxLength(Category.MAX_TITLE_LENGTH).IsRequired();
            builder.Property(c => c.Description).IsRequired();

            builder.HasMany(c => c.Books)
                .WithOne(c => c.Category)
                .HasForeignKey(b => b.CategoryId); 
        }

    }
}