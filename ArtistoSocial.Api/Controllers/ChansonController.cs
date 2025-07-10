using ArtistoSocial.Domaine.Core.DTO.Chanson;
using ArtistoSocial.Domaine.Core.Entities;
using ArtistoSocial.Domaine.Core.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ArtistoSocial.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
 
    public class ChansonController : Controller
    {
        private readonly IChansonRepository _chansonRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<Artiste> _userManager;
        public ChansonController(IChansonRepository chansonRepository,IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment,UserManager<Artiste> userManager)
        {
            _chansonRepository = chansonRepository;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Add([FromForm] ChansonAddDTO dto)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized("utilisateur non connecter");
                }
                //1. Validation du fichier
                //verifie si le fichier n'est pas vide et existe
                if (dto.Fichier == null || dto.Fichier.Length == 0)
                {
                    return BadRequest("aucun fichier fournie");
                }
                //validation d'extension verifie si le fichier est .mp3
                var extension = Path.GetExtension(dto.Fichier.FileName).ToLower();
                if (extension != ".mp3")
                {
                    return BadRequest("seul le fichier .mp3 est authorizer");
                }
                //Generation de nom de fichier pour qu'il soit unique
                var filename = Guid.NewGuid().ToString() + extension;
                //creation du chemin complet
                var path = Path.Combine(_webHostEnvironment.WebRootPath, "upload", filename);

                //Créer le dossier si nécessaire
                var directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                //Sauvegarde le fichier sur le serveur
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await dto.Fichier.CopyToAsync(stream);
                }

                //Generation de l'url publique
                var urlFichier = Path.Combine("uploads", filename).Replace("\\", "/");
                var response = await _chansonRepository.AddAsync(dto, user.Id, urlFichier);
                if (response.Success)
                {
                    await _unitOfWork.SaveChangesAsync();
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response);
                }
            }
            catch (Exception ex) 
            {
                return StatusCode(500, new { message = "Erreur lors de l'ajout de la tâche", error = ex.Message });
            }
        }
    }
}
