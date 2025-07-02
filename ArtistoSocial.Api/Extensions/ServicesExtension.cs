using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ArtistoSocial.Api.Extensions
{
    public static class ServicesExtension
    {
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
