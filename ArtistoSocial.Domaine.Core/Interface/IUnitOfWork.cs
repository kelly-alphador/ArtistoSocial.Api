using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtistoSocial.Domaine.Core.Interface
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
    }
}
