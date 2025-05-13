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
    internal class AuthorConfiguration : IEntityTypeConfiguration<AuthorEntity>
    {
        public void Configure(EntityTypeBuilder<AuthorEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(a => a.Name).HasMaxLength(Author.MAX_TITLE_LENGTH).IsRequired();
            builder.Property(a => a.Surname).IsRequired();

            builder.HasMany(a => a.Books)
                .WithOne(a => a.Author)
                .HasForeignKey(b => b.AuthorId); 
        }

    }
}