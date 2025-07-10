using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtistoSocial.Domaine.Core.DTO.ApiResponse;
using ArtistoSocial.Domaine.Core.DTO.Chanson;
using ArtistoSocial.Domaine.Core.Entities;
using ArtistoSocial.Domaine.Core.Interface;
using ArtistoSocial.Infrastructure.Core.Data;

namespace ArtistoSocial.Infrastructure.Core.Repository
{
    public class ChansonRepository:IChansonRepository
    {
        private readonly ArtistoMusicalDbContext _context;
        public ChansonRepository(ArtistoMusicalDbContext context) 
        {
            _context = context;
        }
        public async Task<ApiResponse> AddAsync(ChansonAddDTO dto, int ArtisteId, string UrlFichier)
        {
            try
            {
                var chanson = new Chanson
                {
                    Titre = dto.Titre,
                    legende = dto.legende,
                    UrlFichier = UrlFichier,
                    DatePublication = DateTime.UtcNow,
                    nbLikes = 0,
                    ArtisteId = ArtisteId,
                };
                await _context.AddAsync(chanson);
                return new ApiResponse
                {
                    Message = "Chanson publiée avec succès",
                    Success = true,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Message = $"une erreur ce produit lors de l'ajout{ex.Message}",
                    Success = false,
                    Errors = [ex.Message]
                };
            }
        }
    }
}
