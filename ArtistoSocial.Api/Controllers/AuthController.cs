using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ArtistoSocial.Domaine.Core.DTO.Artistes;
using ArtistoSocial.Domaine.Core.DTO.Email;
using ArtistoSocial.Domaine.Core.Entities;
using ArtistoSocial.Domaine.Core.Interface;
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
        private readonly IEmailService _emailService;
        //permet de recuperer la configuration de l'application interface dans asp.net core
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AuthController(UserManager<Artiste> userManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] ArtisteRegisterDTO registerModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (registerModel.Password != registerModel.ConfirmPassword)
                return BadRequest("Le password et confirmPassword ne sont pas identiques");

            var userExists = await _userManager.FindByEmailAsync(registerModel.Email);
            if (userExists != null)
                return BadRequest("Email existe déjà");

            // Gestion de l'image profil
            string relativePath = null;
            if (registerModel.ImageProfil != null && registerModel.ImageProfil.Length > 0)
            {
                string directoryPath = Path.Combine(_webHostEnvironment.ContentRootPath, @"wwwroot\image\profil");

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(registerModel.ImageProfil.FileName);
                string filePath = Path.Combine(directoryPath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await registerModel.ImageProfil.CopyToAsync(stream);
                }

                relativePath = $"/image/profil/{uniqueFileName}";
            }

            // Création de l'utilisateur
            var user = new Artiste
            {
                UserName = registerModel.UserName,
                Email = registerModel.Email,
                ImageProfil = relativePath,
                Date_inscription = registerModel.DateTime,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, registerModel.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Génération du token de confirmation
            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Création du lien de confirmation
            var baseUrl = _configuration["AppSettings:BaseUrl"];
            var confirmationLink = $"{baseUrl}/auth/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(emailConfirmationToken)}";

            // Envoi de l'email de confirmation
            try
            {
                await _emailService.SendEmailConfirmationAsync(user.Email, confirmationLink);

                return Ok(new
                {
                    message = "Inscription réussie. Veuillez vérifier votre email pour confirmer votre compte.",
                    userId = user.Id
                });
            }
            catch (Exception ex)
            {
                // Si l'email n'a pas pu être envoyé, supprimer l'utilisateur
                await _userManager.DeleteAsync(user);
                return StatusCode(500, "Erreur lors de l'envoi de l'email de confirmation. Veuillez réessayer.");
            }
        }
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return BadRequest("UserId et token sont requis");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return BadRequest("Utilisateur non trouvé");

            if (user.EmailConfirmed)
                return Ok("Email déjà confirmé");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
                return BadRequest("Token de confirmation invalide ou expiré");

            // Génération du JWT après confirmation
            var jwtToken = GenerateJwtToken(user);

            return Ok(new
            {
                message = "Email confirmé avec succès",
                token = jwtToken,
                user = new
                {
                    id = user.Id,
                    userName = user.UserName,
                    email = user.Email,
                    imageProfil = user.ImageProfil
                }
            });
        }
        [HttpPost("resend-confirmation")]
        public async Task<IActionResult> ResendConfirmation([FromBody] ResendConfirmationDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest("Email non trouvé");

            if (user.EmailConfirmed)
                return BadRequest("Email déjà confirmé");

            // Génération d'un nouveau token
            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var baseUrl = _configuration["AppSettings:BaseUrl"];
            var confirmationLink = $"{baseUrl}/auth/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(emailConfirmationToken)}";

            try
            {
                await _emailService.SendEmailConfirmationAsync(user.Email, confirmationLink);
                return Ok(new { message = "Email de confirmation renvoyé" });
            }
            catch
            {
                return StatusCode(500, "Erreur lors de l'envoi de l'email");
            }
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
       /* private string GenerateJwtToken(Artiste user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtConfig:Secret"]);

            var claims = new[]
            {
                new Claim("Id", user.Id.ToString()), 
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
        }*/
        private string GenerateJwtToken(Artiste user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtConfig:Secret"]);

            var claims = new[]
            {
                // ✅ IMPORTANT : Ici on met l'ID sous le bon claim standard
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // Sub peut aussi contenir l'ID
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(6), // durée de vie du token
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
