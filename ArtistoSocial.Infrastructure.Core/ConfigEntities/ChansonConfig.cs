using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtistoSocial.Domaine.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtistoSocial.Infrastructure.Core.ConfigEntities
{
    public class ChansonConfig:IEntityTypeConfiguration<Chanson>
    {
        public void Configure(EntityTypeBuilder<Chanson> builder) 
        {
            builder.ToTable("Chansons");
            builder.Property(c => c.Titre)
                .IsRequired()
                .HasColumnName("Titre")
                .HasMaxLength(50);
            builder.Property(c => c.legende)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("Legende");
            builder.Property(c => c.UrlFichier)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("cheminFichier");
            builder.Property(c => c.nbLikes)
                .IsRequired()
                .HasMaxLength(4)
                .HasColumnName("nombre likes");
            builder.HasKey(c => c.Id);
            builder.HasOne(c => c.Artiste)
                .WithMany(c => c.ListChansons)
                .HasForeignKey(c => c.ArtisteId);

                

        }
    }
}
