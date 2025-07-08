using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtistoSocial.Domaine.Core.Entities;
using ArtistoSocial.Infrastructure.Core.ConfigEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ArtistoSocial.Infrastructure.Core.Data
{
    //on herite de IdentityDbcontext pour gérer les utilisateurs et les rôles
    public class ArtistoMusicalDbContext:IdentityDbContext<Artiste,IdentityRole<int>,int>
    {
        //permet de configurer la connection a la base de donnees
        public ArtistoMusicalDbContext(DbContextOptions<ArtistoMusicalDbContext> options): base(options)
        {
        }
        public DbSet<Chanson> Chansons { get; set; } // DbSet pour les chansons
        //pour configurer les entités et les relations
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new ArtistesConfig());
            modelBuilder.ApplyConfiguration(new ChansonConfig());
            
        }
       
    }
}
