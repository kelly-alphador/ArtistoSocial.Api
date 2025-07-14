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
using Microsoft.EntityFrameworkCore;

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
        // Repository - Version corrigée et améliorée
        public async Task<ApiResponse> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = "ID invalide",
                        Errors = { "L'ID doit être supérieur à 0" }
                    };
                }

                var existingChanson = await _context.Chansons.FindAsync(id);
                if (existingChanson == null)
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = "Chanson non trouvée",
                        Errors = { $"Aucune chanson trouvée avec l'ID {id}" }
                    };
                }

                _context.Chansons.Remove(existingChanson);
              

                return new ApiResponse
                {
                    Success = true,
                    Message = $"Chanson avec l'ID {id} supprimée avec succès"
                };
            }
            catch (Exception ex)
            {
                // Log l'erreur ici si vous avez un système de logging
                return new ApiResponse
                {
                    Success = false,
                    Message = "Erreur lors de la suppression",
                    Errors = { "Une erreur interne s'est produite" }
                };
            }
        }

        public async Task<List<Chanson>> RechercherParTitreEtArtisteAsync(string nomArtiste, string titre)
        {
            try
            {
                var query = _context.Chansons.Include(c => c.Artiste).AsQueryable();

                if (!string.IsNullOrWhiteSpace(nomArtiste))
                {
                    query = query.Where(c => c.Artiste.UserName.Contains(nomArtiste));
                }

                if (!string.IsNullOrWhiteSpace(titre))
                {
                    query = query.Where(c => c.Titre.Contains(titre));
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                // Log l'erreur
                throw new Exception("Erreur lors de la recherche des chansons", ex);
            }
        }

        public async Task<List<ChansonDTO>> GetAllChansonAsync()
        {
            try
            {
                var chansons = await _context.Chansons
                    .Include(c => c.Artiste)
                    .Select(c => new ChansonDTO
                    {
                        Id = c.Id,
                        Titre = c.Titre,
                        Legende = c.legende,
                        UrlFichier = c.UrlFichier,
                        DatePublication = c.DatePublication,
                        NbLikes = c.nbLikes,
                        ArtisteId = c.ArtisteId,
                        ArtisteUserName = c.Artiste.UserName
                    })
                    .ToListAsync();

                return chansons;
            }
            catch (Exception ex)
            {
                // Log l'erreur
                throw new Exception("Erreur lors de la récupération des chansons", ex);
            }
        }
    }
}
