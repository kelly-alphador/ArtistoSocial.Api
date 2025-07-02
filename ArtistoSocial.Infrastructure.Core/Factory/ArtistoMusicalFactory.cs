using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtistoSocial.Infrastructure.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;



namespace ArtistoSocial.Infrastructure.Core.Factory
{
    public class ArtistoMusicalFactory:IDesignTimeDbContextFactory<ArtistoMusicalDbContext>
    {
        public ArtistoMusicalDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ArtistoMusicalDbContext>();
            optionsBuilder.UseSqlServer("Server=DESKTOP-1PCHEEU\\SQLEXPRESS;Database=ArtistoSocial;Trusted_Connection=True;TrustServerCertificate=True;"); // Replace with your actual connection string
            return new ArtistoMusicalDbContext(optionsBuilder.Options);
        }
    }
}
