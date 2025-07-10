using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtistoSocial.Domaine.Core.DTO.ApiResponse;
using ArtistoSocial.Domaine.Core.DTO.Chanson;

namespace ArtistoSocial.Domaine.Core.Interface
{
    public interface IChansonRepository
    {
        Task<ApiResponse> AddAsync(ChansonAddDTO dto,int ArtisteId,string UrlFichier);
    }
}
