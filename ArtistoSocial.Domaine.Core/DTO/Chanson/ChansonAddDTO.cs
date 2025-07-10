using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtistoSocial.Domaine.Core.Entities;
using Microsoft.AspNetCore.Http;

namespace ArtistoSocial.Domaine.Core.DTO.Chanson
{
    public class ChansonAddDTO
    {
        public string Titre { get; set; }
        public string legende { get; set; }
        public IFormFile Fichier { get; set; }
    }
}
