using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ArtistoSocial.Domaine.Core.DTO.Artistes
{
    public class ArtisteRegisterDTO
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public IFormFile ImageProfil { get; set; } // Changed from string to IFormFile
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public DateTime DateTime { get; set; }
    }
}
