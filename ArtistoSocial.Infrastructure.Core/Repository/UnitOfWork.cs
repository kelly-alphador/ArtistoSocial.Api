using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtistoSocial.Domaine.Core.Interface;
using ArtistoSocial.Infrastructure.Core.Data;

namespace ArtistoSocial.Infrastructure.Core.Repository
{
    public class UnitOfWork:IUnitOfWork
    {
        private readonly ArtistoMusicalDbContext _context;
        public UnitOfWork(ArtistoMusicalDbContext context)
        {
            _context = context;
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
