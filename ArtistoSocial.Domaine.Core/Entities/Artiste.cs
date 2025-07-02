using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ArtistoSocial.Domaine.Core.Entities
{
    public class Artiste:IdentityUser<int>
    {
        public string ImageProfil { get; set; }
        public DateTime Date_inscription { get; set; } = DateTime.UtcNow;
    }
}
