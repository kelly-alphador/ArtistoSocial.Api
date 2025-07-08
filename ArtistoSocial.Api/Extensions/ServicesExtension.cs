using System.Text;
using ArtistoSocial.Domaine.Core.Entities;
using ArtistoSocial.Infrastructure.Core.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace ArtistoSocial.Api.Extensions
{
    public static class ServicesExtension
    {
        public static void AddIdentityWithEmailConfirmation(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddIdentity<Artiste, IdentityRole<int>>(ConfigureIdentityOptions)
                .AddEntityFrameworkStores<ArtistoMusicalDbContext>()
                .AddDefaultTokenProviders();

            // Configuration de la durée de vie des tokens
            services.Configure<DataProtectionTokenProviderOptions>(opt =>
                opt.TokenLifespan = TimeSpan.FromHours(24));
        }

        private static void ConfigureIdentityOptions(IdentityOptions options)
        {
            // l'utilisateur doit confirmer l'email avant de se connecter
            options.SignIn.RequireConfirmedEmail = true;

            // Mot de passe
            options.Password.RequiredLength = 6;
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;

            // Utilisateur
            options.User.RequireUniqueEmail = true;

            // Verrouillage
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // Token
            options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
        }
        public static void AddJwtAuthentication(this IServiceCollection services,IConfiguration config)
        {
            //pour recuperer le cle secret dans appsettings.json
            var key = Encoding.ASCII.GetBytes(config["JwtConfig:Secret"]);

            services.AddAuthentication(options =>
            {
                //pour montrer qu'on utilise JWT ici
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                //pour retourner 401 unauthorize pour les personne aui accede a url proteger
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    //validation de cle signature de token
                    //pour eviter que il y a des fausse token
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    //pour pouvoir verifier que le token vient bien de notre serveur(verification de l'emeteur)
                    //il permet de savoir qui a envoyer cette token
                    ValidateIssuer = false,
                    // indique pour qui le token a été émis.
                    ValidateAudience = false
                };
            });
        }
    }
}
