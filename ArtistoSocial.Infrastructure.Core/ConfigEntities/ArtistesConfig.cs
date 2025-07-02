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
    public class ArtistesConfig:IEntityTypeConfiguration<Artiste>
    {
        public void Configure(EntityTypeBuilder<Artiste> builder)
        {
            //pour specifier le nom de la table dans la base de données pour Identity pardefaut:AspNetUsers
            builder.ToTable("Artistes");
            builder.Property(a => a.ImageProfil)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("ImageProfil");
        }
    }
}
