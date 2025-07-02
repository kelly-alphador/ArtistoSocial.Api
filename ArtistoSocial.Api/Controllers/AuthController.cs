using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ArtistoSocial.Domaine.Core.DTO.Artistes;
using ArtistoSocial.Domaine.Core.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ArtistoSocial.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        //permet gerer les utilisateurs(ajouter, modifier, supprimer, authentifier, etc.)
        private readonly UserManager<Artiste> _userManager;
        //permet de recuperer la configuration de l'application interface dans asp.net core
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AuthController(UserManager<Artiste> userManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] ArtisteRegisterDTO registerModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (registerModel.Password != registerModel.ConfirmPassword)
                return BadRequest("le password et confirmPassword non identique");

            var userExists = await _userManager.FindByEmailAsync(registerModel.Email);
            if (userExists != null)
                return BadRequest("Email Existe deja");

            string relativePath = null;
            if (registerModel.ImageProfil != null && registerModel.ImageProfil.Length > 0)
            {
                // Racine du projet
                string directoryPath = Path.Combine(_webHostEnvironment.ContentRootPath, @"wwwroot\image\profil");

                // Créer le dossier s'il n'existe pas
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Créer un nom unique pour éviter les collisions
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(registerModel.ImageProfil.FileName);

                // Chemin complet du fichier
                string filePath = Path.Combine(directoryPath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    // Corrected: registerModel.ImageProfil is a string, not a file. 
                    // You need to handle file upload properly using IFormFile.
                    await registerModel.ImageProfil.CopyToAsync(stream);
                }

                // Stocker le chemin relatif pour la base
                relativePath = $"/image/profil/{uniqueFileName}";
            }

            var user = new Artiste
            {
                UserName = registerModel.UserName,
                Email = registerModel.Email,
                ImageProfil = relativePath,
                Date_inscription = registerModel.DateTime
            };

            var result = await _userManager.CreateAsync(user, registerModel.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ArtisteLoginDTO loginModel)
        {
            //pour verifier si l'utilisateur existe
            var user = await _userManager.FindByEmailAsync(loginModel.Email);
            if (user == null)
            {
                return Unauthorized("email invalide");
            }
            //pour verifier si le mot de passe est correct
            var result = await _userManager.CheckPasswordAsync(user, loginModel.Password);
            if (!result)
            {
                return Unauthorized("password invalide ");
            }
            //générer un token JWT
            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }
        private string GenerateJwtToken(Artiste user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtConfig:Secret"]);

            var claims = new[]
            {
                new Claim("Id", user.Id.ToString()), // Convert 'int' to 'string' using ToString()
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(6),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }
    }
    /*public class ArtisteRegisterDTO
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public IFormFile ImageProfil { get; set; } // Changed from string to IFormFile
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public DateTime DateTime { get; set; }
    }*/
}
