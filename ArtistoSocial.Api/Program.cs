using ArtistoSocial.Api.Extensions;
using ArtistoSocial.Domaine.Core.DTO.Artistes;
using ArtistoSocial.Domaine.Core.DTO.Email;
using ArtistoSocial.Domaine.Core.Entities;
using ArtistoSocial.Domaine.Core.Interface;
using ArtistoSocial.Infrastructure.Core.Data;
using ArtistoSocial.Infrastructure.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//configuration de l'authentification
//tu appele la methode d'extension
builder.Services.AddJwtAuthentication(builder.Configuration);
//builder.Services.AddIdentityWithEmailConfirmation();
// Configuration des paramètres email
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
// Enregistrement du service email
builder.Services.AddScoped<IEmailService, EmailService>();
//Tu veux lier la section JwtConfig de ton appsettings.json à une classe C# (JwtConfig).
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));
//Tu ajoute identity dans le conteneur d'injection de dependance pour pouvoir utiliser les fonctionnalites de l'authentification et l'autorisation
/*builder.Services.AddIdentity<Artiste, IdentityRole<int>>()
    .AddEntityFrameworkStores<ArtistoMusicalDbContext>();*/
builder.Services.AddIdentity<Artiste, IdentityRole<int>>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
    // tu peux ajouter d'autres options ici si tu veux
})
.AddEntityFrameworkStores<ArtistoMusicalDbContext>()
.AddDefaultTokenProviders();

//pour configurer la BDD et pour enregistrer le dbcontext dans le conteneur injection de dependance 
builder.Services.AddDbContext<ArtistoMusicalDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
app.UseHttpsRedirection();
//Active le systeme d'authentification de JWT
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
