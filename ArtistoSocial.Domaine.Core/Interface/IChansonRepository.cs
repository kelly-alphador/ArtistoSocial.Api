using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtistoSocial.Domaine.Core.DTO.ApiResponse;
using ArtistoSocial.Domaine.Core.DTO.Chanson;
using ArtistoSocial.Domaine.Core.Entities;

namespace ArtistoSocial.Domaine.Core.Interface
{
    public interface IChansonRepository
    {
        Task<ApiResponse> AddAsync(ChansonAddDTO dto,int ArtisteId,string UrlFichier);
        Task<ApiResponse> DeleteAsync(int Id);
        Task<List<Chanson>> RechercherParTitreEtArtisteAsync(string Nom_Artiste,string titre);
        Task<List<ChansonDTO>> GetAllChansonAsync();

    }
}
